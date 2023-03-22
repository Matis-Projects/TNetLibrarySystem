
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
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this library working.")]
        [SerializeField] private TNLSManager TNLSManager;

        [Tooltip("If you want a debug text, put here your UnityEngine.Text .")]
        [SerializeField] private Text text;
        [Tooltip("Put here the number of line before the auto clear .")]
        [SerializeField] private int maxLine = 40;
        [NonSerialized] private int currentLine;

        /// <summary>
        ///     Send into the log a debug message.
        /// </summary>
        public void DebugMessage(string message)
        {
            string text = $"#<color=#e132ff>DEBUG</color> <color=#3264ff>[TNLS]</color> {message}";
            UpdateText(text);
            if (TNLSManager.TNLSSettings.debugMode)
            {
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
        ///     Send into the log a warn message.
        /// </summary>
        public void WarnMessage(string message)
        {
            string text = $"#<color=#ff9632>WARN</color> <color=#3264ff>[TNLS]</color> {message}";
            UpdateText(text);
            Debug.LogWarning(text);
        }

        /// <summary>
        ///     Send into a text.
        /// </summary>
        public void UpdateText(string line)
        {
            if(text != null)
            {
                currentLine++;
                if(currentLine < maxLine)
                {
                    string v = "";
                    if (text.text != "")
                    {
                        v = "\n";
                    }
                    text.text = $"{text.text}{v}{line}";
                }
                else
                {
                    currentLine = 1;
                    text.text = line;
                }
            }
        }
    }
}