using Application.Augments;
using Application.Services;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Application.UI
{
    public static class AugmentsWindow
    {
        private static NetworkUser firstNetworkUser;
        private static CharacterMaster firstPlayer;

        public static void ShowWindow(int windowID)
        {
            firstNetworkUser = PlayerCharacterMasterController.instances[0].networkUser;
            firstPlayer = PlayerCharacterMasterController.instances[0].master;
            var viewModel = AugmentShop.GetShopViewModel(firstPlayer.netId);
            
            GUILayout.BeginHorizontal();

            foreach (var item in viewModel.Items)
            {
                var itemDef = ItemCatalog.GetItemDef(item.ItemIndex);

                GUILayout.Label($"Item name: {itemDef.name}, Points to spend: {item.PointsToSpend}");

                foreach (var augment in item.Augments)
                {                   
                    if (GUILayout.Button(new GUIContent(augment.Name, itemDef.pickupIconTexture)))
                    {
                        AugmentResolver.TryAddOrUpgradeAugmentToPlayer(firstPlayer.netId, itemDef.itemIndex, augment.AugmentId);
                        Chat.AddMessage($"{firstNetworkUser.userName} unlocked augment {augment.Name}");
                    }
                }
            }

            GUILayout.EndHorizontal();

        }
    }
}
