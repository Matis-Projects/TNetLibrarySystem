
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>This is the class for Assign automaticly your script to an network.</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AssignNewScriptToNetwork : UdonSharpBehaviour
    {
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this script working.")]
        [SerializeField] private TNLSManager TNLSManager;


        [Tooltip("Put your udonscript.")]
        [SerializeField] private UdonSharpBehaviour scriptSelf;

        [Tooltip("Put the name of your networked script.")]
        [SerializeField] private string scriptName = "DefaultName";

        [Tooltip("You don't need to fill this thing.")]
        [SerializeField] private int scriptId = 0;


        /// <summary>
        ///     <para>Called when the script has been loaded</para>
        ///     <para>Here we assign the script to the TNLSManager.</para>
        /// </summary>
        public void Start()
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
