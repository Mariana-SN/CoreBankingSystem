using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreBankingSystem.Infrastructure.Data.Configurations;

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("transfers");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id");

        builder.Property(t => t.OriginAccountId)
            .HasColumnName("origin_account_id")
            .IsRequired();

        builder.Property(t => t.DestinationAccountId)
            .HasColumnName("destination_account_id")
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(t => t.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.HasOne(t => t.OriginAccount)
            .WithMany()
            .HasForeignKey(t => t.OriginAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.DestinationAccount)
            .WithMany()
            .HasForeignKey(t => t.DestinationAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}