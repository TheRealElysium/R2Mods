using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Utils;

namespace FasterTPCharge
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Elysium.FasterTPCharge", "FasterTPCharge", "1.0.0")]
    class FasterTPCharge : BaseUnityPlugin
    {
        public void Awake()
        {
            BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.OnServerGameOver += Run_OnServerGameOver;
            InitWrap();
        }

        private void BossGroup_onBossGroupDefeatedServer(BossGroup obj)
        {
            if (isRoboBallBoss)
            {
                isRoboBallBoss = false;
                return;
            }

            if (InstantCharge.Value)
            {
                timeSaved = TeleporterInteraction.instance.remainingChargeTimer;
                TeleporterInteraction.instance.remainingChargeTimer = 0;
            }
            else
            {
                runUpdate = new GameObject().AddComponent<TeleportChargeMultiplier>().gameObject;
            }
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            if (NetworkServer.active)
            {
                CharacterBody enemyBody = obj.victimBody;
                
                if (enemyBody.master && enemyBody.master.name == "SuperRoboBallBossMaster(Clone)")
                {
                    isRoboBallBoss = true;
                }
            }
        }
            
        private static void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            if (NetworkServer.active)
            {
                isRoboBallBoss = false;
                timeSaved = 0;
                obj.remainingChargeTimer = obj.chargeDuration;
            }
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction obj)
        {
            if (NetworkServer.active)
            {
                Destroy(runUpdate);

                if (timeSaved >= 1)
                {
                    if (AddToRunTime.Value)
                    {
                        Run.instance.fixedTime += timeSaved;
                    }

                    if (ShowMsg.Value)
                    {
                        timeSavedTotal += timeSaved;
                        DisplayTimeSavedMsg();
                    }
                }
                timeSaved = 0;
            }
        }

        private static void Run_onRunStartGlobal(Run obj)
        {
            if (NetworkServer.active)
            {
                GetPlayerCountInRadius = typeof(TeleporterInteraction).GetMethodCached("GetPlayerCountInRadius");
                UpdateInterval();
            }
        }

        private static void Run_OnServerGameOver(Run obj, GameResultType resultType)
        {
            if (ShowMsg.Value)
            {
                TimeSpan time = TimeSpan.FromSeconds(timeSavedTotal);
                string hms = time.ToString(@"h\:mm\:ss");
                SendBroadcast($"Total time saved this run: {hms}");
                timeSavedTotal = 0;
            }
        }

        private static void DisplayTimeSavedMsg()
        {
            TimeSpan time = TimeSpan.FromSeconds(timeSaved);
            string msm = time.ToString(@"m\:ss\:fff");
            string addRun = "";

            if (AddToRunTime.Value)
            {
                addRun = ", adding to run time.";
            }
            SendBroadcast($"Time saved: {msm}{addRun}");
        }

        private static void UpdateInterval()
        {
            chargeInterval = Time.fixedDeltaTime * (ChargeMulti.Value - 1);
        }

        public static void SendBroadcast(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = message
            });
        }

        private static GameObject runUpdate;
        private static bool isRoboBallBoss;
        private static float timeSavedTotal;
        public static float timeSaved;
        public static float chargeInterval;
        public static System.Reflection.MethodInfo GetPlayerCountInRadius;

        public static ConfigWrapper<float> ChargeMulti;
        private static ConfigWrapper<bool> ShowMsg;
        private static ConfigWrapper<bool> InstantCharge;
        private static ConfigWrapper<bool> AddToRunTime;

        private void InitWrap()
        {
            ChargeMulti = Config.Wrap(
                "FasterTPCharge",
                "Charge Multiplier",
                "Multiplier applied to charge interval after the teleporter boss is defeated. (1 = Vanilla)",
                3f);

            InstantCharge = Config.Wrap(
                "FasterTPCharge",
                "Charge Instantly",
                "Charge the teleporter instantly after the telepoert boss is defeated.",
                false);

            ShowMsg = Config.Wrap(
                "FasterTPCharge",
                "Chat Message",
                "Display a chat message with the amount of time saved when the teleporter is fully charged.",
                true);

            AddToRunTime = Config.Wrap(
                "FasterTPCharge",
                "Add to Run Time",
                "Adds the time saved from the teleporter charge to the run timer to preserve difficulty time scale.",
                true);
        }

        [ConCommand(commandName = "ftp_chargemulti", flags = ConVarFlags.None, helpText = "Charge Multiplier")]
        private static void CCsetChargeMulti(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!float.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                ChargeMulti.Value = value;
                Debug.Log($"Charge multiplier set to {ChargeMulti.Value}.");
                UpdateInterval();
            }
        }

        [ConCommand(commandName = "ftp_instantcharge", flags = ConVarFlags.None, helpText = "Use instant charge")]
        private static void CCsetInstantCharge(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!bool.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                InstantCharge.Value = value;
                Debug.Log($"Instant charge {InstantCharge.Value}.");
            }
        }

        [ConCommand(commandName = "ftp_showmsg", flags = ConVarFlags.None, helpText = "Show chat message")]
        private static void CCsetChatMsg(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!bool.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                ShowMsg.Value = value;
                Debug.Log($"Show chat message {ShowMsg.Value}.");
            }
        }

        [ConCommand(commandName = "ftp_addtoruntime", flags = ConVarFlags.None, helpText = "Add time saved to run time")]
        private static void CCsetAddRunTime(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!bool.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                AddToRunTime.Value = value;
                Debug.Log($"Add to run time {AddToRunTime.Value}.");
            }
        }
    }
}
