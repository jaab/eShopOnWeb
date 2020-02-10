using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Pages.Wish;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Services
{
    public class WishViewModelService : IWishViewModelService
    {
        private readonly IAsyncRepository<Wish> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public WishViewModelService(IAsyncRepository<Wish> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
        }

        public async Task<WishViewModel> GetOrCreateWishForUser(string userName)
        {
            var basketSpec = new WishWithItemsSpecification(userName);
            var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();

            if (basket == null)
            {
                return await CreateWishForUser(userName);
            }
            return await CreateViewModelFromWish(basket);
        }

        private async Task<WishViewModel> CreateViewModelFromWish(Wish basket)
        {
            var viewModel = new WishViewModel();
            viewModel.Id = basket.Id;
            viewModel.BuyerId = basket.BuyerId;
            viewModel.Items = await GetWishItems(basket.Items); ;
            return viewModel;
        }

        private async Task<WishViewModel> CreateWishForUser(string userId)
        {
            var basket = new Wish() { BuyerId = userId };
            await _basketRepository.AddAsync(basket);

            return new WishViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = new List<WishItemViewModel>()
            };
        }

        private async Task<List<WishItemViewModel>> GetWishItems(IReadOnlyCollection<WishItem> basketItems)
        {
            var items = new List<WishItemViewModel>();
            foreach (var item in basketItems)
            {
                var itemModel = new WishItemViewModel
                {
                    Id = item.Id,
                    UnitPrice = item.UnitPrice,
                    CatalogItemId = item.CatalogItemId
                };
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                itemModel.PictureUrl = _uriComposer.ComposePicUri(catalogItem.PictureUri);
                itemModel.ProductName = catalogItem.Name;
                items.Add(itemModel);
            }

            return items;
        }

      
    }
}
