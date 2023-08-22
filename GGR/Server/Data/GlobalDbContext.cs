using GGR.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GGR.Server.Data;

public class GlobalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<RewardClaim> RewardClaims { get; set; }
    public DbSet<SaleTicket> SaleTickets { get; set; }
    public DbSet<FileRecord> FileRecords { get; set; }
    public DbSet<SaleRecord> SaleRecords { get; set; }

    public GlobalDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Name).IsRequired();
            builder.Property(user => user.LastName).IsRequired();
            builder.Property(user => user.Email).IsRequired();
            builder.Property(user => user.Points).IsRequired();
            builder.Property(user => user.Phone).IsRequired();
            builder.Property(user => user.PhotoUrl);
            builder.Property(user => user.PasswordHash).IsRequired();
            builder.Property(user => user.PasswordSalt).IsRequired();
            builder.Property(user => user.Rol).IsRequired();
        });

        modelBuilder.Entity<Reward>(builder =>
        {
            builder.HasKey(reward => reward.Id);
            builder.Property(reward => reward.Name).IsRequired();
            builder.Property(reward => reward.Description).IsRequired();
            builder.Property(reward => reward.PricePoints).IsRequired();
            builder.Property(reward => reward.PhotoUrl);
            builder.Property(reward => reward.UnitsAvailable).IsRequired();
            builder.Property(reward => reward.Status).IsRequired();
        });

        modelBuilder.Entity<Registration>(builder =>
        {
            builder.HasKey(registration => registration.Id);
            builder.HasOne(registration => registration.User);
            builder.Property(registration => registration.VerificationPhoneCode);
            builder.Property(registration => registration.VerificationToken);
            builder.Property(registration => registration.RegistrationDate).IsRequired();
            builder.Property(registration => registration.ExpiryTime);
            builder.Property(registration => registration.Status).IsRequired();
            builder.Property(registration => registration.VerifiedAt);
            builder.Property(registration => registration.PasswordResetToken);
            builder.Property(registration => registration.ResetTokenExpires);
        });

        modelBuilder.Entity<RewardClaim>(builder =>
        {
            builder.HasKey(claim => claim.Id);
            builder.HasOne(claim => claim.User);
            builder.HasOne(claim => claim.Reward);
            builder.Property(claim => claim.RewardClaimStatus).IsRequired();
            builder.Property(claim => claim.ClaimCreated).IsRequired();
            builder.Property(claim => claim.ClaimUpdated);
        });

        modelBuilder.Entity<SaleTicket>(builder =>
        {
            builder.HasKey(ticket => ticket.Id);
            builder.HasOne(ticket => ticket.User);
            builder.Property(ticket => ticket.Amount);
            builder.Property(ticket => ticket.Points).IsRequired();
            builder.Property(ticket => ticket.Liters);
            builder.Property(ticket => ticket.Folio).IsRequired();
        });

        modelBuilder.Entity<FileRecord>(builder =>
        {
            builder.HasKey(file => file.Id);
            builder.Property(file => file.FileName).IsRequired();
            builder.Property(file => file.FileStorageName).IsRequired();
            builder.Property(file => file.key).IsRequired();
            builder.Property(file => file.UploadedOn).IsRequired();
        });

        modelBuilder.Entity<SaleRecord>(builder =>
        {
            builder.HasKey(record => record.Id);
            builder.Property(record => record.Folio).IsRequired();
            builder.Property(record => record.Amount).IsRequired();
        });
    }
}
