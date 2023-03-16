
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
        public float lastSend = 0f;

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
                if(!TNLSManager.TNLS.executingMethod)
                {
                    var current = queueItems[0];
                    queueItems = queueItems.Remove(0);

                    TNLSManager.TNLS.methodEncoded = (string)current[0];

                    TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Transport of '{TNLSManager.TNLS.methodEncoded}' out of the queue!");

                    TNLSManager.TNLS.executingMethod = true;
                    TNLSManager.TNLS.Receive((string)current[0]);
                    TNLSManager.TNLS.CAAOwner();
                    TNLSManager.TNLS.RequestSerialization();
                }else{
                    TNLSManager.TNLSLogingSystem.DebugMessage("QUEUE-NOTIFICATION --> Can't execute now the method because the older one hasn't finished to be executed!");
                }
            }

            if (queueItems.Length != 0)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", 0.1f);
            }
            else
            {
                queueIsRunning = false;
            }
        }
    }
}