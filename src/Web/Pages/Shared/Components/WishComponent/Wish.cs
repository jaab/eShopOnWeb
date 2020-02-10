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
        private readonly IWishViewModelService _wishService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Wish(IWishViewModelService wishService,
                        SignInManager<ApplicationUser> signInManager)
        {
            _wishService = wishService;
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userName)
        {
            var vm = new WishComponentViewModel();
            vm.ItemsCount = (await GetWishViewModelAsync()).Items.Sum(i => i.Quantity);
            return View(vm);
        }

        private async Task<WishViewModel> GetWishViewModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                return await _wishService.GetOrCreateWishForUser(User.Identity.Name);
            }
            string anonymousId = GetWishIdFromCookie();
            if (anonymousId == null) return new WishViewModel();
            return await _wishService.GetOrCreateWishForUser(anonymousId);
        }

        private string GetWishIdFromCookie()
        {
            if (Request.Cookies.ContainsKey(Constants.Wish_COOKIENAME))
            {
                return Request.Cookies[Constants.Wish_COOKIENAME];
            }
            return null;
        }
    }
}
