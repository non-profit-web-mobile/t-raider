import { Telegraf } from 'telegraf';
import { NewsButton } from '../../../types/news';
import { BroadcastService } from '../broadcastService';

jest.mock('../../../utils/logger');

describe('BroadcastService', () => {
  let broadcastService: BroadcastService;
  let mockBot: jest.Mocked<Telegraf>;

  beforeEach(() => {
    const mockSendMessage = jest.fn().mockResolvedValue({});
    mockBot = {
      telegram: {
        sendMessage: mockSendMessage
      }
    } as any;
    broadcastService = new BroadcastService(mockBot);
  });

  it('should create inline keyboard from buttons', () => {
    const buttons: NewsButton[] = [
      { text: 'Button 1', url: 'https://example1.com' },
      { text: 'Button 2', url: 'https://example2.com' }
    ];
    
    const keyboard = (broadcastService as any).createInlineKeyboard(buttons);
    expect(keyboard).toBeDefined();
  });

  it('should return correct broadcast result structure', async () => {
    const result = await broadcastService.broadcastMessage([123], 'test message');
    expect(result.successful).toBe(1);
    expect(result.totalRecipients).toBe(1);
  });

  it('should handle send message errors', async () => {
    const mockSendMessage = jest.fn().mockRejectedValue(new Error('Send failed'));
    mockBot.telegram.sendMessage = mockSendMessage;
    
    const result = await broadcastService.broadcastMessage([123], 'test');
    expect(result.errors).toBe(1);
    expect(result.successful).toBe(0);
  });

  it('should process multiple recipients', async () => {
    const result = await broadcastService.broadcastMessage([123, 456, 789], 'test');
    expect(result.totalRecipients).toBe(3);
    expect(result.successful).toBe(3);
  });
}); 