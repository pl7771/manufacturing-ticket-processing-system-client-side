using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models.ViewModels
{
    public class TicketEditViewModel
    {
        public int TicketId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0} may not contain more than 100 characters")]
        public string Titel { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string Omschrijving { get; set; }

        [StringLength(int.MaxValue)]
        public string Opmerkingen { get; set; }

        public Status Status { get; set; }


        //bijlage
        [Display(Name ="Toelichting bijlage")]
        public string ImageDescription { set; get; }

        public IFormFile MyFile { set; get; }
        public Bijlage Bijlage { get; set; }


        public TicketEditViewModel()
        {
            
        }


        //deze is om een ticket aan te passen, door klant, indien we deze functionaliteit voorzien
        public TicketEditViewModel(Ticket ticket) : this()
        {
            TicketId = ticket.TicketId;
            Omschrijving = ticket.Omschrijving;
            Titel = ticket.Titel;
            Status = ticket.Status;
            Bijlage = ticket.Bijlage;
            ImageDescription = ticket.ImageDescription;
        }


    }
}
