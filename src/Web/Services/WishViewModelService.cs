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
        private readonly IAsyncRepository<Wish> _wishRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;

        public WishViewModelService(IAsyncRepository<Wish> wishRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _wishRepository = wishRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
        }

        public async Task<WishViewModel> GetOrCreateWishForUser(string userName)
        {
            var wishSpec = new WishWithItemsSpecification(userName);
            var wish = (await _wishRepository.ListAsync(wishSpec)).FirstOrDefault();

            if (wish == null)
            {
                return await CreateWishForUser(userName);
            }
            return await CreateViewModelFromWish(wish);
        }

        private async Task<WishViewModel> CreateViewModelFromWish(Wish wish)
        {
            var viewModel = new WishViewModel();
            viewModel.Id = wish.Id;
            viewModel.BuyerId = wish.BuyerId;
            viewModel.Items = await GetWishItems(wish.Items); ;
            return viewModel;
        }

        private async Task<WishViewModel> CreateWishForUser(string userId)
        {
            var wish = new Wish() { BuyerId = userId };
            await _wishRepository.AddAsync(wish);

            return new WishViewModel()
            {
                BuyerId = wish.BuyerId,
                Id = wish.Id,
                Items = new List<WishItemViewModel>()
            };
        }

        private async Task<List<WishItemViewModel>> GetWishItems(IReadOnlyCollection<WishItem> wishItems)
        {
            var items = new List<WishItemViewModel>();
            foreach (var item in wishItems)
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
