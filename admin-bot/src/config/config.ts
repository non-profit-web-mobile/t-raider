import dotenv from 'dotenv';

dotenv.config();

export const config = {
  BOT_TOKEN: process.env.BOT_TOKEN || '',
  ADMIN_IDS: process.env.ADMIN_IDS?.split(',') || [],
  
  // Kafka
  KAFKA_BROKERS: process.env.KAFKA_BROKERS?.split(',') || ['localhost:9092'],
  KAFKA_CLIENT_ID: process.env.KAFKA_CLIENT_ID || 'telegram-bot',
  KAFKA_ADMIN_SIGNALS_TOPIC: process.env.KAFKA_ADMIN_SIGNALS_TOPIC || 'AdminSignals',
  
  // Logging
  LOG_LEVEL: process.env.LOG_LEVEL || 'info',
}; 