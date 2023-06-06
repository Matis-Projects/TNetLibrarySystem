
using System;
using System.Collections.Specialized;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

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
                if (methodEncoded != "")
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.tnlsOnDeserialization, $"Executing a method");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnDeserialization, $"The method executed: '{methodEncoded}'");
                    Receive(methodEncoded);
                    methodEncoded = "";
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnDeserialization, "The methodEncoded length is 0, ignoring...");
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

            if (currentOwner.Equals(Networking.LocalPlayer) && !tryingToSend && !inSerialization && !ownershipLock && !localExecution && !TNLSManager.TNLSQueue.queueIsExecuting && !waitSerialization || !currentOwner.Equals(Networking.LocalPlayer))
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsOnOwnershipRequest, $"We accept the OwnershipRequest. (requester: {requester.displayName}, newOwner: {newOwner.displayName})");
                if (newOwner.Equals(Networking.LocalPlayer))
                {
                    ownershipLock = true;
                }else if(currentOwner.Equals(Networking.LocalPlayer))
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
            ownershipIsFuckingMine = Networking.GetOwner(gameObject).Equals(Networking.LocalPlayer);
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
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugUnknown, logAuthorList.tnlsOnPostDeserialization, "OnPostSerialization called!");
            methodEncoded = "";
            lockAfterGetIt = false;
            inSerialization = false;
            waitSerialization = false;
            tryingToSend = false;
            TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.tnlsReceive, $"Successfuly finished the sync! {((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds() - TNLSManager.TNLSQueue.queueExecutionTime}ms");
        }
        #endregion

        #region Others
        /// <summary>
        ///     Set the User as the Owner
        /// </summary>
        public bool CAAOwner()
        {
            if (!Networking.IsOwner(gameObject))
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.tnlsCAAOwner, "Transfering the owner to us.");
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                if(Networking.IsOwner(gameObject) )
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.tnlsCAAOwner, "Transfered the owner to us.");
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.tnlsCAAOwner, "Transferring failed!");
                }
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultUnknown, logAuthorList.tnlsCAAOwner, "Why transferring the owning to the LP when LP = Owner?");
            }
            ownershipIsFuckingMine = Networking.GetOwner(gameObject).Equals(Networking.LocalPlayer);
            return Networking.IsOwner(gameObject);
        }

        /// <summary>
        ///     <para>This method will unlock the use of TNLS.</para>
        /// </summary>
        public void UnlockService()
        {
            if (!unlockService)
            {
                unlockService = true;
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.tnlsUnlockService, "TNLS has been successfuly unlocked!");
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.tnlsUnlockService, "No need to unlock TNLS, he is already unlocked!");
            }
        }

        /// <summary>
        ///     <para>Can Receive</para>
        /// </summary>
        public bool checkIfPlayerCanReceive(VRCPlayerApi player, string target)
        {
            bool final = false;
            if (target == "Local") { final = true; }else
            if (target == "All") { final = true; }else
            if (target == "Master" && player.isMaster) { final = true; }else
            if (target.StartsWith("SelectPlayer=") && checkIfPlayerIsInTheCollection(player, target)) { final = true; }
            return final;
        }

        /// <summary>
        ///     <para>This method return a boolean</para>
        /// </summary>
        public bool checkIfPlayerIsInTheCollection(VRCPlayerApi player, string collection)
        {
            string[] collectString = collection.Replace("SelectPlayer=", "").Split('▀');
            if(collectString.Length > 1 ) {
                foreach(string id in collectString )
                {
                    if(Convert.ToInt32(id) == player.playerId)
                    {
                        return true;
                    }
                }
            }
            else if(collectString.Length == 1)
            {
                if (Convert.ToInt32(collectString[0]) == player.playerId)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}

public static class TNLSArrayDefinitions
{
    public static bool Contains<T>(this T[] array, T item) => Array.IndexOf(array, item) != -1;

    public static T[] Add<T>(this T[] array, T item)
    {
        T[] newArray = new T[array.Length + 1];
        Array.Copy(array, newArray, array.Length);
        newArray[array.Length] = item;
        return newArray;
    }

    public static T[] Remove<T>(this T[] array, int index)
    {
        T[] newArray = new T[array.Length - 1];
        Array.Copy(array, newArray, index);
        Array.Copy(array, index + 1, newArray, index, newArray.Length - index);
        return newArray;
    }
}