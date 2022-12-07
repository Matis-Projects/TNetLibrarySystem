
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
        [SerializeField] private TNLSManager TNLSManager;
        [NonSerialized] private UdonSharpBehaviour[] uNetList = null;
        [NonSerialized] private string[] NetList = null;
        [NonSerialized] private int[] iNetList = null;

        #region Initialization
        public void Start()
        {
            NetList = new string[TNLSManager.MaxNetList];
            iNetList = new int[TNLSManager.MaxNetList];
            uNetList = new UdonSharpBehaviour[TNLSManager.MaxNetList];
        }
        #endregion

        #region AddNetwork
        /// <summary>
        ///     <para>This add a NetworkedScript to the list to listening sended event.</para>
        ///     <para>With this one, you can call by the selected Name script.</para>
        /// </summary>
        public int AddANetworkedScript(string Name, UdonSharpBehaviour USB)
        {
            if(NetList.Length + 1 <= TNLSManager.MaxNetList)
            {
                int id = DeclareNewDynamicNetworkingScript(USB);
                int k = NetList.Length;
                NetList[k] = Name;
                iNetList[k] = id;
                TNLSManager.TNLSLogingSystem.InfoMessage($"Added {Name} to the NetworkedList with Name");
                return id;
            }else{
                TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't add one more NetworkedScript! ({NetList.Length+1}>MaxNetList)");
                return -1;
            }
        }

        /// <summary>
        ///     <para>This add a NetworkedScript to the list to listening sended event.</para>
        ///     <para>With this one, you can call by the ScriptId.</para>
        /// </summary>
        public int InsertANetworkedScript(UdonSharpBehaviour USB)
        {
            if(NetList.Length + 1 <= TNLSManager.MaxNetList)
            {
                return DeclareNewDynamicNetworkingScript(USB);
            }else{
                return -1;
            }
        }
        #endregion
        
        #region GetScript
        /// <summary>
        ///     <para>Return a UdonSharpBehaviour.</para>
        ///     <para>This one search by the Id of the script.</para>
        /// </summary>
        public UdonSharpBehaviour GetScriptById(int IdOfScript)
        {
            UdonSharpBehaviour network = null;
            int k = 0;
            foreach (UdonSharpBehaviour item in uNetList)
            {
                if (item != null)
                {
                    if (IdOfScript == k)
                    {
                        network = item;
                        break;
                    }
                    else
                    {
                        k++;
                    }
                }
            }
            return network;
        }

        /// <summary>
        ///     <para>Return a UdonSharpBehaviour.</para>
        ///     <para>This one search by the Name of the script.</para>
        /// </summary>
        public UdonSharpBehaviour GetScriptByName(string NameOfScript)
        {
            UdonSharpBehaviour network = null;
            int k = 0;
            foreach(string currentName in NetList)
            {
                if(currentName == NameOfScript)
                {
                    network = uNetList[k];
                    break;
                }
                k++;
            }
            return network;
        }

        /// <summary>
        ///     <para>Return a ScriptId.</para>
        ///     <para>Need the Name of the script.</para>
        /// </summary>
        public int GetScriptIdByName(string NameOfScript)
        {
            int ScriptId = -1;
            int k = 0;
            foreach(string currentName in NetList)
            {
                if(currentName == NameOfScript)
                {
                    ScriptId = k;
                    break;
                }
                k++;
            }
            return ScriptId;
        }
        #endregion

        #region Others
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Report to the Networking system to add you in the list of NET-scripts.</para>
        /// </summary>
        private int DeclareNewDynamicNetworkingScript(UdonSharpBehaviour NewScript)
        {
            int tmp = GRCoS();
            uNetList[tmp] = NewScript;
            TNLSManager.TNLSLogingSystem.InfoMessage($"Declared a new NetworkingScript! ({tmp})");
            return tmp;
        }
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Return the number of scripts already declared to the Networking system.</para>
        /// </summary>
        private int GRCoS()
        {
            int tmp = 0;
            foreach (UdonSharpBehaviour obj in uNetList)
            {
                if (obj != null)
                {
                    tmp++;
                }
            }
            return tmp;
        }
        #endregion
    }
}