using System.Collections.Concurrent;
using Application.Augments;
using RoR2;
using UnityEngine.Networking;

namespace Application.Domains
{
    public class PlayerAugments
    {
        public PlayerAugments(NetworkInstanceId clientId,
            ConcurrentDictionary<ItemIndex, ConcurrentDictionary<string, AugmentBase>> augments)
        {
            ClientId = clientId;
            Augments = augments;
        }

        public PlayerAugments(NetworkInstanceId clientId)
        {
            ClientId = clientId;
            Augments = new ConcurrentDictionary<ItemIndex, ConcurrentDictionary<string, AugmentBase>>();
        }

        public NetworkInstanceId ClientId { get;  }
        public ConcurrentDictionary<ItemIndex,ConcurrentDictionary<string,AugmentBase>> Augments { get;  }
    }
}