
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    /// The LoggingSystem of the Networking system
    /// </summary>
    public class TNLSLoggingSystem : UdonSharpBehaviour
    {
        [SerializeField] private TNLSManager TNLSManager;
        /// <summary>
        ///     Send into the log a debug message.
        /// </summary>
        public void DebugMessage(string message)
        {
            if (TNLSManager.DebugMode)
            {
                Debug.Log($"<color=#e132ff>DEBUG</color> <color=#3264ff>[gNet]</color> {message}");
            }
        }

        /// <summary>
        ///     Send into the log a info message.
        /// </summary>
        public void InfoMessage(string message)
        {
            Debug.Log($"<color=#5032ff>INFO</color> <color=#3264ff>[gNet]</color> {message}");
        }

        /// <summary>
        ///     Send into the log a error message.
        /// </summary>
        public void ErrorMessage(string message)
        {
            Debug.LogError($"<color=#ff3232>ERROR</color> <color=#3264ff>[gNet]</color> {message}");
        }
    }
}