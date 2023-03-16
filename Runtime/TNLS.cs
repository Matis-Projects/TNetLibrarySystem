
using System;
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
        [SerializeField] private TNLSManager TNLSManager;
        [SerializeField, UdonSynced] public string methodEncoded = "";
        [SerializeField] public bool executingMethod = false;

        #region TheRealMotorOfThatSystem
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Called when the Networking system receive a new method to execute.</para>
        /// </summary>
        public void Receive(string mE)
        {
            string[] methodTable = mE.Split('█');
            UdonSharpBehaviour network = TNLSManager.TNLSScriptManager.GetScriptByName(methodTable[1]);
            if(network == null)
            {
                TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't find the script id '{methodTable[1]}' with the network '{methodTable[0]}' ! ({mE})");
            }else{
                network.SendCustomEvent(methodTable[0]);
                TNLSManager.TNLSLogingSystem.InfoMessage($"Triggered the network '{methodTable[0]}' in the script id '{methodTable[1]}' !");
                executingMethod = false;
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


            if(target != "Local")
            {
                if(!TNLSManager.TNLSQueue.queueIsRunning && Time.timeSinceLevelLoad - TNLSManager.TNLSQueue.lastSend > 0.1f || !executingMethod)
                {
                    TNLSManager.TNLSQueue.lastSend = Time.timeSinceLevelLoad;

                    TNLSManager.TNLSLogingSystem.DebugMessage($"Want transport: '{methodPreparation}'");
                    TNLSManager.TNLSLogingSystem.DebugMessage("Transferring to us.");
                    CAAOwner();
                    methodEncoded = methodPreparation;

                    TNLSManager.TNLSLogingSystem.DebugMessage("Executing in local the method.");
                    executingMethod = true;
                    Receive(methodPreparation);
                    RequestSerialization();
                }else{
                    TNLSManager.TNLSQueue.InsertInTheQueue(methodPreparation);
                }
            }else{
                Receive(methodPreparation);
            }
        }

        /// <summary>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters()
        {
            string[] methodCut = methodEncoded.Split('█');
            object[] newParameters = TNLSManager.TNLSSerialization.GetParameters(TNLSManager.TNLSSerialization.StringToStringArray(methodCut[3]), TNLSManager.TNLSSerialization.StringToStringArray(methodCut[4]));
            TNLSManager.TNLSLogingSystem.DebugMessage($"{newParameters.Length} parameters has been gived!");
            return newParameters;
        }
        #endregion

        #region AllEventOverride
        /// <summary>
        ///     <para>Called when there are a Deserialization.</para>
        ///     <para>Here is where the player get the method!</para>
        /// </summary>
        public override void OnDeserialization()
        {
            TNLSManager.TNLSLogingSystem.DebugMessage("Received a deserialization request");
            if (methodEncoded != "")
            {
                TNLSManager.TNLSLogingSystem.InfoMessage($"Executing the method '{methodEncoded}'");

                Receive(methodEncoded);
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
            TNLSManager.TNLSLogingSystem.DebugMessage("We accept the OwnershipRequest.");
            return true;
        }

        /// <summary>
        ///     <para>Called when the ownership change</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            TNLSManager.TNLSLogingSystem.DebugMessage("We got the OnOwnershipTransferred!");
        }

        /// <summary>
        ///     <para>Called after the serialization has been called</para>
        ///     <para>Here we reset methodEncoded</para>
        ///     <para>Here a debug endpoint</para>
        /// </summary>
        public override void OnPostSerialization(SerializationResult result)
        {
            TNLSManager.TNLSLogingSystem.DebugMessage("OnPostSerialization called!");
            methodEncoded = "";
        }
        #endregion

        #region Others
        /// <summary>
        ///     Set the User as the Owner
        /// </summary>
        public void CAAOwner()
        {
            if(!Networking.IsOwner(gameObject))
            {
                TNLSManager.TNLSLogingSystem.DebugMessage("Transfering the owner to us.");
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                TNLSManager.TNLSLogingSystem.DebugMessage("Transfered the owner to us.");
            }else{
                TNLSManager.TNLSLogingSystem.DebugMessage("Why transferring the owning to the LP when LP = Owner?");
            }
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