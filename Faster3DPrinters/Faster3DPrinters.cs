using BepInEx;
using RoR2;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace Faster3DPrinters
{
    [BepInPlugin("com.Elysium.Fast3D", "Faster3DPrinters", "1.0.0")]
    class Faster3DPrinters : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    typeof(EntityStates.Duplicator.Duplicating).SetFieldValue("initialDelayDuration", 0f);
                    typeof(EntityStates.Duplicator.Duplicating).SetFieldValue("timeBetweenStartAndDropDroplet", 0f);
                }
            };

            On.EntityStates.Duplicator.Duplicating.DropDroplet += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    self.outer.GetComponent<PurchaseInteraction>().Networkavailable = true;
                }
            };

            On.EntityStates.Duplicator.Duplicating.BeginCooking += (orig, self) =>
            {
                if (!NetworkServer.active)
                {
                    orig(self);
                }
            };
        }
    }
}
