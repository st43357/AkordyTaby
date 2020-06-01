using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DiplomovaPrace.Models
{

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Přezdívka")]
        public string Prezdivka { get; set; }

        [Required]
        [Display(Name = "Heslo")]
        [DataType(DataType.Password)]
        public string Heslo { get; set; }

        [Display(Name = "Zapamatovat?")]
        public bool Zapamatovat { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Přezdívka")]
        public string Prezdivka { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí být alespoň {2} znaků dlouhé.", MinimumLength = 6)]
        [Display(Name = "Heslo")]
        [DataType(DataType.Password)]
        public string Heslo { get; set; }

        [Required]
        [Display(Name = "Potvrzení hesla")]
        [DataType(DataType.Password)]
        [Compare("Heslo", ErrorMessage = "Zadaná hesla se neshodují.")]
        public string PotvrzeniHesla { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Jméno")]
        public string Jmeno { get; set; }

        [Required]
        [Display(Name = "Příjmení")]
        public string Prijmeni { get; set; }

        [Required]
        [Display(Name = "Město")]
        public string Mesto { get; set; }

        [Display(Name = "Profilové foto")]
        public string ProfiloveFoto { get; set; }

        [Required]
        [Display(Name = "Datum narození")]
        public string DatumNarození { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Heslo")]
        [DataType(DataType.Password)]
        public string Heslo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        public string PotvrzeniHeslo { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
