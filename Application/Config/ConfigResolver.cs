using RoR2;

namespace Application.Config
{
    public static class ConfigResolver
    {
        public static int ItemCount(ItemTier tier)
        {
            return tier switch
            {
                ItemTier.Tier1 => 5,
                ItemTier.Tier2 => 4,
                ItemTier.Tier3 => 2,
                ItemTier.Lunar => 3,
                ItemTier.Boss => 2,
                _ => 5
            };
        }
    }
}