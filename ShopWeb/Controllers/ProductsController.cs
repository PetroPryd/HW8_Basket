using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWeb.Constants;
using ShopWeb.Data;
using ShopWeb.Data.Entities;
using ShopWeb.Models.Helpers;
using ShopWeb.Models.Products;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ShopWeb.Controllers
{
    
    public class ProductsController : Controller
    {
        private readonly AppEFContext _appContext;
        private readonly IMapper _mapper;

        public ProductsController(AppEFContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        [Authorize(Roles = Roles.User)]
        public IActionResult IndexUser()
        {
            var model = _appContext.Products
                .AsQueryable()
                .Include(x => x.Category)
                .Select(x => _mapper.Map<ProductItemViewModel>(x))
                .ToList();

            return View(model);
        }

        [Authorize(Roles = Roles.Admin)]
        public IActionResult Index()
        {
            //var model = _appContext.Products.Select(x=> new ProductItemViewModel
            //{
            //    Id= x.Id,
            //    Name = x.Name,
            //    Price= x.Price,
            //    CategoryName=x.Category.Name
            //}).ToList();
            var model = _appContext.Products
                .AsQueryable()
                .Include(x => x.Category)
                .Select(x => _mapper.Map<ProductItemViewModel>(x))
                .ToList();

            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductCreateViewModel model = new ProductCreateViewModel();
            model.Categories = _appContext.Categories
                .Select(x => _mapper.Map<SelectItemViewModel>(x))
                .ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ProductCreateViewModel model)
        {
            bool isValide = true; 
            if (!ModelState.IsValid)
                isValide = false; 
            decimal price;
            if (!decimal.TryParse(model.Price, out price))
            {
                this.ModelState.AddModelError("Price", "Введіть число");
                isValide = false;
            }
            else if (price < (decimal)0.01)
            {
                this.ModelState.AddModelError("Price", "Введіть число більше нуля");
                isValide = false;
            }

            if (!isValide)
            {
                if (model.Categories == null)
                {
                    model.Categories = _appContext.Categories
                    .Select(x => _mapper.Map<SelectItemViewModel>(x))
                    .ToList();
                }
                return View(model);
            }
            var prod = new ProductEntity
            {
                DateCreated = DateTime.Now,
                Name = model.Name,
                CategoryId = model.CategoryId,
                Description = model.Description,
                Price = price
            };
            _appContext.Products.Add(prod);
            _appContext.SaveChanges(); 

            foreach(var img in model.Images)
            {
                var item = _appContext.ProductImages.SingleOrDefault(x => x.Id == img);
                item.ProductId = prod.Id;
                _appContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _appContext.Products
                .Include(x=>x.ProductImages)
                .AsQueryable()
                .Where(x=>x.Id==id)
                .Select(x=>_mapper.Map<ProductEditViewModel>(x))
                .SingleOrDefault();

            model.Categories = _appContext.Categories
                                .Select(x => _mapper.Map<SelectItemViewModel>(x))
                                .ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model)
        {
            bool isValide = true;
            if (!ModelState.IsValid)
                isValide = false;
            decimal price;
            if (!decimal.TryParse(model.Price, out price))
            {
                this.ModelState.AddModelError("Price", "Введіть число");
                isValide = false;
            }
            else if (price < (decimal)0.01)
            {
                this.ModelState.AddModelError("Price", "Введіть число більше нуля");
                isValide = false;
            }

            if (!isValide)
            {
                if (model.Categories == null)
                {
                    model.Categories = _appContext.Categories
                    .Select(x => _mapper.Map<SelectItemViewModel>(x))
                    .ToList();
                }
                return View(model);
            }
            if(model.RemoveImages!=null)
            {
                foreach (var img in model.RemoveImages)
                {
                    var del = _appContext.ProductImages
                        .SingleOrDefault(x => x.Name == img);
                    _appContext.Remove(del);
                    _appContext.SaveChanges();
                    string dirSaveImage = Path
                        .Combine(Directory.GetCurrentDirectory(), "images", img);
                    if (System.IO.File.Exists(dirSaveImage))
                        System.IO.File.Delete(dirSaveImage);
                }
            }
            
            var editProduct = _appContext.Products
                .SingleOrDefault(x => x.Id == model.Id);

            editProduct.Name = model.Name;
            editProduct.CategoryId = model.CategoryId;
            editProduct.Description = model.Description;
            editProduct.Price = price;
            _appContext.SaveChanges();

            if(model.Images!=null)
            {
                foreach (var img in model.Images)
                {
                    var item = _appContext.ProductImages.SingleOrDefault(x => x.Id == img);
                    item.ProductId = model.Id;
                    _appContext.SaveChanges();
                }
            }
            
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ProductImageItemViewModel> Upload(ProductImageCreateViewModel model)
        {
            string fileName = string.Empty;
            if (model.File != null)
            {
                //string exp = Path.GetExtension(model.File.FileName);
                fileName = Path.GetRandomFileName() + ".jpg";
                string dirSaveImage = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);
                using (var stream = System.IO.File.Create(dirSaveImage))
                {
                    await model.File.CopyToAsync(stream);
                }
            }
            ProductImageEntity image = new ProductImageEntity();
            image.Name = fileName;
            image.DateCreated = DateTime.UtcNow;
            _appContext.ProductImages.Add(image);
            _appContext.SaveChanges();
            return new ProductImageItemViewModel { Id = image.Id, Name = image.Name };
        }
    }
}
