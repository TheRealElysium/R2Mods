using BepInEx;
using RoR2;
using EntityStates;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace WaxQuailMultiJump
{
    [BepInPlugin("com.Elysium.WaxQuailMultiJump", "WaxQuailMultiJump", "1.0.0")]
    class WaxQuailMultiJump : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.EntityStates.GenericCharacterMain.FixedUpdate += il =>
            {
                var c = new ILCursor(il);

                c.GotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchCall<EntityState>("get_characterMotor"),
                    x => x.MatchLdfld<CharacterMotor>("jumpCount"),
                    x => x.MatchLdarg(0),
                    x => x.MatchCall<EntityState>("get_characterBody"),
                    x => x.MatchLdfld<CharacterBody>("baseJumpCount")
                );

                c.Index += 5;
                c.Remove();
                c.Emit<CharacterBody>(OpCodes.Callvirt, "get_maxJumpCount");
            };
        }
    }
}
