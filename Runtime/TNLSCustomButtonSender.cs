
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TNLSCustomButtonSender : UdonSharpBehaviour
    {
        [SerializeField] private TNLSManager TNLSManager;

        [SerializeField] private string Target = "All";
        [SerializeField] private string ScriptName = "DefaultName";
        [SerializeField] private string MethodName = "DefaultMethod";
        [SerializeField] private string[] arguments = new string[0];

        public void CallTheCustomEvent()
        {
            TNLSManager.CallNamedNetworkedScript(Target, MethodName, ScriptName, arguments);
        }
    }
}