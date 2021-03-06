using MIE.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MIE
{
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<Blog> Blog { get; set; }

        public virtual DbSet<AvailableTime> AvailableTime { get; set; }

        public virtual DbSet<Quiz> Quiz { get; set; }

        public virtual DbSet<Reservation> Reservation { get; set; }

        public virtual DbSet<Submission> Submission { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
                string conn = config.GetConnectionString("MySqlConnection");
                optionsBuilder.UseMySQL(conn);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Submission>()
                .Property(e => e.JudgeResult)
                .HasColumnType("nvarchar(48)");
        }
    }
}
