using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models.ViewModels
{
    public class TicketCreateViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} may not contain more than 100 characters")]
        public string Titel { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string Omschrijving { get; set; }

        [StringLength(int.MaxValue)]
        public string Opmerkingen { get; set; }
        
        [Editable(false)]
        [Display(Name = "Datum aangemaakt")]
        [DataType(DataType.Date)]
        public DateTime DatumAangemaakt { get; set; }
        public string Company { get; set; }
        [Required]
        public string Contract { get; set; }

        //bijlage
        public string ImageDescription { set; get; }
        public IFormFile MyFile { set; get; }




        // deze is om een nieuwe aan te maken
        public TicketCreateViewModel() 
        {
            DatumAangemaakt = DateTime.Now;
        }




        //deze is om een ticket aan te passen, door klant, indien we deze functionaliteit voorzien
        public TicketCreateViewModel(Ticket ticket)
        {
            Titel = ticket.Titel;
            Omschrijving = ticket.Omschrijving;
            Opmerkingen = ticket.Opmerkingen;
            DatumAangemaakt = ticket.DatumAangemaakt;
            ImageDescription = ticket.ImageDescription;
        }
    }
}
