using RamattraMod.SkillStates;
using RamattraMod.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace RamattraMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(ShootShard));

            Modules.Content.AddEntityState(typeof(ThrowPoison));

            Modules.Content.AddEntityState(typeof(ShiftForms));

            Modules.Content.AddEntityState(typeof(ThrowVoidBall));

            Content.AddEntityState(typeof(BaseEmote));
            Content.AddEntityState(typeof(Juggle));
            Content.AddEntityState(typeof(MenuPose));
        }
    }
}