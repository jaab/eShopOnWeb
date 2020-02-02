using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    /// <summary>
    /// Catalog item to be created
    /// </summary>
    public class CatalogItemCreateModel
    {
        [Required]
        [MaxLength(75)]
        public string Name { get; set; }

        public string Description { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        public string PictureUri { get; set; }
        
        public int CatalogTypeId { get; set; }
        
        public int CatalogBrandId { get; set; }

        public bool ShowPrice { get; set; } = true;
    }
}