using Debezium.Shared;
using Microsoft.EntityFrameworkCore;

namespace Debezium.Producer.Dal;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{

	}

	public DbSet<Product> Products { get; set; }

	public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<Product>(builder =>
		{
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Name)
			    .HasMaxLength(25);
		});

        modelBuilder.Entity<Book>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                .HasMaxLength(10);
        });
    }
}
