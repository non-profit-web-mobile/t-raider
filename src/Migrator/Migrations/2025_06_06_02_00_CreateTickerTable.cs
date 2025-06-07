using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 06, 06, 02, 00)]
public class CreateTickerTable : MigrationBase
{
	public override void Up()
	{
		CreateTableIfDoesNotExist(
			Constants.Schema,
			Constants.TickerTable,
			table =>
			{
				table.WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Ticker").Identity();
				table.WithColumn("Symbol").AsString(256).NotNullable();
			});
	}
}