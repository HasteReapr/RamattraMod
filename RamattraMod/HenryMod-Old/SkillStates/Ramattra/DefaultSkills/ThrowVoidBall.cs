using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RamattraMod.SkillStates
{
    public class ThrowVoidBall : GenericProjectileBaseState
    {
        private Ray aimRay;
        public float baseDuration = 0.5f;
        public float duration;
        public float fireTime;
        private Animator animator;
        private bool hasFired;


        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            duration = baseDuration / attackSpeedStat;
            fireTime = 0;//duration;
            hasFired = false;

            base.characterBody.SetAimTimer(duration);

            animator = GetModelAnimator();
            animator.SetBool("attacking", true);
            animator.SetBool("inCombat", true);
            GetModelAnimator().SetFloat("ThrowKnife.playbackRate", attackSpeedStat);

            PlayCrossfade("Gesture, Override", "ThrowBall", "ThrowKnife.playbackRate", duration, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("inCombat", false);
            animator.SetBool("attacking", false);
            //PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        private void Fire()
        {
            hasFired = true;
            Ray aimRay = GetAimRay();
            if (isAuthority)
            {
                FireProjectileInfo proj_info = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = 1f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    projectilePrefab = Modules.Projectiles.voidBall,
                    speedOverride = 80,
                };

                ProjectileManager.instance.FireProjectile(proj_info);
            }
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
            return InterruptPriority.PrioritySkill;
        }
    }
}