using System.Collections.Generic;
using System.Linq;
using Application.Augments;
using Application.Config;
using RoR2;
using UnityEngine.Networking;

namespace Application.Services
{
    public static class AugmentShop
    {
        public static Dictionary<ItemIndex,Dictionary<string,AugmentBase>> GetAvailableItems(NetworkInstanceId clientId)
        {
            var player = PlayerCharacterMasterController.instances.First(x => x.master.netId == clientId);
            var availableAugments = AugmentResolver.GetAvailableAugmentsForPlayer(clientId);
            var activeAugments = AugmentResolver.GetActiveAugmentsForPlayer(clientId);
            var purchasable = new Dictionary<ItemIndex,Dictionary<string,AugmentBase>>();
            
            //For all available, make sure that player meets item cost
            foreach (var (key, augmentList) in availableAugments)
            {
                var itemCount = player.master.inventory.GetItemCount(key);
                
                var itemTier = ItemCatalog.GetItemDef(key).tier;
                var itemCost = ConfigResolver.ItemCount(itemTier);

                //If player has active augments for item, multiply cost
                if (activeAugments.Augments.TryGetValue(key,
                    out var activeList))
                {
                    itemCost *= (activeList.Count + 1);
                }
                
                if (itemCount >= itemCost)
                {
                    purchasable.Add(key,augmentList);
                }
            }
            return purchasable;
        }
    }
}