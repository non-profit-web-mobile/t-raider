import { Telegraf } from 'telegraf';
import { config } from './config/config';
import { logger } from './utils/logger';
import { connectKafka, closeKafka } from './kafka/connection';
import { startAdminSignalsConsumer } from './kafka/consumer';

const bot = new Telegraf(config.BOT_TOKEN);

async function startServer() {
  try {
    await connectKafka();
    logger.info('Kafka connected successfully');

    await startAdminSignalsConsumer(bot);

    bot.start(async (ctx) => {
      logger.info(`Received message from user ${ctx.from?.id}: ${'text' in ctx.message ? ctx.message.text : 'non-text message'}`);
    });

    await bot.launch();
    logger.info('Telegram bot launched successfully');
  } catch (error) {
    logger.error('Failed to start server:', error);
    process.exit(1);
  }
}

process.on('SIGTERM', async () => {
  logger.info('SIGTERM received, shutting down gracefully');
  await closeKafka();
  bot.stop();
  process.exit(0);
});

process.on('SIGINT', async () => {
  logger.info('SIGINT received, shutting down gracefully');
  await closeKafka();
  bot.stop();
  process.exit(0);
});

startServer(); 