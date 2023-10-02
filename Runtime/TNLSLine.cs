
using System;
using System.Linq;
using System.Reflection;
using Tismatis.TNetLibrarySystem;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>The Networking Class of the Networking system</para>
    ///     <para>He will call all events by the ScriptManager</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TNLSLine : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;

        [NonSerialized, UdonSynced] public string methodEncoded = "";

        [NonSerialized] public int idLine;
        [NonSerialized] public ushort idMethod;

        [NonSerialized] public bool wantOwnershipLock = false;
        [NonSerialized] public bool localExecution = false;
        [NonSerialized] public bool waitSerialization = false;
        [NonSerialized] public bool inSerialization = false;
        [NonSerialized] public bool lockAfterGetIt = false;
        [NonSerialized] public bool ownershipIsTrapped = false;

        [NonSerialized] public bool inUseByQueue = false;

        #region AllEvent
        /// <summary>
        ///     <para>Called when there are a Deserialization.</para>
        ///     <para>Here is where the player get the method!</para>
        /// </summary>
        public override void OnDeserialization()
        {
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.lineOnDeserialization, "Received a deserialization request.");
            if (TNLSManager.hasFullyBoot)
            {
                if (methodEncoded == "")
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.lineOnDeserialization, "The methodEncoded length is 0, ignoring...");
                }
                else
                {
                    string[] methodTable = methodEncoded.Split('█');
                    TNLSManager.TNLSConfirmPool.broadcastReceive(idLine, ushort.Parse(methodTable[5]));
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.lineOnDeserialization, $"Executing a networked method.");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.lineOnDeserialization, $"The method executed is '{methodTable[0]}'.");
                    TNLSManager.TNLSLinePool.Receive(methodEncoded, this);
                    methodEncoded = "";
                }
            }
            else
            {
                methodEncoded = "";
            }
        }

        /// <summary>
        ///     <para>Called for check if the player can get the ownership</para>
        ///     <para>For obvious reason, we need to say YES all times</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override bool OnOwnershipRequest(VRCPlayerApi requester, VRCPlayerApi newOwner)
        {
            VRCPlayerApi currentOwner = Networking.GetOwner(gameObject);

            if (currentOwner == Networking.LocalPlayer && TNLSManager.TNLSLinePool.getStateLine(this) == currentState.free || currentOwner != Networking.LocalPlayer)
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.lineOnOwnershipRequest, $"We accept the OwnershipRequest. (requester: {requester.displayName}, newOwner: {newOwner.displayName})");
                if (newOwner == Networking.LocalPlayer)
                {
                    wantOwnershipLock = true;
                }else if(currentOwner == Networking.LocalPlayer)
                {
                    ownershipIsTrapped = false;
                }
                return true;
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.lineOnOwnershipRequest, $"We can't accept the OwnershipRequest because someone is executing something.");
                return false;
            }
        }

        /// <summary>
        ///     <para>Called when the ownership change</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if(wantOwnershipLock)
            {
                wantOwnershipLock = false;
            }
            ownershipIsTrapped = Networking.GetOwner(gameObject) == Networking.LocalPlayer;
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.lineOnOwnershipTransferred, $"The line #{idLine} has been transferred to {player.displayName}.");
        }

        public override void OnPreSerialization()
        {
            inSerialization = true;
        }

        /// <summary>
        ///     <para>Called after the serialization has been called</para>
        ///     <para>Here we reset methodEncoded</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override void OnPostSerialization(SerializationResult result)
        {
            TNLSManager.TNLSConfirmPool.passEveryoneToFalse(idLine);

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugUnknown, logAuthorList.lineOnPreSerialization, $"Successfuly serialized the line #{idLine}.");
            methodEncoded = "";
            lockAfterGetIt = false;
            inSerialization = false;
            waitSerialization = false;
            long curTime = TNLSManager.TNLSOthers.CurTime();
            TNLSManager.TNLSConfirmPool.receiveTimeout = curTime + TNLSManager.TNLSSettings.timeBeforeLCexpire;
            TNLSManager.TNLSQueue.lastExecutionTime = curTime - TNLSManager.TNLSQueue.queueExecutionTime;
            TNLSManager.TNLSConfirmPool.broadcastReceive(idLine, idMethod);
            TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.lineOnPostDeserialization, $"Successfuly finished the sync! {TNLSManager.TNLSQueue.lastExecutionTime}ms");
        }
        #endregion
    }
}