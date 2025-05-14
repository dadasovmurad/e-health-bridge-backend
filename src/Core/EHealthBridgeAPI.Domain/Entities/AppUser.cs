using EHealthBridgeAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
       

        public AppUser() { }

        public AppUser(string username, string email, string passwordHash,
                   string firstName = null, string lastName = null)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            IsActive = true;
        }
    }
}
