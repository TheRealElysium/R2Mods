using UnityEngine;

namespace FasterTPCharge
{
    public sealed class TeleportChargeMultiplier : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (!RoR2.TeleporterInteraction.instance)
            {
                Destroy(this);
                return;
            }

            if (FasterTPCharge.ChargeMulti.Value <= 1f) return;

            float playersInRadius = (int)FasterTPCharge.GetPlayerCountInRadius.Invoke(RoR2.TeleporterInteraction.instance, null);

            if (playersInRadius != 0)
            {
                int playersAlive = RoR2.Run.instance.livingPlayerCount;
                float chargeCoefficient = playersInRadius / playersAlive;
                float timeDecrement = FasterTPCharge.chargeInterval * chargeCoefficient;

                RoR2.TeleporterInteraction.instance.remainingChargeTimer -= timeDecrement;
                FasterTPCharge.timeSaved += timeDecrement;
            }
        }
    }
}
