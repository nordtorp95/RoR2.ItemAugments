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
        private static Dictionary<ItemIndex, Dictionary<string, AugmentBase>> purchasable;

        public static void ShowWindow(int windowID)
        {
            GUILayout.BeginHorizontal();

            firstNetworkUser = PlayerCharacterMasterController.instances[0].networkUser;
            firstPlayer = PlayerCharacterMasterController.instances[0].master;
            purchasable = AugmentShop.GetAvailableItems(firstPlayer.netId);

            foreach (var item in purchasable)
            {
                foreach (var augment in item.Value)
                {
                    var itemDef = ItemCatalog.GetItemDef(item.Key);
                    if (GUILayout.Button(new GUIContent(itemDef.name, itemDef.pickupIconTexture)))
                    {
                        AugmentResolver.TryAddAugmentToPlayer(firstPlayer.netId, itemDef.itemIndex, augment.Value);
                        Chat.AddMessage($"{firstNetworkUser.userName} unlocked augment {augment.Value.Name}");
                    }
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}
