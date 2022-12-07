
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>The LoggingSystem of the Networking system</para>
    ///     <para>He is the loging system</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSLogingSystem : UdonSharpBehaviour
    {
        [SerializeField] private TNLSManager TNLSManager;

        /// <summary>
        ///     Send into the log a debug message.
        /// </summary>
        public void DebugMessage(string message)
        {
            if (TNLSManager.DebugMode)
            {
                Debug.Log($"<color=#e132ff>DEBUG</color> <color=#3264ff>[TNLS]</color> {message}");
            }
        }

        /// <summary>
        ///     Send into the log a info message.
        /// </summary>
        public void InfoMessage(string message)
        {
            Debug.Log($"<color=#5032ff>INFO</color> <color=#3264ff>[TNLS]</color> {message}");
        }

        /// <summary>
        ///     Send into the log a error message.
        /// </summary>
        public void ErrorMessage(string message)
        {
            Debug.LogError($"<color=#ff3232>ERROR</color> <color=#3264ff>[TNLS]</color> {message}");
        }
    }
}