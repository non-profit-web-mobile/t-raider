import { InferInsertModel, InferSelectModel } from 'drizzle-orm';
import { serial, text } from 'drizzle-orm/pg-core';
import { tRaiderSchema } from './schema';

export const tickers = tRaiderSchema.table('Ticker', {
  id: serial('Id').primaryKey(),
  symbol: text('Symbol').notNull(),
}); 

export type Ticker = InferSelectModel<typeof tickers>;
export type InsertTicker = InferInsertModel<typeof tickers>;    