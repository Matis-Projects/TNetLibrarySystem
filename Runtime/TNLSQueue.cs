
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
        [NonSerialized] public string[] queueItems = new string[0];
        [NonSerialized] public bool queueIsExecuting = false;
        [NonSerialized] public long queueExecutionTime = 0;
        [NonSerialized] public long lastExecutionTime = 0;
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
                    queueItems = queueItems.Add(methodEncoded);
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.queueIITQ, $"Adding one method to the waiting list!");
                    if (!queueIsRunning)
                    {
                        SendCustomEventDelayedSeconds("UpdateTheQueue", TNLSManager.TNLSSettings.timeBetweenQueueRunning);
                        queueIsRunning = true;
                    }
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultWarn, logAuthorList.queueIITQ, "Trying to add more item in the queue but you hit the limit.");
                }
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.queueIITQ, $"The settings don't permit to put duplicated things in the queue.");
            }
        }

        /// <summary>
        ///     <para>Update the Queue.</para>
        /// </summary>
        public void UpdateTheQueue()
        {
            if (queueItems.Length != 0)
            {
                if (!TNLSManager.TNLS.tryingToSend && !TNLSManager.TNLS.inSerialization && !TNLSManager.TNLS.ownershipLock && !TNLSManager.TNLS.localExecution && !queueIsExecuting && !TNLSManager.TNLS.waitSerialization)
                {
                    if(TNLSManager.TNLSConfirmPool.everyoneReceived(TNLSManager.TNLS.lastMethodId))
                    {
                        queueIsExecuting = true;

                        string current = queueItems[0];

                        string[] methodDecoded = current.ToString().Split('█');

                        if (TNLSManager.TNLS.lastMethodEncoded != current || TNLSManager.TNLSSettings.autorizeDuplicate)
                        {
                            TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.queueUpdate, $"Processing '{current}'...");

                            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, "Transfering to us.");

                            if (numberOfTry == 0)
                            {
                                queueExecutionTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                            }

                            if (methodDecoded[2] != "Local")
                            {
                                TNLSManager.TNLS.lockAfterGetIt = true;
                                if (TNLSManager.TNLS.CAAOwner())
                                {
                                    queueItems = queueItems.Remove(0);

                                    TNLSManager.TNLS.tryingToSend = true;

                                    TNLSManager.TNLS.methodEncoded = current;
                                    TNLSManager.TNLS.lastMethodEncoded = current;
                                    TNLSManager.TNLS.lastMethodId = int.Parse(methodDecoded[5]);

                                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Sending to everyone (or retry: {numberOfTry})");
                                    TNLSManager.TNLS.waitSerialization = true;
                                    TNLSManager.TNLS.RequestSerialization();

                                    numberOfTry = 0;

                                    if (TNLSManager.TNLS.checkIfPlayerCanReceive(Networking.LocalPlayer, methodDecoded[2]))
                                    {
                                        TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Executing in local the method.");
                                        TNLSManager.TNLS.localExecution = true;
                                        TNLSManager.TNLS.Receive(current);
                                    }
                                }
                                else
                                {
                                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, "Can't process now, gonna retry at next tick.");
                                }
                            }
                            else
                            {
                                queueItems = queueItems.Remove(0);

                                TNLSManager.TNLS.methodEncoded = current;
                                TNLSManager.TNLS.lastMethodEncoded = current;
                                TNLSManager.TNLS.lastMethodId = int.Parse(methodDecoded[5]);

                                numberOfTry = 0;

                                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Executing in local the method.");
                                TNLSManager.TNLS.localExecution = true;
                                TNLSManager.TNLS.Receive(current);
                            }
                        }
                        else
                        {
                            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, $"Same than last call, skipping.");
                        }

                        queueIsExecuting = false;
                    }
                    else
                    {
                        //TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, $"Not everyone received it! Waiting...");
                    }
                }
                else
                {
                    if(TNLSManager.TNLSConfirmPool.everyoneReceived(TNLSManager.TNLS.lastMethodId))
                    {
                        if (numberOfTry == 0)
                        {
                            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.queueUpdate, $"isOwner?: {TNLSManager.TNLS.ownershipIsFuckingMine} tTS: {TNLSManager.TNLS.tryingToSend} iS: {TNLSManager.TNLS.inSerialization} oL: {TNLSManager.TNLS.ownershipLock} lE: {TNLSManager.TNLS.localExecution} qIE: {queueIsExecuting} qIR: {queueIsRunning} wS: {TNLSManager.TNLS.waitSerialization} iFR: {true}");
                        }

                        if (numberOfTry >= 20 && !TNLSManager.TNLS.ownershipIsFuckingMine && TNLSManager.TNLS.tryingToSend && TNLSManager.TNLS.waitSerialization && !TNLSManager.TNLS.inSerialization && !TNLSManager.TNLS.ownershipLock)
                        {
                            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, "We retry now...");
                            if (TNLSManager.TNLS.CAAOwner())
                            {
                                TNLSManager.TNLS.methodEncoded = TNLSManager.TNLS.lastMethodEncoded;

                                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Sending to everyone (or retry: {numberOfTry})");
                                TNLSManager.TNLS.RequestSerialization();

                                numberOfTry = 0;
                            }
                            else
                            {
                                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, "Can't process now, gonna retry at next tick.");
                            }
                        }
                        numberOfTry++;
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.queueUpdate, $"Can't execute now the method because the older one hasn't finished to be executed or synced!");
                    }
                    else
                    {
                        //TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, $"Not everyone received it! Waiting...");
                    }
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