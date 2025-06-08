import { NewsHandler } from '../newsHandler';
import { BroadcastService } from '../../broadcastService';
import { NewsEvent } from '../../../../types/news';

jest.mock('../../../../utils/logger');

describe('NewsHandler', () => {
  let newsHandler: NewsHandler;
  let mockBroadcastService: jest.Mocked<BroadcastService>;

  beforeEach(() => {
    mockBroadcastService = {
      broadcastMessage: jest.fn()
    } as any;
    newsHandler = new NewsHandler(mockBroadcastService);
  });

  it('should handle news event successfully', async () => {
    const newsEvent: NewsEvent = {
      telegramIds: [123, 456],
      message: 'Test news message',
      buttons: [{ text: 'Read More', url: 'https://example.com' }]
    };

    const broadcastResult = { successful: 2, errors: 0, totalRecipients: 2 };
    mockBroadcastService.broadcastMessage.mockResolvedValue(broadcastResult);

    await newsHandler.handleNewsEvent(newsEvent);
    expect(mockBroadcastService.broadcastMessage).toHaveBeenCalledWith(
      newsEvent.telegramIds,
      newsEvent.message,
      newsEvent.buttons
    );
  });

  it('should handle broadcast errors', async () => {
    const newsEvent: NewsEvent = {
      telegramIds: [123],
      message: 'Test message',
      buttons: []
    };

    mockBroadcastService.broadcastMessage.mockRejectedValue(new Error('Broadcast failed'));

    await expect(newsHandler.handleNewsEvent(newsEvent)).rejects.toThrow('Broadcast failed');
  });

  it('should call broadcast service with correct parameters', async () => {
    const newsEvent: NewsEvent = {
      telegramIds: [789],
      message: 'Another test',
      buttons: []
    };

    mockBroadcastService.broadcastMessage.mockResolvedValue({ successful: 1, errors: 0, totalRecipients: 1 });

    await newsHandler.handleNewsEvent(newsEvent);
    expect(mockBroadcastService.broadcastMessage).toHaveBeenCalledTimes(1);
  });

  it('should process empty buttons array', async () => {
    const newsEvent: NewsEvent = {
      telegramIds: [100],
      message: 'No buttons',
      buttons: []
    };

    mockBroadcastService.broadcastMessage.mockResolvedValue({ successful: 1, errors: 0, totalRecipients: 1 });

    await newsHandler.handleNewsEvent(newsEvent);
    expect(mockBroadcastService.broadcastMessage).toHaveBeenCalledWith([100], 'No buttons', []);
  });
}); 