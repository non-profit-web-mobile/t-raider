import { Context } from 'telegraf';
import { UserService } from '../data/userService';
import { KeyboardBuilder } from './ui/keyboardBuilder';
import { BotMessages } from './ui/botMessages';
import { SetupHandler } from './handlers/setupHandler';
import { TickerHandler } from './handlers/tickerHandler';
import { SessionData } from '../../database/schemas/userProfile';
import { logger } from '../../utils/logger';

export class BotService {
  private userService = new UserService();
  private setupHandler = new SetupHandler();
  private tickerHandler = new TickerHandler();

  async handleStart(ctx: Context): Promise<void> {
    const userId = ctx.from?.id;
    if (!userId) return;

    try {
      await this.userService.getOrCreateUser(userId);

      await ctx.reply(
        BotMessages.WELCOME,
        KeyboardBuilder.getWelcomeKeyboard()
      );

      logger.info(`User ${userId} started the bot`);
    } catch (error) {
      logger.error('Error in handleStart:', error);
      await ctx.reply(BotMessages.ERROR_GENERIC);
    }
  }

  async handleCallbackQuery(ctx: Context): Promise<void> {
    // Этот метод больше не используется, так как мы переходим на обычную клавиатуру
    // Оставляем для совместимости, но логика перенесена в handleMessage
    return;
  }

  async handleMessage(ctx: Context): Promise<void> {
    const userId = ctx.from?.id;
    if (!userId || !('text' in ctx.message!)) return;

    const user = await this.userService.getUser(userId);
    if (!user) return;

    const session = user.session;
    const text = ctx.message.text;

    try {
      await this.routeMessage(ctx, userId, text, session);
    } catch (error) {
      logger.error('Error in handleMessage:', error);
      await ctx.reply(BotMessages.ERROR_GENERIC);
    }
  }

  private async routeMessage(
    ctx: Context, 
    userId: number, 
    text: string, 
    session: SessionData
  ): Promise<void> {
    // Обработка текстовых команд от кнопок
    switch (text) {
      // Основные команды настройки
      case BotMessages.BUTTONS.START:
        await this.setupHandler.showSourceSelection(ctx, userId);
        break;
      case BotMessages.BUTTONS.BASIC_SOURCES:
        await this.setupHandler.handleSourceSelection(ctx, userId, 'basic');
        break;
      case BotMessages.BUTTONS.PREMIUM_SOURCES:
        await this.setupHandler.handleSourceSelection(ctx, userId, 'premium');
        break;
      case BotMessages.BUTTONS.CONSERVATIVE:
        await this.setupHandler.handleRiskSelection(ctx, userId, 'conservative');
        break;
      case BotMessages.BUTTONS.MIXED:
        await this.setupHandler.handleRiskSelection(ctx, userId, 'mixed');
        break;
      case BotMessages.BUTTONS.MAXIMUM:
        await this.setupHandler.handleRiskSelection(ctx, userId, 'maximum');
        break;

      // Команды управления тикерами
      case BotMessages.BUTTONS.PORTFOLIO_TICKERS:
        await this.tickerHandler.handlePortfolioTickers(ctx, userId);
        break;
      case BotMessages.BUTTONS.ADD_TICKERS:
        await this.tickerHandler.promptForTickers(ctx, userId);
        break;
      case BotMessages.BUTTONS.MY_TICKERS:
        await this.tickerHandler.showUserTickers(ctx, userId);
        break;

      // Команды завершения
      case BotMessages.BUTTONS.SKIP:
        await this.setupHandler.completeSetup(ctx, userId);
        break;
      case BotMessages.BUTTONS.START_STREAM:
        await this.setupHandler.completeSetup(ctx, userId);
        break;
      case BotMessages.BUTTONS.CHANGE_SETTINGS:
        await this.setupHandler.showSourceSelection(ctx, userId);
        break;

      default:
        await this.handleDefaultMessage(ctx, userId, text, session);
        break;
    }
  }

  private async handleDefaultMessage(
    ctx: Context, 
    userId: number, 
    text: string, 
    session: SessionData
  ): Promise<void> {
    // Обработка ввода тикеров
    if (session.awaitingTickers) {
      await this.tickerHandler.handleTickerInput(ctx, userId, text);
    } else {
      await ctx.reply(BotMessages.USE_START_COMMAND);
    }
  }
}
