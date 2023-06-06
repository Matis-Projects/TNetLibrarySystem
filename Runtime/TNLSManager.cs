
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

        [NonSerialized] public string currentVersion = "03/04/2023 at (UTC)21:20";
        [NonSerialized] public string currentBranch = "dev";
        [NonSerialized] public string descriptionVersion = "Another patch to fix every bugs with ownership.";
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
                TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, "Loading settings...");
                TNLSSettings.Initialize();
                TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "Loaded settings!");

                if(TNLSSettings.lockBeforeFullyBooted)
                {
                    TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, "Adding TNLS to the script list...");
                    AddANamedNetworkedScript("TNLS", TNLS);
                    TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "Added TNLS to the script list!");
                }
                else
                {
                    TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, "You didn't toggle the security when someone join.");
                }

                TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "The system has been started!");

                hasFullyBoot = true;

                TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, $"The current version is from the {currentVersion} in the branch {currentBranch}!");
                if(currentBranch == "beta")
                {
                    TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.managerStart, "This branch isn't very stable, please think about switch to the release branch.");
                }else if(currentBranch == "dev")
                {
                    TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.managerStart, "This branch is a dev one, please switch to the release branch when you can.");
                }else if(currentBranch == "release")
                {
                    TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "You are on the more stable branch!");
                }
                TNLSLogingSystem.sendLog(messageType.defaultUnknown, logAuthorList.managerStart, $"Description of that version: {descriptionVersion}");
            }else{
                Debug.LogError($"#<color=#ff3232>ERROR</color> <color=#3264ff>[TNLS~managerStart]</color> The system can't be start! (Check the prefab and check if all is linked)");
            }
        }

        /// <summary>
        ///     <para>Called when a player join the game</para>
        ///     <para>Here we gonna call the first method to unlock TNLS</para>
        /// </summary>
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (hasFullyBoot)
            {
                if(TNLSSettings.lockBeforeFullyBooted)
                {
                    if (Networking.LocalPlayer.isMaster)
                    {
                        TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerPlayerJoin, "Sent key to unlock a player.");
                        CallNamedNetworkedScript($"SelectPlayer={player.playerId}", "UnlockService", "TNLS", new object[] { });
                    }
                }
            }
            else
            {
                TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.managerPlayerJoin, "The player has join before TNLS has fully booted!");
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
                TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.managerCNNS, $"Can't find the Script to call! ('{scriptName}')");
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
