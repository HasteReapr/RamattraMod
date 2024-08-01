using System;
using RoR2.Skills;

namespace RamattraMod.Survivors.Ramattra
{
    public static class RamattraStaticValues
    {
        public const float pummelDamageCoef = 3f;

        public const float voidShardDamageCoef = 0.2f;

        public const float annihilationDamageCoef = 0.25f;
        
        public const float vortexDamageCoef = 0.25f;

        public const float blockReductionPercent = 0.75f;

        public const float restoreShieldPerc = 0.4f;

        // primaries
        internal static SteppedSkillDef FireStaff;
        internal static SteppedSkillDef ThrowPummel;
        // secondaries
        internal static SkillDef SummonShield;
        internal static SkillDef NemBlock;
        // specials
        internal static SkillDef RavenousVortex;
        internal static SkillDef Annihilation;
    }
}