using System.ComponentModel.DataAnnotations;

namespace GuestList.Web.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Login { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
