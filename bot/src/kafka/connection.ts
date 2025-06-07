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

    producer = kafka.producer();
    consumer = kafka.consumer({ groupId: `${config.KAFKA_CLIENT_ID}-group` });

    await producer.connect();
    await consumer.connect();

    logger.info('Kafka connection established');
    
    return { producer, consumer };
  } catch (error) {
    logger.error('Failed to connect to Kafka:', error);
    throw error;
  }
}

export function getKafkaProducer() {
  if (!producer) {
    throw new Error('Kafka producer not connected. Call connectKafka() first.');
  }
  return producer;
}

export function getKafkaConsumer() {
  if (!consumer) {
    throw new Error('Kafka consumer not connected. Call connectKafka() first.');
  }
  return consumer;
}

export async function sendMessage(topic: string, message: any) {
  try {
    const producer = getKafkaProducer();
    await producer.send({
      topic,
      messages: [
        {
          value: JSON.stringify(message),
          timestamp: new Date().toISOString(),
        },
      ],
    });
    logger.info(`Message sent to topic ${topic}`);
  } catch (error) {
    logger.error(`Failed to send message to topic ${topic}:`, error);
    throw error;
  }
}

export async function closeKafka() {
  try {
    if (producer) {
      await producer.disconnect();
    }
    if (consumer) {
      await consumer.disconnect();
    }
    logger.info('Kafka connections closed');
  } catch (error) {
    logger.error('Error closing Kafka connections:', error);
  }
} 