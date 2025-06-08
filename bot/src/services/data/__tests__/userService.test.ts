import { UserService } from '../userService';

jest.mock('../../../database/connection');
jest.mock('../../../utils/logger');

describe('UserService', () => {
  let userService: UserService;
  let mockDb: any;

  beforeEach(() => {
    mockDb = {
      select: jest.fn().mockReturnThis(),
      from: jest.fn().mockReturnThis(),
      where: jest.fn().mockReturnThis(),
      limit: jest.fn().mockResolvedValue([]),



      insert: jest.fn().mockReturnThis(),
      values: jest.fn().mockReturnThis(),
      returning: jest.fn().mockReturnThis(),
      update: jest.fn().mockReturnThis(),
      set: jest.fn().mockReturnThis()
    };
    
    require('../../../database/connection').getDB = jest.fn().mockReturnValue(mockDb);
    userService = new UserService();
  });

  it('should return existing user when found', async () => {
    const existingUser = { id: 1, telegramId: 123, session: {} };
    mockDb.limit.mockResolvedValue([existingUser]);

    const result = await userService.getOrCreateUser(123);
    expect(result).toEqual(existingUser);
  });

  it('should create new user when not found', async () => {
    const newUser = { id: 1, telegramId: 123, streamEnabled: false, summaryEnabled: false, confidence: 0.5, session: {} };
    mockDb.limit.mockResolvedValueOnce([]);
    mockDb.returning.mockResolvedValue([newUser]);

    const result = await userService.getOrCreateUser(123);
    expect(result).toEqual(newUser);
  });

  it('should return null for non-existent user', async () => {
    mockDb.limit.mockResolvedValue([]);

    const result = await userService.getUser(123);
    expect(result).toBeNull();
  });

  it('should throw error when updating session for non-existent user', async () => {
    userService.getUser = jest.fn().mockResolvedValue(null);

    await expect(userService.updateUserSession(123, { step: 'setup' }))
      .rejects.toThrow('User with telegramId 123 not found');
  });
}); 