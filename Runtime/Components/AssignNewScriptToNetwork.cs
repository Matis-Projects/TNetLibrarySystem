
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>This is the class for Assign automaticly your script to an network.</para>
    /// </summary>
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
    }
}
