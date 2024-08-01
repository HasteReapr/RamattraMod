using BepInEx.Configuration;
using RamattraMod.Modules;
using RamattraMod.Modules.Characters;
using RamattraMod.Survivors.Ramattra.Components;
using RamattraMod.Survivors.Ramattra.SkillStates;
using RamattraMod.Survivors.Ramattra.SkillStates.NemStates;
using RoR2;
using RoR2.Skills;
using R2API;
using R2API.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RamattraMod.Survivors.Ramattra
{
    public class RamattraSurvivor : SurvivorBase<RamattraSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "ramattrabundle"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "RamattraBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "RamattraMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlRamattra";
        public override string displayPrefabName => "RamattraDisplay";

        public const string RAMMY_PREFIX = RammyPlugin.DEVELOPER_PREFIX + "_RAMMY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => RAMMY_PREFIX;

        //public static Material bodyMaterial = Materials.CreateHopooMaterial("rammyBodyMtrl");
        //public static Material robeMaterial = Materials.CreateHopooMaterial("rammyRobeMtrl");
        //public static Material clothMaterial = Materials.CreateHopooMaterial("rammyClothMtrl");
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = RAMMY_PREFIX + "NAME",
            subtitleNameToken = RAMMY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = Color.white,
            sortPosition = 8,

            crosshair = Assets.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 150f,
            shield = 100f,
            healthGrowth = 0.35f,
            healthRegen = 1.5f,
            armor = 10f,
            moveSpeed = 6f,
            damage = 15f,
            attackSpeed = 1,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                //clothes n stuff
                new CustomRendererInfo
                {
                    childName = "Sash",
                    material = assetBundle.LoadMaterial("rammyClothMtrl")
                },
                new CustomRendererInfo
                {
                    childName = "Belt-In",
                    material = assetBundle.LoadMaterial("rammyClothMtrl")
                },
                new CustomRendererInfo
                {
                    childName = "Belt",
                    material = assetBundle.LoadMaterial("rammyClothMtrl")
                },
                new CustomRendererInfo
                {
                    childName = "Pants",
                    material = assetBundle.LoadMaterial("rammyClothMtrl")
                },
                new CustomRendererInfo
                {
                    childName = "Knots",
                    material = assetBundle.LoadMaterial("rammyClothMtrl")
                },
                //main body
                new CustomRendererInfo
                {
                    childName = "Body",
                    material = assetBundle.LoadMaterial("rammyBodyMtrl")
                },
                //buff arms
                new CustomRendererInfo
                {
                    childName = "Arms",
                    material = assetBundle.LoadMaterial("rammyArmsMtrl")
                },
                new CustomRendererInfo
                {
                    childName = "Arm Circles",
                    material = assetBundle.LoadMaterial("rammyArmsMtrl")
                },
                //staff
                new CustomRendererInfo
                {
                    childName = "Staff",
                    material = assetBundle.LoadMaterial("rammyStaffMtrl")
                },
                new CustomRendererInfo
                {
                    childName = "Staff Orb",
                    material = assetBundle.LoadMaterial("rammyStaffMtrl")
                },
                //visor
                new CustomRendererInfo
                {
                    childName = "Visor",
                    material = assetBundle.LoadMaterial("rammyVisorMtrl")
                },
                //robe
                new CustomRendererInfo
                {
                    childName = "Robe",
                    material = assetBundle.LoadMaterial("rammyRobeMtrl")
                },
        };

        public override UnlockableDef characterUnlockableDef => RamattraUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new RamattraItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();

            bodyPrefab.AddComponent<RamattraNemesisController>();
            bodyPrefab.GetComponent<RamattraNemesisController>().shouldSwitch = false;

            bodyPrefab.AddComponent<RamattraAnnihilationController>();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            RamattraUnlockables.Init();

            base.InitializeCharacter();

            RamattraConfig.Init();
            RamattraStates.Init();
            RamattraTokens.Init();

            RamattraAssets.Init(assetBundle);
            RammyBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<HenryWeaponComponent>();
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
            ChildLocator childLocator = characterModelObject.GetComponent<ChildLocator>();
            //replace the materials for the particle effects that need it

            //Material circleMat = UnityEngine.Object.Instantiate(EntityStates.GolemMonster.ChargeLaser.effectPrefab.transform.Find("Particles").Find("Glow").GetComponent<ParticleSystemRenderer>().material);

            //childLocator.FindChild("AnnihilationArea").Find("Indicator").GetComponent<ParticleSystemRenderer>().material = circleMat;
        }

        public void AddHitboxes()
        {
            ChildLocator childLocator = characterModelObject.GetComponent<ChildLocator>();

            //example of how to create a hitbox
            Transform pummelTransform = childLocator.FindChild("pummelHitbox");
            Prefabs.SetupHitbox(prefabCharacterModel.gameObject, pummelTransform, "pummelHitbox");
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs
            //the main "body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(RamattraMainState), typeof(EntityStates.SpawnTeleporterState));
            
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPrmarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }
        private void AddPrmarySkills()
        {
            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            RamattraStaticValues.FireStaff = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "RammyVoidShard",
                    RAMMY_PREFIX + "PRIMARY_STAFF_NAME",
                    RAMMY_PREFIX + "PRIMARY_STAFF_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(ShootShard)),
                    "Weapon",
                    true
                ));
            //custom Skilldefs can have additional fields that you can set manually
            RamattraStaticValues.FireStaff.stepCount = 2;
            RamattraStaticValues.FireStaff.stepGraceDuration = 0.5f;

            RamattraStaticValues.ThrowPummel = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "RammyThrowHands",
                    RAMMY_PREFIX + "PRIMARY_PUNCH_NAME",
                    RAMMY_PREFIX + "PRIMARY_PUNCH_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(ThrowPunch)),
                    "Weapon",
                    true
                ));
            //custom Skilldefs can have additional fields that you can set manually
            RamattraStaticValues.ThrowPummel.stepCount = 2;
            RamattraStaticValues.ThrowPummel.stepGraceDuration = 0.5f;

            Skills.AddPrimarySkills(bodyPrefab, RamattraStaticValues.FireStaff);
        }

        private void AddSecondarySkills()
        {
            //here is a basic skill def with all fields accounted for
            RamattraStaticValues.SummonShield = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RammySummonShield",
                skillNameToken = RAMMY_PREFIX + "SECONDARY_RESTORE_NAME",
                skillDescriptionToken = RAMMY_PREFIX + "SECONDARY_RESTORE_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.SummonShield)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 12f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            RamattraStaticValues.NemBlock = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RammyNemBlock",
                skillNameToken = RAMMY_PREFIX + "SECONDARY_BLOCK_NAME",
                skillDescriptionToken = RAMMY_PREFIX + "SECONDARY_BLOCK_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.NemStates.NemesisBlock)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0.3f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = true,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

            });

            Skills.AddSecondarySkills(bodyPrefab, RamattraStaticValues.SummonShield);
        }

        private void AddUtiitySkills()
        {
            //The shift forms, this swaps between omnic and regular forms, it has a low recharge time so you can swap between things rapidly.
            SkillDef rollSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RammyShift",
                skillNameToken = RAMMY_PREFIX + "UTILITY_SHIFT_NAME",
                skillDescriptionToken = RAMMY_PREFIX + "UTILITY_SHIFT_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ShiftForms)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Frozen,

                baseMaxStock = 1,
                baseRechargeInterval = 1f,

                isCombatSkill = false,
                mustKeyPress = false,
                forceSprintDuringState = false,
                cancelSprintingOnActivation = false,
            });

            Skills.AddUtilitySkills(bodyPrefab, rollSkillDef);
        }

        private void AddSpecialSkills()
        {
            //a basic skill
            RamattraStaticValues.RavenousVortex = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RammyVortex",
                skillNameToken = RAMMY_PREFIX + "SPECIAL_VORTEX_NAME",
                skillDescriptionToken = RAMMY_PREFIX + "SPECIAL_VORTEX_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowVoidBall)),
                activationStateMachineName = "Weapon2", //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 10f,

                isCombatSkill = true,
                mustKeyPress = false,
                forceSprintDuringState = false,
                cancelSprintingOnActivation = false,
            });

            RamattraStaticValues.Annihilation = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RammyAnnihilation",
                skillNameToken = RAMMY_PREFIX + "SPECIAL_SWARM_NAME",
                skillDescriptionToken = RAMMY_PREFIX + "SPECIAL_SWARM_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.NemStates.Annihilation)),
                activationStateMachineName = "Weapon2", //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 10f,

                resetCooldownTimerOnUse = true,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                mustKeyPress = false,
                forceSprintDuringState = false,
                cancelSprintingOnActivation = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, RamattraStaticValues.RavenousVortex);
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            RamattraAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

        }
    }
}