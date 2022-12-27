using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string? Key { get; set; }
        [MaxLength(250)]
        public string? Value { get; set; }
    }
}
