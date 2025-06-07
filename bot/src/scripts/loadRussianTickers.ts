import { connectDB, getDB, closeDB } from '../database/connection';
import { tickers } from '../database/schemas/ticker';
import { logger } from '../utils/logger';

// Популярные российские тикеры акций на Московской бирже
const RUSSIAN_TICKERS = [
  // Голубые фишки
  'SBER',    // Сбербанк
  'GAZP',    // Газпром
  'LKOH',    // Лукойл
  'YNDX',    // Яндекс
  'NVTK',    // Новатэк
  'ROSN',    // Роснефть
  'GMKN',    // ГМК Норильский никель
  'TATN',    // Татнефть
  'MTSS',    // МТС
  'VTBR',    // ВТБ
  'MGNT',    // Магнит
  'OZON',    // Озон
  'PLZL',    // Полюс
  'RUAL',    // Русал
  'ALRS',    // Алроса
  'CHMF',    // Северсталь
  'TCSG',    // ТКС Групп
  'FIXP',    // Группа Самолет
  'FIVE',    // X5 Group
  'MAGN',    // ММК
  'NLMK',    // НЛМК
  'POLY',    // Polymetal
  'SNGS',    // Сургутнефтегаз
  'AFLT',    // Аэрофлот
  'MOEX',    // Московская биржа
  'TRNFP',   // Транснефть привилегированные
  'PHOR',    // ФосАгро
  'HYDR',    // РусГидро
  'IRAO',    // Интер РАО
  'FEES',    // Дальневосточная энергетическая компания
  'RTKM',    // Ростелеком
  'SMLT',    // Самолет
  'CBOM',    // МКБ
  'BSPB',    // Банк Санкт-Петербург
  'UPRO',    // Юнипро
  'PIKK',    // ПИК
  'AFKS',    // Система
  'LSRG',    // ЛСР
  'BANE',    // Башнефть
  'DIXY',    // Дикси
  'QIWI',    // QIWI
  'MAIL',    // VK (Mail.ru)
  'DSKY',    // Детский мир
  'ETLN',    // Etalon Group
  'FLOT',    // Совкомфлот
  'SFIN',    // SFI
  'AQUA',    // Инарктика
  'LIFE',    // Фармсинтез
  'MSNG',    // МосЭнерго
  'ENRU',    // En+ Group
];

async function loadRussianTickers() {
  try {
    logger.info('Starting to load Russian tickers...');
    
    // Подключаемся к базе данных
    await connectDB();
    const db = getDB();
    
    // Проверяем, какие тикеры уже существуют
    const existingTickers = await db.select().from(tickers);
    const existingSymbols = new Set(existingTickers.map(t => t.symbol));
    
    // Фильтруем только новые тикеры
    const newTickers = RUSSIAN_TICKERS.filter(symbol => !existingSymbols.has(symbol));
    
    if (newTickers.length === 0) {
      logger.info('All Russian tickers already exist in the database');
      return;
    }
    
    logger.info(`Found ${newTickers.length} new tickers to insert`);
    
    // Вставляем новые тикеры
    const insertData = newTickers.map(symbol => ({ symbol }));
    
    await db.insert(tickers).values(insertData);
    
    logger.info(`Successfully loaded ${newTickers.length} Russian tickers`);
    logger.info(`Loaded tickers: ${newTickers.join(', ')}`);
    
  } catch (error) {
    logger.error('Error loading Russian tickers:', error);
    throw error;
  } finally {
    await closeDB();
  }
}

// Запускаем скрипт если он вызван напрямую
if (require.main === module) {
  loadRussianTickers()
    .then(() => {
      logger.info('Russian tickers loading completed successfully');
      process.exit(0);
    })
    .catch((error) => {
      logger.error('Failed to load Russian tickers:', error);
      process.exit(1);
    });
}

export { loadRussianTickers }; 