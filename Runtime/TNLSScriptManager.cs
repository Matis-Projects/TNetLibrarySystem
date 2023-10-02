
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
        [SerializeField] public TNLSManager TNLSManager;

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
            switch(name.ToUpper())
            {
                case "SCRIPTLIST":
                    return scriptList.Length;
                case "NETLIST":
                    return netList.Length;
                case "IDNETLIST":
                    return idNetList.Length;
                default:
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
            if (Array.IndexOf(netList, name) == -1)
            {
                int scriptId = scriptList.Length + 1;
                scriptList = scriptList.Add(script);
                netList = netList.Add(name);
                idNetList = idNetList.Add(scriptId);
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.scriptManagerAANS, $"Added '{name}' to the NetworkedList with Name");
                return scriptId;
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.scriptManagerAANS, $"Tried to add '{name}' but that name is already used.");
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
            return scriptList[idOfScript];
        }

        /// <summary>
        ///     <para>Return a UdonSharpBehaviour.</para>
        ///     <para>This one search by the Name of the script.</para>
        /// </summary>
        public UdonSharpBehaviour GetScriptByName(string nameOfScript)
        {
            return scriptList[Array.IndexOf(netList, nameOfScript)];
        }

        /// <summary>
        ///     <para>Return a ScriptId.</para>
        ///     <para>Need the Name of the script.</para>
        /// </summary>
        public int GetScriptIdByName(string nameOfScript)
        {
            return Array.IndexOf(netList, nameOfScript);
        }
        #endregion
    }
}