
using System;
using System.Diagnostics.Tracing;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>Called when the script has been loaded</para>
    ///     <para>Here we assign the script to the TNLSManager.</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TNLSCustomButtonSender : UdonSharpBehaviour
    {
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this script working.")]
        [NonSerialized] public TNLSManager TNLSManager;


        [Tooltip("Put here your target. All/Local")]
        [SerializeField] private string target = "All";

        [Tooltip("Put the name of your networked script.")]
        [SerializeField] private string scriptName = "DefaultName";

        [Tooltip("Put the name of your method.")]
        [SerializeField] private string methodName = "DefaultMethod";

        [Tooltip("Put all arguments here. It's a String array")]
        [SerializeField] private string[] arguments = new string[0];


        /// <summary>
        ///     <para>Called by the SendCustomEvent</para>
        ///     <para>Here we call the networked script using TNLS.</para>
        /// </summary>
        public void CallTheCustomEvent()
        {
            TNLSManager.CallNamedNetworkedScript(target, methodName, scriptName, arguments);
        }
    }
}