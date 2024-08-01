using RamattraMod.Survivors.Ramattra.SkillStates;
using RamattraMod.Survivors.Ramattra.SkillStates.NemStates;

namespace RamattraMod.Survivors.Ramattra
{
    public static class RamattraStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(ShootShard));
            Modules.Content.AddEntityState(typeof(ThrowPunch));

            Modules.Content.AddEntityState(typeof(SummonShield));
            Modules.Content.AddEntityState(typeof(NemesisBlock));

            Modules.Content.AddEntityState(typeof(ShiftForms));

            Modules.Content.AddEntityState(typeof(ThrowVoidBall));
            Modules.Content.AddEntityState(typeof(Annihilation));
        }
    }
}
