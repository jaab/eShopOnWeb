using Microsoft.eShopWeb.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Web.ViewModels;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.eShopWeb.Web.Authorization;
using Microsoft.eShopWeb.ApplicationCore.Constants;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{

    

    public class CatalogController : BaseApiController
    {
        private readonly ICatalogViewModelService _catalogViewModelService;

        public CatalogController(ICatalogViewModelService catalogViewModelService) => _catalogViewModelService = catalogViewModelService;

        [HttpGet]
        [Authorize(ApiAuthorizationConstants.CATALOG_ITEM_READ_SCOPE)]
        public async Task<ActionResult<CatalogIndexViewModel>> List(
            int? brandFilterApplied, int? typesFilterApplied, int? page,
            string searchText = null)
        {
            var itemsPage = 10;           
            var catalogModel = await _catalogViewModelService.GetCatalogItems(
                page ?? 0, itemsPage, searchText, 
                brandFilterApplied, typesFilterApplied, true, HttpContext.RequestAborted);
            return Ok(catalogModel);
        }

        [HttpGet("{id}")]
        [Authorize(ApiAuthorizationConstants.CATALOG_ITEM_READ_SCOPE)]
        public async Task<ActionResult<CatalogItemViewModel>> GetById(int id) {
            try  {
                var catalogItem = await _catalogViewModelService.GetItemById(id);
                return Ok(catalogItem);
            } catch (ModelNotFoundException) {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(ApiAuthorizationConstants.CATALOG_ITEM_WRITE_SCOPE)]
        [Authorize(Roles=AuthorizationConstants.Roles.ADMINISTRATORS)]
        public async Task<ActionResult<CatalogItemViewModel>> Create([FromBody] CatalogItemCreateModel data) {
            try {
                if (!ModelState.IsValid) {
                    return BadRequest();
                }
                var createdCatalogItem = await _catalogViewModelService.Create(
                    data.Name, data.Description, data.PictureUri, data.Price,
                    data.CatalogBrandId, data.CatalogTypeId, data.ShowPrice,
                    HttpContext.RequestAborted);
                return CreatedAtAction("Create", createdCatalogItem);
            } catch (Exception) { // TODO: Should reserve this for explicit
                return StatusCode(500);
            }
        }

   

    }
}
