using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Application.Augments;
using Application.Augments.Mushroom;
using Application.Domains;
using RoR2;
using UnityEngine.Networking;

namespace Application.Services
{
    public class AugmentResolver
    {
        //todo automate registration
        public static readonly IDictionary<ItemIndex, IDictionary<AugmentId, AugmentBase>> AllAvailableAugments = new Dictionary<ItemIndex, IDictionary<AugmentId, AugmentBase>>
        {
            {ItemIndex.Mushroom, new Dictionary<AugmentId, AugmentBase>{{new AugmentId(nameof(FungusStayOnAfterMove)), new FungusStayOnAfterMove()}}}
        };

        private static readonly ConcurrentDictionary<NetworkInstanceId, PlayerAugments> ClientAugmentBinding =
            new ConcurrentDictionary<NetworkInstanceId, PlayerAugments>();

        public static IDictionary<AugmentId, AugmentBase> ListAugmentsForItem(ItemIndex item)
        {
            return AllAvailableAugments.TryGetValue(item,
                out var augments)
                ? augments
                : new Dictionary<AugmentId, AugmentBase>();
        }

        public static PlayerAugments GetActiveAugmentsForPlayer(
            NetworkInstanceId clientId)
        {
            return ClientAugmentBinding.TryGetValue(clientId,
                out var augments) ? augments : new PlayerAugments(clientId);
        }
        
        
        public static IDictionary<ItemIndex, Dictionary<AugmentId, AugmentBase>> GetAvailableAugmentsForPlayer(
            NetworkInstanceId clientId)
        {
            //Get existing augments or empty list
            if (!ClientAugmentBinding.TryGetValue(clientId,
                out var playerAugments))
            {
                playerAugments = new PlayerAugments(clientId);
            }

            var available = new Dictionary<ItemIndex, Dictionary<AugmentId, AugmentBase>>();

            //Check for all availableAugments
            foreach (var (key, value) in AllAvailableAugments)
            {
                //If player already has augment for item, filter out the existing ones
                if (playerAugments.Augments.TryGetValue(key,
                    out var existingAugment))
                {
                    //Get values that does not already exist on the players list of augments
                    var availableValues = value.Where(x => !existingAugment.ContainsKey(x.Key))
                        .ToDictionary(x => x.Key,
                            x => x.Value);

                    //If any augments after filter, add them to result
                    if (availableValues.Any())
                    {
                        available.Add(key,
                            availableValues);
                    }
                }
                //Player has no augments for item, add all available
                else
                {
                    available.Add(key,
                        value.ToDictionary(x => x.Key,
                            x => x.Value));
                }
            }

            return available;
        }

        public static bool IsAugmentActiveForPlayer(ItemIndex itemIndex,
            AugmentId augmentId,
            NetworkInstanceId clientId)
        {
            if (!ClientAugmentBinding.TryGetValue(clientId,
                out var items)) return false;
            return items.Augments.TryGetValue(itemIndex,
                out var augments) && augments.ContainsKey(augmentId);
        }

        public static void TryAddAugmentToPlayer(NetworkInstanceId id,
            ItemIndex itemIndex,
            AugmentBase augmentBase)
        {
            if (ClientAugmentBinding.TryGetValue(id,
                out var existingAugments))
            {
                if (existingAugments.Augments.TryGetValue(itemIndex,
                    out var existingItemAugments))
                {
                    CreateLowestLevelDic(existingItemAugments);
                }
                else
                {
                    existingAugments.Augments.TryAdd(itemIndex,
                        CreateLowestLevelDic());
                }
            }
            else
            {
                var newDic = new ConcurrentDictionary<ItemIndex, ConcurrentDictionary<AugmentId, AugmentBase>>();
                var lowestLevel = CreateLowestLevelDic();
                newDic.TryAdd(itemIndex,
                    lowestLevel);
                ClientAugmentBinding.TryAdd(id,
                    new PlayerAugments(id,
                        newDic));
            }

            ConcurrentDictionary<AugmentId, AugmentBase> CreateLowestLevelDic(
                ConcurrentDictionary<AugmentId, AugmentBase> existingDic = null)
            {
                if (existingDic == null) existingDic = new ConcurrentDictionary<AugmentId, AugmentBase>();
                if (existingDic.TryAdd(new AugmentId(augmentBase.GetType().Name), 
                    augmentBase))
                {
                    augmentBase.Activate();
                }

                return existingDic;
            }
        }
    }
}