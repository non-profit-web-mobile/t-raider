import { logger } from '../utils/logger';

describe('Example Test', () => {
  it('should pass', () => {
    expect(true).toBe(true);
  });

  it('should have logger', () => {
    expect(logger).toBeDefined();
    expect(typeof logger.info).toBe('function');
  });

  it('should perform basic math', () => {
    const sum = 2 + 2;
    expect(sum).toBe(4);
  });
}); 