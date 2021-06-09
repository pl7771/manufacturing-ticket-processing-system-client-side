using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketing_System_TILE03.Models.Domain
{
    public class Bijlage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BijlageId { get; set; }
        public string FileType { get; set; }
        public string Name { get; set; }
        public byte[] DataFiles { get; set; }
    }
}
