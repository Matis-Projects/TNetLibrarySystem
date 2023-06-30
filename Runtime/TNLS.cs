
using System;
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
    public class TNLS : UdonSharpBehaviour
    {
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this library working.")]
        [SerializeField] private TNLSManager TNLSManager;

        [NonSerialized, UdonSynced] public string methodEncoded = "";
        [NonSerialized] public string lastMethodEncoded = "";

        [NonSerialized] public bool ownershipLock = false;
        [NonSerialized] public bool tryingToSend = false;
        [NonSerialized] public bool localExecution = false;
        [NonSerialized] public bool waitSerialization = false;
        [NonSerialized] public bool inSerialization = false;
        [NonSerialized] public bool lockAfterGetIt = false;
        [NonSerialized] public bool ownershipIsFuckingMine = false;

        [NonSerialized] public bool isFullyReceived = true;
        [NonSerialized] public int receiveCount = 0;
        [NonSerialized] public int receiveCountTarget = 0;
        [NonSerialized] public long receiveTimeout = 0;

        [NonSerialized] public int idMaxCount = 0;

        [NonSerialized] public bool unlockService = false;

        #region TheRealMotorOfThatSystem
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Called when the Networking system receive a new method to execute.</para>
        /// </summary>
        public void Receive(string mE)
        {
            string[] methodTable = mE.Split('█');
            if(checkIfPlayerCanReceive(Networking.LocalPlayer, methodTable[2]))
            {
                UdonSharpBehaviour network = TNLSManager.TNLSScriptManager.GetScriptByName(methodTable[1]);
                if (network == null)
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.tnlsReceive, $"Can't find the script id '{methodTable[1]}' with the network '{methodTable[0]}' !");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.tnlsReceive, $"methodEncoded: {mE}");
                }
                else
                {
                    if(TNLSManager.TNLSSettings.lockBeforeFullyBooted && (unlockService || methodTable[0] == "UnlockService") || !TNLSManager.TNLSSettings.lockBeforeFullyBooted)
                    {
                        network.SendCustomEvent(methodTable[0]);
                        localExecution = false;
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.tnlsReceive, $"Successfuly triggered in local '{methodTable[0]}'!");
                    }
                    else
                    {
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.tnlsReceive, $"We can't execute the current method '{methodTable[0]}' because TNLS hasn't finished to load.");
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.tnlsReceive, $"methodEncoded: {mE}");
                    }
                }
            }else{
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.tnlsReceive, $"methodEncoded: {mE}");
            }
        }

        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Send to the Networking system a request to execute a new method on the Network.</para>
        /// </summary>
        public void SendNetwork(string target, string networkName, string scriptId, object[] args)
        {
            string[][] newParams = TNLSManager.TNLSSerialization.SetParameters(args);
            string methodPreparation = $"{networkName}█{scriptId}█{target}█{TNLSManager.TNLSSerialization.StringArrayToString(newParams[0])}█{TNLSManager.TNLSSerialization.StringArrayToString(newParams[1])}";

            TNLSManager.TNLSQueue.InsertInTheQueue(methodPreparation);
        }

        /// <summary>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters()
        {
            object[] newParameters = new object[0];

            string[] methodCut = methodEncoded.Split('█');
            newParameters = TNLSManager.TNLSSerialization.GetParameters(TNLSManager.TNLSSerialization.StringToStringArray(methodCut[3]), TNLSManager.TNLSSerialization.StringToStringArray(methodCut[4]));

            TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.tnlsGetParameters, $"{newParameters.Length} parameters has been gived!");

            return newParameters;
        }
        #endregion

        #region AllEvent
        /// <summary>
        ///     <para>Called when there are a Deserialization.</para>
        ///     <para>Here is where the player get the method!</para>
        /// </summary>
        public override void OnDeserialization()
        {
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnDeserialization, "Received a deserialization request");
            if (TNLSManager.hasFullyBoot)
            {
                if (methodEncoded == "")
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnDeserialization, "The methodEncoded length is 0, ignoring...");
                }
                else
                {
                    SendCustomNetworkEvent(NetworkEventTarget.Owner, "ConfirmWeReceiveIt");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.tnlsOnDeserialization, $"Executing a method");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnDeserialization, $"The method executed: '{methodEncoded}'");
                    Receive(methodEncoded);
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

            //TNLSManager.TNLSLogingSystem.sendLog(messageType.debugUnknown, logAuthorList.tnlsOnOwnershipRequest, $"tTS: {tryingToSend} iS: {inSerialization} oL: {ownershipLock} lE: {localExecution} qIE: {TNLSManager.TNLSQueue.queueIsExecuting} qIR: {TNLSManager.TNLSQueue.queueIsRunning} newOwner=LocalPlayer {newOwner.Equals(Networking.LocalPlayer)} currentOwner=LocalPlayer: {currentOwner.Equals(Networking.LocalPlayer)} wS: {TNLSManager.TNLS.waitSerialization}");

            if (currentOwner == Networking.LocalPlayer && !tryingToSend && !inSerialization && !ownershipLock && !localExecution && !TNLSManager.TNLSQueue.queueIsExecuting && !waitSerialization || currentOwner != Networking.LocalPlayer)
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnOwnershipRequest, $"We accept the OwnershipRequest. (requester: {requester.displayName}, newOwner: {newOwner.displayName})");
                if (newOwner == Networking.LocalPlayer)
                {
                    ownershipLock = true;
                }else if(currentOwner == Networking.LocalPlayer)
                {
                    ownershipIsFuckingMine = false;
                }
                return true;
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.tnlsOnOwnershipRequest, $"We can't accept the OwnershipRequest because someone is executing something.");
                return false;
            }
        }

        /// <summary>
        ///     <para>Called when the ownership change</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if(ownershipLock)
            {
                ownershipLock = false;
            }
            ownershipIsFuckingMine = Networking.GetOwner(gameObject) == Networking.LocalPlayer;
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.tnlsOnOwnershipTransferred, $"We got the OnOwnershipTransferred!");
        }

        public override void OnPreSerialization()
        {
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugUnknown, logAuthorList.tnlsOnPreSerialization, "OnPreSerialization called!");
            inSerialization = true;
        }

        /// <summary>
        ///     <para>Called after the serialization has been called</para>
        ///     <para>Here we reset methodEncoded</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override void OnPostSerialization(SerializationResult result)
        {
            isFullyReceived = false;
            receiveCount = 0;
            receiveCountTarget = VRCPlayerApi.GetPlayerCount();
            idMaxCount = TNLSManager.TNLSOthers.GetPlayerId(TNLSManager.TNLSOthers.GetAllPlayers()[TNLSManager.TNLSOthers.GetPlayerCount() - 1]);

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugUnknown, logAuthorList.tnlsOnPostDeserialization, "OnPostSerialization called!");
            methodEncoded = "";
            lockAfterGetIt = false;
            inSerialization = false;
            waitSerialization = false;
            tryingToSend = false;
            SendCustomNetworkEvent(NetworkEventTarget.All, "ConfirmWeReceiveIt");
            receiveTimeout = TNLSManager.TNLSOthers.CurTime() + TNLSManager.TNLSSettings.timeBeforeLCexpire;
            long curTime = TNLSManager.TNLSOthers.CurTime();
            TNLSManager.TNLSQueue.lastExecutionTime = curTime - TNLSManager.TNLSQueue.queueExecutionTime;
            TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.tnlsReceive, $"Successfuly finished the sync! {TNLSManager.TNLSQueue.lastExecutionTime}ms");
        }

        public void ConfirmWeReceiveIt()
        {
            if(Networking.GetOwner(gameObject) == Networking.LocalPlayer)
            {
                receiveCount++;
                int tg = TNLSManager.TNLSOthers.GetPlayerCount();
                if (0 >= (tg - receiveCount))
                {
                    isFullyReceived = true;
                    receiveTimeout = 0;
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.confirmWeReceiveIt, $"Everyone has received the current method!");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugUnknown, logAuthorList.confirmWeReceiveIt, $"tg: {tg}, rC: {receiveCount}, total: {tg - receiveCount}");
                }
                else if (TNLSManager.TNLSSettings.timeBeforeLCexpire != -1 && TNLSManager.TNLSOthers.CurTime() > receiveTimeout && receiveTimeout != 0)
                {
                    isFullyReceived = true;
                    receiveTimeout = 0;
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.confirmWeReceiveIt, $"{tg - receiveCount} players didn't receive the current method! We skip it.");
                }
                else if (TNLSManager.TNLSSettings.timeBeforeLCexpire == -1 && TNLSManager.TNLSOthers.CurTime() > (receiveTimeout + 30000) && receiveTimeout != 0)
                {
                    isFullyReceived = true;
                    receiveTimeout = 0;
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.confirmWeReceiveIt, $"HARD LIMIT TOUCHED! {tg - receiveCount} players didn't receive the current method! We skip it.");
                }
                else if(receiveTimeout == 0 && isFullyReceived)
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.confirmWeReceiveIt, $"Some magic made one player send it after everything!");
                }
                //TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.confirmWeReceiveIt, $"RECEIVED FOR {lastMethodEncoded}");
            }
        }
        #endregion

        #region Others
        /// <summary>
        ///     Set the User as the Owner
        /// </summary>
        public bool CAAOwner()
        {
            if (Networking.IsOwner(gameObject))
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultUnknown, logAuthorList.tnlsCAAOwner, "Why transferring the owning to the LP when LP = Owner?");
            }
            else {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsCAAOwner, "Transfering the owner to us.");
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                if (Networking.IsOwner(gameObject))
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.tnlsCAAOwner, "Transfered the owner to us.");
                }
                else {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.tnlsCAAOwner, "Transferring failed!");
                }
            }
            ownershipIsFuckingMine = Networking.GetOwner(gameObject) == Networking.LocalPlayer;
            return Networking.IsOwner(gameObject);
        }

        /// <summary>
        ///     <para>This method will unlock the use of TNLS.</para>
        /// </summary>
        public void UnlockService()
        {
            if (unlockService)
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.tnlsUnlockService, "No need to unlock TNLS, he is already unlocked!");
            }
            else
            {
                unlockService = true;
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.tnlsUnlockService, "TNLS has been successfuly unlocked!");
            }
        }

        /// <summary>
        ///     <para>Can Receive</para>
        /// </summary>
        public bool checkIfPlayerCanReceive(VRCPlayerApi player, string target)
        {
            switch(target.ToUpper())
            {
                case "LOCAL":
                    return true;
                case "ALL":
                    return true;
                case "MASTER":
                    return player.isMaster;
                default:
                    return target.StartsWith("SelectPlayer=") && checkIfPlayerIsInTheCollection(player, target);
            }
        }

        /// <summary>
        ///     <para>This method return a boolean</para>
        /// </summary>
        public bool checkIfPlayerIsInTheCollection(VRCPlayerApi player, string collection)
        {
            string[] collectString = collection.Replace("SelectPlayer=", "").Split('▀');
            if (collectString.Length >= 1)
            {
                foreach (string id in collectString)
                {
                    if (Convert.ToInt32(id) == player.playerId)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}