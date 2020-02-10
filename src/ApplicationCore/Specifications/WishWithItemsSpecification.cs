using Microsoft.eShopWeb.ApplicationCore.Entities.WishAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications
{
    public sealed class WishWithItemsSpecification : BaseSpecification<Wish>
    {
        public WishWithItemsSpecification(int wishId)
            :base(b => b.Id == wishId)
        {
            AddInclude(b => b.Items);
        }
        public WishWithItemsSpecification(string buyerId)
            :base(b => b.BuyerId == buyerId)
        {
            AddInclude(b => b.Items);
        }
    }
}
