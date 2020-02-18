using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.ViewModels;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Services
{
    public class CatalogItemViewModelService : ICatalogItemViewModelService
    {
        private readonly IAsyncRepository<CatalogItem> _catalogItemRepository;

        public CatalogItemViewModelService(IAsyncRepository<CatalogItem> catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }

        public async Task UpdateCatalogItem(CatalogItemViewModel viewModel)
        {
            //Get existing CatalogItem
            var existingCatalogItem = await _catalogItemRepository.GetByIdAsync(viewModel.Id);

            //Build updated CatalogItem
            var updatedCatalogItem = existingCatalogItem;
            updatedCatalogItem.Name = viewModel.Name;
            updatedCatalogItem.Price = viewModel.Price;
            updatedCatalogItem.ShowPrice = viewModel.ShowPrice;
            updatedCatalogItem.QtStock = viewModel.QtStock;

            await _catalogItemRepository.UpdateAsync(updatedCatalogItem);
        }
         
         //ADD CatalogItem
        public async Task AddCatalogItem(CatalogItemAddModel viewModel)
        {
            var newCatalogItem = new CatalogItem
            {
                Name = viewModel.Name,
                Price = viewModel.Price,
                PictureUri = viewModel.PictureUri,
                ShowPrice = viewModel.ShowPrice,
                QtStock = viewModel.QtStock
            };
            newCatalogItem = await _catalogItemRepository.AddAsync(newCatalogItem);

        }
        
        //REMOVE CatalogItem
         public async Task RemoveCatalogItem(CatalogItemViewModel viewModel)
        {
            var existingCatalogItem = await _catalogItemRepository.GetByIdAsync(viewModel.Id);

            var removeCatalogItem = existingCatalogItem;
            removeCatalogItem.Id = viewModel.Id;

            await _catalogItemRepository.DeleteAsync(removeCatalogItem);

        }
    }
}
