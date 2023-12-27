using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Tel { get; set; }

        public static implicit operator User(List<User> v)
        {
            throw new NotImplementedException();
        }
    
    }
    public enum Gender
    {
        [Display(Name = "Male")]
        Male,

        [Display(Name = "Female")]
        Female,

        [Display(Name = "Other")]
        Other
    }
}
