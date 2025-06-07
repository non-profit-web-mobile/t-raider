import express from 'express';
import { Telegraf } from 'telegraf';
import { ExtraSetWebhook } from 'telegraf/typings/telegram-types';
import { config } from './config/config';
import { connectDB } from './database/connection';
import { BotService } from './services/bot/botService';
import { logger } from './utils/logger';

const app = express();
const bot = new Telegraf(config.BOT_TOKEN);

// Middleware для парсинга JSON
app.use(express.json());

const validateWebhookSecret = (req: express.Request, res: express.Response, next: express.NextFunction) => {
  if (!config.WEBHOOK_SECRET) {
    logger.warn('WEBHOOK_SECRET not configured, skipping validation');
    return next();
  }

  const receivedSecret = req.headers['x-telegram-bot-api-secret-token'];
  
  if (!receivedSecret || receivedSecret !== config.WEBHOOK_SECRET) {
    logger.warn('Invalid or missing webhook secret token', {
      ip: req.ip,
      headers: req.headers,
    });
    res.status(401).json({ error: 'Unauthorized' });
    return;
  }
  
  next();
};

// Bot handlers will be set up after database connection

// Webhook endpoint
app.use('/webhook', validateWebhookSecret, bot.webhookCallback());

// Health check endpoint
app.get('/health', (_, res) => {
  res.json({ status: 'ok', timestamp: new Date().toISOString() });
});



async function startServer() {
  try {
    // Подключение к базе данных
    await connectDB();
    logger.info('Database connected successfully');

    // Создаем BotService после подключения к БД
    const botService = new BotService();

    // Настраиваем обработчики бота
    bot.start(async (ctx) => {
      await botService.handleStart(ctx);
    });

    bot.on('callback_query', async (ctx) => {
      await botService.handleCallbackQuery(ctx);
    });

    bot.on('message', async (ctx) => {
      try {
        logger.info(`Received message from user ${ctx.from?.id}: ${'text' in ctx.message ? ctx.message.text : 'non-text message'}`);
        await botService.handleMessage(ctx);
      } catch (error) {
        logger.error('Error handling message:', error);
      }
    });

    // // Подключение к Kafka
    // await connectKafka();
    // logger.info('Kafka connected successfully');

    // Установка вебхука
    const webhookUrl = `${config.WEBHOOK_URL}/webhook`;

    const webhookOptions: ExtraSetWebhook = {};

    if (config.WEBHOOK_SECRET !== '') {
      webhookOptions.secret_token = config.WEBHOOK_SECRET;
    }

    await bot.telegram.setWebhook(webhookUrl, webhookOptions);
    logger.info(`Webhook set to: ${webhookUrl}`);

    // Запуск сервера
    const port = config.PORT || 3000;
    app.listen(port, () => {
      logger.info(`Server is running on port ${port}`);
    });
  } catch (error) {
    logger.error('Failed to start server:', error);
    process.exit(1);
  }
}

// Graceful shutdown
process.on('SIGTERM', () => {
  logger.info('SIGTERM received, shutting down gracefully');
  process.exit(0);
});

process.on('SIGINT', () => {
  logger.info('SIGINT received, shutting down gracefully');
  process.exit(0);
});

startServer(); 