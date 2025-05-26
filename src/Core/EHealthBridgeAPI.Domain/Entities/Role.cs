using System.ComponentModel.DataAnnotations.Schema;
using EHealthBridgeAPI.Domain.Entities.Common;

namespace EHealthBridgeAPI.Domain.Entities
{
    [Table("roles")]
    public class Role : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}