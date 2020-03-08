using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace Application.Augments.RustyKey
{
    public class AdditionalChestSpawn : AugmentBase
    {
        private static bool _isActive;
        public override string Name => "Extra hidden cache";
        public override string Description => "Each levels spawns and additional hidden cache on stage spawn";
        public override ItemIndex ItemType => ItemIndex.TreasureCache;
        public override AugmentId Id => new AugmentId(nameof(AdditionalChestSpawn));
        public override int MaxLevel => 5;
        protected override bool GlobalLevels => true;
        public override void Activate()
        {
            if (_isActive) return;
            _isActive = true;
            
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }
        
        private void SceneDirector_PopulateScene(ILContext il)
        {
            //todo?
            // var c = new ILCursor(il);
            // var publicInstance = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            //
            // c.GotoNext(
            //     x => x.MatchLdstr("SpawnCards/InteractableSpawnCard/iscLockbox")
            // );
            // c.GotoNext(
            //     MoveType.After,
            //     x => x.MatchCallvirt(typeof(RoR2.DirectorCore).GetMethod("TrySpawnObject",
            //         publicInstance))
            // );
            // c.GotoNext(MoveType.Before,x => x.MatchRet());
            // c.GotoPrev(x => x.MatchLdstr("SpawnCards/InteractableSpawnCard/iscLockbox"));
        }
    }
}