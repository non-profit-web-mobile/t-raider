import { Telegraf } from 'telegraf';
import { getKafkaConsumer } from './connection';
import { config } from '../config/config';
import { logger } from '../utils/logger';

interface AdminSignalMessage {
  message: string;
}

const MAX_MESSAGE_LENGTH = 4096;

function formatMessageWithMarkup(message: string): string {
  let formattedMessage = message
    .replace(/```json\r?\n([\s\S]*?)\r?\n```/g, (match, jsonContent) => {
      try {
        const parsed = JSON.parse(jsonContent);
        const prettyJson = JSON.stringify(parsed, null, 2);
        return `<pre><code class="language-json">${escapeHtml(prettyJson)}</code></pre>`;
      } catch (error) {
        return `<pre><code>${escapeHtml(jsonContent)}</code></pre>`;
      }
    })
    .replace(/```([\s\S]*?)```/g, '<pre><code>$1</code></pre>')
    .replace(/`([^`]+)`/g, '<code>$1</code>')
    .replace(/\*\*([^*]+)\*\*/g, '<b>$1</b>')
    .replace(/\*([^*]+)\*/g, '<i>$1</i>')
    .replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>')
    .replace(/\r?\n/g, '\n');

  return formattedMessage;
}

async function sendSafeMessage(bot: Telegraf, chatId: string, message: string): Promise<void> {
  try {
    const formattedMessage = formatMessageWithMarkup(message);
    
    if (formattedMessage.length > MAX_MESSAGE_LENGTH) {
      const chunks = splitLongMessage(formattedMessage);
      
      for (let i = 0; i < chunks.length; i++) {
        const chunk = chunks[i];
        const chunkPrefix = chunks.length > 1 ? `<b>Часть ${i + 1}/${chunks.length}:</b>\n\n` : '';
        
        try {
                     await bot.telegram.sendMessage(chatId, chunkPrefix + chunk, {
             parse_mode: 'HTML',
           });
          
          if (i < chunks.length - 1) {
            await new Promise(resolve => setTimeout(resolve, 100));
          }
        } catch (htmlError: any) {
          logger.warn(`HTML parsing failed for chunk ${i + 1}, trying plain text:`, htmlError.message);
          await bot.telegram.sendMessage(chatId, chunkPrefix + stripHtmlTags(chunk));
        }
      }
      return;
    }

    try {
      await bot.telegram.sendMessage(chatId, formattedMessage, {
        parse_mode: 'HTML',
      });
    } catch (htmlError: any) {
      logger.warn(`HTML parsing failed for admin ${chatId}, trying plain text:`, htmlError.message);
      await bot.telegram.sendMessage(chatId, stripHtmlTags(formattedMessage));
     }
  } catch (error) {
    logger.error(`All send methods failed for admin ${chatId}:`, error);
    throw error;
  }
}

function splitLongMessage(message: string): string[] {
  const chunks: string[] = [];
  let currentChunk = '';
  
  const paragraphs = message.split('\n\n');
  
  for (const paragraph of paragraphs) {
    const paragraphWithBreak = paragraph + '\n\n';
    
    if (currentChunk.length + paragraphWithBreak.length > MAX_MESSAGE_LENGTH) {
      if (currentChunk) {
        chunks.push(currentChunk.trim());
        currentChunk = '';
      }
      
      if (paragraphWithBreak.length > MAX_MESSAGE_LENGTH) {
        const lines = paragraph.split('\n');
        let currentLine = '';
        
        for (const line of lines) {
          if (currentLine.length + line.length + 1 > MAX_MESSAGE_LENGTH) {
            if (currentLine) {
              chunks.push(currentLine.trim());
              currentLine = '';
            }
            
            if (line.length > MAX_MESSAGE_LENGTH) {
              const lineChunks = line.match(new RegExp(`.{1,${MAX_MESSAGE_LENGTH - 100}}`, 'g')) || [];
              chunks.push(...lineChunks);
            } else {
              currentLine = line;
            }
          } else {
            currentLine += (currentLine ? '\n' : '') + line;
          }
        }
        
        if (currentLine) {
          currentChunk = currentLine + '\n\n';
        }
      } else {
        currentChunk = paragraphWithBreak;
      }
    } else {
      currentChunk += paragraphWithBreak;
    }
  }
  
  if (currentChunk) {
    chunks.push(currentChunk.trim());
  }
  
  return chunks.filter(chunk => chunk.length > 0);
}

// Функция для экранирования HTML символов
function escapeHtml(text: string): string {
  return text
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
}

// Функция для удаления HTML тегов (fallback для plain text)
function stripHtmlTags(text: string): string {
  return text
    .replace(/<[^>]*>/g, '')
    .replace(/&amp;/g, '&')
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'");
}

export async function startAdminSignalsConsumer(bot: Telegraf) {
  try {
    const consumer = getKafkaConsumer();

    await consumer.subscribe({ topic: config.KAFKA_ADMIN_SIGNALS_TOPIC });

    await consumer.run({
      eachMessage: async ({ message }) => {
        try {
          if (message.value) {
            const messageValue = message.value.toString();
            const adminSignal: AdminSignalMessage = JSON.parse(messageValue);

            logger.info(`Received admin signal from Kafka: ${messageValue.substring(0, 200)}...`);

            for (const adminId of config.ADMIN_IDS) {
              try {
                await sendSafeMessage(bot, adminId, adminSignal.message);
                logger.info(`Admin signal sent to admin ${adminId}`);
              } catch (error) {
                logger.error(
                  `Failed to send admin signal to admin ${adminId}:`,
                  error
                );
              }
            }
          }
        } catch (error) {
          logger.error(
            `Error processing admin signal message from Kafka:`,
            error
          );
        }
      },
    });

    logger.info('AdminSignals consumer started successfully');
  } catch (error) {
    logger.error('Failed to start AdminSignals consumer:', error);
    throw error;
  }
}
