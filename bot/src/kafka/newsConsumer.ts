import { Consumer } from 'kafkajs';
import { getKafkaConsumer } from './connection';
import { NewsEvent } from '../types/news';
import { logger } from '../utils/logger';
import { NewsHandler } from '../services/bot/handlers/newsHandler';
import { config } from '../config/config';

export class NewsConsumer {
  private consumer: Consumer;
  private newsHandler: NewsHandler;

  constructor(newsHandler: NewsHandler) {
    this.newsHandler = newsHandler;
    this.consumer = getKafkaConsumer();
  }

  async startConsuming(): Promise<void> {
    try {
      await this.consumer.subscribe({ topic: config.KAFKA_TOPIC, });

      await this.consumer.run({
        eachMessage: async ({ topic, partition, message }) => {
          try {
            if (!message.value) return;

            const newsEvent: NewsEvent = JSON.parse(message.value.toString());
            
            logger.info(`Processing news event`, {
              telegramIds: newsEvent.telegramIds,
              buttonsCount: newsEvent.buttons.length,
              messageLength: newsEvent.message.length
            });

            await this.newsHandler.handleNewsEvent(newsEvent);

          } catch (error) {
            logger.error('Error processing news message:', {
              error,
              topic,
              partition,
              offset: message.offset
            });
          }
        },
      });

      logger.info('News consumer started successfully');
    } catch (error) {
      logger.error('Error starting news consumer:', error);
      throw error;
    }
  }

  async stopConsuming(): Promise<void> {
    try {
      await this.consumer.disconnect();
      logger.info('News consumer stopped');
    } catch (error) {
      logger.error('Error stopping news consumer:', error);
    }
  }
} 