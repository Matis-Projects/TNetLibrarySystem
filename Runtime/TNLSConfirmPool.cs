
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSConfirmPool : UdonSharpBehaviour
    {
        [SerializeField] private PlayerConfirmReceived[] thePool = new PlayerConfirmReceived[82];
        [SerializeField] private TNLSManager TNLSManager;
        [NonSerialized] public long receiveTimeout;

        public void Initialize()
        {
            foreach (Transform child in transform)
            {
                if(child.name.Contains("("))
                {
                    thePool[int.Parse(child.name.Replace("PlayerConfirmReceived (", "").Replace(")", ""))] = child.gameObject.GetComponent<PlayerConfirmReceived>();
                }
            }

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmInitialize, $"Successfuly set {thePool.Length} items in the pool.");
        }

        public void actualizeList()
        {
            TNLSManager.TNLSOthers.ResortAllValue();

            if(Networking.IsMaster)
            {
                foreach (VRCPlayerApi ply in TNLSManager.TNLSOthers.GetAllPlayers())
                {
                    if (Networking.GetOwner(thePool[TNLSManager.TNLSOthers.GetPlayerIdLocal(ply)].gameObject) != ply)
                    {
                        Networking.SetOwner(ply, thePool[TNLSManager.TNLSOthers.GetPlayerIdLocal(ply)].gameObject);
                    }
                }
            }

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmActualizeList, "We actualized the list.");
        }

        public bool everyoneReceived(int idRequest)
        {
            if(TNLSManager.TNLSOthers.CurTime() > receiveTimeout && receiveTimeout != -1)
            {
                receiveTimeout = -1;
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmEveryoneReceived, "Timeout! We skip that item.");
                return true;
            }else if(receiveTimeout == -1)
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmEveryoneReceived, "Called but already passed or not initialized.");
                return true;
            }
            else
            {
                int confirmed = 0;

                foreach (VRCPlayerApi ply in TNLSManager.TNLSOthers.GetAllPlayers())
                {
                    if (thePool[TNLSManager.TNLSOthers.GetPlayerIdLocal(ply)].received)
                    {
                        confirmed++;
                    }
                }

                bool isReceived = confirmed == TNLSManager.TNLSOthers.GetPlayerCount();
                if(isReceived)
                {
                    receiveTimeout = -1;
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmEveryoneReceived, "The request passed successfuly.");
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, $"Not everyone received it! Waiting... ({confirmed}/{TNLSManager.TNLSOthers.GetPlayerCount()})");
                }
                return isReceived;
            }
        }

        public void passEveryoneToFalse()
        {
            int k = 0;
            foreach (VRCPlayerApi ply in TNLSManager.TNLSOthers.GetAllPlayers())
            {
                thePool[k].received = false;
                k++;
            }
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPassEveryoneToFalse, "We reset everyone to false.");
        }

        public void broadcastReceive(int idRequest)
        {
            int id = TNLSManager.TNLSOthers.GetPlayerIdLocal(Networking.LocalPlayer);
            thePool[id].received = true;
            thePool[id].lastRequest = idRequest;
            thePool[id].RequestSerialization();
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmBroadcastReceive, "We broadcasted we receive it!");
        }
    }
}