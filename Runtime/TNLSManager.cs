
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
        [SerializeField] public bool debugMode;
        [SerializeField] public int maxNetList = 50;
        [SerializeField] public int maxParams = 25;
        [NonSerialized] public bool hasFullyBoot = false; 

        #region Initialization
        public void Start()
        {
            if(TNLS != null && TNLSLogingSystem != null && TNLSScriptManager && TNLSSerialization != null)
            {
                TNLSLogingSystem.InfoMessage("The system has been started!");
                hasFullyBoot = true;
            }else{
                Debug.LogError($"#<color=#ff3232>ERROR</color> <color=#3264ff>[TNLS]</color> The system can't be start! (Check the prefab and check if all is linked)");
            }
        }
        #endregion
        
        #region AliasToCall
        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you can call by the selected Name script.</para>
        /// </summary>
        public int AddANamedNetworkedScript(string name, UdonSharpBehaviour script)
        {
            return TNLSScriptManager.AddANetworkedScript(name, script);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you can call by the ScriptId.</para>
        /// </summary>
        public int AddAIdNetworkedScript(int scriptId, UdonSharpBehaviour script)
        {
            return TNLSScriptManager.AddANetworkedScript(scriptId.ToString(), script);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you call by the ScriptId.</para>
        /// </summary>
        public void CallNetworkedScript(string target, string networkName, int scriptId, object[] args)
        {
            TNLS.SendNetwork(target, networkName, scriptId.ToString(), args);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you call by the ScriptName.</para>
        /// </summary>
        public void CallNamedNetworkedScript(string target, string networkName, string scriptName, object[] args)
        {
            int ScriptId = TNLSScriptManager.GetScriptIdByName(scriptName);
            if(ScriptId == -1)
            {
                TNLSLogingSystem.ErrorMessage($"Can't find the Script to call! ('{scriptName}')");
            }else{
                TNLS.SendNetwork(target, networkName, scriptName, args);
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
