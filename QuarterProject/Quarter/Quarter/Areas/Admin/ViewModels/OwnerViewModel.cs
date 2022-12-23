using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quarter.Areas.Admin.ViewModels
{
    public class OwnerViewModel
    {
        [MaxLength(30)]
        public string Fullname { get; set; }
        public decimal SharePercent { get; set; }
        [MaxLength(500)]
        public string Desc { get; set; }
        public IFormFile? File { get; set; }
    }
}
