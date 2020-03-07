using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Application.Augments;
using Application.Config;
using Application.Domains.Shop;
using RoR2;
using UnityEngine.Networking;

namespace Application.Services
{
    public static class AugmentShop
    {
        public static ShopViewModel GetShopViewModel(NetworkInstanceId clientId)
        {
            var player = PlayerCharacterMasterController.instances.First(x => x.master.netId == clientId);
            var allItems = AugmentResolver.AllAvailableAugments;
            var activeForPlayer = AugmentResolver.GetActiveAugmentsForPlayer(clientId);
            var viewModels = new List<ShopItemViewModel>();

            foreach (var item in allItems)
            {
                if (!activeForPlayer.Augments.TryGetValue(item.Key,
                    out var existingAugments))
                {
                    existingAugments = new ConcurrentDictionary<AugmentId, AugmentBase>();
                }

                var itemCount = player.master.inventory.GetItemCount(item.Key);
                var itemTier = ItemCatalog.GetItemDef(item.Key).tier;
                var itemCost = ConfigResolver.ItemCount(itemTier);

                var pointsToSpend = itemCount / (itemCost * (existingAugments.Count + 1));

                var augmentViewModels = new List<AugmentViewModel>();
                foreach (var augment in item.Value)
                {
                    var active = existingAugments.ContainsKey(augment.Key);
                    var purchasable = pointsToSpend > 0;
                    var viewModel = new AugmentViewModel(augment.Value,
                        active,
                        purchasable);
                    augmentViewModels.Add(viewModel);
                }

                viewModels.Add(new ShopItemViewModel(item.Key,
                    augmentViewModels,
                    pointsToSpend));
            }

            return new ShopViewModel(viewModels);
        }

        public static bool TryPurchaseAugment(ItemIndex itemIndex,
            AugmentId augmentId,
            NetworkInstanceId clientId)
        {
            return AugmentResolver.TryAddAugmentToPlayer(clientId,
                itemIndex,
                augmentId);
        }
    }
}