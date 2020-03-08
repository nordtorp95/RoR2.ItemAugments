using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace Application.Augments.RustyKey
{
    public class BoxSizeScaling : AugmentBase
    {
        private static bool _isActive;
        private const float Scale = 1.1f;
        public override string Name => "Box size scaler";
        public override string Description => $"Each point adds to the box's size by {Scale}";
        public override ItemIndex ItemType => ItemIndex.TreasureCache;
        public override AugmentId Id => new AugmentId(nameof(BoxSizeScaling));
        public override int MaxLevel => 10;
        protected override bool GlobalLevels => true;

        public override void Activate()
        {
            if (_isActive) return;
            _isActive = true;
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }

        private void SceneDirector_PopulateScene(ILContext il)
        {
            var c = new ILCursor(il);
            var lockbox = 0;
            var publicInstance = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

            c.GotoNext(
                x => x.MatchLdstr("SpawnCards/InteractableSpawnCard/iscLockbox")
            );
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallvirt(typeof(RoR2.DirectorCore).GetMethod("TrySpawnObject",
                    publicInstance)),
                x => x.MatchStloc(out lockbox),
                x => x.MatchLdloc(lockbox),
                x => x.MatchCall(out _), //Match any call instruction
                x => x.MatchBrfalse(out _), //match any BrFalse instruction
                x => x.MatchLdloc(lockbox)
            );
            c.Emit(OpCodes.Ldloc,
                lockbox);
            c.EmitDelegate<RuntimeILReferenceBag.FastDelegateInvokers.Action<GameObject>>( //
                (box) =>
                {
                    var a = Scale * Levels.Sum(x => x.Value);
                    Chat.AddMessage($"Scale: {a}");
                    box.transform.localScale *= a;
                }
            );
        }
    }
}