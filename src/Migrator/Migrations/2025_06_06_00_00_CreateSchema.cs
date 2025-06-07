using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 06, 06, 00, 00)]
public class CreateSchema : MigrationBase
{
	public override void Up()
	{
		Create.Schema(Constants.Schema);
	}
}