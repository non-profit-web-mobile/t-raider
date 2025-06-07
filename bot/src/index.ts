import express from 'express';
import { Telegraf } from 'telegraf';
import { logger } from './utils/logger';
import { connectDB } from './database/connection';
import { connectKafka } from './kafka/connection';
import { config } from './config/config';

const app = express();
const bot = new Telegraf(config.BOT_TOKEN);

// Middleware для парсинга JSON
app.use(express.json());

// Обработчик всех сообщений - отвечает "hello"
bot.on('message', async (ctx) => {
  try {
    logger.info(`Received message from user ${ctx.from?.id}: ${'text' in ctx.message ? ctx.message.text : 'non-text message'}`);
    await ctx.reply('hello');
  } catch (error) {
    logger.error('Error handling message:', error);
  }
});

// Webhook endpoint
app.use(bot.webhookCallback('/webhook'));

// Health check endpoint
app.get('/health', (_, res) => {
  res.json({ status: 'ok', timestamp: new Date().toISOString() });
});

async function startServer() {
  try {
    // Подключение к базе данных
    await connectDB();
    logger.info('Database connected successfully');

    // Подключение к Kafka
    await connectKafka();
    logger.info('Kafka connected successfully');

    // Установка вебхука
    const webhookUrl = `${config.WEBHOOK_URL}/webhook`;
    await bot.telegram.setWebhook(webhookUrl);
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