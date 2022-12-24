using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Quarter.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quarter.Models
{
    public class HomeSlider
    {
        public int Id { get; set; }
        [MaxLength(150)]
        public string Title { get; set; }
        [MaxLength(250)]
        public string Desc { get; set; }
        [MaxLength(100)]
        public string? ImageUrl { get; set; }
        public int Order { get; set; }
        [MaxLength(50)]
        public string Btn1Text { get; set; }
        [MaxLength(50)]
        public string Btn2Text { get; set; }

        [MaxLength(100)]
        public string Btn1Url { get; set; }
        [MaxLength(100)]
        public string Btn2Url { get; set; }
        [NotMapped]
        [AllowedFileTypes("image/jpeg", "image/png")]
        [MaxFileSize(2)]
        public IFormFile? File { get; set; }
    }
}
