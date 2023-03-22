
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSQueue : UdonSharpBehaviour
    {
        [Header("Manager")]
        [Tooltip("This is the TNLS Manager, he is required for make this library working.")]
        [SerializeField] private TNLSManager TNLSManager;

        [NonSerialized] public bool queueIsRunning = false;
        [NonSerialized] public object[][] queueItems = new object[0][];
        [NonSerialized] public bool queueIsExecuting = false;
        [NonSerialized] public int numberOfTry = 0;

        /// <summary>
        ///     <para>Insert in the queue a networked method.</para>
        /// </summary>
        public void InsertInTheQueue(string methodEncoded)
        {
            if (TNLSManager.TNLS.lastMethodEncoded != methodEncoded || TNLSManager.TNLSSettings.autorizeDuplicate)
            {
                if (queueItems.Length < TNLSManager.TNLSSettings.limitBeforeNotAcceptNew || TNLSManager.TNLSSettings.limitBeforeNotAcceptNew == -1)
                {
                    queueItems = queueItems.Add(new object[] { methodEncoded });
                    TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-INSERT-SERVICE --> Queue is on! Passing '{methodEncoded}' to the waiting list!");
                    if (!queueIsRunning)
                    {
                        SendCustomEventDelayedSeconds("UpdateTheQueue", TNLSManager.TNLSSettings.timeBetweenQueueRunning);
                        queueIsRunning = true;
                    }
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.WarnMessage("QUEUE-INSERT-SERVICE --> Trying to add more item in the queue but you hit the limit.");
                }
            }
            else
            {
                TNLSManager.TNLSLogingSystem.DebugMessage($"QUEUE-INSERT-SERVICE --> The settings don't permit to put duplicated things in the queue.");
            }
        }

        /// <summary>
        ///     <para>Update the Queue.</para>
        /// </summary>
        public void UpdateTheQueue()
        {
            if (queueItems.Length != 0)
            {
                if(!TNLSManager.TNLS.executingMethod && !TNLSManager.TNLS.sendingMethod && !TNLSManager.TNLS.waitSerialization && !queueIsExecuting && !TNLSManager.TNLS.tryingToTransfert)// || numberOfTry >= 10 && !TNLSManager.TNLS.executingMethod && TNLSManager.TNLS.sendingMethod && !TNLSManager.TNLS.waitSerialization && !queueIsExecuting)
                {
                    queueIsExecuting = true;

                    var current = queueItems[0];

                    if(TNLSManager.TNLS.lastMethodEncoded != (string)current[0] || TNLSManager.TNLSSettings.autorizeDuplicate)
                    {
                        TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Processing '{(string)current[0]}'...");
                        TNLSManager.TNLSLogingSystem.DebugMessage("QUEUE-NOTIFICATION --> Transfering to us.");

                        TNLSManager.TNLS.CAAOwner();

                        if (Networking.IsOwner(Networking.LocalPlayer, TNLSManager.TNLS.gameObject))
                        {
                            queueItems = queueItems.Remove(0);

                            TNLSManager.TNLS.methodEncoded = (string)current[0];
                            TNLSManager.TNLS.lastMethodEncoded = (string)current[0];

                            TNLSManager.TNLSLogingSystem.DebugMessage($"QUEUE-NOTIFICATION --> Sending to everyone (or retry: {numberOfTry})");
                            TNLSManager.TNLS.sendingMethod = true;
                            TNLSManager.TNLS.RequestSerialization();

                            
                            if (numberOfTry == 0)
                            {
                                TNLSManager.TNLSLogingSystem.InfoMessage($"QUEUE-NOTIFICATION --> Executing in local the method.");
                                TNLSManager.TNLS.executingMethod = true;
                                TNLSManager.TNLS.Receive((string)current[0]);
                            }
                            else
                            {
                                numberOfTry = 0;
                            }
                        }
                        else
                        {
                            TNLSManager.TNLSLogingSystem.WarnMessage("QUEUE-NOTIFICATION --> Can't process now, gonna retry at next tick.");
                        }
                    }
                    else
                    {
                        TNLSManager.TNLSLogingSystem.DebugMessage($"QUEUE-NOTIFICATION --> Same than last call, skipping.");
                    }

                    queueIsExecuting = false;
                }
                else
                {
                    numberOfTry++;
                    TNLSManager.TNLSLogingSystem.DebugMessage($"QUEUE-NOTIFICATION --> Can't execute now the method because the older one hasn't finished to be executed or synced! (eM: {TNLSManager.TNLS.executingMethod} sM: {TNLSManager.TNLS.sendingMethod} wS: {TNLSManager.TNLS.waitSerialization} qIE: {queueIsExecuting} tTT: {TNLSManager.TNLS.tryingToTransfert})");
                }
            }

            if (queueItems.Length != 0)
            {
                SendCustomEventDelayedSeconds("UpdateTheQueue", TNLSManager.TNLSSettings.timeBetweenQueueRunning);
            }
            else
            {
                queueIsRunning = false;
            }
        }
    }
}