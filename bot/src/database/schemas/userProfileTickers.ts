import { pgTable, integer, primaryKey } from 'drizzle-orm/pg-core';
import { userProfiles } from './userProfile';
import { tickers } from './ticker';
import { tRaiderSchema } from './schema';

export const userProfileTickers = tRaiderSchema.table(
  'UserProfileTicker',
  {
    userProfileId: integer('UserProfileId')
      .notNull()
      .references(() => userProfiles.id),
    tickerId: integer('TickerId')
      .notNull()
      .references(() => tickers.id),
  },
  (table) => {
    return {
      pk: primaryKey({ columns: [table.userProfileId, table.tickerId] }),
    };
  }
);
