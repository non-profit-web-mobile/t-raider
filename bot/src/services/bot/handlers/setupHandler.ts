import { Context } from 'telegraf';
import { SessionData } from '../../../database/schemas/userProfile';
import { logger } from '../../../utils/logger';
import { TickerService } from '../../data/tickerService';
import { UserService } from '../../data/userService';
import { BotMessages, ConfidenceValues, RiskProfiles, SourceTypes } from '../ui/botMessages';
import { KeyboardBuilder } from '../ui/keyboardBuilder';

export class SetupHandler {
  private userService = new UserService();
  private tickerService = new TickerService();

  async showSourceSelection(ctx: Context, userId: number): Promise<void> {
    await this.userService.updateUserSession(userId, {
      step: 'source_selection',
    });

    await ctx.reply(
      BotMessages.SOURCE_SELECTION,
      KeyboardBuilder.getSourceSelectionKeyboard()
    );
  }

  async handleSourceSelection(
    ctx: Context,
    userId: number,
    source: string
  ): Promise<void> {
    await this.userService.updateUserSession(userId, {
      selectedSources: source,
      step: 'risk_selection',
    });

    const sourceText = source === 'basic' ? SourceTypes.basic : SourceTypes.premium;

    await ctx.reply(
      BotMessages.FORMATTERS.sourceSelected(sourceText) + BotMessages.RISK_SELECTION,
      KeyboardBuilder.getRiskSelectionKeyboard()
    );
  }

  async handleRiskSelection(
    ctx: Context,
    userId: number,
    risk: string
  ): Promise<void> {
    await this.userService.updateUserSession(userId, {
      selectedRiskProfile: risk,
      step: 'ticker_selection',
    });

    const riskText = this.getRiskProfileText(risk);
    const confidenceValue = this.getConfidenceValue(risk);

    await this.userService.updateUser(userId, { confidence: confidenceValue });

    await ctx.reply(
      BotMessages.FORMATTERS.riskSelected(riskText) + BotMessages.TICKER_SELECTION
    );

    await ctx.reply(
      'Выберите действие:',
      KeyboardBuilder.getTickerSelectionKeyboard()
    );
  }

  async completeSetup(ctx: Context, userId: number): Promise<void> {
    try {
      const user = await this.userService.getUser(userId);
      if (!user) return;

      const session = await this.userService.getUserSession(userId);
      if (!session) return;

      await this.userService.updateUser(userId, {
        streamEnabled: true,
        summaryEnabled: false,
      });

      const userTickers = await this.tickerService.getUserTickers(user.id);
      const message = this.buildSetupCompleteMessage(session, userTickers);

      await ctx.reply(message, KeyboardBuilder.getSettingsKeyboard());
      await this.clearUserSession(userId);

      logger.info(`User ${userId} completed setup with stream enabled`);
    } catch (error) {
      logger.error('Error completing setup:', error);
      await ctx.reply(BotMessages.ERROR_SETUP);
    }
  }

  private buildSetupCompleteMessage(session: SessionData, userTickers: string[]): string {
    const sourcesText = session.selectedSources === 'basic' ? SourceTypes.basic : SourceTypes.premium;
    const riskText = this.getRiskProfileText(session.selectedRiskProfile!);
    const tickersText = userTickers.length > 0 ? userTickers.sort().join(', ') : 'не выбраны';

    return BotMessages.SETUP_COMPLETE +
      `📊 Источники: ${sourcesText}\n` +
      `⚖️ Риск-профиль: ${riskText}\n` +
      `📋 Тикеры: ${tickersText}\n\n` +
      'Теперь вы будете получать актуальные новости для трейдинга!';
  }

  private async clearUserSession(userId: number): Promise<void> {
    await this.userService.updateUserSession(userId, {
      step: undefined,
      awaitingTickers: undefined,
    });
  }

  private getConfidenceValue(risk: string): number {
    return ConfidenceValues[risk as keyof typeof ConfidenceValues] || 0.5;
  }

  private getRiskProfileText(risk: string): string {
    return RiskProfiles[risk as keyof typeof RiskProfiles] || risk;
  }
} 