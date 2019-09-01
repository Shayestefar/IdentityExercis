using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Models.Entities
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
    }
}
