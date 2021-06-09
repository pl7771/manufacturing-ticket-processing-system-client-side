using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{
    public static class Extensions
    {
        public static bool ValidateStartDatum(this DateTime date)
        {
            return date >= DateTime.Now.AddDays(-1) && date <= DateTime.Now.AddYears(1);
        }
    }
}
