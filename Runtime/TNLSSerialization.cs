
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

        #region Get/Set Parameters
        /// <summary>
        ///     <para>Return two string[] containing the type and the value of all parameters from a object[].</para>
        /// </summary>
        public string[][] SetParameters(object[] objs)
        {
            string[][] final = new string[2][];
            string[] ListParams = new string[0];
            string[] VarsParams = new string[0];

            int i = 0;
            int max = TNLSManager.MaxParams;
            if(objs.Length <= max)
            {
                max = objs.Length;

                while (i < max)
                {
                    object obj = objs[i];

                    string type = SerializeGetType(obj);
                    if(type != "InvalidType")
                    {
                        ListParams = ListParams.Add(type);
                        VarsParams = VarsParams.Add(SerializeGetValue(obj, type));
                    }else{
                        ListParams = ListParams.Add("InvalidType");
                        VarsParams = VarsParams.Add("false");
                        TNLSManager.TNLSLogingSystem.WarnMessage("Don't support this type ! Skipping...");
                    }

                    i++;
                }
            }else{
                TNLSManager.TNLSLogingSystem.ErrorMessage("Can't set parameters ! You have more parameters than the maximum defined. Check the Manager configuration.");
            }

            final[0] = ListParams;
            final[1] = VarsParams;

            return final;
        }

        /// <summary>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters(string[] ListParams, string[] VarsParams)
        {
            object[] final = new object[0];

            int i = 0;
            while(i < ListParams.Length)
            {
                string Obj = VarsParams[i];
                string type = ListParams[i];
                
                final = final.Add(DeserializeObject(type, Obj));

                // Add 1 to Index
                i++;
            }
            return final;
        }
        #endregion
        #region StringTransformation
        public string StringArrayToString(string[] objs)
        {
            string tmp = "";
            foreach(string str in objs)
            {
                if(tmp == "")
                {
                    tmp = str;
                }
                else
                {
                    tmp = $"{tmp}▀{str}";
                }
            }
            return tmp;
        }
        public string[] StringToStringArray(string objs)
        {
            string[] original = objs.Split('▀');
            string[] tmp = new string[original.Length];
            int i = 0;
            foreach(string obj in tmp)
            {
                tmp[i] = original[i];
                i++;
            }
            return tmp;
        }
        #endregion
        #region SerializationSystem
        private string SerializeGetType(object obj)
        {
            string final = "InvalidType";
            string RealType = obj.GetType().ToString();

            switch(RealType)
            {
                case "System.Int16":
                    final = "Int16";
                    break;
                case "System.Int16[]":
                    final = "Int16[]";
                    break;
                case "System.UInt16":
                    final = "UInt16";
                    break;
                case "System.UInt16[]":
                    final = "UInt16[]";
                    break;
                case "System.Int32":
                    final = "Int32";
                    break;
                case "System.Int32[]":
                    final = "Int32[]";
                    break;
                case "System.UInt32":
                    final = "UInt32";
                    break;
                case "System.UInt32[]":
                    final = "UInt32[]";
                    break;
                case "System.Int64":
                    final = "Int64";
                    break;
                case "System.Int64[]":
                    final = "Int64[]";
                    break;
                case "System.UInt64":
                    final = "UInt64";
                    break;
                case "System.UInt64[]":
                    final = "UInt64[]";
                    break;
                case "System.Single":
                    final = "Single";
                    break;
                case "System.Single[]":
                    final = "Single[]";
                    break;
                case "System.Double":
                    final = "Double";
                    break;
                case "System.Double[]":
                    final = "Double[]";
                    break;
                case "System.Bool":
                    final = "Bool";
                    break;
                case "System.Bool[]":
                    final = "Bool[]";
                    break;
                case "System.Byte":
                    final = "Byte";
                    break;
                case "System.Byte[]":
                    final = "Byte[]";
                    break;
                case "System.SByte":
                    final = "SByte";
                    break;
                case "System.SByte[]":
                    final = "SByte[]";
                    break;
                case "System.String":
                    final = "String";
                    break;
                case "VRC.SDKBase.VRCPlayerApi":
                    final = "VRCPlayerApi";
                    break;
                default:
                    final = "InvalidType";
                    break;
            }


            return final;
        }
        private string SerializeGetValue(object obj, string type)
        {
            string final = "";
            switch (type)
            {
                case "Initialize !!!!!DONT-TOUCH-OR-TRY-TO-MAKE-IT-WORKING!!!!!":
                object rObj = null;
                object rObjA = null;

                string tmp = "";

                break;
            case "Int16":
                // Transform value to string
                rObj = (Int16)obj;
                final = rObj.ToString();
                break;
            case "Int16[]":
                rObjA = (Int16[])obj;

                foreach (Int16 Value in (Int16[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "UInt16":
                // Transform value to string
                rObj = (UInt16)obj;
                final = rObj.ToString();
                        break;
            case "UInt16[]":
                rObjA = (UInt16[])obj;
                
                foreach (UInt16 Value in (UInt16[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Int32":
                // Transform value to string
                rObj = (Int32)obj;
                final = rObj.ToString();
                break;
            case "Int32[]":
                rObjA = (Int32[])obj;
                foreach (Int32 Value in (Int32[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "UInt32":
                // Transform value to string
                rObj = (UInt32)obj;
                final = rObj.ToString();
                break;
            case "UInt32[]":
                rObjA = (UInt32[])obj;

                foreach (UInt32 Value in (UInt32[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Int64":
                // Transform value to string
                rObj = (Int64)obj;
                final = rObj.ToString();
                        break;
            case "Int64[]":
                rObjA = (Int64[])obj;

                foreach (Int64 Value in (Int64[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "UInt64":
                // Transform value to string
                rObj = (UInt64)obj;
                final = rObj.ToString();
                break;
            case "UInt64[]":
                rObjA = (UInt64[])obj;

                foreach(UInt64 Value in (UInt64[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Single":
                // Transform value to string
                rObj = (Single)obj;
                final = rObj.ToString();
                break;
            case "Single[]":
                rObjA = (Single[])obj;
                
                foreach (Single Value in (Single[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Double":
                // Transform value to string
                rObj = (Double)obj;
                final = rObj.ToString();
                break;
            case "Double[]":
                rObjA = (Double[])obj;
                
                foreach (Double Value in (Double[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Bool":
                // Transform value to string
                rObj = (bool)obj;
                if ((bool)rObj)
                {
                    final = "true";
                } else {
                    final = "false";
                }
                break;
            case "Bool[]":
                rObjA = (bool[])obj;
                
                foreach (bool Value in (bool[])rObjA)
                {
                    // Transform value to string
                    if (Value)
                    {
                        tmp = "true";
                    } else {
                        tmp = "false";
                    }

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Byte":
                // Transform value to string
                rObj = (Byte)obj;
                final = rObj.ToString();
                break;
            case "Byte[]":
                rObjA = (Byte[])obj;

                foreach (Byte Value in (Byte[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "SByte":
                // Transform value to string
                rObj = (SByte)obj;
                final = rObj.ToString();
                break;
            case "SByte[]":
                rObjA = (SByte[])obj;

                foreach (SByte Value in (SByte[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    } else {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "String":
                final = (string)obj;
                break;
            case "VRCPlayerApi":
                VRCPlayerApi ply = (VRCPlayerApi)obj;
                final = VRCPlayerApi.GetPlayerId(ply).ToString();
                break;
            default:
                TNLSManager.TNLSLogingSystem.ErrorMessage($"Can't support that type. '{type}'");
                    break;
            }

            return final;
        }
        private object DeserializeObject(string type, string strObj)
        {
            object final = null;
            if(type == null || type == "")
            {
                final = false;
            }
            else if (type == "InvalidType")
            {
                final = false;
            }
            else if (type.Equals("Int16"))
            {
                final = Convert.ToInt16(strObj);
            }
            else if (type.Equals("Int16[]"))
            {
                Int16[] tmp = new Int16[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToInt16(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("UInt16"))
            {
                final = Convert.ToUInt16(strObj);
            }
            else if (type.Equals("UInt16[]"))
            {
                UInt16[] tmp = new UInt16[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToUInt16(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("Int32"))
            {
                final = Convert.ToInt32(strObj);
            }
            else if (type.Equals("Int32[]"))
            {
                Int32[] tmp = new Int32[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToInt32(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("UInt32"))
            {
                final = Convert.ToUInt32(strObj);
            }
            else if (type.Equals("UInt32[]"))
            {
                UInt32[] tmp = new UInt32[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToUInt32(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("Int64"))
            {
                final = Convert.ToInt64(strObj);
            }
            else if (type.Equals("Int64[]"))
            {
                Int64[] tmp = new Int64[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToInt64(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("UInt64"))
            {
                final = Convert.ToUInt64(strObj);
            }
            else if (type.Equals("UInt64[]"))
            {
                UInt64[] tmp = new UInt64[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToUInt64(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("Single"))
            {
                final = Convert.ToSingle(strObj);
            }
            else if (type.Equals("Single[]"))
            {
                Single[] tmp = new Single[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToSingle(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("Double"))
            {
                final = Convert.ToDouble(strObj);
            }
            else if (type.Equals("Double[]"))
            {
                Double[] tmp = new Double[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToDouble(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("Bool"))
            {
                final = Convert.ToBoolean(strObj);
            }
            else if (type.Equals("Bool[]"))
            {
                bool[] tmp = new bool[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToBoolean(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("Byte"))
            {
                final = Convert.ToByte(strObj);
            }
            else if (type.Equals("Byte[]"))
            {
                Byte[] tmp = new Byte[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToByte(strObj));
                }
                final = tmp;
            }
            else if (type.Equals("SByte"))
            {
                final = Convert.ToSByte(strObj);
            }
            else if (type.Equals("SByte[]"))
            {
                SByte[] tmp = new SByte[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToSByte(strObj));
                }
                final = tmp;
            }
            else if(type.Equals("String"))
            {
                final = Convert.ToString(strObj);
            }
            else if (type.Equals("VRCPlayerApi"))
            {
                VRCPlayerApi ply = VRCPlayerApi.GetPlayerById(Convert.ToInt32(strObj));
                final = ply;
            }
            return final;
        }
        #endregion
    }
}