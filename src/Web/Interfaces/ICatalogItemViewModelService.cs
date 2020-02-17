using Microsoft.eShopWeb.Web.ViewModels;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Web.Interfaces
{
    public interface ICatalogItemViewModelService
    {
        Task UpdateCatalogItem(CatalogItemViewModel viewModel);

        Task AddCatalogItem(CatalogItemAddModel viewModel);

        Task RemoveCatalogItem(CatalogItemViewModel viewModel);
    }
}
