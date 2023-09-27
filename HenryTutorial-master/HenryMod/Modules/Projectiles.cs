using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static R2API.DamageAPI;

namespace AssassinMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject dagger;
        internal static GameObject poison;
        internal static GameObject virulentPoison;
        internal static GameObject virulentDOTZone;
        internal static GameObject clusterPoison;
        internal static GameObject recursivePoison;
        internal static GameObject enderPearl;
        internal static GameObject cloudyPotion;

        internal static GameObject masteryDagger;
        internal static GameObject grandMasteryDagger;

        public static ModdedDamageType daggerDmgType = ReserveDamageType();
        public static ModdedDamageType poisonDmgType = ReserveDamageType();
        public static ModdedDamageType smokeDmgType = ReserveDamageType();
        public static ModdedDamageType backStabDmg = ReserveDamageType();

        internal static void RegisterProjectiles()
        {
            CreateDagger();
            AddProjectile(dagger);

            CreatePoison();
            AddProjectile(poison);

            virulentDOTZone = Assets.projAssetBundle.LoadAsset<GameObject>("virulent_DOT_Zone");
            AddProjectile(virulentDOTZone);

            CreateVirulentPoison();
            AddProjectile(virulentPoison);

            CreateRecursivePoison();
            AddProjectile(clusterPoison);

            CreateRecursiveClusterPoison();
            AddProjectile(recursivePoison);

            CreateEnderPearl();
            AddProjectile(enderPearl);

            CreateCloudyPotion();
            AddProjectile(cloudyPotion);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateDagger()
        {
            dagger = CloneProjectilePrefab("Bandit2ShivProjectile", "dagger");

            dagger.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            dagger.AddComponent<ModdedDamageTypeHolderComponent>().Add(daggerDmgType);
            dagger.GetComponent<ProjectileSingleTargetImpact>().hitSoundString = "Play_dagger_impact_ground";
            dagger.GetComponent<ProjectileSingleTargetImpact>().enemyHitSoundString = " ";

            Rigidbody daggerRigidBody = dagger.GetComponent<Rigidbody>();
            if (!daggerRigidBody)
            {
                daggerRigidBody = dagger.AddComponent<Rigidbody>();
            }

            ProjectileController daggerController = dagger.GetComponent<ProjectileController>();
            daggerController.rigidbody = daggerRigidBody;
            daggerController.rigidbody.useGravity = true;
            daggerController.procCoefficient = 0.9f;

            daggerController.GetComponent<ProjectileStickOnImpact>().alignNormals = false;

            daggerController.ghostPrefab = CreateGhostPrefab("mdlKnife");
            masteryDagger = CreateGhostPrefab("mdlKnifeMastery");
            grandMasteryDagger = CreateGhostPrefab("mdlKnifeGrandMastery");

            var knifeTrailDupe = Assets.knifeTrail;
            knifeTrailDupe.transform.parent = daggerController.ghostPrefab.transform;

            var masteryTrailDupe = Assets.masteryKnifeTrail;
            masteryTrailDupe.transform.parent = masteryDagger.transform;

            var grandTrailDupe = Assets.grandKnifeTrail;
            grandTrailDupe.transform.parent = grandMasteryDagger.transform;

            Object.Destroy(dagger.transform.GetChild(0).gameObject);
        }

        private static void CreatePoison()
        {
            poison = CloneProjectilePrefab("CommandoGrenadeProjectile", "poison");

            poison.AddComponent<ModdedDamageTypeHolderComponent>().Add(poisonDmgType);

            Rigidbody poisonRigidBody = poison.GetComponent<Rigidbody>();
            if (!poisonRigidBody)
            {
                poisonRigidBody = poison.AddComponent<Rigidbody>();
            }

            ProjectileImpactExplosion poisonExplosion = poison.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(poisonExplosion);

            //EffectComponent effectComponent = Assets.poisonExplosionEffect.GetComponent<EffectComponent>();
            //effectComponent.soundName = "assassinBottleBreak";

            poisonExplosion.blastRadius = 6f;
            poisonExplosion.destroyOnEnemy = true;
            poisonExplosion.destroyOnWorld = true;
            poisonExplosion.impactEffect = Assets.poisonExplosionEffect;
            poisonExplosion.lifetime = 12f;
            poisonExplosion.timerAfterImpact = true;
            poisonExplosion.lifetimeAfterImpact = 0.5f;

            ProjectileController poisonController = poison.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlPoison") != null) poisonController.ghostPrefab = CreateGhostPrefab("mdlPoison");

            //poisonController.ghostPrefab.transform.Find("poison_trail").GetComponent<ParticleSystemRenderer>().SetMaterial(Assets.smokeTrailMat);

            var poisonTrailDupe = Assets.poisonTrail;
            poisonTrailDupe.transform.parent = poisonController.ghostPrefab.transform;

            poisonController.rigidbody = poisonRigidBody;
            poisonController.rigidbody.useGravity = true;
            poisonController.procCoefficient = 0.5f;
        }

        private static void CreateVirulentPoison()
        {
            virulentPoison = CloneProjectilePrefab("CommandoGrenadeProjectile", "virulentVenom");

            Rigidbody virulentRigidBody = virulentPoison.GetComponent<Rigidbody>();
            if (!virulentRigidBody)
            {
                virulentRigidBody = virulentPoison.AddComponent<Rigidbody>();
            }

            ProjectileImpactExplosion virulentExplosion = virulentPoison.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(virulentExplosion);

            virulentExplosion.blastRadius = 6f;
            virulentExplosion.destroyOnEnemy = true;
            virulentExplosion.destroyOnWorld = true;
            virulentExplosion.impactEffect = Assets.venomExplosionEffect;
            virulentExplosion.lifetime = 12f;
            virulentExplosion.timerAfterImpact = true;
            virulentExplosion.lifetimeAfterImpact = 10f;
            //virulentExplosion.childrenCount = 1;
            //virulentExplosion.childrenProjectilePrefab = virulentDOTZone;
            virulentPoison.AddComponent<virulentOnHit>();

            ProjectileController virulentController = virulentPoison.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlVenom") != null) virulentController.ghostPrefab = CreateGhostPrefab("mdlVenom");

            var poisonTrailDupe = Assets.venomTrail;
            poisonTrailDupe.transform.parent = virulentController.ghostPrefab.transform;

            virulentController.rigidbody = virulentRigidBody;
            virulentController.rigidbody.useGravity = true;
            virulentController.procCoefficient = 0.5f;
        }

        internal class virulentOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                ProjectileController recursiveCtrl = GetComponent<ProjectileController>();
                Vector3 impactPos = recursiveCtrl.transform.position;

                var characterBody = recursiveCtrl.owner.GetComponent<CharacterBody>();

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    owner = characterBody.gameObject,
                    damage = 3.75f * characterBody.damage,
                    force = 0,
                    position = new Vector3(impactPos.x, impactPos.y, impactPos.z),
                    rotation = Quaternion.Euler(0, 0, 0),
                    projectilePrefab = virulentDOTZone,
                    speedOverride = 0,
                    //damageTypeOverride = (DamageType?)poisonDmgType
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

        /*private static void CreateVirulentDOTZone()
        {
            virulentDOTZone = CloneProjectilePrefab("CommandoGrenadeProjectile", "virulentDOTZone");

            Rigidbody virulentRigidBody = virulentDOTZone.GetComponent<Rigidbody>();
            if (!virulentRigidBody)
            {
                virulentRigidBody = virulentDOTZone.AddComponent<Rigidbody>();
            }

            virulentDOTZone.AddComponent<ProjectileDotZone>();
            //virulentDOTZone.GetComponent<ProjectileDotZone>().
            virulentDOTZone.GetComponent<ProjectileDotZone>().enabled = true;
            virulentDOTZone.GetComponent<ProjectileDotZone>().fireFrequency = 1f;
            virulentDOTZone.GetComponent<ProjectileDotZone>().lifetime = 5f;

            virulentDOTZone.GetComponent<ProjectileController>().ghostPrefab.transform.localScale = Vector3.zero;
        }*/

        private static void CreateRecursivePoison()
        {
            clusterPoison = CloneProjectilePrefab("CommandoGrenadeProjectile", "recursivePoison");

            clusterPoison.AddComponent<ModdedDamageTypeHolderComponent>().Add(poisonDmgType);
            clusterPoison.AddComponent<recursiveOnHit>();

            Rigidbody poisonRigidBody = clusterPoison.GetComponent<Rigidbody>();
            if (!poisonRigidBody)
            {
                poisonRigidBody = clusterPoison.AddComponent<Rigidbody>();
            }

            ProjectileImpactExplosion poisonExplosion = clusterPoison.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(poisonExplosion);

            //EffectComponent effectComponent = Assets.poisonExplosionEffect.GetComponent<EffectComponent>();
            //effectComponent.soundName = "assassinBottleBreak";

            poisonExplosion.blastRadius = 6f;
            poisonExplosion.destroyOnEnemy = true;
            poisonExplosion.destroyOnWorld = true;
            poisonExplosion.impactEffect = Assets.poisonExplosionEffect;
            //poisonExplosion.explosionSoundString = Assets.poisonExplosionEffect;
            poisonExplosion.lifetime = 12f;
            poisonExplosion.timerAfterImpact = true;
            poisonExplosion.lifetimeAfterImpact = 0.5f;

            ProjectileController poisonController = clusterPoison.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlRecursivePoison") != null) poisonController.ghostPrefab = CreateGhostPrefab("mdlRecursivePoison");

            poisonController.rigidbody = poisonRigidBody;
            poisonController.rigidbody.useGravity = true;
            poisonController.procCoefficient = 0.5f;
        }

        internal class recursiveOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                ProjectileController recursiveCtrl = GetComponent<ProjectileController>();
                Vector3 impactPos = recursiveCtrl.transform.position;

                var characterBody = recursiveCtrl.owner.GetComponent<CharacterBody>();
                if (Util.HasEffectiveAuthority(characterBody.gameObject))
                {
                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y + 2, impactPos.z),
                        rotation = Quaternion.Euler(0, 0, 0),
                        projectilePrefab = poison,
                        speedOverride = 16,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };
                    FireProjectileInfo info2 = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y + 2, impactPos.z),
                        rotation = Quaternion.Euler(0, 120, 0),
                        projectilePrefab = poison,
                        speedOverride = 16,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };
                    FireProjectileInfo info3 = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y + 2, impactPos.z),
                        rotation = Quaternion.Euler(0, 240, 0),
                        projectilePrefab = poison,
                        speedOverride = 16,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };

                    ProjectileManager.instance.FireProjectile(info);
                    ProjectileManager.instance.FireProjectile(info2);
                    ProjectileManager.instance.FireProjectile(info3);
                }
            }
        }

        private static void CreateRecursiveClusterPoison()
        {
            recursivePoison = CloneProjectilePrefab("CommandoGrenadeProjectile", "recursivePoison");

            recursivePoison.AddComponent<ModdedDamageTypeHolderComponent>().Add(poisonDmgType);
            recursivePoison.AddComponent<recursiveClusterOnHit>();

            Rigidbody poisonRigidBody = recursivePoison.GetComponent<Rigidbody>();
            if (!poisonRigidBody)
            {
                poisonRigidBody = recursivePoison.AddComponent<Rigidbody>();
            }

            ProjectileImpactExplosion poisonExplosion = recursivePoison.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(poisonExplosion);

            //EffectComponent effectComponent = Assets.poisonExplosionEffect.GetComponent<EffectComponent>();
            //effectComponent.soundName = "assassinBottleBreak";

            poisonExplosion.blastRadius = 6f;
            poisonExplosion.destroyOnEnemy = true;
            poisonExplosion.destroyOnWorld = true;
            poisonExplosion.impactEffect = Assets.poisonExplosionEffect;
            //poisonExplosion.explosionSoundString = Assets.poisonExplosionEffect;
            poisonExplosion.lifetime = 12f;
            poisonExplosion.timerAfterImpact = true;
            poisonExplosion.lifetimeAfterImpact = 0.5f;

            ProjectileController poisonController = recursivePoison.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlRecursivePoison") != null) poisonController.ghostPrefab = CreateGhostPrefab("mdlRecursivePoison");

            poisonController.rigidbody = poisonRigidBody;
            poisonController.rigidbody.useGravity = true;
            poisonController.procCoefficient = 0.5f;
        }

        internal class recursiveClusterOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                ProjectileController recursiveCtrl = GetComponent<ProjectileController>();
                Vector3 impactPos = recursiveCtrl.transform.position;

                var characterBody = recursiveCtrl.owner.GetComponent<CharacterBody>();
                if (Util.HasEffectiveAuthority(characterBody.gameObject))
                {
                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y + 2, impactPos.z),
                        rotation = Quaternion.Euler(0, 0, 0),
                        projectilePrefab = clusterPoison,
                        speedOverride = 24,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };
                    FireProjectileInfo info2 = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y + 2, impactPos.z),
                        rotation = Quaternion.Euler(0, 120, 0),
                        projectilePrefab = clusterPoison,
                        speedOverride = 24,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };
                    FireProjectileInfo info3 = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = (2.5f * characterBody.damage) * 0.3f,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y + 2, impactPos.z),
                        rotation = Quaternion.Euler(0, 240, 0),
                        projectilePrefab = clusterPoison,
                        speedOverride = 24,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };

                    ProjectileManager.instance.FireProjectile(info);
                    ProjectileManager.instance.FireProjectile(info2);
                    ProjectileManager.instance.FireProjectile(info3);
                }
            }
        }

        private static void CreateEnderPearl()
        {
            enderPearl = CloneProjectilePrefab("CommandoGrenadeProjectile", "enderPearl");

            Rigidbody enderPearlRigidBody = enderPearl.GetComponent<Rigidbody>();
            if (!enderPearlRigidBody)
            {
                enderPearlRigidBody = enderPearl.AddComponent<Rigidbody>();
            }

            enderPearl.AddComponent<enderPearlOnHit>();

            ProjectileImpactExplosion pearlExplosion = enderPearl.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(pearlExplosion);

            pearlExplosion.blastRadius = 0f;
            pearlExplosion.lifetime = 12f;
            pearlExplosion.destroyOnEnemy = true;
            pearlExplosion.destroyOnWorld = true;
            pearlExplosion.impactEffect = Modules.Assets.pearlImpactEffect;

            ProjectileController enderPearlController = enderPearl.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlTpPotion") != null) enderPearlController.ghostPrefab = CreateGhostPrefab("mdlTpPotion");

            var poisonTrailDupe = Assets.pearlTrail;
            poisonTrailDupe.transform.parent = enderPearlController.ghostPrefab.transform;

            enderPearlController.rigidbody = enderPearlRigidBody;
            enderPearlController.rigidbody.useGravity = true;
            enderPearlController.procCoefficient = 0f;
        }

        private static void CreateCloudyPotion()
        {
            cloudyPotion = CloneProjectilePrefab("CommandoGrenadeProjectile", "smokeBomb");

            cloudyPotion.GetComponent<ProjectileDamage>().damageType = DamageType.Stun1s;
            cloudyPotion.AddComponent<ModdedDamageTypeHolderComponent>().Add(smokeDmgType);

            Rigidbody cloudRigidBody = cloudyPotion.GetComponent<Rigidbody>();
            if (!cloudRigidBody)
            {
                cloudRigidBody = cloudyPotion.AddComponent<Rigidbody>();
            }

            ProjectileImpactExplosion cloudExplosion = cloudyPotion.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(cloudExplosion);

            cloudExplosion.blastRadius = 6f;
            cloudExplosion.destroyOnEnemy = true;
            cloudExplosion.destroyOnWorld = true;
            cloudExplosion.impactEffect = Modules.Assets.smokeExplosionEffect;
            cloudExplosion.lifetime = 12f;

            cloudExplosion.timerAfterImpact = true;
            cloudExplosion.lifetimeAfterImpact = 0.5f;

            ProjectileController cloudController = cloudyPotion.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlSmokeBomb") != null) cloudController.ghostPrefab = CreateGhostPrefab("mdlSmokeBomb");

            GameObject smokeTrailDupe = Assets.smokeTrail;
            smokeTrailDupe.transform.parent = cloudController.ghostPrefab.transform;

            cloudController.rigidbody = cloudRigidBody;
            cloudController.rigidbody.useGravity = true;
            cloudController.procCoefficient = 0.2f;
        }

        internal class enderPearlOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                if (impactInfo.collider)
                {
                    Util.PlaySound("Play_ender_warp", gameObject);

                    ProjectileController tpControl = GetComponent<ProjectileController>();
                    Vector3 tpPosition = tpControl.transform.position;

                    var characterBody = tpControl.owner.GetComponent<CharacterBody>();

                    characterBody.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, 3);

                    /*Vector3 validTeleportPos;
                    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("moon2"))
                        validTeleportPos = tpPosition;
                    else
                        validTeleportPos = (Vector3)RoR2.TeleportHelper.FindSafeTeleportDestination(tpPosition, characterBody, RoR2Application.rng);*/

                    RoR2.TeleportHelper.TeleportBody(characterBody, tpPosition);

                    ProjectileController recursiveCtrl = GetComponent<ProjectileController>();
                    Vector3 impactPos = recursiveCtrl.transform.position;

                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        owner = characterBody.gameObject,
                        damage = 2.5f * characterBody.damage,
                        force = 0,
                        position = new Vector3(impactPos.x, impactPos.y, impactPos.z),
                        rotation = Quaternion.Euler(0, 0, 0),
                        projectilePrefab = poison,
                        speedOverride = 0,
                        //damageTypeOverride = (DamageType?)poisonDmgType
                    };

                    if(characterBody.HasBuff(Buffs.assassinDrugsBuff))
                        ProjectileManager.instance.FireProjectile(info);

                    Object.Destroy(gameObject);
                }
            }
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
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
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