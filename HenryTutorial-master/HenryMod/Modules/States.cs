using AssassinMod.SkillStates;
using AssassinMod.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace AssassinMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(ThrowDagger));

            Modules.Content.AddEntityState(typeof(ThrowPoison));

            Modules.Content.AddEntityState(typeof(ThrowPearl));

            Modules.Content.AddEntityState(typeof(DrugSelf));

            Content.AddEntityState(typeof(BaseEmote));
            Content.AddEntityState(typeof(Juggle));
            Content.AddEntityState(typeof(MenuPose));
        }
    }
}