using AutoMapper;
using ShopWeb.Data.Entities;
using ShopWeb.Models.Helpers;
using ShopWeb.Models.Products;
using ShopWeb.Models.Users;
using System.Linq;

namespace ShopWeb.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile() 
        {
            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(p=>p.CategoryName, 
                    opt=> opt.MapFrom(entity=> entity.Category.Name));

            CreateMap<ProductEntity, ProductEditViewModel>()
                .ForMember(p => p.OldImages,
                    opt => opt.MapFrom(entity => 
                                        entity.ProductImages.Select(x=>x.Name)));

            CreateMap<CategoryEntity, SelectItemViewModel>();
        }
    }
}
