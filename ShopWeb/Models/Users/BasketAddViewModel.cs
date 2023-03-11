using ShopWeb.Models.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ShopWeb.Models.Users
{
    public class BasketAddViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Кількість")]
        public short Count { get; set; }
    }
}
