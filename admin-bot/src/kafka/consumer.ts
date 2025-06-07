import { Telegraf } from 'telegraf';
import { getKafkaConsumer } from './connection';
import { config } from '../config/config';
import { logger } from '../utils/logger';

interface AdminSignalMessage {
  message: string;
}

export async function startAdminSignalsConsumer(bot: Telegraf) {
  try {
    const consumer = getKafkaConsumer();

    await consumer.subscribe({ topic: config.KAFKA_ADMIN_SIGNALS_TOPIC });

    await consumer.run({
      eachMessage: async ({ message }) => {
        try {
          if (message.value) {
            const messageValue = message.value.toString();
            const adminSignal: AdminSignalMessage = JSON.parse(messageValue);

            logger.info(`Received admin signal from Kafka: ${messageValue}`);

            for (const adminId of config.ADMIN_IDS) {
              try {
                await bot.telegram.sendMessage(adminId, adminSignal.message);
                logger.info(`Admin signal sent to admin ${adminId}`);
              } catch (error) {
                logger.error(
                  `Failed to send admin signal to admin ${adminId}:`,
                  error
                );
              }
            }
          }
        } catch (error) {
          logger.error(
            `Error processing admin signal message from Kafka:`,
            error
          );
        }
      },
    });

    logger.info('AdminSignals consumer started successfully');
  } catch (error) {
    logger.error('Failed to start AdminSignals consumer:', error);
    throw error;
  }
}
