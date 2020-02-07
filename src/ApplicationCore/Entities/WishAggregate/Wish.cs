using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.WishAggregate
{
    public class Wish : BaseEntity, IAggregateRoot
    {
        public string BuyerId { get; set; }
        private readonly List<WishItem> _items = new List<WishItem>();
        public IReadOnlyCollection<WishItem> Items => _items.AsReadOnly();

        public void AddItem(int catalogItemId, decimal unitPrice)
        {
            if (!Items.Any(i => i.CatalogItemId == catalogItemId))
            {
                _items.Add(new WishItem()
                {
                    CatalogItemId = catalogItemId,
                    UnitPrice = unitPrice
                });
                return;
            }
            var existingItem = Items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
           
        }

        public void RemoveEmptyItems()
        {
           // _items.RemoveAll(i => i.Quantity == 0);
        }
    }
}
