using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using RamattraMod.Modules;

namespace RamattraMod.SkillStates
{
    public class ShiftForms : GenericCharacterMain
    {
        public float baseDuration = 0.1f;
        public float duration;
        private Animator animator;
        //private bool nemesisForm;

        GenericSkill overridingSkillSlotPrimary;
        GenericSkill overridingSkillSlotSecondary;
        GenericSkill overridingSkillSlotSpecial;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration;// / attackSpeedStat;

            base.characterBody.SetAimTimer(duration);

            /*overridingSkillSlotPrimary = base.skillLocator.primary;
            overridingSkillSlotSecondary = base.skillLocator.secondary;
            overridingSkillSlotSpecial = base.skillLocator.special;*/

            animator = GetModelAnimator();
            animator.SetBool("attacking", true);
            animator.SetBool("inCombat", true);
            GetModelAnimator().SetFloat("EnderPearl.playbackRate", attackSpeedStat);
            PlayCrossfade("Gesture, Override", "ThrowEnderPearl", "EnderPearl.playbackRate", duration, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("inCombat", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge >= baseDuration)
            {
                /*if (overridingSkillSlotPrimary != null)
                {
                    if(overridingSkillSlotPrimary == StaticValues.FireStaff)
                        overridingSkillSlotPrimary.SetSkillOverride(this, StaticValues.ThrowPummel, GenericSkill.SkillOverridePriority.Contextual);
                    else if(overridingSkillSlotPrimary == StaticValues.ThrowPummel)
                        overridingSkillSlotPrimary.SetSkillOverride(this, StaticValues.FireStaff, GenericSkill.SkillOverridePriority.Contextual);
                }
                else
                {
                    //if the skill replacement failed
                    if (base.skillLocator.secondary)
                    {
                        this.outer.SetNextStateToMain();
                    }
                }*/
            }

            if (base.fixedAge >= duration && isAuthority )
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}