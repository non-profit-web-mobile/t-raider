import { NewsEvent } from '../../../types/news';
import { logger } from '../../../utils/logger';
import { BroadcastService } from '../broadcastService';

export class NewsHandler {
  private broadcastService: BroadcastService;

  constructor(broadcastService: BroadcastService) {
    this.broadcastService = broadcastService;
  }

  async handleNewsEvent(newsEvent: NewsEvent): Promise<void> {
    try {
      logger.info(`Processing news event`, {
        telegramIds: newsEvent.telegramIds,
        buttonsCount: newsEvent.buttons.length,
        messageLength: newsEvent.message.length,
      });

      // Отправляем рассылку с кнопками
      const result = await this.broadcastService.broadcastMessage(
        newsEvent.telegramIds,
        newsEvent.message,
        newsEvent.buttons
      );

      logger.info(`News event processed successfully`, {
        ...result,
      });
    } catch (error) {
      logger.error('Error processing news event:', {
        error,
        telegramIdsCount: newsEvent.telegramIds.length,
      });
      throw error;
    }
  }
}
