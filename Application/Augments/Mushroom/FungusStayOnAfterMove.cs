using System;
using System.Diagnostics;
using Application.Services;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace Application.Augments.Mushroom
{
    public class FungusStayOnAfterMove : AugmentBase
    {
        private static Stopwatch _stopwatch;
        private static bool _isActive;
        private const float DurationMilliSeconds = 1000;

        public override string Name => "Movement buffer";
        public override string Description => "Shroom stays active for a period after moving";
        public override ItemIndex ItemType => ItemIndex.Mushroom;
        public override AugmentId Id => new AugmentId(nameof(FungusStayOnAfterMove));
        public override int MaxLevel => 1;

        public override void Activate()
        {
            if (_isActive) return;
            _isActive = true;
            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += (ILContext.Manipulator) (il =>
            {
                var ilCursor = new ILCursor(il);
                ilCursor.GotoNext(MoveType.Before,
                    (Func<Instruction, bool>) (x => x.MatchCallvirt<RoR2.CharacterBody>("GetNotMoving")));
                ilCursor.Remove();
                ilCursor.EmitDelegate<Func<CharacterBody, bool>>((cb) =>
                {
                    if (!AugmentResolver.IsAugmentActiveForPlayer(ItemIndex.Crowbar,
                        Id, 
                        cb.master.netId))
                    {
                        return cb.GetNotMoving();
                    }
                    if (_stopwatch == null)
                    {
                        _stopwatch = new Stopwatch();
                        _stopwatch.Start();
                        return false;
                    }

                    var timeSinceMovement = cb.GetFieldValue<float>("notMovingStopwatch");
                    if (timeSinceMovement >= 2)
                    {
                        _stopwatch.Restart();
                        return true;
                    }

                    if (!(_stopwatch.ElapsedMilliseconds <= DurationMilliSeconds)) return false;
                    if (timeSinceMovement > 0)
                    {
                        _stopwatch.Restart();
                    }
                    return true;
                });
            });

            On.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate +=
                (On.RoR2.CharacterBody.MushroomItemBehavior.hook_FixedUpdate) ((orig,
                    self) =>
                {
                    orig.Invoke(self);
                    var fieldValue = self.GetFieldValue<GameObject>("mushroomWard");
                    if (fieldValue != null)
                    {
                        fieldValue.transform.position = self.body.footPosition;
                    }
                });
        }
    }
}