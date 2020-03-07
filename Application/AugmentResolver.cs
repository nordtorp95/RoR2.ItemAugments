using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Application.Augments;
using Application.DomainModels;
using RoR2;
using UnityEngine.Networking;

namespace Application
{
    public class AugmentResolver
    {
        private static IDictionary<ItemIndex, IDictionary<string, AugmentBase>> _availableAugments;

        private static
            ConcurrentDictionary<NetworkIdentity,
                ConcurrentDictionary<ItemIndex, ConcurrentDictionary<string, AugmentBase>>> _clientAugmentBinding;

        public static IDictionary<string, AugmentBase> ResolveAugment(ItemIndex item)
        {
            if (_availableAugments == null) InitiateAugments();

            if (_availableAugments != null && _availableAugments.TryGetValue(item,
                out var augments))
            {
                return augments;
            }

            return new Dictionary<string, AugmentBase>();
        }

        public static bool IsActiveFor(ItemIndex itemIndex,
            string augmentName,
            NetworkIdentity clientId)
        {
            var activeAugments = _clientAugmentBinding.SelectMany(x => x.Value.SelectMany(y => y.Value).Select(z=>z.Key));
            var augmentList = "";
            foreach (var augment in activeAugments)
            {
                augmentList += $", {augment}";
            }
            Chat.AddMessage($"Augments: {augmentList}");
            if (!_clientAugmentBinding.TryGetValue(clientId,
                out var items)) return false;
            return items.TryGetValue(itemIndex,
                out var augments) && augments.ContainsKey(augmentName);
        }

        public static void TryAddAugment(NetworkIdentity id,
            ItemIndex itemIndex,
            AugmentBase augmentBase)
        {
            if (_clientAugmentBinding == null)
                _clientAugmentBinding =
                    new ConcurrentDictionary<NetworkIdentity,
                        ConcurrentDictionary<ItemIndex, ConcurrentDictionary<string, AugmentBase>>>();
            if (_clientAugmentBinding.TryGetValue(id,
                out var existingAugments))
            {
                if (existingAugments.TryGetValue(itemIndex,
                    out var existingItemAugments))
                {
                    CreateLowestLevelDic(existingItemAugments);
                }
                else
                {
                    existingAugments.TryAdd(itemIndex,
                        CreateLowestLevelDic());
                }
            }
            else
            {
                var newDic = new ConcurrentDictionary<ItemIndex, ConcurrentDictionary<string, AugmentBase>>();
                var lowestLevel = CreateLowestLevelDic();
                newDic.TryAdd(itemIndex,
                    lowestLevel);
                _clientAugmentBinding.TryAdd(id,
                    newDic);
            }

            ConcurrentDictionary<string, AugmentBase> CreateLowestLevelDic(
                ConcurrentDictionary<string, AugmentBase> existingDic = null)
            {
                if (existingDic == null) existingDic = new ConcurrentDictionary<string, AugmentBase>();
                if (existingDic.TryAdd(augmentBase.Name,
                    augmentBase))
                {
                    augmentBase.Activate();
                }

                return existingDic;
            }
        }

        private static void InitiateAugments()
        {
            _availableAugments = new Dictionary<ItemIndex, IDictionary<string, AugmentBase>>();

            var fungus = new FungusStayOnAfterMove();
            _availableAugments.Add(ItemIndex.Mushroom,
                new Dictionary<string, AugmentBase>
                {
                    {fungus.Name, fungus}
                });

            //Todo get this to work 
            // foreach (var type in
            //     Assembly.GetExecutingAssembly().GetTypes()
            //         .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(AugmentBase))))
            // {
            //     var createdObject = Activator.CreateInstance(type);
            //
            //     if (createdObject is AugmentBase augment)
            //     {
            //         AddAugment(augment);
            //     }
            // }
        }

        private static void AddAugment(AugmentBase augment)
        {
            if (_availableAugments.TryGetValue(augment.ItemType,
                out var existingAugments))
            {
                if (!existingAugments.ContainsKey(augment.Name))
                {
                    existingAugments.Add(augment.Name,
                        augment);
                }
            }
            else
            {
                var newDic = new Dictionary<string, AugmentBase> {{augment.Name, augment}};
                _availableAugments.TryAdd(augment.ItemType,
                    newDic);
            }
        }
    }
}