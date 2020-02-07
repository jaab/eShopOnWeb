using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopWeb.Web.Pages.Wish
{
    public class WishViewModel
    {
        public int Id { get; set; }
        public List<WishItemViewModel> Items { get; set; } = new List<WishItemViewModel>();
        public string BuyerId { get; set; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice), 2);
        }
    }
}
