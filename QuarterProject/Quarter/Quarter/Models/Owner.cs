using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quarter.Models
{
    public class Owner
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Fullname { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SharePercent { get; set; }
        [MaxLength(100)]
        public string ImageUrl { get; set; }
        [MaxLength(500)]
        public string Desc { get; set; }

        public List<House>? Houses { get; set; }

    }
}
