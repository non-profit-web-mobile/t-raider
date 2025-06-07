import { getDB } from '../../database/connection';
import { InsertUserProfile, SessionData, UserProfile, userProfiles } from '../../database/schemas/userProfile';
import { logger } from '../../utils/logger';
import { eq } from 'drizzle-orm';

export class UserService {
  private db = getDB();

  async getOrCreateUser(telegramId: number): Promise<UserProfile> {
    try {
      const [existingUser] = await this.db
        .select()
        .from(userProfiles)
        .where(eq(userProfiles.telegramId, telegramId))
        .limit(1);

      if (existingUser) {
        return existingUser;
      }

      const userData: InsertUserProfile = {
        telegramId,
        streamEnabled: false,
        summaryEnabled: false,
        confidence: 0.5,
        session: {}
      };

      const [user] = await this.db.insert(userProfiles).values(userData).returning();
      logger.info(`User created: ${user.telegramId}`);
      return user;
    } catch (error) {
      logger.error('Error getting or creating user:', error);
      throw error;
    }
  }

  async updateUser(telegramId: number, updates: Partial<UserProfile>): Promise<UserProfile> {
    try {
      const [user] = await this.db
        .update(userProfiles)
        .set(updates)
        .where(eq(userProfiles.telegramId, telegramId))
        .returning();
      
      logger.info(`User updated: ${user.telegramId}`);
      return user;
    } catch (error) {
      logger.error('Error updating user:', error);
      throw error;
    }
  }

  async getUser(telegramId: number): Promise<UserProfile | null> {
    try {
      const [user] = await this.db
        .select()
        .from(userProfiles)
        .where(eq(userProfiles.telegramId, telegramId))
        .limit(1);
      
      return user || null;
    } catch (error) {
      logger.error('Error getting user:', error);
      throw error;
    }
  }

  async updateUserSession(telegramId: number, sessionUpdates: Partial<SessionData>): Promise<SessionData> {
    try {
      const user = await this.getUser(telegramId);
      if (!user) {
        throw new Error(`User with telegramId ${telegramId} not found`);
      }

      const updatedSession = { ...user.session, ...sessionUpdates };
      
      const [updatedUser] = await this.db
        .update(userProfiles)
        .set({ session: updatedSession })
        .where(eq(userProfiles.telegramId, telegramId))
        .returning();
      
      logger.info(`User session updated: ${updatedUser.telegramId}`);
      return updatedUser.session;
    } catch (error) {
      logger.error('Error updating user session:', error);
      throw error;
    }
  }

  async getUserSession(telegramId: number): Promise<SessionData | undefined> {
    try {
      const user = await this.getUser(telegramId);

      if (!user) {
        return undefined;
      }

      return user?.session ?? {};
    } catch (error) {
      logger.error('Error getting user session:', error);
      throw error;
    }
  }
} 