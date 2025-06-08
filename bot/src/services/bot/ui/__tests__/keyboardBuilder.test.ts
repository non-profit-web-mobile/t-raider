import { KeyboardBuilder } from '../keyboardBuilder';

describe('KeyboardBuilder', () => {
  it('should create welcome keyboard', () => {
    const keyboard = KeyboardBuilder.getWelcomeKeyboard();
    expect(keyboard).toBeDefined();
    expect(keyboard.reply_markup.resize_keyboard).toBe(true);
  });

  it('should create source selection keyboard', () => {
    const keyboard = KeyboardBuilder.getSourceSelectionKeyboard();
    expect(keyboard).toBeDefined();
    expect(keyboard.reply_markup.keyboard).toHaveLength(2);
  });

  it('should create risk selection keyboard', () => {
    const keyboard = KeyboardBuilder.getRiskSelectionKeyboard();
    expect(keyboard).toBeDefined();
    expect(keyboard.reply_markup.keyboard).toHaveLength(3);
  });

  it('should create remove keyboard', () => {
    const keyboard = KeyboardBuilder.removeKeyboard();
    expect(keyboard).toBeDefined();
    expect(keyboard.reply_markup.remove_keyboard).toBe(true);
  });
}); 