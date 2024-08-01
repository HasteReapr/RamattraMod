using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using RamattraMod.Modules.BaseStates;

namespace RamattraMod.Survivors.Ramattra.SkillStates.NemStates
{
    public class ThrowPunch : BaseMeleeAttack
    {
        private float shootProjPerc = 0.2f;
        private float fireTime;
        private bool hasFired;
        public override void OnEnter()
        {
            hitboxGroupName = "pummelHitbox";

            damageType = DamageType.Generic;
            damageCoefficient = RamattraStaticValues.pummelDamageCoef;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 1.25f;

            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 0.2f;

            earlyExitPercentTime = 1.35f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.25f;
            hitHopVelocity = 1f;
            //hitSoundString = "Play_Backstab";

            duration = baseDuration/this.attackSpeedStat;

            fireTime = duration * shootProjPerc;
            hasFired = false;

            characterBody.SetAimTimer(duration);

            animator = GetModelAnimator();
            animator.SetBool("attacking", true);
            animator.SetBool("inCombat", true);
            GetModelAnimator().SetFloat("Backstab.playbackRate", 1);

            PlayCrossfade("Gesture, Override", "Backstab", "Backstab.playbackRate", duration, 0.1f);

            base.OnEnter();
        }

        private void Fire()
        {
            Ray aimRay = GetAimRay();
            if (isAuthority)
            {
                FireProjectileInfo proj_info = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = (RamattraStaticValues.pummelDamageCoef * 0.5f) * characterBody.damage, //projectile does half the damage the punch does
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    projectilePrefab = Content.RammyProjectiles.voidShard,
                    speedOverride = 100,
                };

                ProjectileManager.instance.FireProjectile(proj_info);
                hasFired = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("inCombat", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime && !hasFired)
            {
                Fire();
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}