
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
        [NonSerialized] private PlayerConfirmReceived[] thePool = new PlayerConfirmReceived[82];
        [SerializeField] public TNLSManager TNLSManager;
        [NonSerialized] public long receiveTimeout = -1;

        public void Initialize()
        {
            foreach (Transform child in transform)
            {
                if(child.name.Contains("("))
                {
                    int idPool = int.Parse(child.name.Replace("PlayerConfirmReceived (", "").Replace(")", ""));
                    thePool[idPool] = child.gameObject.GetComponent<PlayerConfirmReceived>();
                    thePool[idPool].receivedLines = new ushort[TNLSManager.TNLSLinePool.GetLinesCount()];
                }
            }

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolInitialize, $"Successfuly set {thePool.Length} items in the pool.");
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

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolActualizeList, "We actualized the list.");
        }

        public bool everyoneReceived(int idLine, ushort requestedMethodIdent)
        {
            if(TNLSManager.TNLSOthers.CurTime() > receiveTimeout && receiveTimeout != -1)
            {
                receiveTimeout = -1;
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolEveryoneReceived, "Timeout! We skip that item.");
                return true;
            }else if(receiveTimeout == -1)
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolEveryoneReceived, "Called but already passed or not initialized.");
                return true;
            }
            else
            {
                int confirmed = 0;

                foreach (VRCPlayerApi ply in TNLSManager.TNLSOthers.GetAllPlayers())
                {
                    if (thePool[TNLSManager.TNLSOthers.GetPlayerIdLocal(ply)].receivedLines[idLine] == requestedMethodIdent)
                    {
                        confirmed++;
                    }
                }

                bool isReceived = confirmed == TNLSManager.TNLSOthers.GetPlayerCount();
                if(isReceived)
                {
                    receiveTimeout = -1;
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolEveryoneReceived, "The request passed successfuly.");
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, $"Not everyone received it! Waiting... ({confirmed}/{TNLSManager.TNLSOthers.GetPlayerCount()})");
                }
                return isReceived;
            }
        }

        public void passEveryoneToFalse(int idLine)
        {
            int k = 0;
            foreach (VRCPlayerApi ply in TNLSManager.TNLSOthers.GetAllPlayers())
            {
                thePool[k].receivedLines[idLine] = 0;
                k++;
            }
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolPassEveryoneToFalse, $"We reset everyone to false. {idLine}");
        }

        public void broadcastReceive(int idLine, ushort requestedMethodIdent)
        {
            int id = TNLSManager.TNLSOthers.GetPlayerIdLocal(Networking.LocalPlayer);
            thePool[id].receivedLines[idLine] = requestedMethodIdent;
            thePool[id].RequestSerialization();
            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.confirmPoolBroadcastReceive, $"We broadcasted we receive! {requestedMethodIdent}");
        }
    }
}