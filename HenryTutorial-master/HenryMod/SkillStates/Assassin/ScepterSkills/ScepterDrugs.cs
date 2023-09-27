using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace AssassinMod.SkillStates
{
    public class ScepterDrugs : GenericProjectileBaseState
    {
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = 0.8f;

            animator = GetModelAnimator();
            animator.SetBool("attacking", true);
            GetModelAnimator().SetFloat("PoisonFlurry.playbackRate", attackSpeedStat);

            PlayCrossfade("Gesture, Override", "Enrage", "PoisonFlurry.playbackRate", duration, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(Modules.Buffs.assassinDrugsBuff, 5);
                characterBody.AddTimedBuff(Modules.Buffs.hardcoreDrugsBuff, 5);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}