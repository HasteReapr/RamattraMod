using RamattraMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RamattraMod.SkillStates
{
    public class ShootShard : BaseSkillState
    {
        private Ray aimRay;
        public float baseDuration = 0.04f;
        public float duration;
        public float fireTime;
        public float recoil = 0f;
        private string handString;
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

            PlayCrossfade("Gesture, Override", "ShootStaff", "ThrowKnife.playbackRate", duration, 0.1f);
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
                    damage = 0.2f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)),
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    projectilePrefab = Modules.Projectiles.voidShard,
                    speedOverride = 80,
                    //damageTypeOverride = characterBody.HasBuff(Modules.Buffs.assassinDrugsBuff) ? (DamageType?)Modules.Projectiles.poisonDmgType : (DamageType?)Modules.Projectiles.poisonDmgType,
                };

                ProjectileManager.instance.FireProjectile(proj_info);

                /*ProjectileManager.instance.FireProjectile(info);

                if (characterBody.HasBuff(Modules.Buffs.assassinDrugsBuff))
                {
                    ProjectileManager.instance.FireProjectile(info_L);
                    ProjectileManager.instance.FireProjectile(info_R);
                }*/
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
            return InterruptPriority.Skill;
        }
    }
}