
using System;
using UdonSharp;
using UnityEditor;
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
        [SerializeField] public TNLSLinePool TNLSLinePool;

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

        [Header("ConfirmPool")]
        [Tooltip("This is the pool for confirm we received the request.")] // btw, a potential switch into byte will conduct us to create another pool for had multiple line of network (Oh spoilers ^^')
        [SerializeField] public TNLSConfirmPool TNLSConfirmPool;

        [Header("Others")]
        [Tooltip("This is the class required for make working the system.")]
        [SerializeField] public TNLSOthers TNLSOthers;

        [Header("Version")] // RAPPEL: Para alterar a versao, você deve passar pela funçao inicial do script. Le boloss qui traduit ça pour passer au travers des mises à jours, je fais du carglouch avec sa tête.
        [NonSerialized] public string currentVersion = TNLSVersion.getVersion();
        [NonSerialized] public string currentBranch = TNLSVersion.getBranch();
        [NonSerialized] public string descriptionVersion = TNLSVersion.getDescription();
        [NonSerialized] public bool hasFullyBoot = false;

        #region Initialization
        /// <summary>
        ///     <para>Called when the script has been loaded</para>
        ///     <para>Here we gonna initialize all script.</para>
        /// </summary>
        public void Start()
        {
            currentVersion = TNLSVersion.getVersion();
            currentBranch = TNLSVersion.getBranch();
            descriptionVersion = TNLSVersion.getDescription();

            if (TNLSSettings != null && TNLSLinePool != null && TNLSLogingSystem != null && TNLSScriptManager != null && TNLSSerialization != null && TNLSOthers != null && TNLSConfirmPool != null)
            {
                TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, "Loading settings...");
                TNLSSettings.Initialize();
                TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "Loaded settings!");

                TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, "Creating line pool...");
                TNLSLinePool.Initialize();
                TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "The line pool has been created!");

                TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, "Creating receiving pool...");
                TNLSConfirmPool.Initialize();
                TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "The receiving pool has been created!");

                TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "The system has been started!");

                hasFullyBoot = true;

                TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.managerStart, $"The current version is from the {currentVersion} in the branch {currentBranch}!");
                switch (currentBranch.ToUpper())
                {
                    case "BETA":
                        TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.managerStart, "This branch isn't very stable, please think about switch to the release branch.");
                        break;
                    case "DEV":
                        TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.managerStart, "This branch is a dev one, please switch to the release branch when you can.");
                        break;
                    case "RELEASE":
                        TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.managerStart, "You are on the more stable branch!");
                        break;
                    default:
                        TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.managerStart, "Your branch can't be define! Please check if you are using the version from the github.");
                        break;
                }
                TNLSLogingSystem.sendLog(messageType.defaultUnknown, logAuthorList.managerStart, $"Description of that version: {descriptionVersion}");
            } else {
                Debug.LogError($"#<color=#ff3232>ERROR</color> <color=#3264ff>[TNLS~managerStart]</color> The system can't be start! (Check the prefab and check if all is linked)");
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
            TNLSLinePool.SendNetwork(target, networkName, scriptId.ToString(), args);
        }

        /// <summary>
        ///     <strong>ALIAS</strong>
        ///     <para>With this one, you call by the ScriptName.</para>
        /// </summary>
        public void CallNamedNetworkedScript(string target, string networkName, string scriptName, object[] args)
        {
            int ScriptId = TNLSScriptManager.GetScriptIdByName(scriptName);
            if (ScriptId == -1)
            {
                TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.managerCNNS, $"Can't find the Script to call! ('{scriptName}')");
            } else {
                TNLSLinePool.SendNetwork(target, networkName, scriptName, args);
            }
        }
        #endregion
    }

    public class TNLSVersion
    {
        public static string getVersion()
        {
            return "21-07-2023";
        }

        public static string getBranch()
        {
            return "stable";
        }

        public static string getDescription()
        {
            return "MultiLanes fully added.";
        }
    }
}
