using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Models.ViewModels
{
    public class ContractViewModel
    {
        [Required (ErrorMessage ="Dit veld is verplicht")]
        public int Days { get; set; }
        [Required(ErrorMessage = "Dit veld is verplicht")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Dit veld is verplicht")]
        [Display(Name ="Kies een startdatum:")]
        [DataType(DataType.Date)]
        public DateTime StartDatum { get; set; }

        public ContractViewModel()
        {
            StartDatum = DateTime.Now;
        }

        public ContractViewModel(Contract contract)
        {
            

        }
    }
}
