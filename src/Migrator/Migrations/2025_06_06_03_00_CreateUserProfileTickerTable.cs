using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 06, 06, 03, 00)]
public class CreateUserProfilesTickers : MigrationBase
{
	public override void Up()
	{
		CreateTableIfDoesNotExist(
			Constants.Schema,
			Constants.UserProfileTickerTable,
			table =>
			{
				table.WithColumn("UserProfileId").AsInt32().NotNullable();
				table.WithColumn("TickerId").AsInt32().NotNullable();
			});

		CreateForeignKeyIfDoesNotExist(
			Constants.Schema,
			Constants.UserProfileTickerTable,
			$"FK_{Constants.UserProfileTickerTable}_UserProfileId",
			constraint => constraint
				.FromTable(Constants.UserProfileTickerTable).InSchema(Constants.Schema).ForeignColumn("UserProfileId")
				.ToTable(Constants.UserProfileTable).InSchema(Constants.Schema).PrimaryColumn("Id"));
        
		CreateIndexIfDoesNotExist(
			Constants.Schema,
			Constants.UserProfileTickerTable,
			$"IX_{Constants.UserProfileTickerTable}_UserProfileId",
			index => index
				.OnColumn("UserProfileId")
				.Ascending()
				.WithOptions()
				.NonClustered());

		CreateForeignKeyIfDoesNotExist(
			Constants.Schema,
			Constants.UserProfileTickerTable,
			$"FK_{Constants.UserProfileTickerTable}_TickerId",
			constraint => constraint
				.FromTable(Constants.UserProfileTickerTable).InSchema(Constants.Schema).ForeignColumn("TickerId")
				.ToTable(Constants.TickerTable).InSchema(Constants.Schema).PrimaryColumn("Id"));
        
		CreateIndexIfDoesNotExist(
			Constants.Schema,
			Constants.UserProfileTickerTable,
			$"IX_{Constants.UserProfileTickerTable}_TickerId",
			index => index
				.OnColumn("TickerId")
				.Ascending()
				.WithOptions()
				.NonClustered());
	}
}