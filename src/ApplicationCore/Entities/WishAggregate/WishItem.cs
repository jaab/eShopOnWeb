namespace Microsoft.eShopWeb.ApplicationCore.Entities.WishAggregate
{
    public class WishItem : BaseEntity
    {
        public decimal UnitPrice { get; set; }
        public int CatalogItemId { get; set; }
        public int WishId { get; private set; }
    }
}
