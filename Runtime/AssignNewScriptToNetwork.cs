
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AssignNewScriptToNetwork : UdonSharpBehaviour
    {
        [SerializeField] private UdonSharpBehaviour ScriptSelf;
        [SerializeField] private string ScriptName = "DefaultName";
        [SerializeField] private int ScriptId = 0;

        [SerializeField] private TNLSManager TNLSManager;
        void Start()
        {
            if(TNLSManager != null)
            {
                if(ScriptName != "")
                {
                    if(ScriptSelf != null)
                    {
                            ScriptId = TNLSManager.AddANamedNetworkedScript(ScriptName, ScriptSelf);
                            TNLSManager.TNLSLogingSystem.DebugMessage($"'{ScriptName}' has been added to the networked script by ANSTN!");
                    }
                    else{
                        TNLSManager.TNLSLogingSystem.ErrorMessage($"The script himself is not defined for '{ScriptName}'! Can't continue.");
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
