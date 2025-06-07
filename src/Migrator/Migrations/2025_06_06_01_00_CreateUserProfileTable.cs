using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 06, 06, 01, 00)]
public class CreateUserProfileTable : MigrationBase
{
	public override void Up()
	{
		CreateTableIfDoesNotExist(
			Constants.Schema,
			Constants.UserProfileTable,
			table =>
			{
				table.WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UserProfile").Identity();
				table.WithColumn("TelegramId").AsInt64().NotNullable();
				table.WithColumn("StreamEnabled").AsBoolean().NotNullable();
				table.WithColumn("SummaryEnabled").AsBoolean().NotNullable();
				table.WithColumn("Confidence").AsDouble().NotNullable();
				table.WithColumn("Session").AsString(4000).NotNullable();
			});
	}
}