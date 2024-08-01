using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

using static R2API.DamageAPI;

namespace RamattraMod.Survivors.Ramattra.Content
{
    internal static class RammyProjectiles
    {
        internal static GameObject voidShard; //staff projectile
        internal static GameObject voidVortex; //ravenous vortex
        internal static GameObject voidBall; //this is the ball that spawns the voidVortex
        internal static GameObject punchForce;

        public static ModdedDamageType voidShardType = ReserveDamageType();
        public static ModdedDamageType vortexDmgType = ReserveDamageType();

        private static AssetBundle _assetBundle;
        internal static void RegisterProjectiles(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;

            CreateVoidShard();
            AddProjectile(voidShard);

            CreateVoidBall();
            AddProjectile(voidBall);

            CreateVortex();
            AddProjectile(voidVortex);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateVoidShard()
        {
            voidShard = CloneProjectilePrefab("Bandit2ShivProjectile", "voidshard");

            voidShard.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            voidShard.AddComponent<ModdedDamageTypeHolderComponent>().Add(voidShardType);
            voidShard.GetComponent<ProjectileSingleTargetImpact>().hitSoundString = "";
            voidShard.GetComponent<ProjectileSingleTargetImpact>().enemyHitSoundString = "";

            //voidShard.GetComponent<ProjectileOverlapAttack>().maximumOverlapTargets = int.MaxValue;

            Rigidbody shardRigidBody = voidShard.GetComponent<Rigidbody>();
            if (!shardRigidBody)
            {
                shardRigidBody = voidShard.AddComponent<Rigidbody>();
            }

            ProjectileController shardController = voidShard.GetComponent<ProjectileController>();
            shardController.rigidbody = shardRigidBody;
            shardController.rigidbody.useGravity = false;
            shardController.procCoefficient = 0.3f;

            shardController.GetComponent<ProjectileSingleTargetImpact>().destroyOnWorld = true;
            shardController.GetComponent<ProjectileSingleTargetImpact>().destroyWhenNotAlive = true;

            Object.Destroy(voidShard.transform.GetChild(0).gameObject);
        }

        private static void CreateVoidBall()
        {
            voidBall = CloneProjectilePrefab("CommandoGrenadeProjectile", "vortexBall");
            voidBall.AddComponent<voidBallOnHit>();

            Rigidbody voidballRigidBody = voidBall.GetComponent<Rigidbody>();
            if (!voidballRigidBody)
            {
                voidballRigidBody = voidBall.AddComponent<Rigidbody>();
            }

            ProjectileImpactExplosion voidExplosion = voidBall.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(voidExplosion);

            voidBall.GetComponent<ProjectileSimple>().lifetime = 20f;

            //EffectComponent effectComponent = Assets.poisonExplosionEffect.GetComponent<EffectComponent>();
            //effectComponent.soundName = "assassinBottleBreak";

            voidExplosion.blastRadius = 0f;
            voidExplosion.destroyOnEnemy = true;
            voidExplosion.destroyOnWorld = true;
            //poisonExplosion.impactEffect = Assets.poisonExplosionEffect;
            voidExplosion.lifetime = 12f;
            voidExplosion.timerAfterImpact = true;
            voidExplosion.lifetimeAfterImpact = 0f;
            voidExplosion.childrenCount = 1;
            voidExplosion.childrenProjectilePrefab = voidVortex;

            ProjectileController voidController = voidBall.GetComponent<ProjectileController>();

            voidController.rigidbody = voidballRigidBody;
            voidController.rigidbody.useGravity = true;
            voidController.procCoefficient = 0.5f;
        }

        internal class voidBallOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                ProjectileController controller = GetComponent<ProjectileController>();
                Vector3 impactPos = controller.transform.position;

                var characterBody = controller.owner.GetComponent<CharacterBody>();
                if (Util.HasEffectiveAuthority(characterBody.gameObject))
                {
                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = impactPos,
                        rotation = Quaternion.Euler(0, 0, 0),
                        projectilePrefab = voidVortex,
                        speedOverride = 0,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };

                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        private static void CreateVortex()
        {
            voidVortex = CloneProjectilePrefab("FireTornado", "voidex");
            //voidVortex.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            voidVortex.AddComponent<ModdedDamageTypeHolderComponent>().Add(vortexDmgType);

            voidVortex.transform.localScale = new Vector3(1, 10, 1);

            //voidVortex.AddComponent<RamattraMod.Modules.Misc.VortexProjectileController>();

            Rigidbody vortexRigidBody = voidVortex.GetComponent<Rigidbody>();
            if (!vortexRigidBody)
            {
                vortexRigidBody = voidVortex.AddComponent<Rigidbody>();
            }

            ProjectileController vortexController = voidVortex.GetComponent<ProjectileController>();
            vortexController.rigidbody = vortexRigidBody;
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = _assetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}