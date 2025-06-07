using Microsoft.EntityFrameworkCore;
using Model.Domain;

namespace Model.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("t-raider");

		modelBuilder.Entity<UserProfile>().ToTable(nameof(UserProfile));

		modelBuilder.Entity<Ticker>().ToTable(nameof(Ticker));

		modelBuilder.Entity<UserProfile>()
			.HasMany(x => x.Tickers)
			.WithMany(x => x.UserProfiles)
			.UsingEntity(
				"UserProfileTicker",
				x => x
					.HasOne(typeof(Ticker))
					.WithMany()
					.HasForeignKey("TickerId")
					.HasPrincipalKey(nameof(Ticker.Id)),
				x => x
					.HasOne(typeof(UserProfile))
					.WithMany()
					.HasForeignKey("UserProfileId")
					.HasPrincipalKey(nameof(UserProfile.Id)),
				x => x
					.HasKey("UserProfileId", "TickerId"));
	}
}