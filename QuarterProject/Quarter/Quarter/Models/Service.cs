using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class Service
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Desc { get; set; }

        [MaxLength(100)]
        public string? Icon { get; set; }
        public bool IsFeatured { get; set; }
    }
}
