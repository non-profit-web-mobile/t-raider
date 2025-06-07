using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 06, 06, 01, 00)]
public class CreateItemsTable : MigrationBase
{
	public override void Up()
	{
		CreateTableIfDoesNotExist(
			"t-raider",
			"Items",
			table =>
			{
				table.WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Items").Identity();
				table.WithColumn("Value").AsString().NotNullable();
			});
	}
}