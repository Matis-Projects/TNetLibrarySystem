
using System;
using Tismatis.TNetLibrarySystem.Coding;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    public enum currentState
    {
        free,
        waitingSerialization,
        waitEndOfSerialization,
        waitingOwnership,
        waitingEndOfExecution,
        invalidState,
        queueUseLine,
        alreadyInUse
    };
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class TNLSLinePool : UdonSharpBehaviour
    {
        [SerializeField] private TNLSLine[] lines;
        [SerializeField] public TNLSManager TNLSManager;

        [NonSerialized] public string lastMethodEncoded;

        [NonSerialized] public bool unlockService = false;

        public void Initialize()
        {
            int lgth = 0;
            foreach (Transform child in transform)
            {
                if (child.name.Contains("("))
                {
                    lgth++;
                }
            }

            lines = new TNLSLine[lgth];

            foreach (Transform child in transform)
            {
                if (child.name.Contains("("))
                {
                    int idLine = int.Parse(child.name.Replace("NetworkLine (", "").Replace(")", ""));
                    lines[idLine] = child.gameObject.GetComponent<TNLSLine>();
                    lines[idLine].idLine = idLine;
                    lines[idLine].TNLSManager = TNLSManager;
                }
            }

            TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.linePoolInitialize, $"Successfuly set {lines.Length} items in the pool.");
        }

        public int GetLinesCount()
        {
            return lines.Length;
        }

        public TNLSLine GetLineById(int line)
        {
            return lines[line];
        }

        public TNLSLine GetFreeLine()
        {
            foreach (TNLSLine line in lines)
            {
                switch (getStateLine(line))
                {
                    case currentState.free:
                        return line;
                    case currentState.waitingOwnership:
                        break;
                    case currentState.waitingSerialization:
                        break;
                    default:
                        break;
                }
            }

            return null;
        }

        public currentState getStateLine(TNLSLine line)
        {
            if (!line.inUseByQueue)
            {
                if (line.lockAfterGetIt || !line.wantOwnershipLock)
                {
                    // check if we wait Ownership
                    if (line.wantOwnershipLock)
                    {
                        return currentState.waitingOwnership;
                    }
                    // check if we wait end of sync
                    else if (line.inSerialization)
                    {
                        return currentState.waitEndOfSerialization;
                    }
                    // check if we wait request
                    else if (line.waitSerialization)
                    {
                        return currentState.waitingSerialization;
                    }
                    // cehck if we wait end of execution
                    else if (line.localExecution || !TNLSManager.TNLSConfirmPool.everyoneReceived(line.idLine, line.idMethod))
                    {
                        return currentState.waitingEndOfExecution;
                    }
                    else
                    {
                        return currentState.free;
                    }
                }
                else
                {
                    if (TNLSManager.TNLSLinePool.CAAOwner(line.idLine))
                    {
                        return currentState.free;
                    }
                    else
                    {
                        return currentState.alreadyInUse;
                    }
                }
            }
            else
            {
                return currentState.queueUseLine;
            }
        }

        public currentState getStateLineNeutral(TNLSLine line)
        {
            if (!line.inUseByQueue)
            {
                if (line.lockAfterGetIt || !line.wantOwnershipLock)
                {
                    // check if we wait Ownership
                    if (line.wantOwnershipLock)
                    {
                        return currentState.waitingOwnership;
                    }
                    // check if we wait end of sync
                    else if (line.inSerialization)
                    {
                        return currentState.waitEndOfSerialization;
                    }
                    // check if we wait request
                    else if (line.waitSerialization)
                    {
                        return currentState.waitingSerialization;
                    }
                    // cehck if we wait end of execution
                    else if (line.localExecution)
                    {
                        return currentState.waitingEndOfExecution;
                    }
                    else
                    {
                        return currentState.free;
                    }
                }
                else
                {
                    if (Networking.GetOwner(line.gameObject) == Networking.LocalPlayer)
                    {
                        return currentState.free;
                    }
                    else
                    {
                        return currentState.alreadyInUse;
                    }
                }
            }
            else
            {
                return currentState.queueUseLine;
            }
        }

        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Called when the Networking system receive a new method to execute.</para>
        /// </summary>
        public void Receive(string mE, TNLSLine line)
        {
            string[] methodTable = mE.Split('█');

            if (TNLSManager.TNLSOthers.checkIfPlayerCanReceive(Networking.LocalPlayer, methodTable[2]))
            {
                UdonSharpBehaviour network = TNLSManager.TNLSScriptManager.GetScriptByName(methodTable[1]);
                if (network == null)
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.linePoolReceive, $"Can't find the script id '{methodTable[1]}' with the network '{methodTable[0]}' !");
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.linePoolReceive, $"methodEncoded: {mE}");
                }
                else
                {
                    ((NetworkedClass)network).TNLScallMethod(methodTable[0], TNLSManager.TNLSSerialization.GetParameters(TNLSManager.TNLSSerialization.StringToStringArray(methodTable[3]), TNLSManager.TNLSSerialization.StringToStringArray(methodTable[4])));

                    if (line != null)
                        line.localExecution = false;
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultSuccess, logAuthorList.linePoolReceive, $"Successfuly triggered in local '{methodTable[0]}'!");
                }
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.linePoolReceive, $"Received but not targetted by this method.");
            }
        }

        #region Networking
        /// <summary>
        ///     <strong>INTERNAL</strong>
        ///     <para>Send to the Networking system a request to execute a new method on the Network.</para>
        /// </summary>
        public void SendNetwork(string target, string networkName, string scriptId, object[] args)
        {
            string[][] newParams = TNLSManager.TNLSSerialization.SetParameters(args);
            string methodPreparation = $"{networkName}█{scriptId}█{target}█{TNLSManager.TNLSSerialization.StringArrayToString(newParams[0])}█{TNLSManager.TNLSSerialization.StringArrayToString(newParams[1])}█{UnityEngine.Random.Range(0, 65535)}";

            TNLSManager.TNLSQueue.InsertInTheQueue(methodPreparation);
        }
        #endregion Networking

        #region Ownership
        /// <summary>
        ///     Set the User as the Owner
        /// </summary>
        public bool CAAOwner(int line)
        {
            if (Networking.IsOwner(lines[line].gameObject))
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultUnknown, logAuthorList.linePoolCAAOwner, "Why transferring the owning to the LP when LP = Owner?");
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.debugInfo, logAuthorList.linePoolCAAOwner, "Transfering the owner to us.");
                Networking.SetOwner(Networking.LocalPlayer, lines[line].gameObject);
                if (Networking.IsOwner(lines[line].gameObject))
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugSuccess, logAuthorList.linePoolCAAOwner, "Transfered the owner to us.");
                }
                else
                {
                    TNLSManager.TNLSLogingSystem.sendLog(messageType.debugError, logAuthorList.linePoolCAAOwner, "Transferring failed!");
                }
            }
            lines[line].ownershipIsTrapped = Networking.GetOwner(lines[line].gameObject) == Networking.LocalPlayer;
            return Networking.IsOwner(lines[line].gameObject);
        }
        #endregion Ownership
    }
}