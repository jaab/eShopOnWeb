using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IWishService
    {
        Task<int> GetWishItemCountAsync(string userName);
        Task TransferWishAsync(string anonymousId, string userName);
        Task AddItemToWish(int wishId, int catalogItemId, decimal price);
        Task DeleteWishAsync(int wishId);
    }
}
