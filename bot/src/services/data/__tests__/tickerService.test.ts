import { TickerService } from '../tickerService';

jest.mock('../../../database/connection');
jest.mock('../../../utils/logger');

describe('TickerService', () => {
  let tickerService: TickerService;
  let mockDb: any;

  beforeEach(() => {
    mockDb = {
      select: jest.fn().mockReturnThis(),
      from: jest.fn().mockReturnThis(),
      where: jest.fn().mockReturnThis(),
      limit: jest.fn().mockReturnThis(),
      insert: jest.fn().mockReturnThis(),
      values: jest.fn().mockReturnThis(),
      returning: jest.fn().mockReturnThis(),
      orderBy: jest.fn().mockReturnThis()
    };
    
    require('../../../database/connection').getDB = jest.fn().mockReturnValue(mockDb);
    tickerService = new TickerService();
  });

  it('should validate empty ticker array', async () => {
    const result = await tickerService.validateTickers([]);
    expect(result.valid).toEqual([]);
    expect(result.invalid).toEqual([]);
  });

  it('should normalize ticker symbols to uppercase', async () => {
    mockDb.select.mockReturnValue(mockDb);
    mockDb.from.mockReturnValue(mockDb);
    mockDb.where.mockResolvedValue([{ symbol: 'AAPL' }, { symbol: 'GOOGL' }]);

    const result = await tickerService.validateTickers(['aapl', 'googl']);
    expect(result.valid).toEqual(['AAPL', 'GOOGL']);
  });

  it('should return random portfolio tickers with correct count', async () => {
    tickerService.getAvailableTickers = jest.fn().mockResolvedValue(['AAPL', 'GOOGL', 'MSFT', 'TSLA']);
    
    const result = await tickerService.getRandomPortfolioTickers(2);
    expect(result).toHaveLength(2);
  });

  it('should handle empty available tickers for portfolio', async () => {
    tickerService.getAvailableTickers = jest.fn().mockResolvedValue([]);
    
    const result = await tickerService.getRandomPortfolioTickers(5);
    expect(result).toEqual([]);
  });
}); 