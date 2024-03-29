﻿
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tismatis.TNetLibrarySystem
{
    /// <summary>
    ///     <para>This is the class used for store all settings.</para>
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TNLSSettings : UdonSharpBehaviour
    {
        [SerializeField] public TNLSManager TNLSManager;



        [Header("Script Networked Settings")]

        [Tooltip("This is the limit who blocking any request with that number of params.\n\nFor disable that option, put -1.")]
        [SerializeField] public int maxParams = 25;



        [Header("Queue Settings")]

        [Tooltip("This is the limit before we don't accept new item in the queue.\n\nFor disable that option, put -1.")]
        [SerializeField] public int limitBeforeNotAcceptNew = 500;
        [Tooltip("This is the number of time per second we will pass one item in the queue. (aka number of networked call will be executed per second.)")]
        [SerializeField] public int numberOfTickPerSecond = 10;



        [Header("Request")]

        [Tooltip("If this is turn true, any request following with the same content will not be executed.")]
        [SerializeField] public bool autorizeDuplicate = false;



        [Header("Security")]

        [Tooltip("Times before the \"last chance\" of sync expire and pass to the next request. (in ms)")]
        [SerializeField] public int timeBeforeLCexpire = 500;


        [Header("Debug Mode")]

        [Tooltip("Turn on the print of log, That is required for see debug message and others. WARNING: EVERYONE CAN SEE LOGS!")]
        [SerializeField] public bool enableLog = true;

        [Tooltip("Turn on the debug mode permit to see every debug's message in the console.")]
        [SerializeField] public bool debugMode;

        [Tooltip("Whitelist with displayname. Put nothing to remove the whitelist mode.")]
        [SerializeField] public string[] debugWhitelist = new string[0];



        // Final Value
        [NonSerialized] public float timeBetweenQueueRunning = 0.1f;

        /// <summary>
        ///     <para>Called when the main script has been loaded.</para>
        ///     <para>Here we update the value timeBetweenQueueRunning for TNLSQueue.</para>
        /// </summary>
        public void Initialize()
        {
            if(numberOfTickPerSecond > 0)
            {
                timeBetweenQueueRunning = 1 / numberOfTickPerSecond;
            }
            else
            {
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultError, logAuthorList.settingsInitialize, "The value entered for numberOfTickPerSecond is not valid.");
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.settingsInitialize, "Passing into 10 ticks per second.");
            }

            if(timeBeforeLCexpire == -1)
            {
                timeBeforeLCexpire = 30000;
                TNLSManager.TNLSLogingSystem.sendLog(messageType.defaultInfo, logAuthorList.settingsInitialize, "You put -1 at TBLCe, we passing at 30s.");
            }
        }
    }
}