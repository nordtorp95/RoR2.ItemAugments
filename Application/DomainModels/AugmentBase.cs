using RoR2;
using UnityEngine.Networking;

namespace Application.DomainModels
{
    public abstract class AugmentBase
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ItemIndex ItemType { get; }

        public abstract void Activate();
    }
}