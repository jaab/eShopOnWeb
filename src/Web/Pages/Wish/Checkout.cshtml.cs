using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Pages.Wish
{
    public class CheckoutModel : PageModel
    {
        private readonly IWishService _wishService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrderService _orderService;
        private string _username = null;
        private readonly IWishViewModelService _wishViewModelService;

        public CheckoutModel(IWishService wishService,
            IWishViewModelService wishViewModelService,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService)
        {
            _wishService = wishService;
            _signInManager = signInManager;
            _orderService = orderService;
            _wishViewModelService = wishViewModelService;
        }

        public WishViewModel WishModel { get; set; } = new WishViewModel();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(Dictionary<string, int> items)
        {
            await SetWishModelAsync();

            await _wishService.SetQuantities(WishModel.Id, items);

            await _orderService.CreateOrderAsync(WishModel.Id, new Address("123 Main St.", "Kent", "OH", "United States", "44240"));

            await _wishService.DeleteWishAsync(WishModel.Id);

            return RedirectToPage();
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
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.Wish_COOKIENAME, _username, cookieOptions);
        }
    }
}
