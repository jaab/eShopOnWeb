using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.WishAggregate;
namespace Ardalis.GuardClauses
{
    public static class BasketGuards
    {
        public static void NullBasket(this IGuardClause guardClause, int basketId, Basket basket)
        {
            if (basket == null)
                throw new BasketNotFoundException(basketId);
        }
    }

    public static class WishGuards
    {
        public static void NullWish(this IGuardClause guardClause, int wishId, Wish wish)
        {
            if (wish == null)
                throw new WishNotFoundException(wishId);
        }
    }
}