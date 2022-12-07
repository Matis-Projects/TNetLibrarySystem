
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    /// The Main Class of the Networking system
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSManager : UdonSharpBehaviour
    {
        [Header("Link to others scripts")]
        [SerializeField] public TNLS TNLS;
        [SerializeField] public TNLSLoggingSystem TNLSLoggingSystem;
        [Header("Settings")]
        [SerializeField] public bool DebugMode;
        [SerializeField] public int MaxNetList = 50;
        [Header("Don't touch under this! (DEBUG)")]
        [SerializeField] private string[] NetList = null;
        [SerializeField] private int[] iNetList = null;

        #region ScriptManagment
        /// <summary>
        ///     <para>This add a NetworkedScript to the list to listening sended event.</para>
        ///     <para>With this one, you can call by the selected Name script.</para>
        /// </summary>
        public int AddANetworkedScript(string Name, UdonSharpBehaviour USB)
        {
            if(NetList.Length + 1 <= MaxNetList)
            {
                int id = TNLS.DeclareNewDynamicNetworkingScript(USB);
                int k = NetList.Length;
                NetList[k] = Name;
                iNetList[k] = iNetList;
                TNLSLoggingSystem.InfoMessage($"Added {Name} to the NetworkedList with Name");
                return id;
            }else{
                TNLSLoggingSystem.ErrorMessage($"Can't add one more NetworkedScript! ({NetList.Length+1}>MaxNetList)");
                return -1;
            }
        }

        /// <summary>
        ///     <para>This add a NetworkedScript to the list to listening sended event.</para>
        ///     <para>With this one, you can call by the ScriptId.</para>
        /// </summary>
        public int InsertANetworkedScript(UdonSharpBehaviour USB)
        {
            if(NetList.Length + 1 <= MaxNetList)
            {
                return TNLS.DeclareNewDynamicNetworkingScript(USB);
            }else{
                return -1;
            }
        }
        #endregion

        #region AliasToCall
        /// <summary>
        ///     <para>ALIAS to call a Networked Script event.</para>
        ///     <para>With this one, you call by the ScriptId.</para>
        /// </summary>
        public void CallNetworkedScript(string Target, string NetworkName, int ScriptId, object[] args)
        {
            TNLS.SendNetwork(Target, NetworkName, ScriptId, args);
        }
        /// <summary>
        ///     <para>ALIAS to call a Networked Script event.</para>
        ///     <para>With this one, you call by the ScriptName.</para>
        /// </summary>
        public void CallNamedNetworkedScript(string Target, string NetworkName, string ScriptName, object[] args)
        {
            int ScriptId = 0;
            
            int k = 0;
            foreach(string name in NetList)
            {
                if(name == ScriptName)
                {
                    ScriptId = k;
                    break;
                }
                k++;
            }
            TNLS.SendNetwork(Target, NetworkName, ScriptId, args);
        }
        #endregion

        #region Initialization
        public void Start()
        {
            NetList = new string[MaxNetList];
            iNetList = new int[MaxNetList];
        }
        #endregion
    }
}
