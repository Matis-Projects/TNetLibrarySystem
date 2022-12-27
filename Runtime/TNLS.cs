
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
        [SerializeField, UdonSynced] public string[] ListParams = new string[0];
        [SerializeField, UdonSynced] public string[] ValParams = new string[0];

        #region TheRealMotorOfThatSystem
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Called when the Networking system receive a new method to execute.</para>
        /// </summary>
        public void Receive(string mE)
        {
            string[] mttable = mE.Split('█');
            UdonSharpBehaviour network = TNLSManager.TNLSScriptManager.GetScriptByName(mttable[1]);
            if(network == null)
            {
                TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't find the network {mttable[1]} with the func {mttable[0]} ! ({mE})");
            }else{
                network.SendCustomEvent(mttable[0]);
                TNLSManager.TNLSLogingSystem.InfoMessage($"Triggered the network {mttable[0]} in the script id {mttable[1]} !");
            }
        }

        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Send to the Networking system a request to execute a new method on the Network.</para>
        /// </summary>
        public void SendNetwork(string Target, string NetworkName, string ScriptId, object[] args)
        {
            string tmp = $"{NetworkName}█{ScriptId}█{Target}";

            if(Target != "Local")
            {
                if(!TNLSManager.TNLSQueue.QueueIsRunning && Time.timeSinceLevelLoad - TNLSManager.TNLSQueue.lastSend > 0.1f)
                {
                    TNLSManager.TNLSQueue.lastSend = Time.timeSinceLevelLoad;
                    TNLSManager.TNLSLogingSystem.DebugMessage($"Want transport: '{tmp}'");
                    TNLSManager.TNLSLogingSystem.DebugMessage($"Transferring to us.");
                    CAAOwner();
                    methodEncoded = tmp;
                    string[][] newParams = TNLSManager.TNLSSerialization.SetParameters(args);
                    
                    ListParams = newParams[0];
                    ValParams = newParams[1];

                    Receive(tmp);
                    RequestSerialization();
                }else{
                    TNLSManager.TNLSQueue.InsertInTheQueue(tmp);
                }
            }else{
                Receive(tmp);
            }
        }

        /// <summary>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters()
        {
            object[] tmp = TNLSManager.TNLSSerialization.GetParameters(ListParams, ValParams);
            ListParams = new string[0];
            ValParams = new string[0];
            TNLSManager.TNLSLogingSystem.DebugMessage($"Parameters has been gived, removed! {ListParams.Length} vs {tmp.Length}");
            return tmp;
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
                TNLSManager.TNLSLogingSystem.InfoMessage($"Executing the method {methodEncoded}");
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