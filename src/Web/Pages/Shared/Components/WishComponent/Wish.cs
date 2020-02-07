using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Pages.Wish;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Pages.Shared.Components.WishComponent
{
    public class Wish : ViewComponent
    {
        private readonly IWishViewModelService _basketService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Wish(IWishViewModelService basketService,
                        SignInManager<ApplicationUser> signInManager)
        {
            _basketService = basketService;
            _signInManager = signInManager;
        }

        public IViewComponentResult Invoke(string userName)
        {
            var vm = new WishComponentViewModel();
            // vm.ItemsCount = (await GetWishViewModelAsync()).Items.Sum(i => i.Quantity);
            return View(vm);
        }

        private async Task<WishViewModel> GetWishViewModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return await _basketService.GetOrCreateWishForUser(User.Identity.Name);
            }
            string anonymousId = GetWishIdFromCookie();
            if (anonymousId == null) return new WishViewModel();
            return await _basketService.GetOrCreateWishForUser(anonymousId);
        }

        private string GetWishIdFromCookie()
        {
            if (Request.Cookies.ContainsKey(Constants.WISH_COOKIENAME))
            {
                return Request.Cookies[Constants.WISH_COOKIENAME];
            }
            return null;
        }
    }
}
