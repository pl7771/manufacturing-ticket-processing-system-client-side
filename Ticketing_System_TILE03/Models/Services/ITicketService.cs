using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketing_System_TILE03.Models.Domain;
using Ticketing_System_TILE03.Models.ViewModels;

namespace Ticketing_System_TILE03.Models.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> FindTicketsByStatusAndUserIdAndRole(Status? status, string userId, List<string> userRoles);
        void CreateTicketForUser(ApplicationUser user, TicketCreateViewModel ticketCreateViewModel, List<string> userRoles);
        void UpdateTicket(int ticketId, TicketEditViewModel ticketEditViewModel, ApplicationUser user);
        Ticket FindTicket(int ticketId);
        void DeleteTicket(int ticketId);
    }
}
