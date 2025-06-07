import { drizzle } from 'drizzle-orm/node-postgres';
import { Pool } from 'pg';
import { config } from '../config/config';
import { logger } from '../utils/logger';

let db: ReturnType<typeof drizzle>;
let pool: Pool;

export async function connectDB() {
  try {
    pool = new Pool({
      host: config.DB_HOST,
      port: config.DB_PORT,
      database: config.DB_NAME,
      user: config.DB_USER,
      password: config.DB_PASSWORD,
    });

    // Тестируем подключение
    await pool.query('SELECT 1');
    
    db = drizzle(pool);
    logger.info('Database connection established');
    
    return db;
  } catch (error) {
    logger.error('Failed to connect to database:', error);
    throw error;
  }
}

export function getDB() {
  if (!db) {
    throw new Error('Database not connected. Call connectDB() first.');
  }
  return db;
}

export async function closeDB() {
  if (pool) {
    await pool.end();
    logger.info('Database connection closed');
  }
} 