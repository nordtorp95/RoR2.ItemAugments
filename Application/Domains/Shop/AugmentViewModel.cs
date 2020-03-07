using Application.Augments;

namespace Application.Domains.Shop
{
    public class AugmentViewModel
    {
        public AugmentViewModel(AugmentBase augment,
            bool active,
            bool purchasable)
        {
            Augment = augment;
            Active = active;
            Purchasable = !active && purchasable;
        }

        public AugmentBase Augment { get;  }
        public bool Active { get;  }
        public bool Purchasable { get;  }
    }
}