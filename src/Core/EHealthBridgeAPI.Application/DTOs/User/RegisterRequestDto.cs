using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.DTOs
{
    public record RegisterRequestDto(string FirstName, string LastName, string Email, string UserName, string Password);
}