import { Context } from 'telegraf';
import { UserService } from '../../data/userService';
import { TickerService } from '../../data/tickerService';
import { BotMessages } from '../ui/botMessages';
import { KeyboardBuilder } from '../ui/keyboardBuilder';
import { logger } from '../../../utils/logger';

export class TickerHandler {
  private userService = new UserService();
  private tickerService = new TickerService();

  async promptForTickers(ctx: Context, userId: number): Promise<void> {
    await this.userService.updateUserSession(userId, { awaitingTickers: true });

    await ctx.reply(
      BotMessages.TICKER_INPUT_PROMPT,
      KeyboardBuilder.removeKeyboard()
    );
  }

  async handleTickerInput(
    ctx: Context,
    userId: number,
    text: string
  ): Promise<void> {
    try {
      const user = await this.userService.getUser(userId);
      if (!user) return;

      const inputTickers = this.parseTickerInput(text);
      if (inputTickers.length === 0) {
        await ctx.reply(BotMessages.ENTER_AT_LEAST_ONE_TICKER);
        return;
      }

      // Валидация тикеров
      const { valid, invalid } =
        await this.tickerService.validateTickers(inputTickers);

      // Добавляем только валидные тикеры
      await this.addTickersToUser(user.id, valid);

      await this.userService.updateUserSession(userId, {
        awaitingTickers: false,
      });

      const message = await this.buildTickerResponseMessage(valid, invalid);

      if (valid.length > 0) {
        await ctx.reply(message, KeyboardBuilder.getMainMenuKeyboard());
      } else {
        await ctx.reply(message);
      }

      logger.info(
        `User ${userId} - valid tickers: ${valid.join(', ')}, invalid: ${invalid.join(', ')}`
      );
    } catch (error) {
      logger.error('Error handling ticker input:', error);
      await ctx.reply(BotMessages.ERROR_TICKER_ADD);
    }
  }

  async handlePortfolioTickers(ctx: Context, userId: number): Promise<void> {
    try {
      const user = await this.userService.getUser(userId);
      if (!user) return;

      // Получаем случайные тикеры из доступных
      const portfolioTickers =
        await this.tickerService.getRandomPortfolioTickers(5);

      const tickers = [...portfolioTickers, 'SBER', 'GAZP', 'LKOH', 'YNDX', 'ROSN'];
      const uniqueTickers = [...new Set(tickers)].sort();

      await this.addTickersToUser(user.id, uniqueTickers);

      await ctx.reply(
        BotMessages.FORMATTERS.tickersFromPortfolio(uniqueTickers),
        KeyboardBuilder.getMainMenuKeyboard()
      );

      logger.info(
        `User ${userId} added portfolio tickers: ${portfolioTickers.join(', ')}`
      );
    } catch (error) {
      logger.error('Error handling portfolio tickers:', error);
      await ctx.reply(BotMessages.ERROR_PORTFOLIO);
    }
  }

  async showUserTickers(ctx: Context, userId: number): Promise<void> {
    try {
      const user = await this.userService.getUser(userId);
      if (!user) return;

      const userTickers = await this.tickerService.getUserTickers(user.id);

      if (userTickers.length === 0) {
        await ctx.reply(
          BotMessages.NO_TICKERS,
          KeyboardBuilder.getUserManagementKeyboard()
        );
      } else {
        await ctx.reply(
          BotMessages.FORMATTERS.userTickersList(
            userTickers.length,
            userTickers
          ),
          KeyboardBuilder.getUserManagementKeyboard()
        );
      }
    } catch (error) {
      logger.error('Error showing user tickers:', error);
      await ctx.reply(BotMessages.ERROR_TICKERS_GET);
    }
  }

  async showAvailableTickers(ctx: Context): Promise<void> {
    try {
      const availableTickers = await this.tickerService.getAvailableTickers();

      if (availableTickers.length === 0) {
        await ctx.reply(
          BotMessages.NO_AVAILABLE_TICKERS,
          KeyboardBuilder.getUserManagementKeyboard()
        );
        return;
      }

      const message = this.formatAvailableTickersMessage(availableTickers);
      await ctx.reply(message, KeyboardBuilder.getUserManagementKeyboard());
    } catch (error) {
      logger.error('Error showing available tickers:', error);
      await ctx.reply(BotMessages.ERROR_AVAILABLE_TICKERS);
    }
  }

  async clearUserTickers(ctx: Context, userId: number): Promise<void> {
    try {
      const user = await this.userService.getUser(userId);
      if (!user) return;

      const userTickers = await this.tickerService.getUserTickers(user.id);

      if (userTickers.length === 0) {
        await ctx.reply(
          BotMessages.NO_TICKERS_TO_CLEAR,
          KeyboardBuilder.getUserManagementKeyboard()
        );
        return;
      }

      // Удаляем все тикеры пользователя
      await this.removeAllUserTickers(user.id, userTickers);

      await ctx.reply(
        BotMessages.FORMATTERS.tickersCleared(userTickers.length, userTickers),
        KeyboardBuilder.getUserManagementKeyboard()
      );

      logger.info(
        `User ${userId} cleared all tickers: ${userTickers.join(', ')}`
      );
    } catch (error) {
      logger.error('Error clearing user tickers:', error);
      await ctx.reply(BotMessages.ERROR_CLEAR_TICKERS);
    }
  }

  async showTickerStats(ctx: Context): Promise<void> {
    try {
      const stats = await this.tickerService.getTickerStats();

      const message = BotMessages.FORMATTERS.stats(
        stats.totalTickers,
        stats.totalUserAssociations
      );
      await ctx.reply(message, KeyboardBuilder.getUserManagementKeyboard());
    } catch (error) {
      logger.error('Error showing ticker stats:', error);
      await ctx.reply(BotMessages.ERROR_STATS);
    }
  }

  private parseTickerInput(text: string): string[] {
    return text
      .split(/[,\s]+/)
      .map((t) => t.trim().toUpperCase())
      .filter((t) => t.length > 0);
  }

  private async addTickersToUser(
    userProfileId: number,
    tickers: string[]
  ): Promise<void> {
    for (const ticker of tickers) {
      await this.tickerService.addUserTicker(userProfileId, ticker);
    }
  }

  private async removeAllUserTickers(
    userProfileId: number,
    tickers: string[]
  ): Promise<void> {
    for (const ticker of tickers) {
      await this.tickerService.removeUserTicker(userProfileId, ticker);
    }
  }

  private async buildTickerResponseMessage(
    valid: string[],
    invalid: string[]
  ): Promise<string> {
    let message = '';

    if (valid.length > 0) {
      message += BotMessages.FORMATTERS.tickersAdded(valid);
    }

    if (invalid.length > 0) {
      const availableTickers = await this.tickerService.getAvailableTickers();
      message += BotMessages.FORMATTERS.invalidTickers(
        invalid,
        availableTickers
      );
    }

    if (valid.length > 0) {
      message += '🎉 Настройка завершена!';
    } else {
      message += 'Попробуйте ввести тикеры из списка доступных.';
    }

    return message;
  }

  private formatAvailableTickersMessage(tickers: string[]): string {
    // Разбиваем на части для удобного отображения
    const chunks = [];
    for (let i = 0; i < tickers.length; i += 10) {
      chunks.push(tickers.slice(i, i + 10));
    }

    let message = BotMessages.AVAILABLE_TICKERS_TITLE;
    chunks.forEach((chunk) => {
      message += `${chunk.join(', ')}\n`;
    });

    message += BotMessages.FORMATTERS.totalAvailable(tickers.length);
    return message;
  }
}
