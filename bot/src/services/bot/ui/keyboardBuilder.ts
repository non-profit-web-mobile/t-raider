import { Markup } from 'telegraf';
import { BotMessages } from './botMessages';

export class KeyboardBuilder {
  static getWelcomeKeyboard() {
    return Markup.keyboard([[BotMessages.BUTTONS.START]])
      .resize()
      .oneTime();
  }

  static getSourceSelectionKeyboard() {
    return Markup.keyboard([
      [BotMessages.BUTTONS.BASIC_SOURCES],
      [BotMessages.BUTTONS.PREMIUM_SOURCES]
    ])
      .resize()
      .oneTime();
  }

  static getRiskSelectionKeyboard() {
    return Markup.keyboard([
      [BotMessages.BUTTONS.CONSERVATIVE],
      [BotMessages.BUTTONS.MIXED],
      [BotMessages.BUTTONS.MAXIMUM]
    ])
      .resize()
      .oneTime();
  }

  static getTickerSelectionKeyboard() {
    return Markup.keyboard([
      [BotMessages.BUTTONS.PORTFOLIO_TICKERS],
      [BotMessages.BUTTONS.ADD_TICKERS],
      [BotMessages.BUTTONS.SKIP]
    ])
      .resize()
      .oneTime();
  }

  static getMainMenuKeyboard() {
    return Markup.keyboard([[BotMessages.BUTTONS.START_STREAM]])
      .resize()
      .oneTime();
  }

  static getUserManagementKeyboard() {
    return Markup.keyboard([
      [BotMessages.BUTTONS.CHANGE_SETTINGS, BotMessages.BUTTONS.MY_TICKERS],
    ]).resize();
  }

  static getSettingsKeyboard() {
    return Markup.keyboard([
      [BotMessages.BUTTONS.CHANGE_SETTINGS]
    ]).resize();
  }

  static removeKeyboard() {
    return Markup.removeKeyboard();
  }
} 