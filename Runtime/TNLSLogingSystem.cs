
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    public enum messageType
    {
        debugInfo,
        debugError,
        debugSuccess,
        debugWarn,
        debugUnknown,
        defaultInfo,
        defaultError,
        defaultSuccess,
        defaultWarn,
        defaultUnknown
    }
    public enum logAuthorList
    {
        tnlsReceive,
        tnlsGetParameters,
        tnlsOnDeserialization,
        tnlsOnOwnershipRequest,
        tnlsOnOwnershipTransferred,
        tnlsOnPreSerialization,
        tnlsOnPostDeserialization,
        tnlsCAAOwner,
        tnlsUnlockService,
        managerStart,
        managerPlayerJoin,
        managerCNNS,
        queueIITQ,
        queueUpdate,
        scriptManagerAANS,
        scriptManagerPlayerJoin,
        serializationSetParameters,
        serializationGetParameters,
        serializationSerializeGetValue,
        serializationDeserializeObject,
        anstnStart,
        settingsInitialiaztion,
        confirmWeReceiveIt
    }
    /// <summary>
    ///     <para>The LoggingSystem of the Networking system</para>
    ///     <para>He is the loging system</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSLogingSystem : UdonSharpBehaviour
    {
        [Header("Manager")]

        [Tooltip("This is the TNLS Manager, he is required for make this library working.")]
        [SerializeField] private TNLSManager TNLSManager;


        [Header("External Log")]

        [Tooltip("If you want a debug text, put here your UnityEngine.Text .")]
        [SerializeField] private Text text;

        [Tooltip("Put here the number of line before the auto clear .")]
        [SerializeField] private int maxLine = 40;

        [NonSerialized] private int currentLine;
        [NonSerialized] private bool allowDebug;

        public void Start()
        {
            if (TNLSManager.TNLSSettings.debugWhitelist.Length > 0)
            {
                string localName = Networking.LocalPlayer.displayName;
                foreach (string whitelistItem in TNLSManager.TNLSSettings.debugWhitelist)
                {
                    if (localName == whitelistItem)
                    {
                        allowDebug = true;
                        break;
                    }
                }
            }
            else {
                allowDebug = true;
            }
        }

        /// <summary>
        ///     Send into the log.
        ///     
        /// </summary>
        public void sendLog(messageType type, logAuthorList author, string message)
        {
            if(TNLSManager.TNLSSettings.enableLog || author == logAuthorList.managerStart)
            {
                string text = "";
                string tmp = "";

                int typeInt = Convert.ToInt32(type);
                if (typeInt <= 4)
                {
                    switch (typeInt)
                    {
                        case 0:
                            tmp = "<color=#5032ff>INFO</color>";
                            break;
                        case 1:
                            tmp = "<color=#ff3232>ERROR</color>";
                            break;
                        case 2:
                            tmp = "<color=#4dff32>SUCCESS</color>";
                            break;
                        case 3:
                            tmp = "<color=#ff9632>WARN</color>";
                            break;
                        default:
                            tmp = "<color=#464646>UNKNOWN</color>";
                            break;
                    }
                    text = $"<color=#3264ff>[</color><color=#e132ff>DEBUG</color><color=#3264ff>~</color>{tmp}<color=#3264ff>]</color>";
                    if (TNLSManager.TNLSSettings.debugMode && allowDebug)
                    {
                        Debug.LogError($"#{text} <color=#3264ff>[TNLS~{author}]</color> {message}");
                    }
                    UpdateText($"#{text} <color=#3264ff>[TNLS~{author}]</color> {message}");
                }
                else {
                    switch (typeInt)
                    {
                        case 5:
                            tmp = "<color=#5032ff>INFO</color>";
                            break;
                        case 6:
                            tmp = "<color=#ff3232>ERROR</color>";
                            break;
                        case 7:
                            tmp = "<color=#4dff32>SUCCESS</color>";
                            break;
                        case 8:
                            tmp = "<color=#ff9632>WARN</color>";
                            break;
                        default:
                            tmp = "<color=#464646>UNKNOWN</color>";
                            break;
                    }
                    text = $"{tmp}";
                    Debug.LogError($"#{text} <color=#3264ff>[TNLS~{author}]</color> {message}");
                    UpdateText($"#{text} <color=#3264ff>[TNLS~{author}]</color> {message}");
                }
            }
        }

        /// <summary>
        ///     Send into a text.
        /// </summary>
        public void UpdateText(string line)
        {
            if(text != null)
            {
                currentLine++;
                if (currentLine < maxLine)
                {
                    string v = "";
                    if (text.text != "")
                    {
                        v = "\n";
                    }
                    text.text = $"{text.text}{v}{line}";
                }
                else {
                    currentLine = 1;
                    text.text = line;
                }
            }
        }
    }
}