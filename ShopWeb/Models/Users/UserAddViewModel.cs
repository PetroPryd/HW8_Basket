using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ShopWeb.Models.Users
{
    public class UserAddViewModel
    {
        [Display(Name = "Ім'я")]
        [Required(ErrorMessage = "Вкажіть ім'я")]
        public string FirstName { get; set; }

        [Display(Name = "Прізвище")]
        [Required(ErrorMessage = "Вкажіть прізвище")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Вкажіть email")]
        public string Email { get; set; }

        [Display(Name = "UserName")]
        [Required(ErrorMessage = "Вкажіть username")]
        public string UserName { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Вкажіть пароль")]
        [StringLength(250)]
        [MinLength(8)]
        public string Password { get; set; }

        [Display(Name = "Фото")]
        public IFormFile UploadImage { get; set; }
    }
}
