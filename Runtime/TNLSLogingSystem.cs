
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private Text text;

        /// <summary>
        ///     Send into the log a debug message.
        /// </summary>
        public void DebugMessage(string message)
        {
            if (TNLSManager.DebugMode)
            {
                string text = $"#<color=#e132ff>DEBUG</color> <color=#3264ff>[TNLS]</color> {message}";
                UpdateText(text);
                Debug.Log(text);
            }
        }

        /// <summary>
        ///     Send into the log a info message.
        /// </summary>
        public void InfoMessage(string message)
        {
            string text = $"#<color=#5032ff>INFO</color> <color=#3264ff>[TNLS]</color> {message}";
            UpdateText(text);
            Debug.Log(text);
        }

        /// <summary>
        ///     Send into the log a error message.
        /// </summary>
        public void ErrorMessage(string message)
        {
            string text = $"#<color=#ff3232>ERROR</color> <color=#3264ff>[TNLS]</color> {message}";
            UpdateText(text);
            Debug.LogError(text);
        }

        /// <summary>
        ///     Send into a text.
        /// </summary>
        public void UpdateText(string line)
        {
            if(text != null)
            {
                string v = "";
                if (text.text != "")
                {
                    v = "\n";
                }
                text.text = $"{text.text}{v}{line}";
            }
        }
    }
}