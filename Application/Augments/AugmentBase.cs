using RoR2;

namespace Application.Augments
{
    public abstract class AugmentBase
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ItemIndex ItemType { get; }
        public abstract AugmentId Id { get; }
        public abstract void Activate();
    }
}