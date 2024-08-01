using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using RamattraMod.Modules;

namespace RamattraMod.Survivors.Ramattra.SkillStates
{
    public class ShiftForms : BaseState
    {
        public float baseDuration = 0.1f;
        public float duration;
        private Animator animator;
        //private bool nemesisForm;

        private Components.RamattraNemesisController nemCtrl;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration;// / attackSpeedStat;

            nemCtrl = base.GetComponent<Components.RamattraNemesisController>();

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
            if (nemCtrl.isNem)
            {
                //sets primary, secondary and special skills to the nemesis form alternative
                if (base.skillLocator.primary != null)
                {
                    base.skillLocator.primary.SetSkillOverride(gameObject, RamattraStaticValues.ThrowPummel, GenericSkill.SkillOverridePriority.Contextual);
                }
                
                if (base.skillLocator.secondary != null)
                {
                    base.skillLocator.secondary.SetSkillOverride(gameObject, RamattraStaticValues.NemBlock, GenericSkill.SkillOverridePriority.Contextual);
                }

                if (base.skillLocator.special != null)
                {
                    base.skillLocator.special.SetSkillOverride(gameObject, RamattraStaticValues.Annihilation, GenericSkill.SkillOverridePriority.Contextual);
                }
            }

            else
            {
                //unsets the nemesis skills, reverting back to omnic skills
                if (base.skillLocator.primary != null)
                {
                    base.skillLocator.primary.UnsetSkillOverride(gameObject, RamattraStaticValues.ThrowPummel, GenericSkill.SkillOverridePriority.Contextual);
                }

                if (base.skillLocator.secondary != null)
                {
                    base.skillLocator.secondary.UnsetSkillOverride(gameObject, RamattraStaticValues.NemBlock, GenericSkill.SkillOverridePriority.Contextual);
                }

                if (base.skillLocator.special != null)
                {
                    base.skillLocator.special.UnsetSkillOverride(gameObject, RamattraStaticValues.Annihilation, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
                

            animator.SetBool("inCombat", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= baseDuration)
            {
                //animation stuff goes here
                nemCtrl.isNem = !nemCtrl.isNem;
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

            if (base.fixedAge >= duration && isAuthority)
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