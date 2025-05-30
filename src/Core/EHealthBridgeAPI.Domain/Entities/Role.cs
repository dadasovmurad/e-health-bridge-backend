using System.ComponentModel.DataAnnotations.Schema;
using EHealthBridgeAPI.Domain.Entities.Common;

namespace EHealthBridgeAPI.Domain.Entities
{
    [Table("roles")]
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}