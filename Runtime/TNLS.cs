
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Tismatis.TNetLibrarySystem
{
    /*
        TODO: Add a queue system
    */

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
            if (Target != "Local")
            {
                TNLSManager.TNLSLogingSystem.DebugMessage($"Want transport: '{tmp}'");
                TNLSManager.TNLSLogingSystem.DebugMessage($"Transferring to us.");
                CAAOwner();
                methodEncoded = tmp;
                Receive(tmp);
                RequestSerialization();
            }
            else
            {
                Receive(tmp);
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