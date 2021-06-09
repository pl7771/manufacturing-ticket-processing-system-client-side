using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketing_System_TILE03.Models.Domain
{
    public class LoginAttempt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string LoginAttemptId { get; set; }
        public DateTime AttemptTimeStamp { get; set; }
        public string UserName { get; set; }
        public bool Succeeded { get; set; }

        public LoginAttempt(string userName)
        {
            UserName = userName;
            AttemptTimeStamp = DateTime.Now;
        }

        public void LoginSucces()
        {
            Succeeded = true;
        }

        public void LoginFail()
        {
            Succeeded = false;
        }
    }
}
