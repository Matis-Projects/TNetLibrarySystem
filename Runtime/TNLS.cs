
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
        [SerializeField, UdonSynced] private string methodEncoded = "";
        [SerializeField] private object[] Parameters = null;
        
        private bool QueueIsRunning = false;
        private object[][] QueueItems = new object[50][];
        private float lastSend = 0f;

        #region TheRealMotorOfThatSystem
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Called when the Networking system receive a new method to execute.</para>
        /// </summary>
        public void Receive(string mE)
        {
            string[] mttable = mE.Split('█');
            UdonSharpBehaviour network = TNLSManager.TNLSScriptManager.GetScriptById(int.Parse(mttable[1]));
            if(network == null)
            {
                TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't find the network {mttable[0]} ! ({mE})");
            }else{
                Parameters = TNLSManager.TNLSSerialization.StrToParameters(mttable[2]);
                network.SendCustomEvent(mttable[0]);
                TNLSManager.TNLSLogingSystem.InfoMessage($"Triggered the network {mttable[0]} in the script id {mttable[1]} with args '{mttable[2]}'");
            }
        }

        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Send to the Networking system a request to execute a new method on the Network.</para>
        /// </summary>
        public void SendNetwork(string Target, string NetworkName, int ScriptId, object[] args)
        {
            string tmp = $"{NetworkName}█{ScriptId}█{TNLSManager.TNLSSerialization.ParametersToStr(args)}█Target";
            
            bool isLocal = false;

            if (Target == "Local")
            {
                isLocal = true;
                Receive(tmp);
            }

            if (Target != "Others")
            {
                Receive(tmp);
            }
            
            if(!isLocal)
            {
                if(!QueueIsRunning && Time.timeSinceLevelLoad - lastSend > 0.1f)
                {
                    lastSend = Time.timeSinceLevelLoad;
                    TNLSManager.TNLSLogingSystem.DebugMessage($"Want transport: '{tmp}'");
                    TNLSManager.TNLSLogingSystem.DebugMessage($"Transferring to us.");
                    CAAOwner();
                    methodEncoded = tmp;
                    RequestSerialization();
                }else{
                    InsertInTheQueue(tmp);
                }
            }
        }

        /// <summary>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters()
        {
            object[] tmp = Parameters;
            Parameters = null;
            TNLSManager.TNLSLogingSystem.DebugMessage($"Parameters has been gived, removed!");
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
        /// </summary>
        public override bool OnOwnershipRequest(VRCPlayerApi requester, VRCPlayerApi newOwner)
        {
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
        /// </summary>
        public override void OnPostSerialization(SerializationResult result)
        {
            methodEncoded = "";
        }
        #endregion

        #region Queue
        public void InsertInTheQueue(string mE)
        {
            QueueItems = QueueItems.Add(new object[] {mE});
            TNLSManager.TNLSLogingSystem.InfoMessage($"Queue is on! Passing '{mE}' to the waiting list!");
            if(!QueueIsRunning)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.1f);
                QueueIsRunning = true;
            }
        }
        public void UpdateTheQueue()
        {
            if(QueueItems.Length != 0)
            {
                var Current = QueueItems[0];
                QueueItems = QueueItems.Remove(0);

                methodEncoded = (string) Current[0];

                TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Migration of {methodEncoded}!");

                CAAOwner();
                RequestSerialization();
            }

            if(QueueItems.Length != 0)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.1f);
            }else{
                QueueIsRunning = false;
            }
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
                TNLSManager.TNLSLogingSystem.DebugMessage("Transfered the owning to LP!");
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                TNLSManager.TNLSLogingSystem.DebugMessage($"Transfered the owner to us.");
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