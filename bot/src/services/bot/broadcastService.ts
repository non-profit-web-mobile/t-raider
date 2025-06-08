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

    // Создаем inline клавиатуру если есть кнопки
    const keyboard = buttons && buttons.length > 0 
      ? this.createInlineKeyboard(buttons)
      : undefined;

    for (const telegramId of telegramIds) {
      try {
        const sendOptions: ExtraReplyMessage = { 
          parse_mode: 'Markdown',
        };

        if (keyboard) {
          sendOptions.reply_markup = keyboard.reply_markup;
        }

        await this.botInstance.telegram.sendMessage(telegramId, message, sendOptions);
        successCount++;
        
        // Небольшая задержка между отправками чтобы избежать rate limiting
        await new Promise(resolve => setTimeout(resolve, 50));
      } catch (error) {
        errorCount++;
        logger.error(`Failed to send message to user ${telegramId}:`, error);
      }
    }

    const result: BroadcastResult = {
      successful: successCount,
      errors: errorCount,
      totalRecipients: telegramIds.length
    };

    logger.info(`Broadcast completed`, {
      buttonsCount: buttons?.length || 0,
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