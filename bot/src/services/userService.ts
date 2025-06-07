import { eq } from 'drizzle-orm';
import { getDB } from '../database/connection';
import { users } from '../database/schemas';
import { NewUser, User } from '../types/database';
import { logger } from '../utils/logger';

export class UserService {
  private db = getDB();

  async createUser(userData: NewUser): Promise<User> {
    try {
      const [user] = await this.db.insert(users).values(userData).returning();
      logger.info(`User created: ${user.telegramId}`);
      return user;
    } catch (error) {
      logger.error('Error creating user:', error);
      throw error;
    }
  }

  async getUserByTelegramId(telegramId: number): Promise<User | null> {
    try {
      const [user] = await this.db
        .select()
        .from(users)
        .where(eq(users.telegramId, telegramId))
        .limit(1);
      
      return user || null;
    } catch (error) {
      logger.error('Error getting user by telegram ID:', error);
      throw error;
    }
  }

  async updateUser(telegramId: number, userData: Partial<NewUser>): Promise<User | null> {
    try {
      const [user] = await this.db
        .update(users)
        .set({ ...userData, updatedAt: new Date() })
        .where(eq(users.telegramId, telegramId))
        .returning();
      
      if (user) {
        logger.info(`User updated: ${user.telegramId}`);
      }
      
      return user || null;
    } catch (error) {
      logger.error('Error updating user:', error);
      throw error;
    }
  }

  async getOrCreateUser(telegramId: number, userData: Partial<NewUser>): Promise<User> {
    let user = await this.getUserByTelegramId(telegramId);
    
    if (!user) {
      user = await this.createUser({
        telegramId,
        ...userData,
      });
    }
    
    return user;
  }
} 