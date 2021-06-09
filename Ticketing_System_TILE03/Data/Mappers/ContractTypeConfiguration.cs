using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data.Mappers
{
    public class ContractTypeConfiguration : IEntityTypeConfiguration<ContractType>
    {
        public void Configure(EntityTypeBuilder<ContractType> builder)
        {
            builder.ToTable("ContractType");
            builder.HasKey(e => e.ContractTypeId);
            builder.Property(e => e.Naam).IsRequired().HasMaxLength(100);
            builder.Property(e => e.ManierAanmakenTicket).IsRequired();
            builder.Property(e => e.GedekteTijdstippen).IsRequired();
            builder.Property(e => e.MaximaleAfhandeltijd).IsRequired();
            builder.Property(e => e.MinimaleDoorlooptijd).IsRequired();
            builder.Property(e => e.Prijs).IsRequired();

        }
     }
}
