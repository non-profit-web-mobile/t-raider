import { BotService } from '../botService';
import { Telegraf } from 'telegraf';

jest.mock('../../../utils/logger');
jest.mock('../../data/userService');
jest.mock('../handlers/setupHandler');
jest.mock('../handlers/tickerHandler');

describe('BotService', () => {
  let botService: BotService;
  let mockBot: jest.Mocked<Telegraf>;

  beforeEach(() => {
    mockBot = {} as any;
    botService = new BotService(mockBot);
  });

  it('should initialize with bot instance', () => {
    expect(botService).toBeDefined();
  });

  it('should return news handler', () => {
    const newsHandler = botService.getNewsHandler();
    expect(newsHandler).toBeDefined();
  });

  it('should handle callback query', async () => {
    const mockCtx = {
      answerCbQuery: jest.fn()
    } as any;

    await botService.handleCallbackQuery(mockCtx);
    expect(mockCtx.answerCbQuery).toHaveBeenCalled();
  });

  it('should handle start command without user id', async () => {
    const mockCtx = {
      from: undefined,
      reply: jest.fn()
    } as any;

    await botService.handleStart(mockCtx);
    expect(mockCtx.reply).not.toHaveBeenCalled();
  });
}); 