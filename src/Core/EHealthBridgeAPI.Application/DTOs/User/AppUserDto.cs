using EHealthBridgeAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.DTOs.User
{
    public record AppUserDto(string FirstName, string LastName, string Email, string Username, string PasswordHash, bool IsActive = true);
}
