
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
        managerStart,
        managerCNNS,
        queueIITQ,
        queueUpdate,
        scriptManagerAANS,
        serializationSetParameters,
        serializationGetParameters,
        serializationSerializeGetValue,
        serializationDeserializeObject,
        anstnStart,
        settingsInitialize,
        confirmPoolInitialize,
        confirmPoolActualizeList,
        confirmPoolEveryoneReceived,
        confirmPoolPassEveryoneToFalse,
        confirmPoolBroadcastReceive,
        linePoolInitialize,
        linePoolCAAOwner,
        linePoolReceive,
        lineOnDeserialization,
        lineOnOwnershipRequest,
        lineOnOwnershipTransferred,
        lineOnPreSerialization,
        lineOnPostDeserialization,
        buildFileRebuildAllFiles,
        buildFileImplementFile,
        buildFileAutoSelectManager,
        buildFileDetectUncompatibleComponent
    }
    /// <summary>
    ///     <para>The LoggingSystem of the Networking system</para>
    ///     <para>He is the loging system</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSLogingSystem : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;


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
                
                if (((int)type) <= 4)
                {
                    text = TNLSLog.LogFormat(type, author, message);
                    if (TNLSManager.TNLSSettings.debugMode && allowDebug)
                    {
                        Debug.Log(text);
                    }
                    UpdateText(text);
                }
                else {
                    text = TNLSLog.LogFormat(type, author, message);
                    Debug.Log(text);
                    UpdateText(text);
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

    public class TNLSLog
    {
        public static string LogFormat(messageType messageType, logAuthorList logAuthor, string message)
        {
            switch (messageType)
            {
                case messageType.defaultInfo:
                    return $"#<color=#0037DA>INFO</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.defaultError:
                    return $"#<color=#C50F1F>ERROR</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.defaultSuccess:
                    return $"#<color=#16C60C>SUCCESS</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.defaultWarn:
                    return $"#<color=#C19C00>WARN</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.debugInfo:
                    return $"#<color=#B4009E>INFO</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.debugError:
                    return $"#<color=#B4009E>ERROR</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.debugSuccess:
                    return $"#<color=#B4009E>SUCCESS</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.debugWarn:
                    return $"#<color=#B4009E>WARN</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                case messageType.debugUnknown:
                    return $"#<color=#B4009E>UNKNOWN</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
                default:
                    return $"#<color=#767676>UNKNOWN</color> [<color=#3B78FF>TNLS~{getAuthor(logAuthor)}</color>] {message}";
            }
        }

        public static string getAuthor(logAuthorList author)
        {
            switch (author)
            {
                case logAuthorList.managerStart:
                    return "manager~Start";
                case logAuthorList.managerCNNS:
                    return "manager~CallNamedNetworkedScript";
                case logAuthorList.queueIITQ:
                    return "queue~InsertInTheQueue";
                case logAuthorList.queueUpdate:
                    return "queue~UpdateTheQueue";
                case logAuthorList.scriptManagerAANS:
                    return "scriptManager~AddANetworkedScript";
                case logAuthorList.serializationSetParameters:
                    return "serialization~SetParameters";
                case logAuthorList.serializationGetParameters:
                    return "serialization~GetParameters";
                case logAuthorList.serializationSerializeGetValue:
                    return "serialization~SerializeGetValue";
                case logAuthorList.serializationDeserializeObject:
                    return "serialization~DeserializeObject";
                case logAuthorList.anstnStart:
                    return "anstn~Start";
                case logAuthorList.settingsInitialize:
                    return "settings~Initialize";
                case logAuthorList.confirmPoolInitialize:
                    return "confirmPool~Initialize";
                case logAuthorList.confirmPoolActualizeList:
                    return "confirmPool~ActualizeList";
                case logAuthorList.confirmPoolEveryoneReceived:
                    return "confirmPool~EveryoneReceived";
                case logAuthorList.confirmPoolPassEveryoneToFalse:
                    return "confirmPool~PassEveryoneToFalse";
                case logAuthorList.confirmPoolBroadcastReceive:
                    return "confirmPool~BroadcastReceive";
                case logAuthorList.linePoolInitialize:
                    return "linePool~Initialize";
                case logAuthorList.linePoolCAAOwner:
                    return "linePool~CAAOwner";
                case logAuthorList.linePoolReceive:
                    return "linePool~Receive";
                case logAuthorList.lineOnDeserialization:
                    return "line~OnDeserialization";
                case logAuthorList.lineOnOwnershipRequest:
                    return "line~OnOwnershipRequest";
                case logAuthorList.lineOnOwnershipTransferred:
                    return "line~OnOwnershipTransferred";
                case logAuthorList.lineOnPreSerialization:
                    return "line~OnPreSerialization";
                case logAuthorList.lineOnPostDeserialization:
                    return "line~OnPostDeserialization";
                case logAuthorList.buildFileRebuildAllFiles:
                    return "buildFile~RebuildAllFiles";
                case logAuthorList.buildFileImplementFile:
                    return "buildFile~ImplementFile";
                case logAuthorList.buildFileAutoSelectManager:
                    return "buildFile~AutoSelectManager";
                default:
                    return "Unknown~Unknown";
            }
        }

        public static void SendLog(messageType messageType, logAuthorList logAuthor, string message)
        {
            Debug.Log(LogFormat(messageType, logAuthor, message));
        }
    }
}