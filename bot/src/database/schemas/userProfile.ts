import { InferInsertModel, InferSelectModel } from 'drizzle-orm';
import { serial, bigint, boolean, doublePrecision, pgSchema, text, customType } from 'drizzle-orm/pg-core';
import { tRaiderSchema } from './schema';

export interface SessionData {
  step?: string;
  selectedSources?: string;
  selectedRiskProfile?: string;
  awaitingTickers?: boolean;
}

const jsonType = <T>() => customType<{ data: T; driverData: string }>({
  dataType() {
    return 'text';
  },
  toDriver(value: T): string {
    return JSON.stringify(value);
  },
  fromDriver(value: string): T {
    return JSON.parse(value);
  },
});

export const userProfiles = tRaiderSchema.table('UserProfile', {
  id: serial('Id').primaryKey(),
  telegramId: bigint('TelegramId', { mode: 'number' }).notNull().unique(),
  streamEnabled: boolean('StreamEnabled').notNull(),
  summaryEnabled: boolean('SummaryEnabled').notNull(),
  confidence: doublePrecision('Confidence').notNull(),
  session: jsonType<SessionData>()('Session').notNull(),
});

export type UserProfile = InferSelectModel<typeof userProfiles>;
export type InsertUserProfile = InferInsertModel<typeof userProfiles>;