using System.Collections.Generic;
using RoR2;

namespace Application.Domains.Shop
{
    public class ShopItemViewModel
    {
        public ShopItemViewModel(ItemIndex itemIndex,
            IEnumerable<AugmentViewModel> augments,
            int pointsToSpend)
        {
            ItemIndex = itemIndex;
            Augments = augments;
            PointsToSpend = pointsToSpend;
        }

        public ItemIndex ItemIndex { get;  }
        public IEnumerable<AugmentViewModel> Augments { get;  }
        public int PointsToSpend { get;  }
    }
}