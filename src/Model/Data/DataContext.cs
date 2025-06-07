using Microsoft.EntityFrameworkCore;
using Model.Domain;

namespace Model.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("t-raider");
		modelBuilder.Entity<Item>().ToTable("Items");
	}
}