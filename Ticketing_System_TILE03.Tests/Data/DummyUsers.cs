using System;
using System.Collections.Generic;
using System.Text;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;

namespace Ticketing_System_TILE03.Tests.Data
{
    public class DummyUsers
    {
        public IList<ApplicationUser> ApplicationUsers {get;}

        public DummyUsers()
        {
            ApplicationUsers = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "123",
                UserName = "client@gmail.com",
                Email = "client@gmail.com",
                FirstName = "Client",
                LastName = "Ient",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LastSession = new DateTime(2020, 11, 1, 15, 30, 30)
            },
            new ApplicationUser
            {
                UserName = "technician@gmail.com",
                Email = "technician@gmail.com",
                FirstName = "Technician",
                LastName = "Nician",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LastSession = new DateTime(2021, 01, 1, 1, 1, 1)
            },
            new ApplicationUser
            {
                UserName = "supportmanager@gmail.com",
                Email = "supportmanager@gmail.com",
                FirstName = "SupportManager",
                LastName = "Manager",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LastSession = new DateTime(2021, 01, 1, 1, 1, 1)
            },
            new ApplicationUser
            {
                UserName = "administrator@gmail.com",
                Email = "administrator@gmail.com",
                FirstName = "Administrator",
                LastName = "Istrator",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            }

    };
            
        }
    }
}
