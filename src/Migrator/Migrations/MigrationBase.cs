using FluentMigrator;
using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Builders.Create.Constraint;
using FluentMigrator.Builders.Create.ForeignKey;
using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Builders.Create.Table;

namespace Migrator.Migrations;

public abstract class MigrationBase : Migration
{
	public override void Down()
	{
		throw new NotSupportedException();
	}

	protected void CreateColumnIfDoesNotExist(
		string schema,
		string tableName,
		string columnName,
		Action<ICreateColumnAsTypeSyntax> configure)
	{
		if (Schema.Schema(schema).Table(tableName).Column(columnName).Exists())
			return;

		var table = Create.Column(columnName).OnTable(tableName).InSchema(schema);
		configure(table);
	}

	protected void CreateTableIfDoesNotExist(
		string schema,
		string tableName,
		Action<ICreateTableWithColumnSyntax> configure)
	{
		if (Schema.Schema(schema).Table(tableName).Exists())
			return;

		var table = Create.Table(tableName).InSchema(schema);
		configure(table);
	}

	protected void CreateIndexIfDoesNotExist(
		string schemaName,
		string tableName,
		string indexName,
		Action<ICreateIndexOnColumnSyntax> configure)
	{
		if (Schema.Schema(schemaName).Table(tableName).Index(indexName).Exists())
			return;

		var index = Create.Index(indexName).OnTable(tableName).InSchema(schemaName);
		configure(index);
	}

	protected void CreateUniqueConstraintIfDoesNotExist(
		string schemaName,
		string tableName,
		string indexName,
		Action<ICreateConstraintColumnsSyntax> configure)
	{
		if (Schema.Schema(schemaName).Table(tableName).Constraint(indexName).Exists())
			return;

		var index = Create.UniqueConstraint(indexName).OnTable(tableName).WithSchema(schemaName);
		configure(index);
	}

	protected void CreateForeignKeyIfDoesNotExist(
		string schema,
		string tableName,
		string constraintName,
		Action<ICreateForeignKeyFromTableSyntax> configure)
	{
		if (Schema.Schema(schema).Table(tableName).Constraint(constraintName).Exists())
			return;

		var constraint = Create.ForeignKey(constraintName);
		configure(constraint);
	}
}