
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>Here is all method used everywhere.</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSOthers : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;
        [NonSerialized] private VRCPlayerApi[] AllPlayers = new VRCPlayerApi[0];

        #region Check Receive
        /// <summary>
        ///     <para>Can Receive</para>
        /// </summary>
        public bool checkIfPlayerCanReceive(VRCPlayerApi player, string target)
        {
            switch (target.ToUpper())
            {
                case "LOCAL":
                    return true;
                case "ALL":
                    return true;
                case "MASTER":
                    return player.isMaster;
                default:
                    return target.StartsWith("SelectPlayer=") && checkIfPlayerIsInTheCollection(player, target);
            }
        }

        /// <summary>
        ///     <para>This method return a boolean</para>
        /// </summary>
        public bool checkIfPlayerIsInTheCollection(VRCPlayerApi player, string collection)
        {
            string[] collectString = collection.Replace("SelectPlayer=", "").Split('▀');
            if (collectString.Length >= 1)
            {
                foreach (string id in collectString)
                {
                    if (Convert.ToInt32(id) == player.playerId)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        #endregion Check Receive

        #region Player
        /// <summary>
        ///     <para>Get all players in a collection.</para>
        /// </summary>
        public VRCPlayerApi[] GetAllPlayers()
        {
            return AllPlayers;
        }

        /// <summary>
        ///     <para>Method to get the player id.</para>
        /// </summary>
        public int GetPlayerId(VRCPlayerApi player)
        {
            return VRCPlayerApi.GetPlayerId(player);
        }

        /// <summary>
        ///     <para>Get the player count by the local collection.</para>
        /// </summary>
        public int GetPlayerCount()
        {
            return AllPlayers.Length;
        }

        /// <summary>
        ///     <para>Get the player id from collection.</para>
        /// </summary>
        public int GetPlayerIdLocal(VRCPlayerApi player)
        {
            return Array.IndexOf(GetAllPlayers(), player);
        }

        /// <summary>
        ///     <para>Sert à trouver un joueur par son ID networked</para>
        /// </summary>
        public VRCPlayerApi GetPlayerById(int id)
        {
            return VRCPlayerApi.GetPlayerById(id);
        }

        /// <summary>
        ///     <para>Get the player id from collection.</para>
        /// </summary>
        public void ResortAllValue()
        {
            VRCPlayerApi[] sorted = new VRCPlayerApi[GetPlayerCount()];
            int[] ids = new int[GetPlayerCount()];

            int k = 0;
            foreach (VRCPlayerApi i in AllPlayers)
            {
                ids[k] = GetPlayerId(i);
                k++;
            }

            k = 0;
            for (int i = 0; i <= ids.Length - 1; i++)
            {
                for (int j = i + 1; j < ids.Length; j++)
                {
                    if (ids[i] > ids[j])
                    {
                        k = ids[i];
                        ids[i] = ids[j];
                        ids[j] = k;
                    }
                }
            }

            k = 0;
            foreach (int i in ids)
            {
                sorted[k] = GetPlayerById(i);
                k++;
            }

            AllPlayers = sorted;
        }
        #endregion Player

        #region Time
        /// <summary>
        ///     <para>Returns the current time in ms.</para>
        /// </summary>
        public long CurTime()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
        #endregion Time

        #region Others
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            AllPlayers = AllPlayers.Add(player);

            TNLSManager.TNLSConfirmPool.actualizeList();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            int idLocal = GetPlayerIdLocal(player);
            if (idLocal != -1)
            {
                AllPlayers = AllPlayers.Remove(idLocal);
            }

            TNLSManager.TNLSConfirmPool.actualizeList();
        }
        #endregion Others
    }
}

public static class TNLSArrayDefinitions
{
    public static bool Contains<T>(this T[] array, T item) => Array.IndexOf(array, item) != -1;

    public static T[] Add<T>(this T[] array, T item)
    {
        T[] newArray = new T[array.Length + 1];
        Array.Copy(array, newArray, array.Length);
        newArray[array.Length] = item;
        return newArray;
    }

    public static T[] Remove<T>(this T[] array, int index)
    {
        T[] newArray = new T[array.Length - 1];
        Array.Copy(array, newArray, index);
        Array.Copy(array, index + 1, newArray, index, newArray.Length - index);
        return newArray;
    }

    public static T[] Insert<T>(this T[] array, int index, T item)
    {
        int length = array.Length;
        index = Mathf.Clamp(index, 0, length);
        T[] newArray = new T[length + 1];
        newArray.SetValue(item, index);
        if (index == 0)
        {
            Array.Copy(array, 0, newArray, 1, length);
        }
        else if (index == length)
        {
            Array.Copy(array, 0, newArray, 0, length);
        }
        else
        {
            Array.Copy(array, 0, newArray, 0, index);
            Array.Copy(array, index, newArray, index + 1, length - index);
        }
        return newArray;
    }
}