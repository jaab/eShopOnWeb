using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
namespace Microsoft.eShopWeb.Web.ViewModels
{
    public class CatalogItemAddModel
    {
        [Required]
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public decimal Price { get; set; }
        public bool ShowPrice { get; set; } = true;
        public Currency PriceUnit { get; set; }
        public int QtStock { get; set; }
    }

}
