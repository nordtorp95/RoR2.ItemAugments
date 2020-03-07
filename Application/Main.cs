using System.Linq;
using Application.Services;
using BepInEx;
using RoR2;
using UnityEngine;

namespace Application
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Nordtorp.ItemTalentTree",
        "Item talent tree",
        "0.1.0")]
    public class MyModName : BaseUnityPlugin
    {

        public void Awake()
        {
            On.RoR2.GenericPickupController.GrantItem += InventoryHook;
        }
        
        public void Update()
        {
            //This if statement checks if the player has currently pressed F2, and then proceeds into the statement:
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var index = PickupCatalog.FindPickupIndex(ItemIndex.Mushroom);
                var trans = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                PickupDropletController.CreatePickupDroplet(index, trans.position, trans.forward * 20f);
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                var firstPlayer = PlayerCharacterMasterController.instances[0].master.netId;
                var purchasable = AugmentShop.GetAvailableItems(firstPlayer);
                var purchaseString = "Available augments: ";
                foreach (var item in purchasable)
                {
                    var augmentString = "";
                    foreach (var augment in item.Value)
                    {
                        augmentString += $", {augment.Value.Name}";
                    }
                    purchaseString += $"Item: {item.Key}, Augments: {augmentString}";
                }
                Chat.AddMessage(purchaseString);
            }
        }

        private static void InventoryHook(On.RoR2.GenericPickupController.orig_GrantItem orig,
            GenericPickupController self,
            CharacterBody body,
            Inventory inventory)
        {
            var networkIdentity = body.master.netId;
            var item = PickupCatalog.GetPickupDef(self.pickupIndex);
            var itemDef = ItemCatalog.GetItemDef(item.itemIndex);

            var itemCount = inventory.GetItemCount(itemDef.itemIndex);
            if (itemCount >= Application.Config.ConfigResolver.ItemCount(itemDef.tier) - 1)
            {
                var augments = AugmentResolver.ListAugmentsForItem(itemDef.itemIndex);
                // AugmentResolver.TryAddAugmentToPlayer(networkIdentity,
                //     itemDef.itemIndex,
                //     augments.FirstOrDefault().Value);
            }

            orig(self,
                body,
                inventory);
        }

      

    }
}