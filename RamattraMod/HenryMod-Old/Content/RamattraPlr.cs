using BepInEx.Configuration;
using RamattraMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using R2API;
using System;
using System.Collections.Generic;
using UnityEngine;
using RamattraMod.Modules;
using RamattraMod.SkillStates.Ramattra.NemSkills;

namespace RamattraMod.Modules.Survivors
{
    internal class RamattraPlr : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "Ramattra";

        public const string ASSASSIN_PREFIX = RammyPlugin.DEVELOPER_PREFIX + "_RAMATTRA_BODY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => ASSASSIN_PREFIX;
        public static Material bodyMaterial = Materials.CreateHopooMaterial("rammyBodyMtrl");
        public static Material robeMaterial = Materials.CreateHopooMaterial("rammyRobeMtrl");
        public static Material clothMaterial = Materials.CreateHopooMaterial("rammyClothMtrl");

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "RamattraSurvivorBody",
            bodyNameToken = ASSASSIN_PREFIX + "NAME",
            subtitleNameToken = ASSASSIN_PREFIX + "SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texAssassinIcon"),
            bodyColor = new Color(0.25882352941176470588235294117647f, 0.25882352941176470588235294117647f, 0.42352941176470588235294117647059f), //(66, 66, 108)
            sortPosition = 16,
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

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

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
                new CustomRendererInfo
                {
                    childName = "Sash",
                    material = clothMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Belt-In",
                    material = clothMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Belt",
                    material = clothMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Pants",
                    material = clothMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Knots",
                    material = clothMaterial
                },

                new CustomRendererInfo
                {
                    childName = "Body",
                    material = bodyMaterial
                },

                new CustomRendererInfo
                {
                    childName = "Robe",
                    material = robeMaterial
                },
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(SkillStates.Ramattra.RamattraMainState);

        public override ItemDisplaysBase itemDisplays => new AssassinItemDisplays();

        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef grandMasterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = UnlockableAPI.AddUnlockable<Achievements.MasteryAchievement>();
            //grandMasterySkinUnlockableDef = UnlockableAPI.AddUnlockable<Achievements.GrandMasteryAchievement>();
            //ABSOLUTELY HAVE TO CHANGE THIS TO USE BASEACHIEVEMENT FROM ROR2, INSTEAD OF THIS SHITTY OBSOLETE CODE
            //masterySkinUnlockableDef = Achievements.MasteryAchievement;
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //example of how to create a hitbox
            Transform pummelTransform = childLocator.FindChild("pummelHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, pummelTransform, "pummelHitbox");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = RammyPlugin.DEVELOPER_PREFIX;

            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();

            #region Primary
            //Creates a skilldef for a typical primary 
            StaticValues.FireStaff = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "GRAHHH",
                skillNameToken = prefix + "GRAHHH",
                skillDescriptionToken = prefix + "GRAHHH",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ShootShard)),
                activationStateMachineName = "Weapon",
                canceledFromSprinting = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
            });

            StaticValues.ThrowPummel = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "GRAHHH",
                skillNameToken = prefix + "GRAHHH",
                skillDescriptionToken = prefix + "GRAHHH",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowPunch)),
                activationStateMachineName = "Weapon",
                canceledFromSprinting = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
            });

            Modules.Skills.AddPrimarySkills(bodyPrefab, StaticValues.FireStaff);
            #endregion

            #region Secondary
            StaticValues.SummonShield = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowPoison)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, StaticValues.SummonShield);
            #endregion

            #region Utility
            SkillDef shiftSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_UTILITY_CLOAK_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_UTILITY_CLOAK_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_UTILITY_CLOAK_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ShiftForms)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, shiftSkillDef);
            #endregion

            #region Special
            StaticValues.RavenousVortex = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowVoidBall)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 15f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, StaticValues.RavenousVortex);
            #endregion
        }

        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();
            #region fuckyou
            /*GameObject cloak = childLocator.FindChild("Body").gameObject;
            GameObject body = childLocator.FindChild("Cloak").gameObject;
            GameObject knifeL = childLocator.FindChild("Knife_L").gameObject;
            GameObject knifeR = childLocator.FindChild("Knife_R").gameObject;

            //these are the silly things that the knives go coocoo for cocoa puffs
            GameObject grandMasteryAuraL = childLocator.FindChild("grandMasteryAura_L").gameObject;
            GameObject grandMasteryAuraR = childLocator.FindChild("grandMasteryAura_R").gameObject;
            GameObject masteryAuraL = childLocator.FindChild("masteryAura_L").gameObject;
            GameObject masteryAuraR = childLocator.FindChild("masteryAura_R").gameObject;

            Skins.allGameObjectActivations.Add(cloak);
            Skins.allGameObjectActivations.Add(body);
            Skins.allGameObjectActivations.Add(knifeL);
            Skins.allGameObjectActivations.Add(knifeR);

            SkinDef.GameObjectActivation[] defaultActivations = Skins.getActivations();
            SkinDef.GameObjectActivation[] masteryActivations = Skins.getActivations(masteryAuraL, masteryAuraR);
            SkinDef.GameObjectActivation[] gMasteryActivations = Skins.getActivations(grandMasteryAuraL, grandMasteryAuraR);*/

            /*GameObject grandMasteryAuraL = GameObject.Instantiate(childLocator.FindChild("grandMasteryAura_L").gameObject);
            GameObject grandMasteryAuraR = GameObject.Instantiate(childLocator.FindChild("grandMasteryAura_R").gameObject);
            GameObject masteryAuraL = GameObject.Instantiate(childLocator.FindChild("masteryAura_L").gameObject);
            GameObject masteryAuraR = GameObject.Instantiate(childLocator.FindChild("masteryAura_R").gameObject);*/
            #endregion

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(ASSASSIN_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            /*defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "defaultChest",
                "defaultCloak",
                "defaultKnife",
                "defaultKnife");*/

            /*childLocator.FindChild("grandMasteryAura_L").localScale = Vector3.zero;
            childLocator.FindChild("grandMasteryAura_R").localScale = Vector3.zero;
            childLocator.FindChild("masteryAura_L").localScale = Vector3.zero;
            childLocator.FindChild("masteryAura_R").localScale = Vector3.zero;*/

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            //Skins.AddCSSSkinChangeResponse(defaultSkin, Skins.assassinCSSEffect.DEFAULT);

            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        protected override void InitializeDisplayPrefab()
        {
            base.InitializeDisplayPrefab();
            if (displayPrefab) displayPrefab.AddComponent<Components.MenuSoundComponent>();
        }
    }
}