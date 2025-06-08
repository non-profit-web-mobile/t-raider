import { NewsConsumer } from '../newsConsumer';
import { NewsHandler } from '../../services/bot/handlers/newsHandler';

jest.mock('../connection');
jest.mock('../../utils/logger');

describe('NewsConsumer', () => {
  let newsConsumer: NewsConsumer;
  let mockNewsHandler: jest.Mocked<NewsHandler>;
  let mockConsumer: any;

  beforeEach(() => {
    mockConsumer = {
      subscribe: jest.fn(),
      run: jest.fn(),
      disconnect: jest.fn()
    };

    require('../connection').getKafkaConsumer = jest.fn().mockReturnValue(mockConsumer);

    mockNewsHandler = {
      handleNewsEvent: jest.fn()
    } as any;

    newsConsumer = new NewsConsumer(mockNewsHandler);
  });

  it('should initialize with news handler', () => {
    expect(newsConsumer).toBeDefined();
  });

  it('should start consuming successfully', async () => {
    mockConsumer.subscribe.mockResolvedValue(undefined);
    mockConsumer.run.mockResolvedValue(undefined);

    await newsConsumer.startConsuming();
    expect(mockConsumer.subscribe).toHaveBeenCalled();
    expect(mockConsumer.run).toHaveBeenCalled();
  });

  it('should stop consuming successfully', async () => {
    mockConsumer.disconnect.mockResolvedValue(undefined);

    await newsConsumer.stopConsuming();
    expect(mockConsumer.disconnect).toHaveBeenCalled();
  });

  it('should handle disconnect errors gracefully', async () => {
    mockConsumer.disconnect.mockRejectedValue(new Error('Disconnect failed'));

    await expect(newsConsumer.stopConsuming()).resolves.not.toThrow();
  });
}); 