
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem.Coding
{
    public class NetworkedClass : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;

        [Tooltip("Put the name of your networked script.")]
        [SerializeField] private string scriptName = "NetworkedClass";

        public virtual void Start()
        {
            if (scriptName.Length > 0)
            {
                TNLSManager.AddANamedNetworkedScript(scriptName, this);
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.anstnStart, $"'{scriptName}' has been added in TNLS.");
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.anstnStart, $"'{scriptName}' can't be added because his name is invalid.");
            }
        }

        public void SendNetworkedEvent(string target, string name, object[] parameters)
        {
            TNLSManager.CallNamedNetworkedScript(target, name, scriptName, parameters);
        }

        public void SendExternalNetworkedEvent(string target, string name, string scriptName, object[] parameters)
        {
            TNLSManager.CallNamedNetworkedScript(target, name, scriptName, parameters);
        }

        public virtual void TNLScallMethod(string methodName, object[] parameters)
        {
            Debug.Log("[TNLS-Dynamic] This is the base method.");
        }
    }
}