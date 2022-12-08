
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>The Serialization system to support new variables.</para>
    ///     <para>There are some method to manage all parameters types</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSSerialization : UdonSharpBehaviour
    {
        [SerializeField] private TNLSManager TNLSManager;

        #region TransfertHimSelf
        /// <summary>
        ///     <para>Transform all Parameters into a String.</para>
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
                    if(type == "System.String") {
                        tp = $"STR┬{objs[g]}";
                    } else if(type == "System.Int16") {
                        tp = $"I16┬{objs[g]}";
                    } else if(type == "System.Int16[]") {
                        string tma = "";
                        foreach(Int16 nbr in (Int16[])objs[g])
                        {
                            if(tma == "")
                            {
                                tma += $"{nbr}";
                            }else{
                                tma += $"┴{nbr}";
                            }
                        }
                        tp = $"I16┬{tma}";
                    } else if(type == "System.Int32") {
                        tp = $"aI32┬{objs[g]}";
                    } else if(type == "System.Int32[]") {
                        string tma = "";
                        foreach(Int32 nbr in (Int32[])objs[g])
                        {
                            if(tma == "")
                            {
                                tma += $"{nbr}";
                            }else{
                                tma += $"┴{nbr}";
                            }
                        }
                        tp = $"I32┬{tma}";
                    } else if(type == "System.Int64") {
                        tp = $"I64┬{objs[g]}";
                    } else if(type == "System.Int64[]") {
                        string tma = "";
                        foreach(Int64 nbr in (Int64[])objs[g])
                        {
                            if(tma == "")
                            {
                                tma += $"{nbr}";
                            }else{
                                tma += $"┴{nbr}";
                            }
                        }
                        tp = $"aI64┬{tma}";
                    } else if(type == "System.Boolean") {
                        bool t = (bool)objs[g];
                        if (t)
                        {
                            tp = "BOOL┬T";
                        }
                        else
                        {
                            tp = "BOOL┬F";
                        }
                    } else if(type == "System.Boolean[]") {
                        string tma = "";
                        foreach(bool item in (bool[])objs[g])
                        {
                            string tm = "";
                            bool t = item;
                            if (t)
                            {
                                tm = "T";
                            }
                            else
                            {
                                tm = "F";
                            }

                            if(tma == "")
                            {
                                tma += $"{tm}";
                            }else{
                                tma += $"┴{tm}";
                            }
                        }
                        tp = $"aBOOL┬{tma}";
                    } else if(type == "System.Byte") {
                        tp = $"BYTE┬{objs[g]}";
                    } else if(type == "System.Byte[]") {
                        string tma = "";
                        foreach(Byte nbr in (Byte[])objs[g])
                        {
                            if(tma == "")
                            {
                                tma += $"{nbr}";
                            }else{
                                tma += $"┴{nbr}";
                            }
                        }
                        tp = $"aBYTE┬{tma}";
                    } else if(type == "VRC.SDKBase.VRCPlayerApi")
                    {
                        VRCPlayerApi player = (VRCPlayerApi)objs[g];
                        tp = $"VRCPA┬{VRCPlayerApi.GetPlayerId(player)}";
                    } else
                    {
                        TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't use the type {type} with params!");
                    }
                    tmp = $"{tp}@";
                }
            }
            return tmp;
        }

        /// <summary>
        ///     <para>Transform a String into a object array.</para>
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
                    int kt = 0;
                    if(type[0] == "STR")
                    {
                        VarsParams[g] = Convert.ToString(type[1]);
                    }else if(type[0] == "I16") {
                        VarsParams[g] = Convert.ToInt16(type[1]);
                    }else if(type[0] == "aI16") {
                        string[] resultat = Convert.ToString(type[1]).Split('┴');
                        Int16[] tmpa = new Int16[resultat.Length];
                        kt = 0;
                        foreach(string tmp in resultat)
                        {
                            tmpa[k] = Convert.ToInt16(tmp);
                            kt++;
                        }
                        VarsParams[g] = tmpa;
                    }else if(type[0] == "I32") {
                        VarsParams[g] = Convert.ToInt32(type[1]);
                    }else if(type[0] == "aI32") {
                        string[] resultat = Convert.ToString(type[1]).Split('┴');
                        Int32[] tmpa = new Int32[resultat.Length];
                        kt = 0;
                        foreach(string tmp in resultat)
                        {
                            tmpa[k] = Convert.ToInt32(tmp);
                            kt++;
                        }
                        VarsParams[g] = tmpa;
                    }else if(type[0] == "I64") {
                        VarsParams[g] = Convert.ToInt64(type[1]);
                    }else if(type[0] == "aI64") {
                        string[] resultat = Convert.ToString(type[1]).Split('┴');
                        Int64[] tmpa = new Int64[resultat.Length];
                        kt = 0;
                        foreach(string tmp in resultat)
                        {
                            tmpa[k] = Convert.ToInt64(tmp);
                            kt++;
                        }
                        VarsParams[g] = tmpa;
                    }else if(type[0] == "BOOL") {
                        bool booly = false;
                        if (type[1] == "T") { booly = true; }
                        VarsParams[g] = booly;
                    }else if(type[0] == "aBOOL") {
                        string[] resultat = Convert.ToString(type[1]).Split('┴');
                        bool[] tmpa = new bool[resultat.Length];
                        kt = 0;
                        foreach(string tmp in resultat)
                        {
                            bool booly = false;
                            if (tmp == "T") { booly = true; }
                            tmpa[k] = booly;
                            kt++;
                        }
                        VarsParams[g] = tmpa;
                    }else if(type[0] == "BYTE") {
                        VarsParams[g] = Convert.ToByte(type[1]);
                    }else if(type[0] == "aBYTE") {
                        string[] resultat = Convert.ToString(type[1]).Split('┴');
                        Byte[] tmpa = new Byte[resultat.Length];
                        kt = 0;
                        foreach(string tmp in resultat)
                        {
                            tmpa[k] = Convert.ToByte(tmp);
                            kt++;
                        }
                        VarsParams[g] = tmpa;
                    }else if(type[0] == "VRCPA") {
                        VarsParams[g] = VRCPlayerApi.GetPlayerById(Convert.ToInt32(type[1]));
                    }else{
                        TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't currently use the type {type[0]} with params!");
                        k--;
                    }
                }
            }
            return VarsParams;
        }
        #endregion
    }
}