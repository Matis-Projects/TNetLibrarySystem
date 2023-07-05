
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerConfirmReceived : UdonSharpBehaviour
    {
        [NonSerialized,UdonSynced] public bool received = false;
        [NonSerialized] public int lastRequest = 0;

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            return true;
        }

        public override void OnPostSerialization(SerializationResult result)
        {
            Debug.Log($"Sended ! status: {result.success} / {result.byteCount} / {gameObject.name}");
        }

        public override void OnDeserialization()
        {
            Debug.Log($"DESERIALIZED {gameObject.name}");
        }
    }
}
