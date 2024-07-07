using System.ComponentModel.DataAnnotations;

namespace GuestList.Web.Models.Entities
{
    public class Guest
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public bool Confirmed { get; set; }
    }

}
