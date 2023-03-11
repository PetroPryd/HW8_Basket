using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShopWeb.Data.Entities.Identity;

namespace ShopWeb.Data.Entities
{
    [Table("tblBaskets")]
    public class BasketEntity
    {
        public short Count { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual ProductEntity Product { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
