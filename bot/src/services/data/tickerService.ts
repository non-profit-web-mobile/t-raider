import { getDB } from '../../database/connection';
import { tickers, userProfileTickers } from '../../database/schemas';
import { logger } from '../../utils/logger';
import { eq, and, inArray } from 'drizzle-orm';

export class TickerService {
  private db = getDB();

  /**
   * Получить или создать тикер в базе данных
   * @param symbol - Символ тикера
   * @returns ID созданного или найденного тикера
   */
  async getOrCreateTicker(symbol: string): Promise<number> {
    try {
      const normalizedSymbol = symbol.toUpperCase();
      
      const [existingTicker] = await this.db
        .select()
        .from(tickers)
        .where(eq(tickers.symbol, normalizedSymbol))
        .limit(1);

      if (existingTicker) {
        return existingTicker.id;
      }

      const [ticker] = await this.db
        .insert(tickers)
        .values({ symbol: normalizedSymbol })
        .returning();
      
      logger.info(`Ticker created: ${ticker.symbol}`);
      return ticker.id;
    } catch (error) {
      logger.error(`Error getting or creating ticker ${symbol}:`, error);
      throw error;
    }
  }

  /**
   * Валидация тикеров - проверяет, что тикеры существуют в базе данных
   * @param tickerSymbols - Массив символов тикеров для валидации
   * @returns Объект с валидными и невалидными тикерами
   */
  async validateTickers(tickerSymbols: string[]): Promise<{ valid: string[], invalid: string[] }> {
    try {
      if (tickerSymbols.length === 0) {
        return { valid: [], invalid: [] };
      }

      const normalizedSymbols = tickerSymbols.map(symbol => symbol.toUpperCase());
      
      const availableTickers = await this.db
        .select({ symbol: tickers.symbol })
        .from(tickers)
        .where(inArray(tickers.symbol, normalizedSymbols));

      const availableSymbols = new Set(availableTickers.map(t => t.symbol));
      
      const valid: string[] = [];
      const invalid: string[] = [];

      normalizedSymbols.forEach(symbol => {
        if (availableSymbols.has(symbol)) {
          valid.push(symbol);
        } else {
          invalid.push(symbol);
        }
      });

      return { valid, invalid };
    } catch (error) {
      logger.error('Error validating tickers:', error);
      throw error;
    }
  }

  /**
   * Получить список всех доступных тикеров из базы данных
   * @returns Массив символов тикеров
   */
  async getAvailableTickers(): Promise<string[]> {
    try {
      const availableTickers = await this.db
        .select({ symbol: tickers.symbol })
        .from(tickers)
        .orderBy(tickers.symbol);

      return availableTickers.map(t => t.symbol);
    } catch (error) {
      logger.error('Error getting available tickers:', error);
      throw error;
    }
  }

  /**
   * Получить случайные тикеры для портфеля
   * @param count - Количество тикеров для выборки (по умолчанию 10)
   * @returns Массив случайных символов тикеров
   */
  async getRandomPortfolioTickers(count: number = 10): Promise<string[]> {
    try {
      const availableTickers = await this.getAvailableTickers();
      
      if (availableTickers.length === 0) {
        logger.warn('No tickers available in database for portfolio selection');
        return [];
      }

      const actualCount = Math.min(count, availableTickers.length);
      const shuffled = [...availableTickers].sort(() => 0.5 - Math.random());
      
      return shuffled.slice(0, actualCount);
    } catch (error) {
      logger.error('Error getting random portfolio tickers:', error);
      throw error;
    }
  }

  /**
   * Добавить тикер пользователю
   * @param userProfileId - ID профиля пользователя
   * @param tickerSymbol - Символ тикера
   */
  async addUserTicker(userProfileId: number, tickerSymbol: string): Promise<void> {
    try {
      const tickerId = await this.getOrCreateTicker(tickerSymbol);
      
      await this.db
        .insert(userProfileTickers)
        .values({ userProfileId, tickerId })
        .onConflictDoNothing();
      
      logger.info(`Ticker ${tickerSymbol} added to user ${userProfileId}`);
    } catch (error) {
      logger.error(`Error adding ticker ${tickerSymbol} to user ${userProfileId}:`, error);
      throw error;
    }
  }

  /**
   * Удалить тикер у пользователя
   * @param userProfileId - ID профиля пользователя
   * @param tickerSymbol - Символ тикера
   */
  async removeUserTicker(userProfileId: number, tickerSymbol: string): Promise<void> {
    try {
      const normalizedSymbol = tickerSymbol.toUpperCase();
      
      const [ticker] = await this.db
        .select()
        .from(tickers)
        .where(eq(tickers.symbol, normalizedSymbol))
        .limit(1);

      if (!ticker) {
        logger.warn(`Ticker ${normalizedSymbol} not found in database`);
        return;
      }

      await this.db
        .delete(userProfileTickers)
        .where(
          and(
            eq(userProfileTickers.userProfileId, userProfileId),
            eq(userProfileTickers.tickerId, ticker.id)
          )
        );
      
      logger.info(`Ticker ${normalizedSymbol} removed from user ${userProfileId}`);
    } catch (error) {
      logger.error(`Error removing ticker ${tickerSymbol} from user ${userProfileId}:`, error);
      throw error;
    }
  }

  /**
   * Получить все тикеры пользователя
   * @param userProfileId - ID профиля пользователя
   * @returns Массив символов тикеров пользователя
   */
  async getUserTickers(userProfileId: number): Promise<string[]> {
    try {
      const results = await this.db
        .select({ symbol: tickers.symbol })
        .from(userProfileTickers)
        .innerJoin(tickers, eq(userProfileTickers.tickerId, tickers.id))
        .where(eq(userProfileTickers.userProfileId, userProfileId))
        .orderBy(tickers.symbol);

      return results.map(r => r.symbol);
    } catch (error) {
      logger.error(`Error getting tickers for user ${userProfileId}:`, error);
      throw error;
    }
  }

  /**
   * Проверить, есть ли тикер у пользователя
   * @param userProfileId - ID профиля пользователя
   * @param tickerSymbol - Символ тикера
   * @returns true если тикер есть у пользователя, false иначе
   */
  async userHasTicker(userProfileId: number, tickerSymbol: string): Promise<boolean> {
    try {
      const normalizedSymbol = tickerSymbol.toUpperCase();
      
      const [ticker] = await this.db
        .select()
        .from(tickers)
        .where(eq(tickers.symbol, normalizedSymbol))
        .limit(1);

      if (!ticker) {
        return false;
      }

      const [userTicker] = await this.db
        .select()
        .from(userProfileTickers)
        .where(
          and(
            eq(userProfileTickers.userProfileId, userProfileId),
            eq(userProfileTickers.tickerId, ticker.id)
          )
        )
        .limit(1);

      return !!userTicker;
    } catch (error) {
      logger.error(`Error checking if user ${userProfileId} has ticker ${tickerSymbol}:`, error);
      throw error;
    }
  }

  /**
   * Получить статистику по тикерам
   * @returns Объект со статистикой
   */
  async getTickerStats(): Promise<{
    totalTickers: number;
    totalUserAssociations: number;
  }> {
    try {
      const [tickerCount] = await this.db
        .select({ count: tickers.id })
        .from(tickers);

      const [associationCount] = await this.db
        .select({ count: userProfileTickers.userProfileId })
        .from(userProfileTickers);

      return {
        totalTickers: Number(tickerCount?.count) || 0,
        totalUserAssociations: Number(associationCount?.count) || 0
      };
    } catch (error) {
      logger.error('Error getting ticker stats:', error);
      throw error;
    }
  }
} 