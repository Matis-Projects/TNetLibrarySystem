﻿
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    /// The Main Class of the Networking system
    /// </summary>
    /// [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TNLS : UdonSharpBehaviour
    {
        [NonSerialized] private UdonSharpBehaviour[] AllDeclaredNetworkingScript = new UdonSharpBehaviour[500];
        [SerializeField, UdonSynced] private string methodEncoded = "";
        [SerializeField] private string local_mE = "";
        [SerializeField] private int nope_mE = 0; 
        [SerializeField] private int mnope_mE = 0;
        [SerializeField] private string tmp_mE = "";
        [SerializeField] private object[] Parameters = null;
        [SerializeField] private bool Debug_Mode = true;
        [SerializeField] private bool NeedUpdate = false;

        #region ReceiverManagement
        /// <summary>
        ///     Report to the Networking system to add you in the list of NET-scripts.
        /// </summary>
        public int DeclareNewDynamicNetworkingScript(UdonSharpBehaviour NewScript)
        {
            int tmp = GRCoS();
            AllDeclaredNetworkingScript[tmp] = NewScript;
            DebugL($"Declared a new NetworkingScript! ({tmp})");
            return tmp;
        }

        /// <summary>
        ///     Report to the Networking system to add you in the list of NET-scripts.
        /// </summary>
        public int DeclareNewNetworkingScript(UdonSharpBehaviour NewScript, int ScriptId)
        {
            int tmp = ScriptId;
            AllDeclaredNetworkingScript[tmp] = NewScript;
            DebugL($"Declared a new NetworkingScript! ({tmp})");
            return tmp;
        }

        /// <summary>
        ///     Return the number of scripts already declared to the Networking system.
        /// </summary>
        public int GRCoS()
        {
            int tmp = 0;
            foreach (UdonSharpBehaviour obj in AllDeclaredNetworkingScript)
            {
                if (obj != null)
                {
                    tmp++;
                }
            }
            return tmp;
        }
        #endregion

        #region TheRealMotorOfThatSystem
        /// <summary>
        ///     Called when the Networking system receive a new method to execute.
        /// </summary>
        public void Receive(string mE)
        {
            string[] mttable = mE.Split('█');
            UdonSharpBehaviour network = GetScriptById(int.Parse(mttable[1]));
            if(network == null)
            {
                DebugLE("Can't find the network!");
            }else{
                Parameters = StrToParameters(mttable[2]);
                network.SendCustomEvent(mttable[0]);
                if(mttable[3] == "All")
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "MKE");
                }
                DebugL($"Triggered the network {mttable[0]} in the script id {mttable[1]} with args '{mttable[2]}'");
            }
        }

        /// <summary>
        ///     Send to the Networking system a request to execute a new method on the Network.
        /// </summary>
        public void SendNetwork(string Target, string NetworkName, int ScriptId, object[] args)
        {
            if (Target != "Local")
            {
                string tmp = $"{NetworkName}█{ScriptId}█{ParametersToStr(args)}█Target";

                DebugL($"Want transport: '{tmp}'");
                DebugL($"Transferring to us.");
                tmp_mE = tmp;
                CAAOwner();
                //RequestSerialization();
                //OnDeserialization();
            }
            else
            {
                string tmp = $"{NetworkName}█{ScriptId}█{ParametersToStr(args)}█Target";
                Receive(tmp);
            }
        }

        /// <summary>
        ///     <para>Called when there are a Deserialization.</para>
        ///     <para>Here is where the player get the method!</para>
        /// </summary>
        public override void OnDeserialization()
        {
            DebugL("Received a deserialization request");
            if (methodEncoded != "")
            {
                DebugL($"Executing the method {methodEncoded}");
                local_mE = methodEncoded;
                Receive(local_mE);
                local_mE = "";
            }
        }

        /// <summary>
        ///     <para>Called for check if the player can get the ownership</para>
        ///     <para>For obvious reason, we need to say YES all times</para>
        /// </summary>
        public override bool OnOwnershipRequest(VRCPlayerApi requester, VRCPlayerApi newOwner)
        {
            return true;
        }

        /// <summary>
        ///     <para>Called when the ownership change</para>
        ///     <para>Here we execute the method and send it to all players</para>
        /// </summary>
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            DebugL("We got the OnOwnershipTransferred!");
            if(tmp_mE != "")
            {
                DebugL($"We have {tmp_mE}");
            }
            ReSync();
        }

        public void Update()
        {
            if(NeedUpdate)
            {
                if(methodEncoded != "")
                {
                    DebugL("Change networked one");
                    NeedUpdate = false;
                    DebugL("NeedUpdate = false");
                    OnDeserialization();
                }else if(tmp_mE != "")
                {
                    DebugL("Change local one");
                    NeedUpdate = false;
                    DebugL("NeedUpdate = false");
                    methodEncoded = tmp_mE;
                    local_mE = tmp_mE;
                    tmp_mE = "";
                    RequestSerialization();
                    OnDeserialization();
                }
            }
        }

        public void ReSync()
        {
            DebugL("NeedUpdate = true");
            NeedUpdate = true;
        }

        public void MKE()
        {
            mnope_mE = VRCPlayerApi.GetPlayerCount();
            nope_mE += 1;
            if(nope_mE == mnope_mE)
            {
                methodEncoded = "";
                RequestSerialization();
            }
        }
        #endregion

        #region Parameters
        /// <summary>String
        ///     Transform all Parameters into a String.
        /// </summary>
        public string ParametersToStr(object[] objs)
        {
            string tmp = "";

            if (objs != null)
            {
                int k = 0;
                foreach (object obj in objs)
                {
                    var type = objs[k].GetType().ToString();
                    var tp = "";

                    k++;
                    int g = k - 1;
                    switch (type)
                    {
                        case "System.String":
                            tp = $"STR┬{objs[g]}";
                            break;
                        case "System.Int16":
                            tp = $"I16┬{objs[g]}";
                            break;
                        case "System.Int16[]":
                            string tmp = "";
                            foreach(Int16 nbr in objs[g])
                            {
                                if(tmp == "")
                                {
                                    tmp = $"{nbr}";
                                }else{
                                    tmp = $"{nbr}┴";
                                }
                            }
                            tp = $"I16┬{tmp}";
                            break;
                        case "System.Int32":
                            tp = $"aI32┬{objs[g]}";
                            break;
                        case "System.Int32[]":
                            string tmp = "";
                            foreach(Int32 nbr in objs[g])
                            {
                                if(tmp == "")
                                {
                                    tmp = $"{nbr}";
                                }else{
                                    tmp = $"{nbr}┴";
                                }
                            }
                            tp = $"I32┬{tmp}";
                            break;
                        case "System.Int64":
                            tp = $"I64┬{objs[g]}";
                            break;
                        case "System.Int64[]":
                            string tmp = "";
                            foreach(Int64 nbr in objs[g])
                            {
                                if(tmp == "")
                                {
                                    tmp = $"{nbr}";
                                }else{
                                    tmp = $"{nbr}┴";
                                }
                            }
                            tp = $"aI64┬{tmp}";
                            break;
                        case "System.Bool":
                            bool t = (bool)objs[g];
                            if (t)
                            {
                                tp = "BOOL┬T";
                            }
                            else
                            {
                                tp = "BOOL┬F";
                            }
                            break;
                        case "System.Bool[]":
                            string tmp = "";
                            foreach(bool item in objs[g])
                            {
                                string tm = "";
                                bool t = (bool)item;
                                if (t)
                                {
                                    tm = "T";
                                }
                                else
                                {
                                    tm = "F";
                                }

                                if(tmp == "")
                                {
                                    tmp = $"{nbr}";
                                }else{
                                    tmp = $"{nbr}┴";
                                }
                            }
                            tp = $"aBOOL┬{tmp}";
                            break;
                        case "System.Byte":
                            tp = $"BYTE┬{objs[g]}";
                            break;
                        case "System.Byte[]":
                            string tmp = "";
                            foreach(Byte nbr in objs[g])
                            {
                                if(tmp == "")
                                {
                                    tmp = $"{nbr}";
                                }else{
                                    tmp = $"{nbr}┴";
                                }
                            }
                            tp = $"aBYTE┬{tmp}";
                            break;
                        case "VRC.SDKBase.VRCPlayerApi":
                            VRCPlayerApi player = (VRCPlayerApi)objs[g];
                            tp = $"VRCPA┬{VRCPlayerApi.GetPlayerId(player)}";
                            break;
                        default:
                            DebugLE($"Can't use the type {type} with params!");
                            break;
                    }
                    tmp = $"{tp}@";
                }
            }
            return tmp;
        }

        /// <summary>
        ///     Transform a String into a object array.
        /// </summary>
        public object[] StrToParameters(string parameters)
        {
            object[] VarsParams = new object[25];

            string[] tmp_PS = parameters.Split('@');

            int k = 0;
            foreach (string str in tmp_PS)
            {
                if (str != "")
                {
                    string[] type = str.Split('┬');

                    k++;
                    int g = k - 1;
                    switch (type[0])
                    {
                        case "STR":
                            VarsParams[g] = Convert.ToString(type[1]);
                            break;
                        case "I16":
                            VarsParams[g] = Convert.ToInt16(type[1]);
                            break;
                        case "I32":
                            VarsParams[g] = Convert.ToInt32(type[1]);
                            break;
                        case "I64":
                            VarsParams[g] = Convert.ToInt64(type[1]);
                            break;
                        case "BOOL":
                            bool booly = false;
                            if (type[1] == "T") { booly = true; }
                            VarsParams[g] = booly;
                            break;
                        case "BYTE":
                            VarsParams[g] = Convert.ToByte(type[1]);
                            break;
                        case "VRCPA":
                            VarsParams[g] = VRCPlayerApi.GetPlayerById(Convert.ToInt32(type[1]));
                            break;
                        default:
                            DebugLE($"Can't currently use the type {type[0]} with params!");
                            k--;
                            break;
                    }
                }
            }

            return VarsParams;
        }

        /// <summary>
        ///     Return null value.
        /// </summary>
        public object[] ReturnNothing()
        {
            return null;
        }

        /// <summary>
        ///     Return the parameters collections.
        /// </summary>
        public object[] GetParameters()
        {
            object[] tmp = Parameters;
            Parameters = null;
            DebugL($"Parameters has been gived, removed!");
            return tmp;
        }
        #endregion

        #region Others
        /// <summary>
        ///     Set the User as the Owner
        /// </summary>
        public void CAAOwner()
        {
            if(!Networking.IsOwner(gameObject))
            {
                DebugL("Transfered the owning to LP!");
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                DebugL($"Transfered the owner to us.");
                //OnOwnershipRequest(Networking.LocalPlayer, Networking.LocalPlayer);
            }else{
                DebugL("Why transferring the owning to the LP when LP = Owner?");
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ReSync");
            }
        }

        /// <summary>
        ///     Return a UdonSharpBehaviour with the IdOfScript.
        /// </summary>
        public UdonSharpBehaviour GetScriptById(int IdOfScript)
        {
            UdonSharpBehaviour network = null;
            int k = 0;
            foreach (UdonSharpBehaviour item in AllDeclaredNetworkingScript)
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
        ///     Send into the log a debug message.
        /// </summary>
        public void DebugL(string message)
        {
            if (Debug_Mode)
            {
                Debug.Log($"[gNet] {message}");
            }
        }

        /// <summary>
        ///     Send into the log a debug message. (ERROR)
        /// </summary>
        public void DebugLE(string message)
        {
            Debug.LogError($"[gNet-ERRROR] {message}");
        }
        #endregion
    }
}