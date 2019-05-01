using System;
using BepInEx;
using RoR2;
using BepInEx.Configuration;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace IncreasingShrineReturns
{
    [BepInPlugin("com.Elysium.IncChanceShrineReturns", "Increasing Chance Shrine Returns", "1.0.0")]
    class IncreasingShrineReturns : BaseUnityPlugin
    {
        private static ConfigWrapper<int> PercentageConfig;
        private static ConfigWrapper<bool> MessageConfig;
        private static ConfigWrapper<bool> RemoveDefaultMessage;
        private static ConfigWrapper<bool> CooldownConfig;
        private int count;

        public void Awake()
        {
            PercentageConfig = Config.Wrap(
                "Chance Shrine",
                "Percentage",
                "The percentage to modify the chance to fail. 100 = off 50 = half",
                93);

            MessageConfig = Config.Wrap(
                "Chance Shrine",
                "Chat Display",
                "Display the chance to fail in chat.",
                true);

            RemoveDefaultMessage = Config.Wrap(
                "Chance Shrine",
                "Remove Default Message",
                "Removes the normal shrine message (Option disabled if Percentage is higher than 100. Affects all players)",
                false);

            CooldownConfig = Config.Wrap(
                "Chance Shrine",
                "Cooldown",
                "Remove the cooldown on chance shrines.",
                true);

            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);

                orig(self);
            };

            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                orig(self, interactor);

                CalculateSuccess(self);

                if (CooldownConfig.Value)
                {
                    self.SetFieldValue("refreshTimer", 0f);
                }
            };

            // Reset counter on stage load
            On.RoR2.Stage.Start += (orig, self) =>
            {
                count = 0;

                orig(self);
            };

            if (RemoveDefaultMessage.Value & PercentageConfig.Value < 101)
            {
                IL.RoR2.ShrineChanceBehavior.AddShrineStack += il =>
                {
                    var c = new ILCursor(il);

                    var sendBroadcastChat = typeof(Chat).GetMethod("SendBroadcastChat", new[] { typeof(Chat.ChatMessageBase) });
                    while (c.TryGotoNext(x => x.MatchCall(sendBroadcastChat)))
                    {
                        c.Emit(OpCodes.Pop);
                        c.Remove();
                    }
                };
            }
        }

        private void CalculateSuccess(ShrineChanceBehavior instance)
        {
            var p = Convert.ToSingle(PercentageConfig.Value);

            // Turn mod off if value has no effect
            if (p == 100)
            {
                return;
            }

            ++count;
            var success = instance.GetFieldValue<int>("successfulPurchaseCount");
            var fail = count - success;
            var percentage = p / 100f;
            var chance = instance.failureWeight / (instance.failureWeight + 12.2f) * 100;
            var chancemsg = Mathf.Round(chance).ToString();

            if (p < 100 && instance.maxPurchaseCount < 3)
            {
                var newfailureWeight = 10.1f * Mathf.Pow(percentage, fail);

                if (MessageConfig.Value)
                {
                    if (instance.failureWeight != newfailureWeight)
                    {
                        sendChatMessage("<color=#e7543a>Lose! </color><color=#8296ae>with a </color><color=#ffffff>" + chancemsg + "% </color><color=#8296ae>chance to fail.</color>");
                    }
                    else
                    {
                        sendChatMessage("<color=#efeb1c>Win! </color><color=#8296ae>with a </color><color=#ffffff>" + chancemsg + "% </color><color=#8296ae>chance to fail.</color>");
                    }
                }

                instance.failureWeight = newfailureWeight;

            }
            // InfiniteChance support
            else
            {
                if (MessageConfig.Value)
                {
                    sendChatMessage("<color=#efeb1c>" + chancemsg + "% </color><color=#8296ae>chance to fail.</color>");
                }

                instance.failureWeight *= percentage;
            }

            if (success >= instance.maxPurchaseCount)
            {
                count = 0;
            }

        }

        private void sendChatMessage(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = message
            });
        }

        [ConCommand(commandName = "icsr_message", flags = ConVarFlags.None, helpText = "Display the chance to fail in chat.")]
        private static void CCChatDisplay(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!bool.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                MessageConfig.Value = value;
                Debug.Log($"Chat Display is {MessageConfig.Value}.");
            }
        }

        [ConCommand(commandName = "icsr_cooldown", flags = ConVarFlags.None, helpText = "Remove the cooldown on chance shrines.")]
        private static void CCCooldown(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!bool.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                CooldownConfig.Value = value;
                Debug.Log($"Remove Cooldown is {CooldownConfig.Value}.");
            }
        }

        [ConCommand(commandName = "icsr_percent", flags = ConVarFlags.None, helpText = "The percentage to modify the chance to fail.")]
        private static void CCSetPercentage(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!int.TryParse(args[0], out var percentage))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                PercentageConfig.Value = percentage;
                Debug.Log($"Percentage set to {PercentageConfig.Value}.");
            }
        }
    }
}
