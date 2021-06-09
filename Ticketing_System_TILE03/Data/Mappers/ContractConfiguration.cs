using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data.Mappers
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contract");
            builder.HasKey(e => e.ContractId);
            builder.HasOne(e => e.Type)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.StartDatum).IsRequired();
            builder.Property(e => e.EindDatum).IsRequired();

        }
    }
}
