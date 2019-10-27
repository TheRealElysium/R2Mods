using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ScalingBloodShrines
{
    [BepInPlugin("com.Elysium.ScalingBloodShrines", "ScalingBloodShrines", "1.0.0")]
    class EBloodShrine : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.ShrineBloodBehavior.Start += ShrineBloodBehavior_Start;
            InitWrap();
        }

        private void ShrineBloodBehavior_Start(On.RoR2.ShrineBloodBehavior.orig_Start orig, ShrineBloodBehavior self)
        {
            orig(self);
            if (NetworkServer.active) StartCoroutine(WaitForPlayerBody(self));
        }

        IEnumerator WaitForPlayerBody(ShrineBloodBehavior instance)
        {
            yield return new WaitForSeconds(2);

            foreach (var playerCharacterMasterController in PlayerCharacterMasterController.instances)
            {
                var body = playerCharacterMasterController.master.GetBody();

                if (body)
                {
                    var maxHealth = body.healthComponent.fullCombinedHealth;
                    if (maxHealth > teamMaxHealth) teamMaxHealth = (int)maxHealth;
                }
            }

            float baseCost = 25 * Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            float maxMoneyTotal = baseCost * ChestAmount.Value;
            float maxMulti = maxMoneyTotal / teamMaxHealth / 2.18f; ;

            if (maxMulti > 0.5f) instance.goldToPaidHpRatio = maxMulti;
        }

        private static int teamMaxHealth;
        private static ConfigWrapper<float> ChestAmount;

        private void InitWrap()
        {
            ChestAmount = Config.Wrap(
                "ScalingBloodShrines",
                "Chest Amount",
                "Chest equivalence of a full blood shrine.",
                4f);
        }

        [ConCommand(commandName = "sbs_chests", flags = ConVarFlags.None, helpText = "Chest Amount")]
        private static void CCsetChestAmount(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            if (!float.TryParse(args[0], out var value))
            {
                Debug.Log("Invalid argument.");
            }
            else
            {
                ChestAmount.Value = value;
                Debug.Log($"Chest amount set to {ChestAmount.Value}.");
            }
        }
    }
}
