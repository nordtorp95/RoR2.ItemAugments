using System.Collections.Generic;

namespace Application.Domains.Shop
{
    public class ShopViewModel
    {
        public ShopViewModel(IEnumerable<ShopItemViewModel> items)
        {
            Items = items;
        }

        public IEnumerable<ShopItemViewModel> Items { get;  }
    }
}