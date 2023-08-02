using GGR.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace GGR.Server.Data;

public class GlobalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<RewardClaim> RewardClaims { get; set; }
    public DbSet<SaleTicket> SaleTickets { get; set; }

    public GlobalDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuiler)
    {
        modelBuiler.Entity<User>(builder =>
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

        modelBuiler.Entity<Reward>(builder =>
        {
            builder.HasKey(reward => reward.Id);
            builder.Property(reward => reward.Name).IsRequired();
            builder.Property(reward => reward.Description).IsRequired();
            builder.Property(reward => reward.PricePoints).IsRequired();
            builder.Property(reward => reward.PhotoUrl);
            builder.Property(reward => reward.UnitsAvailable).IsRequired();
            builder.Property(reward => reward.Status).IsRequired();
        });

        modelBuiler.Entity<Registration>(builder =>
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

        modelBuiler.Entity<RewardClaim>(builder =>
        {
            builder.HasKey(claim => claim.Id);
            builder.HasOne(claim => claim.User);
            builder.HasOne(claim => claim.Reward);
            builder.Property(claim => claim.RewardClaimStatus).IsRequired();
            builder.Property(claim => claim.ClaimCreated).IsRequired();
            builder.Property(claim => claim.ClaimUpdated);
        });

        modelBuiler.Entity<SaleTicket>(builder =>
        {
            builder.HasKey(ticket => ticket.Id);
            builder.HasOne(ticket => ticket.User);
            builder.Property(ticket => ticket.Amount);
            builder.Property(ticket => ticket.Points).IsRequired();
            builder.Property(ticket => ticket.Liters);
        });
    }
}
