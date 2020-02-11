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

        public async Task AddItemToWish(int wishId, int catalogItemId, decimal price)
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);

            wish.AddItem(catalogItemId, price);

            await _wishRepository.UpdateAsync(wish);
        }

        public Task AddItemToWish(int wishId, int catalogItemId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteWishAsync(int wishId)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteWishtAsync(int wishId)
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
           // int count = basket.Items.Sum(i => i.Quantity);
           int count = 0;
            _logger.LogInformation($"Wish for {userName} has {count} items.");
            return count;
        }

       /** public async Task SetQuantities(int basketId, Dictionary<string, int> quantities)
        {
            Guard.Against.Null(quantities, nameof(quantities));
            var basket = await _basketRepository.GetByIdAsync(basketId);
            Guard.Against.NullBasket(basketId, basket);
            foreach (var item in basket.Items)
            {
                if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
                {
                    if(_logger != null) _logger.LogInformation($"Updating quantity of item ID:{item.Id} to {quantity}.");
                    item.Quantity = quantity;
                }
            }
            basket.RemoveEmptyItems();
            await _basketRepository.UpdateAsync(basket);
        }**/

      /**  public async Task TransferBasketAsync(string anonymousId, string userName)
        {
            Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            var basketSpec = new BasketWithItemsSpecification(anonymousId);
            var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();
            if (basket == null) return;
            basket.BuyerId = userName;
            await _basketRepository.UpdateAsync(basket);
        }**/

        public Task TransferWishAsync(string anonymousId, string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}
