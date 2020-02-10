using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Pages.Wish
{
    public class IndexModel : PageModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public decimal Price { get; set; }
        private readonly IWishService _wishService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private string _username = null;
        private readonly IWishViewModelService _wishViewModelService;

        public IndexModel(IWishService wishService,
            IWishViewModelService wishViewModelService,
            SignInManager<ApplicationUser> signInManager)
        {
            _wishService = wishService;
            _signInManager = signInManager;
            _wishViewModelService = wishViewModelService;
        }

        public WishViewModel WishModel { get; set; } = new WishViewModel();

        public async Task OnGet()
        {
            await SetWishModelAsync();
        }

        public async Task<IActionResult> OnPost(CatalogItemViewModel productDetails)
        {
            if (productDetails?.Id == null)
            {
                return RedirectToPage("/Index");
            }
            await SetWishModelAsync();

            await _wishService.AddItemToWish(WishModel.Id, productDetails.Id, productDetails.Price);

            await SetWishModelAsync();

            return RedirectToPage();
        }

        public async Task OnPostUpdate(Dictionary<string, int> items)
        {
            await SetWishModelAsync();
            await _wishService.SetQuantities(WishModel.Id, items);

            await SetWishModelAsync();
        }

        private async Task SetWishModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                WishModel = await _wishViewModelService.GetOrCreateWishForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetWishCookieAndUserName();
                WishModel = await _wishViewModelService.GetOrCreateWishForUser(_username);
            }
        }

        private void GetOrSetWishCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.Wish_COOKIENAME))
            {
                _username = Request.Cookies[Constants.Wish_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true };
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.Wish_COOKIENAME, _username, cookieOptions);
        }
    }
}
