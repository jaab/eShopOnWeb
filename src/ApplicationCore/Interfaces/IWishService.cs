using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IWishService
    {
        Task<int> GetWishItemCountAsync(string userName);
        Task TransferWishAsync(string anonymousId, string userName);
        Task AddItemToWish(int wishId, int catalogItemId, decimal price, int quantity = 1);
        Task SetQuantities(int wishId, Dictionary<string, int> quantities);
        Task DeleteWishAsync(int wishId);
    }
}
