import { listAllTickers } from '../listTickers';

jest.mock('../../database/connection');
jest.mock('../../utils/logger');

describe('listAllTickers', () => {
  let mockDb: any;

  beforeEach(() => {
    mockDb = {
      select: jest.fn().mockReturnThis(),
      from: jest.fn().mockResolvedValue([])
    };

    require('../../database/connection').connectDB = jest.fn();
    require('../../database/connection').getDB = jest.fn().mockReturnValue(mockDb);
    require('../../database/connection').closeDB = jest.fn();
  });

  it('should fetch tickers from database', async () => {
    const mockTickers = [
      { id: 1, symbol: 'AAPL' },
      { id: 2, symbol: 'GOOGL' }
    ];
    mockDb.from.mockResolvedValue(mockTickers);

    await listAllTickers();
    expect(mockDb.select).toHaveBeenCalled();
    expect(mockDb.from).toHaveBeenCalled();
  });

  it('should handle empty ticker list', async () => {
    mockDb.from.mockResolvedValue([]);

    await listAllTickers();
    expect(mockDb.select).toHaveBeenCalled();
  });

  it('should close database connection', async () => {
    const closeDB = require('../../database/connection').closeDB;
    
    await listAllTickers();
    expect(closeDB).toHaveBeenCalled();
  });

  it('should close database even on error', async () => {
    const closeDB = require('../../database/connection').closeDB;
    mockDb.from.mockRejectedValue(new Error('Database error'));

    await expect(listAllTickers()).rejects.toThrow('Database error');
    expect(closeDB).toHaveBeenCalled();
  });
}); 