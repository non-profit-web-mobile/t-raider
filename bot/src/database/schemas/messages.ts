import { pgTable, serial, text, timestamp, bigint } from 'drizzle-orm/pg-core';

export const messages = pgTable('messages', {
  id: serial('id').primaryKey(),
  telegramId: bigint('telegram_id', { mode: 'number' }).notNull(),
  userId: bigint('user_id', { mode: 'number' }).notNull(),
  messageText: text('message_text'),
  messageType: text('message_type').default('text'),
  createdAt: timestamp('created_at').defaultNow(),
}); 