using System.Collections.Concurrent;
using System.Collections.Generic;
using Application.Augments;
using Application.Augments.Mushroom;
using Application.Augments.RustyKey;
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
            {ItemIndex.Mushroom, new Dictionary<AugmentId, AugmentBase>
            {
                {new AugmentId(nameof(FungusStayOnAfterMove)), new FungusStayOnAfterMove()}
            }},
            {ItemIndex.TreasureCache, new Dictionary<AugmentId, AugmentBase>
            {
                //{new AugmentId(nameof(ChestTurnsRed)), new ChestTurnsRed()},
                {new AugmentId(nameof(BoxSizeScaling)), new BoxSizeScaling()},
                //{new AugmentId(nameof(AdditionalChestSpawn)), new AdditionalChestSpawn()}
            }}
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

        public static bool IsAugmentActiveForPlayer(ItemIndex itemIndex,
            AugmentId augmentId,
            NetworkInstanceId clientId)
        {
            if (!ClientAugmentBinding.TryGetValue(clientId,
                out var items)) return false;
            return items.Augments.TryGetValue(itemIndex,
                out var augments) && augments.ContainsKey(augmentId);
        }

        public static bool TryAddOrUpgradeAugmentToPlayer(NetworkInstanceId id,
            ItemIndex itemIndex,
            AugmentId augmentId)
        {
            if (!AllAvailableAugments.TryGetValue(itemIndex,
                out var availableAugments) || !availableAugments.TryGetValue(augmentId,
                out var augmentToAdd))
            {
                return false;
            }

            var (getAugmentsSuccess, augmentList) = GetAugments();
            if (!getAugmentsSuccess) return false;

            var existsAlready = false;
            //If list contains augments for item
            if (augmentList.Augments.TryGetValue(itemIndex,
                out var existingItemAugments))
            {
                //If player already has augment, or if it couldnt be added
                existsAlready = existingItemAugments.ContainsKey(augmentId);
                if (!existsAlready && !existingItemAugments.TryAdd(augmentId,augmentToAdd))
                {
                    return false;
                }
            }
            //Player does not have any augments for item
            else
            {
                //Create new dictionary of augments for item
                var newAugmentDictionary = new ConcurrentDictionary<AugmentId, AugmentBase>();
                if (!newAugmentDictionary.TryAdd(augmentId,
                    augmentToAdd))
                {
                    return false;
                }

                if (!augmentList.Augments.TryAdd(itemIndex,
                    newAugmentDictionary))
                {
                    return false;
                }
            }

            Chat.AddMessage($"Exists: {existsAlready}");
            if (existsAlready) return augmentToAdd.TryLevelUp(id);
            Chat.AddMessage($"Activate {augmentToAdd.Name}");
            augmentToAdd.Activate();
            augmentToAdd.TryLevelUp(id);
            return true;

            (bool succes, PlayerAugments existingAugments) GetAugments()
            {
                //If client does not have any augments, add a new list.
                if (!ClientAugmentBinding.ContainsKey(id))
                {
                    if (!ClientAugmentBinding.TryAdd(id,
                        new PlayerAugments(id)))
                    {
                        return (false,null);
                    };
                }
                //Try get the existing or newly added list.
                if (!ClientAugmentBinding.TryGetValue(id,
                    out var existingAugments))
                {
                    return (false,null);
                }

                return (true, existingAugments);
            }
        }
    }
}