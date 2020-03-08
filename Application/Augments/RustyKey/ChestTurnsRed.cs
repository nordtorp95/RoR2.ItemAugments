using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace Application.Augments.RustyKey
{
    public class ChestTurnsRed : AugmentBase
    {
        private static bool _isActive;
        private static Material _material;
        public override string Name => "Red Chests";
        public override string Description => "Hidden caches are red instead of brown";
        public override ItemIndex ItemType => ItemIndex.TreasureCache;
        public override AugmentId Id => new AugmentId(nameof(ChestTurnsRed));
        public override int MaxLevel => 1;

        public override void Activate()
        {
            if (_isActive) return;
            _isActive = true;
            IL.RoR2.SceneDirector.PopulateScene += SceneDirector_PopulateScene;
        }
        
        private static void SceneDirector_PopulateScene(ILContext il)
        {
            var c = new ILCursor(il);
            var lockbox = 0;
            var publicInstance = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            c.GotoNext(
                MoveType.After,
                x => x.MatchCallvirt(typeof(RoR2.DirectorCore).GetMethod("TrySpawnObject", publicInstance)),
                x => x.MatchStloc(out lockbox),
                x => x.MatchLdloc(lockbox),
                x => x.MatchCall(out _),//Match any call instruction
                x => x.MatchBrfalse(out _),//match any BrFalse instruction
                x => x.MatchLdloc(lockbox)
            );
            c.Emit(OpCodes.Ldloc, lockbox);
            c.EmitDelegate<Action<GameObject>>(//
                (box) =>
                {
                    var renderer = box.GetComponent<MeshRenderer>();
                    if (_material == null)
                    {
                        _material = new Material(renderer.material);
                        _material.color = Color.red;
                    }

                    renderer.material = _material;
                }
            );
        }
    }
}