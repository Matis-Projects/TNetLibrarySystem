
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

        [SerializeField] private string target = "All";
        [SerializeField] private string scriptName = "DefaultName";
        [SerializeField] private string methodName = "DefaultMethod";
        [SerializeField] private string[] arguments = new string[0];

        public void CallTheCustomEvent()
        {
            TNLSManager.CallNamedNetworkedScript(target, methodName, scriptName, arguments);
        }
    }
}