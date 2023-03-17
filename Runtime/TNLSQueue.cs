
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSQueue : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;

        public bool queueIsRunning = false;
        public object[][] queueItems = new object[0][];

        /// <summary>
        ///     <para>Insert in the queue a networked method.</para>
        /// </summary>
        public void InsertInTheQueue(string methodEncoded)
        {
            queueItems = queueItems.Add(new object[] { methodEncoded });
            TNLSManager.TNLSLogingSystem.InfoMessage($"Queue is on! Passing '{methodEncoded}' to the waiting list!");
            if (!queueIsRunning)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.1f);
                queueIsRunning = true;
            }
        }

        /// <summary>
        ///     <para>Update the Queue.</para>
        /// </summary>
        public void UpdateTheQueue()
        {
            if (queueItems.Length != 0)
            {
                if(!TNLSManager.TNLS.executingMethod && !TNLSManager.TNLS.sendingMethod && !TNLSManager.TNLS.WaitSerialization)
                {
                    var current = queueItems[0];
                    queueItems = queueItems.Remove(0);

                    if(TNLSManager.TNLS.lastMethodEncoded != (string)current[0])
                    {
                        TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Processing '{(string)current[0]}'...");
                        TNLSManager.TNLSLogingSystem.DebugMessage("QUEUE-NOTIFICATION --> Transfering to us.");
                        TNLSManager.TNLS.CAAOwner();
                        TNLSManager.TNLS.methodEncoded = (string)current[0];
                        TNLSManager.TNLS.lastMethodEncoded = (string)current[0];

                        TNLSManager.TNLSLogingSystem.DebugMessage("QUEUE-NOTIFICATION --> Sending to everyone");
                        TNLSManager.TNLS.sendingMethod = true;
                        TNLSManager.TNLS.RequestSerialization();

                        TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Executing in local the method.");
                        TNLSManager.TNLS.executingMethod = true;
                        TNLSManager.TNLS.Receive((string)current[0]);
                    }
                    else
                    {
                        TNLSManager.TNLSLogingSystem.DebugMessage($"QUEUE-NOTIFICATION --> Same than last call, skipping. (eM: {TNLSManager.TNLS.executingMethod} sM: {TNLSManager.TNLS.sendingMethod} wS: {TNLSManager.TNLS.WaitSerialization})");
                    }
                }else
                {
                    TNLSManager.TNLSLogingSystem.DebugMessage($"QUEUE-NOTIFICATION --> Can't execute now the method because the older one hasn't finished to be executed or synced! (eM: {TNLSManager.TNLS.executingMethod} sM: {TNLSManager.TNLS.sendingMethod} wS: {TNLSManager.TNLS.WaitSerialization})");
                }
            }

            if (queueItems.Length != 0)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.05f);
            }
            else
            {
                queueIsRunning = false;
            }
        }
    }
}