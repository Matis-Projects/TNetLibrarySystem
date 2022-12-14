﻿
using UdonSharp;
using UnityEngine;
using Tismatis.TNetLibrarySystem;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSQueue : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;

        public bool QueueIsRunning = false;
        public object[][] QueueItems = new object[0][];
        public float lastSend = 0f;

        public void InsertInTheQueue(string mE)
        {
            QueueItems = QueueItems.Add(new object[] { mE });
            TNLSManager.TNLSLogingSystem.InfoMessage($"Queue is on! Passing '{mE}' to the waiting list!");
            if (!QueueIsRunning)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.1f);
                QueueIsRunning = true;
            }
        }
        public void UpdateTheQueue()
        {
            if (QueueItems.Length != 0)
            {
                var Current = QueueItems[0];
                QueueItems = QueueItems.Remove(0);

                TNLSManager.TNLS.methodEncoded = (string)Current[0];

                TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Transport of {TNLSManager.TNLS.methodEncoded} out of the queue!");

                TNLSManager.TNLS.Receive((string)Current[0]);
                TNLSManager.TNLS.CAAOwner();
                TNLSManager.TNLS.RequestSerialization();
            }

            if (QueueItems.Length != 0)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.1f);
            }
            else
            {
                QueueIsRunning = false;
            }
        }
    }
}