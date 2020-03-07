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

        public GameObject ModCanvas = null;

        public bool showAugmentMenu = false;

        public void Awake()
        {
            On.RoR2.GenericPickupController.GrantItem += InventoryHook;
        }

        void OnGUI()
        {
            if (showAugmentMenu)
            {
                GUILayout.Window(0, new Rect(50, 50, 500, 500), UI.AugmentsWindow.ShowWindow, "Augments");
            }
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
                showAugmentMenu = !showAugmentMenu;
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