import { connectDB, getDB, closeDB } from '../database/connection';
import { tickers } from '../database/schemas/ticker';
import { logger } from '../utils/logger';

async function listAllTickers() {
  try {
    logger.info('Fetching all tickers from database...');
    
    // Подключаемся к базе данных
    await connectDB();
    const db = getDB();
    
    // Получаем все тикеры
    const allTickers = await db.select().from(tickers);
    
    logger.info(`Found ${allTickers.length} tickers in database:`);
    
    if (allTickers.length > 0) {
      console.table(allTickers);
      logger.info(`Ticker symbols: ${allTickers.map(t => t.symbol).join(', ')}`);
    } else {
      logger.info('No tickers found in database');
    }
    
  } catch (error) {
    logger.error('Error fetching tickers:', error);
    throw error;
  } finally {
    await closeDB();
  }
}

// Запускаем скрипт если он вызван напрямую
if (require.main === module) {
  listAllTickers()
    .then(() => {
      logger.info('Ticker listing completed successfully');
      process.exit(0);
    })
    .catch((error) => {
      logger.error('Failed to list tickers:', error);
      process.exit(1);
    });
}

export { listAllTickers }; 