using System.ComponentModel.DataAnnotations;

namespace TreeStruct.Models
{
    //modele zawierają dane potrzebne do przekazywania pomiędzy akcjami i widokami
    //pojawiają się też odpowiednie adnotacje
    //odpowiadające za walidaję danych wejściowych i wyświetlanie informacji
    public class CreateModel
    {
        [Required]
        [Display(Name = "User Name")]
        [StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(80)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "The e-mail address is not valid.")]
        [StringLength(80)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The password is too long.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Invalid password confirmation.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}