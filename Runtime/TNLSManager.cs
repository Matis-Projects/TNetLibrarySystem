
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
        [Header("Networking")]
        [Tooltip("This is the networking part of the system.")]
        [SerializeField] public TNLS TNLS;

        [Header("Loging System")]
        [Tooltip("This is the log part of the system.")]
        [SerializeField] public TNLSLogingSystem TNLSLogingSystem;

        [Header("Serialization")]
        [Tooltip("This is the networking part of the system.")]
        [SerializeField] public TNLSSerialization TNLSSerialization;

        [Header("Script Manager")]
        [Tooltip("This is the script managment of the system.")]
        [SerializeField] public TNLSScriptManager TNLSScriptManager;

        [Header("Settings")]
        [Tooltip("This is all settings of the system.")]
        [SerializeField] public TNLSSettings TNLSSettings;

        [Header("Queue")]
        [Tooltip("This is the queue of the system.")]
        [SerializeField] public TNLSQueue TNLSQueue;

        [NonSerialized] public bool hasFullyBoot = false;

        #region Initialization
        /// <summary>
        ///     <para>Called when the script has been loaded</para>
        ///     <para>Here we gonna initialize all script.</para>
        /// </summary>
        public void Start()
        {
            if(TNLSSettings != null && TNLS != null && TNLSLogingSystem != null && TNLSScriptManager && TNLSSerialization != null)
            {
                TNLSSettings.Initialize();
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
