
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AssignNewScriptToNetwork : UdonSharpBehaviour
    {
        [SerializeField] private UdonSharpBehaviour scriptSelf;
        [SerializeField] private string scriptName = "DefaultName";
        [SerializeField] private int scriptId = 0;

        [SerializeField] private TNLSManager TNLSManager;
        void Start()
        {
            if(TNLSManager != null)
            {
                if(scriptName != "")
                {
                    if(scriptSelf != null)
                    {
                            scriptId = TNLSManager.AddANamedNetworkedScript(scriptName, scriptSelf);
                            TNLSManager.TNLSLogingSystem.DebugMessage($"'{scriptName}' has been added to the networked script by ANSTN!");
                    }
                    else{
                        TNLSManager.TNLSLogingSystem.ErrorMessage($"The script himself is not defined for '{scriptName}'! Can't continue.");
                    }
                }else{
                    TNLSManager.TNLSLogingSystem.ErrorMessage("The name of the script not defined! Can't continue.");
                }
            }else{
                Debug.LogError($"[TNLS-AssignNewScriptToNetwork] TNLSManager isn't set, can't continue.");
            }
        }
    }
}
