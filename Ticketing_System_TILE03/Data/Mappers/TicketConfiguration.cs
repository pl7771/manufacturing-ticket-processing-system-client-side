
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Data.Mappers
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Ticket");
            builder.HasKey(e => e.TicketId);
            builder.Property(e => e.Titel).IsRequired().HasMaxLength(100);
            builder.Property(e => e.DatumAangemaakt).IsRequired();
            builder.Property(e => e.Omschrijving).IsRequired();
        }
    }
}