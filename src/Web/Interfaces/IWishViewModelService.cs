using Microsoft.eShopWeb.Web.Pages.Wish;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface IWishViewModelService
    {
        Task<WishViewModel> GetOrCreateWishForUser(string userName);
    }
}
