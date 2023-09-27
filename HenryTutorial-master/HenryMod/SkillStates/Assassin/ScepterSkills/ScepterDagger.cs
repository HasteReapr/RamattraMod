using AssassinMod.SkillStates.BaseStates;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace AssassinMod.SkillStates
{
    public class ScepterDagger : BaseSkillState
    {
        private Ray aimRay;
        public float baseDuration = 0.4f;
        public float duration;
        public float fireTime;
        public float recoil = 0f;
        private string handString;
        private Animator animator;


        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            duration = baseDuration / attackSpeedStat;
            fireTime = duration;

            base.characterBody.SetAimTimer(duration);

            animator = GetModelAnimator();
            animator.SetBool("attacking", true);
            GetModelAnimator().SetFloat("ThrowKnife.playbackRate", attackSpeedStat);

            string[] animStringList = { "ThrowKnife1", "ThrowKnife2", "ThrowKnife3", "ThrowKnife4" };
            int chosenNum = Random.RandomRangeInt(0, 4);
            string[] handStringList = { "Hand_R", "Hand_L", "Hand_R", "Hand_L" };
            handString = handStringList[chosenNum];
            PlayCrossfade("Gesture, Override", animStringList[chosenNum], "ThrowKnife.playbackRate", duration, 0.1f);
            Util.PlaySound("Play_dagger_sfx", gameObject);

            //if we are cloaked, uncloak upon attacking
            if (characterBody.HasBuff(RoR2Content.Buffs.Cloak))
                characterBody.ClearTimedBuffs(RoR2Content.Buffs.Cloak);
            if (characterBody.HasBuff(RoR2Content.Buffs.CloakSpeed))
                characterBody.ClearTimedBuffs(RoR2Content.Buffs.CloakSpeed);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
            //PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        private void Fire()
        {
            Ray aimRay = GetAimRay();
            if (isAuthority)
            {
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = 1.25f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    projectilePrefab = Modules.Projectiles.dagger,
                    speedOverride = 256,
                    //damageTypeOverride = characterBody.HasBuff(Modules.Buffs.assassinDrugsBuff) ? (DamageType?)Modules.Projectiles.poisonDmgType : (DamageType?)Modules.Projectiles.poisonDmgType,
                };
                FireProjectileInfo info_L = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = 1.25f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * Quaternion.Euler(0, -1f, 0),
                    projectilePrefab = Modules.Projectiles.dagger,
                    speedOverride = 256,
                    //damageTypeOverride = (DamageType?)Modules.Projectiles.poisonDmgType
                };
                FireProjectileInfo info_R = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = 1.25f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * Quaternion.Euler(0, 1f, 0),
                    projectilePrefab = Modules.Projectiles.dagger,
                    speedOverride = 256,
                    //damageTypeOverride = (DamageType?)Modules.Projectiles.poisonDmgType
                };
                FireProjectileInfo info_L1 = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = 1.25f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * Quaternion.Euler(0, -2f, 0),
                    projectilePrefab = Modules.Projectiles.dagger,
                    speedOverride = 256,
                    //damageTypeOverride = (DamageType?)Modules.Projectiles.poisonDmgType
                };
                FireProjectileInfo info_R1 = new FireProjectileInfo()
                {
                    owner = gameObject,
                    damage = 1.25f * characterBody.damage,
                    force = 0,
                    position = aimRay.origin,
                    crit = characterBody.RollCrit(),
                    //position = FindModelChild(handString).position,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * Quaternion.Euler(0, 2f, 0),
                    projectilePrefab = Modules.Projectiles.dagger,
                    speedOverride = 256,
                    //damageTypeOverride = (DamageType?)Modules.Projectiles.poisonDmgType
                };

                ProjectileManager.instance.FireProjectile(info);
                ProjectileManager.instance.FireProjectile(info_L);
                ProjectileManager.instance.FireProjectile(info_R);

                if (characterBody.HasBuff(Modules.Buffs.assassinDrugsBuff))
                {
                    ProjectileManager.instance.FireProjectile(info_L1);
                    ProjectileManager.instance.FireProjectile(info_R1);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime)
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