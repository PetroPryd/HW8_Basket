using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShopWeb.Models.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Електронна пошта")]
        [Required(ErrorMessage = "Вкажіть пошту")]
        public string Email { get; set; }
        [Display(Name = "Вкажіть пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}
