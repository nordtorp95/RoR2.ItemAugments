using Application.Augments;
using UnityEngine.Networking;

namespace Application.Domains.Shop
{
    public class AugmentViewModel
    {
        public AugmentViewModel(AugmentBase augment,
            bool active,
            bool purchasable,
            int level)
        {
            AugmentId = augment.Id;
            Active = active;
            Purchasable = !active && purchasable;
            Level = level;
            MaxLevel = augment.MaxLevel;
            Name = augment.Name;
            Description = augment.Description;
        }

        public AugmentId AugmentId { get;  }
        public bool Active { get;  }
        public bool Purchasable { get;  }

        public int Level { get;  }
        public int MaxLevel { get;  }
        public string Name { get;  }
        public string Description { get;  }
    }
}