using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine.Networking;

namespace Application.Augments
{
    public abstract class AugmentBase
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ItemIndex ItemType { get; }
        public abstract AugmentId Id { get; }
        protected IDictionary<NetworkInstanceId,int> Levels { get; } = new Dictionary<NetworkInstanceId, int>();
        public abstract int MaxLevel { get;  }
        protected abstract bool GlobalLevels { get; }
        public abstract void Activate();

        public int GetLevel(NetworkInstanceId id)
        {
            return Levels.TryGetValue(id,
                out var level) ? level : 0;
        }

        public bool TryLevelUp(NetworkInstanceId id)
        {
            if (Levels.TryGetValue(id,
                out var augmentLevel))
            {
                if (augmentLevel < MaxLevel)
                {
                    Levels[id] = augmentLevel+1;
                    return true;
                }
            }
            else
            {
                Levels.Add(id, Math.Min(1,MaxLevel));
            }
            return false;
        }
    }
}