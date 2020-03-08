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
    public class ItemAugments : BaseUnityPlugin
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
                
                var index2 = PickupCatalog.FindPickupIndex(ItemIndex.Hoof);
                PickupDropletController.CreatePickupDroplet(index2, trans.position, trans.right * 20f);
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                showAugmentMenu = !showAugmentMenu;
            }
            
            if (Input.GetKeyDown(KeyCode.F4))
            {
                var trans = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                
                var index = PickupCatalog.FindPickupIndex(ItemIndex.TreasureCache);
                PickupDropletController.CreatePickupDroplet(index, trans.position, trans.forward * 40f);

                Xoroshiro128Plus xoroshiro128Plus = new Xoroshiro128Plus((ulong)12);
                GameObject gameObject3 = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscLockbox"), new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    position = trans.position,
                }, xoroshiro128Plus));
                if (gameObject3)
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    ChestBehavior component5 = gameObject3.GetComponent<ChestBehavior>();
                    if (component5)
                    {
                        component5.tier3Chance *= Mathf.Pow((float)10, 2f);
                    }
                }
            }
            
            if (Input.GetKeyDown(KeyCode.F5))
            {
                var scene = RoR2.Run.instance.nextStageScene;
                RoR2.Run.instance.AdvanceStage(scene);
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
            var itemCost = Application.Config.ConfigResolver.ItemCount(itemDef.tier);
            var canAdd = itemCount > 1 && itemCount % itemCost == 0;
            
            if (canAdd)
            {
                var augments = AugmentResolver.ListAugmentsForItem(itemDef.itemIndex);
                if (augments != null && augments.Any())
                {
                    AugmentResolver.TryAddOrUpgradeAugmentToPlayer(networkIdentity,
                        itemDef.itemIndex,
                        augments.FirstOrDefault().Value.Id);
                }
            }

            orig(self,
                body,
                inventory);
        }

      

    }
}