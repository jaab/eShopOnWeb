using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using System.Linq;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class WishService : IWishService
    {
        private readonly IAsyncRepository<Wish> _wishRepository;
        private readonly IAppLogger<WishService> _logger;

        public WishService(IAsyncRepository<Wish> wishRepository,
            IAppLogger<WishService> logger)
        {
            _wishRepository = wishRepository;
            _logger = logger;
        }

        public async Task AddItemToWish(int wishId, int catalogItemId, decimal price, int quantity = 1)
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);

            wish.AddItem(catalogItemId, price, quantity);

            await _wishRepository.UpdateAsync(wish);
        }

        public async Task DeleteWishAsync(int wishId)
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);
            await _wishRepository.DeleteAsync(wish);
        }

        public async Task<int> GetWishItemCountAsync(string userName)
        {
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            var wishSpec = new WishWithItemsSpecification(userName);
            var wish = (await _wishRepository.ListAsync(wishSpec)).FirstOrDefault();
            if (wish == null)
            {
                _logger.LogInformation($"No wish found for {userName}");
                return 0;
            }
            int count = wish.Items.Sum(i => i.Quantity);
            _logger.LogInformation($"Wish for {userName} has {count} items.");
            return count;
        }

        public async Task SetQuantities(int wishId, Dictionary<string, int> quantities)
        {
            Guard.Against.Null(quantities, nameof(quantities));
            var wish = await _wishRepository.GetByIdAsync(wishId);
            Guard.Against.NullWish(wishId, wish);
            foreach (var item in wish.Items)
            {
                if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
                {
                    if(_logger != null) _logger.LogInformation($"Updating quantity of item ID:{item.Id} to {quantity}.");
                    item.Quantity = quantity;
                }
            }
            wish.RemoveEmptyItems();
            await _wishRepository.UpdateAsync(wish);
        }

        public async Task TransferWishAsync(string anonymousId, string userName)
        {
            Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            var wishSpec = new WishWithItemsSpecification(anonymousId);
            var wish = (await _wishRepository.ListAsync(wishSpec)).FirstOrDefault();
            if (wish == null) return;
            wish.BuyerId = userName;
            await _wishRepository.UpdateAsync(wish);
        }
    }
}
