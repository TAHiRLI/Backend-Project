using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class City
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        public List<House>? Houses { get; set; }
        public bool IsDeleted { get; set; }


    }
}
