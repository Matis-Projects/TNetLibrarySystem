
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>The Main Class of the Networking system</para>
    ///     <para>He make shortcut for call method</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSManager : UdonSharpBehaviour
    {
        [Header("Link to others scripts")]
        [SerializeField] public TNLS TNLS;
        [SerializeField] public TNLSLogingSystem TNLSLogingSystem;
        [SerializeField] public TNLSSerialization TNLSSerialization;
        [SerializeField] public TNLSScriptManager TNLSScriptManager;
        [SerializeField] public TNLSQueue TNLSQueue;
        [Header("Settings")]
        [SerializeField] public bool DebugMode;
        [SerializeField] public int MaxNetList = 50;
        [NonSerialized] public bool HasFullyBoot = false; 

        #region Initialization
        public void Start()
        {
            if(TNLS != null && TNLSLogingSystem != null && TNLSScriptManager && TNLSSerialization != null)
            {
                HasFullyBoot = true;
                Debug.Log($"<color=#5032ff>INFO</color> <color=#3264ff>[TNLS]</color> The system has been started!");
            }else{
                Debug.LogError($"<color=#ff3232>ERROR</color> <color=#3264ff>[TNLS]</color> The system can't be start! (Check the prefab and check if all is linked)");
            }
        }
        #endregion
        
        #region AliasToCall
        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you can call by the selected Name script.</para>
        /// </summary>
        public int AddANamedNetworkedScript(string Name, UdonSharpBehaviour USB)
        {
            return TNLSScriptManager.AddANetworkedScript(Name, USB);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you can call by the ScriptId.</para>
        /// </summary>
        public int AddAIdNetworkedScript(int ScriptId, UdonSharpBehaviour USB)
        {
            return TNLSScriptManager.AddANetworkedScript(ScriptId.ToString(), USB);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you call by the ScriptId.</para>
        /// </summary>
        public void CallNetworkedScript(string Target, string NetworkName, int ScriptId, object[] args)
        {
            TNLS.SendNetwork(Target, NetworkName, ScriptId.ToString(), args);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you call by the ScriptName.</para>
        /// </summary>
        public void CallNamedNetworkedScript(string Target, string NetworkName, string ScriptName, object[] args)
        {
            int ScriptId = TNLSScriptManager.GetScriptIdByName(ScriptName);
            if(ScriptId == -1)
            {
                TNLSLogingSystem.ErrorMessage($"Can't find the Script to call! ({ScriptName})");
            }else{
                TNLS.SendNetwork(Target, NetworkName, ScriptName, args);
            }
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters()
        {
            return TNLS.GetParameters();
        }
        #endregion
    }
}
