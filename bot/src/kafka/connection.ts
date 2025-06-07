import { Kafka, Producer, Consumer } from 'kafkajs';
import { config } from '../config/config';
import { logger } from '../utils/logger';

let kafka: Kafka;
let producer: Producer;
let consumer: Consumer;

export async function connectKafka() {
  try {
    kafka = new Kafka({
      clientId: config.KAFKA_CLIENT_ID,
      brokers: config.KAFKA_BROKERS,
    });

    consumer = kafka.consumer({ groupId: `${config.KAFKA_CLIENT_ID}-group` });

    await consumer.connect();

    logger.info('Kafka connection established');
    
    return { consumer };
  } catch (error) {
    logger.error('Failed to connect to Kafka:', error);
    throw error;
  }
}

export function getKafkaConsumer() {
  if (!consumer) {
    throw new Error('Kafka consumer not connected. Call connectKafka() first.');
  }
  return consumer;
}

export async function closeKafka() {
  try {
    if (consumer) {
      await consumer.disconnect();
    }
    logger.info('Kafka connections closed');
  } catch (error) {
    logger.error('Error closing Kafka connections:', error);
  }
} 