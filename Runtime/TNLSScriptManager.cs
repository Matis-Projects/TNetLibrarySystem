
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>The ScriptManager Class of the Networking system</para>
    ///     <para>He will managing all scripts.</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TNLSScriptManager : UdonSharpBehaviour
    {
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this library working.")]
        [SerializeField] private TNLSManager TNLSManager;

        [NonSerialized] private UdonSharpBehaviour[] scriptList = new UdonSharpBehaviour[0];
        [NonSerialized] private string[] netList = new string[0];
        [NonSerialized] private int[] idNetList = new int[0];

        #region GetStats
        /// <summary>
        ///     <para>This is a function who return the current value of private vars here.</para>
        ///     <para>You can choice 'scriptList', 'netList' or 'idNetList'.</para>
        /// </summary>
        public int GetStats(string name)
        {
            if(name == "scriptList")
            {
                return scriptList.Length;
            }else if(name == "netList")
            {
                return netList.Length;
            }else if(name == "idNetList")
            {
                return idNetList.Length;
            }
            else
            {
                return -1;
            }
        }
        #endregion GetStats

        #region AddNetwork
        /// <summary>
        ///     <para>This add a NetworkedScript to the list to listening sended event.</para>
        ///     <para>With this one, you can add with the selected Name script.</para>
        /// </summary>
        public int AddANetworkedScript(string name, UdonSharpBehaviour script)
        {
            if(netList.Length + 1 <= TNLSManager.TNLSSettings.maxNetList)
            {
                int scriptId = scriptList.Length + 1;
                scriptList = scriptList.Add(script);
                netList = netList.Add(name);
                idNetList = idNetList.Add(scriptId);
                TNLSManager.TNLSLogingSystem.InfoMessage($"Added '{name}' to the NetworkedList with Name");
                return scriptId;
            }else{
                TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't add one more NetworkedScript! ({netList.Length+1}>MaxNetList)");
                return -1;
            }
        }
        #endregion
        
        #region GetScript
        /// <summary>
        ///     <para>Return a UdonSharpBehaviour.</para>
        ///     <para>This one search by the Id of the script.</para>
        /// </summary>
        public UdonSharpBehaviour GetScriptById(int idOfScript)
        {
            UdonSharpBehaviour script = null;
            int k = 0;
            foreach (UdonSharpBehaviour item in scriptList)
            {
                if (item != null)
                {
                    if (idOfScript == k)
                    {
                        script = item;
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }
            }
            return script;
        }

        /// <summary>
        ///     <para>Return a UdonSharpBehaviour.</para>
        ///     <para>This one search by the Name of the script.</para>
        /// </summary>
        public UdonSharpBehaviour GetScriptByName(string nameOfScript)
        {
            UdonSharpBehaviour script = null;
            int k = 0;
            foreach(string currentName in netList)
            {
                if(currentName == nameOfScript)
                {
                    script = scriptList[k];
                    break;
                }
                k++;
            }
            return script;
        }

        /// <summary>
        ///     <para>Return a ScriptId.</para>
        ///     <para>Need the Name of the script.</para>
        /// </summary>
        public int GetScriptIdByName(string nameOfScript)
        {
            int scriptId = -1;
            int k = 0;
            foreach(string currentName in netList)
            {
                if(currentName == nameOfScript)
                {
                    scriptId = k;
                    break;
                }
                k++;
            }
            return scriptId;
        }
        #endregion
    }
}