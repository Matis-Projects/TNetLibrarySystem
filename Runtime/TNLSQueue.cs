
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
        [SerializeField] public TNLSManager TNLSManager;

        [NonSerialized] public bool queueIsRunning = false;
        [NonSerialized] public string[] queueItems = new string[0];
        [NonSerialized] public bool queueIsExecuting = false;
        [NonSerialized] public long queueExecutionTime = 0;
        [NonSerialized] public long lastExecutionTime = 0;

        /// <summary>
        ///     <para>Insert in the queue a networked method.</para>
        /// </summary>
        public void InsertInTheQueue(string methodEncoded)
        {
            if (TNLSManager.TNLSLinePool.lastMethodEncoded != methodEncoded || TNLSManager.TNLSSettings.autorizeDuplicate)
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
                string current = queueItems[0];
                string[] methodDecoded = current.ToString().Split('█');

                if (methodDecoded[2] != "Local")
                {
                    var line = TNLSManager.TNLSLinePool.GetFreeLine();
                    if (line != null)
                    {
                        line.inUseByQueue = true;
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.queueUpdate, $"Processing '{current}' on line #{line.idLine}...");

                        TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Transfering to us the line #{line.idLine}.");

                        queueExecutionTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

                        line.lockAfterGetIt = true;
                        if (TNLSManager.TNLSLinePool.CAAOwner(line.idLine))
                        {
                            queueItems = queueItems.Remove(0);

                            line.idMethod = ushort.Parse(methodDecoded[5]);
                            line.methodEncoded = current;
                            TNLSManager.TNLSLinePool.lastMethodEncoded = current;

                            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Broadcasting the line #{line.idLine}.");
                            line.waitSerialization = true;
                            line.RequestSerialization();

                            if (TNLSManager.TNLSOthers.checkIfPlayerCanReceive(Networking.LocalPlayer, methodDecoded[2]))
                            {
                                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Executing in local the method.");
                                line.localExecution = true;
                                TNLSManager.TNLSLinePool.Receive(current, line);
                            }
                        }
                        else
                        {
                            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, "Can't process now, gonna retry at next tick.");
                        }

                        line.inUseByQueue = false;
                    }
                    else
                    {
                        TNLSManager.TNLSLogingSystem.sendLog(messageType.debugWarn, logAuthorList.queueUpdate, "All lines is currently in use.");
                    }
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.queueUpdate, $"Executing a local method.");
                    TNLSManager.TNLSLinePool.Receive(current, null);
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
}