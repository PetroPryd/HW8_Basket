using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ShopWeb.Constants;
using ShopWeb.Data.Entities;
using ShopWeb.Data;
using ShopWeb.Data.Entities.Identity;
using ShopWeb.Models.Categories;
using ShopWeb.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShopWeb.Models.Account;
using ShopWeb.Models.Helpers;
using ShopWeb.Models.Products;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ShopWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppEFContext _appContext;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        public AccountController(UserManager<UserEntity> userManager,
            AppEFContext appEFContext,
            SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appContext = appEFContext; // конструктор
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserAddViewModel model)
        {
            string imageName = String.Empty;
            if (model.UploadImage != null)
            {
                string exp = Path.GetExtension(model.UploadImage.FileName);
                imageName = Path.GetRandomFileName() + exp + "user";
                string dirSaveImage = Path.Combine(Directory.GetCurrentDirectory(), "images", imageName);
                using (var stream = System.IO.File.Create(dirSaveImage))
                {
                    await model.UploadImage.CopyToAsync(stream);
                }
            }

            UserEntity user = new UserEntity
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                Image = imageName
            };
            var result = _userManager.CreateAsync(user, model.Password)
                .Result;
            if (result.Succeeded)
            {
                result = _userManager
                    .AddToRoleAsync(user, Roles.User)
                    .Result;
            }

            //_appEFContext.Users.Add(user);
            //_appEFContext.SaveChanges();

            return RedirectToAction("Index", "Categories");
        }
        [HttpGet]
        public IActionResult Login(string returnUrl="/")
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Redirect(model.ReturnUrl??"/");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Дані вказано невірно!");
                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Categories");
        }

        //[HttpGet]
        public IActionResult Basket()
        {
            var nameUser = User.Identity.Name;
            var user = _userManager.FindByNameAsync(nameUser).Result;
            
            var model = _appContext.Basket.Include(x => x.Product).Include(x => x.Product.Category).Where(x => x.UserId == user.Id).ToList();

            return View(model);
        }

        //[HttpPost]
        //public IActionResult Basket(BasketAddViewModel model)
        //{
        //    var modelprod2 = _appContext.Products.FirstOrDefault(x => x.Id == model.Id);
        //    var modelprod = _appContext.Products.Where(x => x.Id == model.Id);
        //    for (int i = 0; i < _appContext.Products.Count(); i++)
        //    {
        //        if (model.Id == modelprod2.Id)
        //        {
        //            model.
        //        }
        //    }

        //    //var model2 = _appContext.Products
        //    //    .AsQueryable()
        //    //    .Include(x => x.Category)
        //    //    .Select(x => _mapper.Map<ProductItemViewModel>(x))
        //    //    .Where(x => x.Id == model.Id)
        //    //    .ToList();

        //    return View(model);
        //}

        [HttpGet]
        public IActionResult AddToBasket(int id)
        {
            BasketAddViewModel model = new BasketAddViewModel();
            model.Id = id;

            return View(model);
        }

        [HttpPost]
        public IActionResult AddToBasket(BasketAddViewModel model)
        {
            var nameUser = User.Identity.Name;
            var user = _userManager.FindByNameAsync(nameUser).Result;

            if (_appContext.Basket.FirstOrDefault(x => x.ProductId == model.Id) != null)
            {
                //var basketUpdate = new BasketEntity
                //{
                //    ProductId = model.Id,
                //    UserId = user.Id,
                //    Count = model.Count
                //};
                model.Count++;
                var basketUpdate2 = new BasketEntity
                {
                    ProductId = model.Id,
                    UserId = user.Id,
                    Count = model.Count
                };
                _appContext.Entry(basketUpdate2).State = EntityState.Modified;
                //_appContext.Basket.Remove(basketUpdate);
                //_appContext.Basket.Add(basketUpdate2);
                _appContext.Basket.Update(basketUpdate2);
            }
            else
            {
                var basket = new BasketEntity
                {
                    ProductId = model.Id,
                    UserId = user.Id,
                    Count = model.Count
                };
                _appContext.Basket.Add(basket);
            }
            _appContext.SaveChanges();
            //Console.WriteLine("add to basket");

            return RedirectToAction("IndexUser");
        }

        [HttpGet]
        public IActionResult BasketCountPlus(int id)
        {
            BasketAddViewModel model = new BasketAddViewModel();
            model.Id = id;

            return View(model);
        }

        [HttpPost]
        public IActionResult BasketCountPlus(BasketAddViewModel model)
        {
            var nameUser = User.Identity.Name;
            var user = _userManager.FindByNameAsync(nameUser).Result;

            //var basketUpdate = new BasketEntity
            //{
            //    ProductId = model.Id,
            //    UserId = user.Id,
            //    Count = model.Count
            //};
            model.Count++;
            var basketUpdate2 = new BasketEntity
            {
                ProductId = model.Id,
                UserId = user.Id,
                Count = model.Count
            };
            _appContext.Entry(basketUpdate2).State = EntityState.Modified;
            //_appContext.Basket.Remove(basketUpdate);
            //_appContext.Basket.Add(basketUpdate2);
            _appContext.Basket.Update(basketUpdate2);

            _appContext.SaveChanges();

            return RedirectToAction("Basket");
        }

        [HttpGet]
        public IActionResult BasketCountMinus(int id)
        {
            BasketAddViewModel model = new BasketAddViewModel();
            model.Id = id;

            return View(model);
        }

        [HttpPost]
        public IActionResult BasketCountMinus(BasketAddViewModel model)
        {
            var nameUser = User.Identity.Name;
            var user = _userManager.FindByNameAsync(nameUser).Result;

            //var basketUpdate = new BasketEntity
            //{
            //    ProductId = model.Id,
            //    UserId = user.Id,
            //    Count = model.Count
            //};
            if(model.Count >= 2)
            {
                model.Count--;
            }
            else
            {
                var basketUpdate2 = new BasketEntity
                {
                    ProductId = model.Id,
                    UserId = user.Id,
                    Count = model.Count
                };
                _appContext.Entry(basketUpdate2).State = EntityState.Modified;
                //_appContext.Basket.Remove(basketUpdate);
                //_appContext.Basket.Add(basketUpdate2);
                _appContext.Basket.Update(basketUpdate2);

                _appContext.SaveChanges();
            }

            return RedirectToAction("Basket");
        }
    }
}
