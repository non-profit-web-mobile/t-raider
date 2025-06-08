import { TickerHandler } from '../tickerHandler';

jest.mock('../../../../utils/logger');
jest.mock('../../../data/userService');
jest.mock('../../../data/tickerService');

describe('TickerHandler', () => {
  let tickerHandler: TickerHandler;

  beforeEach(() => {
    tickerHandler = new TickerHandler();
  });

  it('should parse ticker input correctly', () => {
    const result = (tickerHandler as any).parseTickerInput('AAPL, GOOGL MSFT,TSLA');
    expect(result).toEqual(['AAPL', 'GOOGL', 'MSFT', 'TSLA']);
  });

  it('should handle empty ticker input', () => {
    const result = (tickerHandler as any).parseTickerInput('');
    expect(result).toEqual([]);
  });

  it('should normalize tickers to uppercase', () => {
    const result = (tickerHandler as any).parseTickerInput('aapl googl');
    expect(result).toEqual(['AAPL', 'GOOGL']);
  });

  it('should filter out empty tickers', () => {
    const result = (tickerHandler as any).parseTickerInput('AAPL,  ,, GOOGL, ');
    expect(result).toEqual(['AAPL', 'GOOGL']);
  });
}); 