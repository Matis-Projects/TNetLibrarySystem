
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
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this library working.")]
        [SerializeField] private TNLSManager TNLSManager;

        #region Get/Set Parameters
        /// <summary>
        ///     <para>Return two string[] containing the type and the value of all parameters from a object[].</para>
        /// </summary>
        public string[][] SetParameters(object[] objs)
        {
            string[][] final = new string[2][];
            string[] listParams = new string[0];
            string[] varsParams = new string[0];

            int i = 0;
            int max = TNLSManager.TNLSSettings.maxParams;
            if(objs.Length <= max)
            {
                max = objs.Length;

                while (i < max)
                {
                    object obj = objs[i];

                    string type = SerializeGetType(obj);
                    if(type != "InvalidType")
                    {
                        listParams = listParams.Add(type);
                        varsParams = varsParams.Add(SerializeGetValue(obj, type));
                    }else{
                        listParams = listParams.Add("InvalidType");
                        varsParams = varsParams.Add("false");
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.serializationSetParameters, "Don't support this type ! Skipping...");
                    }

                    i++;
                }
            }else{
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.serializationSetParameters, "Can't set parameters ! You have more parameters than the maximum defined. Check the Manager configuration.");
            }

            final[0] = listParams;
            final[1] = varsParams;

            return final;
        }

        /// <summary>
        ///     <para>Return the parameters collections.</para>
        /// </summary>
        public object[] GetParameters(string[] listParams, string[] varsParams)
        {
            object[] final = new object[0];

            int i = 0;
            while(i < listParams.Length)
            {
                if(i <= (listParams.Length - 1))
                {
                    string obj = varsParams[i];
                    string type = listParams[i];

                    final = final.Add(DeserializeObject(type, obj));

                    // Add 1 to Index
                    i++;
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.serializationGetParameters, "Can't receive all parameters, please contact an administrator and check the debug log.");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.serializationGetParameters, $"lP: {listParams.Length} ; vP: {varsParams.Length} i: {i}");
                }
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
        #region SafeString
        private string ProtectString(string str)
        {
            string final = str
                .Replace("┴", "%(U+2534)%")
                .Replace("█", "%(U+2588)%")
                .Replace("▀", "%(U+2580)%");
            return final;
        }
        private string UnProtectString(string str)
        {
            string final = str
                .Replace("%(U+2534)%", "┴")
                .Replace("%(U+2588)%", "█")
                .Replace("%(U+2580)%", "▀");
            return final;
        }
        #endregion SafeString
        #region SerializationSystem
        private string SerializeGetType(object obj)
        {
            string final = "InvalidType";
            string realType = obj.GetType().ToString();

            switch(realType)
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
                case "System.Boolean":
                    final = "Boolean";
                    break;
                case "System.Boolean[]":
                    final = "Boolean[]";
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
                case "System.String[]":
                    final = "String[]";
                    break;
                case "System.Char":
                    final = "Char";
                    break;
                case "System.Char[]":
                    final = "Char[]";
                    break;
                case "UnityEngine.Color":
                    final = "Color";
                    break;
                case "UnityEngine.Color[]":
                    final = "Color[]";
                    break;
                case "UnityEngine.Color32":
                    final = "Color32";
                    break;
                case "UnityEngine.Color32[]":
                    final = "Color32[]";
                    break;
                case "UnityEngine.Quaternion":
                    final = "Quaternion";
                    break;
                case "UnityEngine.Quaternion[]":
                    final = "Quaternion[]";
                    break;
                case "UnityEngine.Vector2":
                    final = "Vector2";
                    break;
                case "UnityEngine.Vector2[]":
                    final = "Vector2[]";
                    break;
                case "UnityEngine.Vector2Int":
                    final = "Vector2Int";
                    break;
                case "UnityEngine.Vector2Int[]":
                    final = "Vector2Int[]";
                    break;
                case "UnityEngine.Vector3":
                    final = "Vector3";
                    break;
                case "UnityEngine.Vector3[]":
                    final = "Vector3[]";
                    break;
                case "UnityEngine.Vector3Int":
                    final = "Vector3Int";
                    break;
                case "UnityEngine.Vector3Int[]":
                    final = "Vector3Int[]";
                    break;
                case "UnityEngine.Vector4":
                    final = "Vector4";
                    break;
                case "UnityEngine.Vector4[]":
                    final = "Vector4[]";
                    break;
                case "System.Decimal":
                    final = "Decimal";
                    break;
                case "System.Decimal[]":
                    final = "Decimal[]";
                    break;
                case "VRC.SDKBase.VRCPlayerApi":
                    final = "VRCPlayerApi";
                    break;
                case "VRC.SDKBase.VRCPlayerApi[]":
                    final = "VRCPlayerApi[]";
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
            case "Boolean":
                // Transform value to string
                rObj = (bool)obj;
                if ((bool)rObj)
                {
                    final = "true";
                } else {
                    final = "false";
                }
                break;
            case "Boolean[]":
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
                final = ProtectString((string)obj);
                break;
            case "String[]":
                rObjA = (string[])obj;

                foreach (string Value in (string[])rObjA)
                {
                    // Transform value to string
                    tmp = ProtectString(Value);

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Char":
                final = ProtectString(Convert.ToString(obj));
                break;
            case "Char[]":
                rObjA = (char[])obj;

                foreach (char Value in (char[])rObjA)
                {
                    // Transform value to string
                    tmp = ProtectString(Convert.ToString(Value));

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Color":
                rObjA = (Color)obj;
                final = $"{((Color)rObjA).a},{((Color)rObjA).r},{((Color)rObjA).g},{((Color)rObjA).b}";
                break;
            case "Color[]":
                rObjA = (Color[])obj;
                foreach (Color Value in (Color[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.a},{Value.r},{Value.g},{Value.b}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Color32":
                rObjA = (Color32)obj;
                final = $"{((Color32)rObjA).a},{((Color32)rObjA).r},{((Color32)rObjA).g},{((Color32)rObjA).b}";
                break;
            case "Color32[]":
                rObjA = (Color32[])obj;
                foreach (Color32 Value in (Color32[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.a},{Value.r},{Value.g},{Value.b}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Quaternion":
                rObjA = (Quaternion)obj;
                final = $"{((Quaternion)rObjA).eulerAngles.x},{((Quaternion)rObjA).eulerAngles.y},{((Quaternion)rObjA).eulerAngles.z}";
                break;
            case "Quaternion[]":
                rObjA = (Quaternion[])obj;
                foreach (Quaternion Value in (Quaternion[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.eulerAngles.x},{Value.eulerAngles.y},{Value.eulerAngles.z}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Vector2":
                rObjA = (Vector2)obj;
                final = $"{((Vector2)rObjA).x},{((Vector2)rObjA).y}";
                break;
            case "Vector2[]":
                rObjA = (Vector2[])obj;
                foreach (Vector2 Value in (Vector2[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.x},{Value.y}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Vector2Int":
                rObjA = (Vector2Int)obj;
                final = $"{((Vector2Int)rObjA).x},{((Vector2Int)rObjA).y}";
                break;
            case "Vector2Int[]":
                rObjA = (Vector2Int[])obj;
                foreach (Vector2Int Value in (Vector2Int[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.x},{Value.y}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Vector3":
                rObjA = (Vector3)obj;
                final = $"{((Vector3)rObjA).x},{((Vector3)rObjA).y},{((Vector3)rObjA).z}";
                break;
            case "Vector3[]":
                rObjA = (Vector3[])obj;
                foreach (Vector3 Value in (Vector3[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.x},{Value.y},{Value.z}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Vector3Int":
                rObjA = (Vector3Int)obj;
                final = $"{((Vector3Int)rObjA).x},{((Vector3Int)rObjA).y},{((Vector3Int)rObjA).z}";
                break;
            case "Vector3Int[]":
                rObjA = (Vector3Int[])obj;
                foreach (Vector3Int Value in (Vector3Int[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.x},{Value.y},{Value.z}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Vector4":
                rObjA = (Vector4)obj;
                final = $"{((Vector4)rObjA).x},{((Vector4)rObjA).y},{((Vector4)rObjA).z},{((Vector4)rObjA).w}";
                break;
            case "Vector4[]":
                rObjA = (Vector4[])obj;
                foreach (Vector4 Value in (Vector4[])rObjA)
                {
                    // Transform value to string
                    tmp = $"{Value.x},{Value.y},{Value.z},{Value.w}";

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "Decimal":
                // Transform value to string
                rObj = (Decimal)obj;
                final = rObj.ToString();
                break;
            case "Decimal[]":
                rObjA = (Decimal[])obj;

                foreach (Decimal Value in (Decimal[])rObjA)
                {
                    // Transform value to string
                    tmp = Value.ToString();

                    // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            case "VRCPlayerApi":
                VRCPlayerApi ply = (VRCPlayerApi)obj;
                final = VRCPlayerApi.GetPlayerId(ply).ToString();
                break;
            case "VRCPlayerApi[]":
                rObjA = (VRCPlayerApi[])obj;

                foreach (VRCPlayerApi Value in (VRCPlayerApi[])rObjA)
                {
                    // Transform value to string
                    tmp = VRCPlayerApi.GetPlayerId(Value).ToString();

                        // Add the transformed value into a 'array'
                    if (final == "")
                    {
                        final = $"{tmp}";
                    }
                    else
                    {
                        final += $"┴{tmp}";
                    }
                }
                break;
            default:
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.serializationSerializeGetValue, $"Can't support that type. '{type}'");
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
                    tmp = tmp.Add(Convert.ToInt16(obj));
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
                    tmp = tmp.Add(Convert.ToUInt16(obj));
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
                    tmp = tmp.Add(Convert.ToInt32(obj));
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
                    tmp = tmp.Add(Convert.ToUInt32(obj));
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
                    tmp = tmp.Add(Convert.ToInt64(obj));
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
                    tmp = tmp.Add(Convert.ToUInt64(obj));
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
                    tmp = tmp.Add(Convert.ToSingle(obj));
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
                    tmp = tmp.Add(Convert.ToDouble(obj));
                }
                final = tmp;
            }
            else if (type.Equals("Boolean"))
            {
                final = Convert.ToBoolean(strObj);
            }
            else if (type.Equals("Boolean[]"))
            {
                bool[] tmp = new bool[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToBoolean(obj));
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
                    tmp = tmp.Add(Convert.ToByte(obj));
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
                    tmp = tmp.Add(Convert.ToSByte(obj));
                }
                final = tmp;
            }
            else if(type.Equals("String"))
            {
                final = UnProtectString(Convert.ToString(strObj));
            }
            else if (type.Equals("String[]"))
            {
                string[] tmp = new string[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(UnProtectString(Convert.ToString(obj)));
                }
                final = tmp;
            }
            else if (type.Equals("Char"))
            {
                final = Convert.ToChar(UnProtectString(Convert.ToString(strObj)));
            }
            else if (type.Equals("Char[]"))
            {
                char[] tmp = new char[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToChar(UnProtectString(Convert.ToString(obj))));
                }
                final = tmp;
            }
            else if (type.Equals("Color"))
            {
                string[] tmp = strObj.Split(',');
                final = new Color(Convert.ToSingle(tmp[1]), Convert.ToSingle(tmp[2]), Convert.ToSingle(tmp[3]), Convert.ToSingle(tmp[0]));
            }
            else if (type.Equals("Color[]"))
            {
                Color[] tmp = new Color[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] color = obj.Split(',');
                    tmp.Add(new Color(Convert.ToSingle(color[1]), Convert.ToSingle(color[2]), Convert.ToSingle(color[3]), Convert.ToSingle(color[0])));
                }
                final = tmp;
            }
            else if (type.Equals("Color32"))
            {
                string[] tmp = strObj.Split(',');
                final = new Color32(Convert.ToByte(tmp[1]), Convert.ToByte(tmp[2]), Convert.ToByte(tmp[3]), Convert.ToByte(tmp[0]));
            }
            else if (type.Equals("Color32[]"))
            {
                Color32[] tmp = new Color32[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] color = obj.Split(',');
                    tmp.Add(new Color32(Convert.ToByte(color[1]), Convert.ToByte(color[2]), Convert.ToByte(color[3]), Convert.ToByte(color[0])));
                }
                final = tmp;
            }
            else if (type.Equals("Quaternion"))
            {
                string[] tmp = strObj.Split(',');
                final = Quaternion.Euler(Convert.ToSingle(tmp[0]), Convert.ToSingle(tmp[1]), Convert.ToSingle(tmp[2]));
            }
            else if (type.Equals("Quaternion[]"))
            {
                Quaternion[] tmp = new Quaternion[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] quaternion = obj.Split(',');
                    tmp.Add(Quaternion.Euler(Convert.ToSingle(quaternion[0]), Convert.ToSingle(quaternion[1]), Convert.ToSingle(quaternion[2])));
                }
                final = tmp;
            }
            else if (type.Equals("Vector2"))
            {
                string[] tmp = strObj.Split(',');
                final = new Vector2(Convert.ToSingle(tmp[0]), Convert.ToSingle(tmp[1]));
            }
            else if (type.Equals("Vector2[]"))
            {
                Vector2[] tmp = new Vector2[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] vector = obj.Split(',');
                    tmp.Add(new Vector2(Convert.ToSingle(vector[0]), Convert.ToSingle(vector[1])));
                }
                final = tmp;
            }
            else if (type.Equals("Vector2Int"))
            {
                string[] tmp = strObj.Split(',');
                final = new Vector2Int(Convert.ToInt32(tmp[0]), Convert.ToInt32(tmp[1]));
            }
            else if (type.Equals("Vector2Int[]"))
            {
                Vector2Int[] tmp = new Vector2Int[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] vector = obj.Split(',');
                    tmp.Add(new Vector2Int(Convert.ToInt32(vector[0]), Convert.ToInt32(vector[1])));
                }
                final = tmp;
            }
            else if (type.Equals("Vector3"))
            {
                string[] tmp = strObj.Split(',');
                final = new Vector3(Convert.ToSingle(tmp[0]), Convert.ToSingle(tmp[1]), Convert.ToSingle(tmp[2]));
            }
            else if (type.Equals("Vector3[]"))
            {
                Vector3[] tmp = new Vector3[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] vector = obj.Split(',');
                    tmp.Add(new Vector3(Convert.ToSingle(vector[0]), Convert.ToSingle(vector[1]), Convert.ToSingle(vector[2])));
                }
                final = tmp;
            }
            else if (type.Equals("Vector3Int"))
            {
                string[] tmp = strObj.Split(',');
                final = new Vector3Int(Convert.ToInt32(tmp[0]), Convert.ToInt32(tmp[1]), Convert.ToInt32(tmp[2]));
            }
            else if (type.Equals("Vector3Int[]"))
            {
                Vector3Int[] tmp = new Vector3Int[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] vector = obj.Split(',');
                    tmp.Add(new Vector3Int(Convert.ToInt32(vector[0]), Convert.ToInt32(vector[1]), Convert.ToInt32(vector[2])));
                }
                final = tmp;
            }
            else if (type.Equals("Vector4"))
            {
                string[] tmp = strObj.Split(',');
                final = new Vector4(Convert.ToSingle(tmp[0]), Convert.ToSingle(tmp[1]), Convert.ToSingle(tmp[2]), Convert.ToSingle(tmp[3]));
            }
            else if (type.Equals("Vector4[]"))
            {
                Vector4[] tmp = new Vector4[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    string[] vector = obj.Split(',');
                    tmp.Add(new Vector4(Convert.ToSingle(vector[0]), Convert.ToSingle(vector[1]), Convert.ToSingle(vector[2]), Convert.ToSingle(vector[3])));
                }
                final = tmp;
            }
            else if (type.Equals("Decimal"))
            {
                final = Convert.ToDecimal(strObj);
            }
            else if (type.Equals("Decimal[]"))
            {
                Decimal[] tmp = new Decimal[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(Convert.ToDecimal(obj));
                }
                final = tmp;
            }
            else if (type.Equals("VRCPlayerApi"))
            {
                VRCPlayerApi ply = VRCPlayerApi.GetPlayerById(Convert.ToInt32(strObj));
                final = ply;
            }
            else if (type.Equals("VRCPlayerApi[]"))
            {
                VRCPlayerApi[] tmp = new VRCPlayerApi[0];
                string[] Objs = strObj.Split('┴');
                foreach (string obj in Objs)
                {
                    tmp = tmp.Add(VRCPlayerApi.GetPlayerById(Convert.ToInt32(obj)));
                }
                final = tmp;
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.serializationDeserializeObject, $"Can't transform the type '{type}'!");
            }
            return final;
        }
        #endregion
    }
}