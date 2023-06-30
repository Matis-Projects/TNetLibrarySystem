
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
        [NonSerialized] private VRCPlayerApi[] AllPlayers = new VRCPlayerApi[0];

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
            return Array.IndexOf(AllPlayers, player);
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
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            int idLocal = GetPlayerIdLocal(player);
            if(idLocal != -1)
            {
                AllPlayers = AllPlayers.Remove(idLocal);
            }
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
}