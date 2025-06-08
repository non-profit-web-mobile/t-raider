import { Telegraf, Markup } from 'telegraf';
import { logger } from '../../utils/logger';
import { NewsButton } from '../../types/news';
import { ExtraReplyMessage } from 'telegraf/typings/telegram-types';

export interface BroadcastResult {
  successful: number;
  errors: number;
  totalRecipients: number;
}

export class BroadcastService {
  private botInstance: Telegraf;
  private readonly maxRetries = 3;
  private readonly baseDelay = 100; // базовая задержка в мс

  constructor(botInstance: Telegraf) {
    this.botInstance = botInstance;
  }

  async broadcastMessage(
    telegramIds: number[], 
    message: string, 
    buttons?: NewsButton[],
  ): Promise<BroadcastResult> {
    let successCount = 0;
    let errorCount = 0;

    const keyboard = buttons && buttons.length > 0 
      ? this.createInlineKeyboard(buttons)
      : undefined;

    for (const telegramId of telegramIds) {
      let success = false;
      
      for (let attempt = 1; attempt <= this.maxRetries; attempt++) {
        try {
          const sendOptions: ExtraReplyMessage = { 
            parse_mode: 'Markdown',
          };

          if (keyboard) {
            sendOptions.reply_markup = keyboard.reply_markup;
          }

          await this.botInstance.telegram.sendMessage(telegramId, message, sendOptions);
          successCount++;
          success = true;
          
          if (attempt > 1) {
            logger.info(`Message sent successfully to user ${telegramId} on attempt ${attempt}`);
          }
          
        } catch (error) {
          const isLastAttempt = attempt === this.maxRetries;
          
          if (isLastAttempt) {
            errorCount++;
            logger.error(`Failed to send message to user ${telegramId} after ${this.maxRetries} attempts:`, error);
          } else {
            logger.warn(`Attempt ${attempt} failed for user ${telegramId}, retrying...`, error);
            
            const delay = this.baseDelay * Math.pow(2, attempt - 1);
            await new Promise(resolve => setTimeout(resolve, delay));
          }
        }
      }

      if (success) {
        await new Promise(resolve => setTimeout(resolve, 50));
      }
    }

    const result: BroadcastResult = {
      successful: successCount,
      errors: errorCount,
      totalRecipients: telegramIds.length
    };

    logger.info(`Broadcast completed`, {
      buttonsCount: buttons?.length || 0,
      maxRetries: this.maxRetries,
      ...result
    });

    return result;
  }

  private createInlineKeyboard(buttons: NewsButton[]) {
    const inlineButtons = buttons.map(button => 
      Markup.button.url(button.text, button.url)
    );
    
    // Создаем клавиатуру с кнопками в один ряд
    return Markup.inlineKeyboard(inlineButtons);
  }
} 