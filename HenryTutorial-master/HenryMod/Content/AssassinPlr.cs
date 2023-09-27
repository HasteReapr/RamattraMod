using BepInEx.Configuration;
using AssassinMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using R2API;
using System;
using System.Collections.Generic;
using UnityEngine;
using AssassinMod.Modules;

namespace AssassinMod.Modules.Survivors
{
    internal class AssassinPlr : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "Assassin";

        public const string ASSASSIN_PREFIX = AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => ASSASSIN_PREFIX;
        public static Material assassinMaterial = Materials.CreateHopooMaterial("assassinMtrl");

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "AssassinSurvivorBody",
            bodyNameToken = ASSASSIN_PREFIX + "NAME",
            subtitleNameToken = ASSASSIN_PREFIX + "SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texAssassinIcon"),
            bodyColor = new Color(0.25882352941176470588235294117647f, 0.25882352941176470588235294117647f, 0.42352941176470588235294117647059f), //(66, 66, 108)
            sortPosition = 16,
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 125f,
            healthRegen = 1.5f,
            armor = 0f,
            moveSpeed = 9f,
            damage = 15f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
                new CustomRendererInfo
                {
                    childName = "Body",
                    material = assassinMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Cloak",
                    material = assassinMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Knife_L",
                    material = assassinMaterial
                },
                new CustomRendererInfo
                {
                    childName = "Knife_R",
                    material = assassinMaterial
                },
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(SkillStates.Assassin.AssassinMainState);

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
            masterySkinUnlockableDef = UnlockableAPI.AddUnlockable<Achievements.MasteryAchievement>();
            grandMasterySkinUnlockableDef = UnlockableAPI.AddUnlockable<Achievements.GrandMasteryAchievement>();
            //ABSOLUTELY HAVE TO CHANGE THIS TO USE BASEACHIEVEMENT FROM ROR2, INSTEAD OF THIS SHITTY OBSOLETE CODE
            //masterySkinUnlockableDef = Achievements.MasteryAchievement;
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //example of how to create a hitbox
            Transform backStabTransform = childLocator.FindChild("backstabHitbox");
            Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, backStabTransform, "BackstabHitBox");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = AssassinPlugin.DEVELOPER_PREFIX;

            SkillLocator sk = bodyPrefab.GetComponent<SkillLocator>();

            #region Primary
            //Creates a skilldef for a typical primary 
            SkillDef stockDaggerSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "_ASSASSIN_BODY_PRIMARY_DAGGER_NAME",
                                                                                      prefix + "_ASSASSIN_BODY_PRIMARY_DAGGER_DESCRIPTION",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowDagger)),
                                                                                      "Weapon",
                                                                                      true));

            stockDaggerSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_DAGGER_WC"
            };
            Modules.Skills.AddPrimarySkills(bodyPrefab, stockDaggerSkillDef);

            //Scepter Dagger
            /*SkillDef scepterDaggerSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo(prefix + "_ASSASSIN_BODY_PRIMARY_DAGGER_NAME",
                                                                                      prefix + "_ASSASSIN_BODY_PRIMARY_DAGGER_DESCRIPTION_SCEPTER",
                                                                                      Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIconScepter"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SkillStates.ScepterDagger)),
                                                                                      "Weapon",
                                                                                      true));


            Modules.Content.AddSkillDef(scepterDaggerSkillDef);*/
            //AssassinPlugin.SetupScepterStandalone("AssassinSurvivorBody", scepterDaggerSkillDef, SkillSlot.Primary, 0);
            #endregion

            #region Secondary
            SkillDef poisonSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
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

            poisonSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_POISON_WC"
            };

            Modules.Skills.AddSecondarySkills(bodyPrefab, poisonSkillDef);

            //Scepter Poison
            SkillDef scepterPoisonDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_DESCRIPTION_SCEPTER",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIconScepter"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ScepterPoison)),
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

            //Modules.Skills.AddSecondarySkills(bodyPrefab, scepterPoisonDef);
            Modules.Content.AddSkillDef(scepterPoisonDef);
            AssassinPlugin.SetupScepterStandalone("AssassinSurvivorBody", scepterPoisonDef, SkillSlot.Secondary, 0);
            #endregion

            #region AltSecondary
            SkillDef venomSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME_ALT",
                skillNameToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME_ALT",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_DESCRIPTION_ALT",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowVirulent)),
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

            //Modules.Skills.AddSecondarySkills(bodyPrefab, venomSkillDef);

            venomSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_VENOM_WC"
            };

            Skills.AddSkillToFamily(sk.secondary.skillFamily, venomSkillDef);

            SkillDef venomScepterSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME_ALT",
                skillNameToken = prefix + "_ASSASSIN_BODY_SECONDARY_POISON_NAME_ALT",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SCEPTER_SECONDARY_POISON_DESCRIPTION_ALT",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltScepterSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ScepterVenom)),
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

            Modules.Content.AddSkillDef(venomScepterSkillDef);
            AssassinPlugin.SetupScepterStandalone("AssassinSurvivorBody", venomScepterSkillDef, SkillSlot.Secondary, 1);
            #endregion

            #region Utility
            SkillDef cloakSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_UTILITY_CLOAK_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_UTILITY_CLOAK_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_UTILITY_CLOAK_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowPearl)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 5f,
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

            cloakSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_TELEPORT_WC"
            };

            Modules.Skills.AddUtilitySkills(bodyPrefab, cloakSkillDef);
            #endregion

            #region AltUltility
            SkillDef rollSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_UTILITY_ROLL_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_UTILITY_ROLL_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_UTILITY_ROLL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Roll)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
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

            UnlockableDef rollUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            rollUnlock.cachedName = "Skills.Assassin.Roll";
            rollUnlock.nameToken = "ACHIEVEMENT_ASSASSINROLLUNLOCK_NAME";
            rollUnlock.achievementIcon = rollSkillDef.icon;
            Modules.ContentPacks.unlockableDefs.Add(rollUnlock);
            rollSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_ROLL_WC"
            };

            Skills.AddSkillToFamily(sk.utility.skillFamily, rollSkillDef);
            //Modules.Skills.AddUtilitySkills(bodyPrefab, rollSkillDef);
            #endregion

            #region Special
            SkillDef warCrySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.DrugSelf)),
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

            Modules.Skills.AddSpecialSkills(bodyPrefab, warCrySkillDef);

            SkillDef warCryScepterDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SPECIAL_POISON_SPAM_DESCRIPTION_SCEPTER",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIconScepter"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.DrugSelf)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 20f,
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

            Modules.Content.AddSkillDef(warCryScepterDef);
            //AssassinPlugin.SetupScepterStandalone("AssassinSurvivorBody", warCryScepterDef, SkillSlot.Special, 0);
            #endregion

            #region AltSpecial
            SkillDef backstabSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_ASSASSIN_BODY_SPECIAL_BACKSTAB_NAME",
                skillNameToken = prefix + "_ASSASSIN_BODY_SPECIAL_BACKSTAB_NAME",
                skillDescriptionToken = prefix + "_ASSASSIN_BODY_SPECIAL_BACKSTAB_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Backstab)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = Config.BackstabInsta.Value ? 30 : 15,
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

            UnlockableDef backstabUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            backstabUnlock.cachedName = "Skills.Assassin.BackStab";
            backstabUnlock.nameToken = "ACHIEVEMENT_ASSASSINBACKSTABUNLOCK_NAME";
            backstabUnlock.achievementIcon = backstabSkillDef.icon;
            Modules.ContentPacks.unlockableDefs.Add(backstabUnlock);
            Skills.AddSkillToFamily(sk.special.skillFamily, backstabSkillDef);
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
            
            #region MasterySkin
            
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(ASSASSIN_PREFIX + "MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasterySkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                masterySkinUnlockableDef);

            /*childLocator.FindChild("grandMasteryAura_L").localScale = Vector3.zero;
            childLocator.FindChild("grandMasteryAura_R").localScale = Vector3.zero;
            childLocator.FindChild("masteryAura_L").localScale = Vector3.zero;
            childLocator.FindChild("masteryAura_R").localScale = Vector3.zero;*/

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "MasteryChest",
                "MasteryCloak",
                "MasteryKnife",
                "MasteryKnife");

            masterySkin.projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[] {
                new SkinDef.ProjectileGhostReplacement
                {
                    projectilePrefab = Projectiles.dagger,
                    projectileGhostReplacementPrefab = Projectiles.masteryDagger
                }
            };

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinMasteryMtrl");
            masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinMasteryMtrl");
            masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinMasteryMtrl");
            masterySkin.rendererInfos[3].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinMasteryMtrl");

            //Skins.AddCSSSkinChangeResponse(masterySkin, Skins.assassinCSSEffect.MASTERY);

            skins.Add(masterySkin);

            #endregion

            #region GrandMasterySkin

            //creating a new skindef as we did before
            SkinDef grandMasterySkin = Modules.Skins.CreateSkinDef(ASSASSIN_PREFIX + "GRAND_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGrandMasterySkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                grandMasterySkinUnlockableDef);

            /*childLocator.FindChild("grandMasteryAura_L").localScale = Vector3.zero;
            childLocator.FindChild("grandMasteryAura_R").localScale = Vector3.zero;
            childLocator.FindChild("masteryAura_L").localScale = Vector3.zero;
            childLocator.FindChild("masteryAura_R").localScale = Vector3.zero;*/

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            grandMasterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "GrandMasteryChest",
                "GrandMasteryCloak",
                "GrandMasteryKnife",
                "GrandMasteryKnife");

            grandMasterySkin.projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[] {
                new SkinDef.ProjectileGhostReplacement
                {
                    projectilePrefab = Projectiles.dagger,
                    projectileGhostReplacementPrefab = Projectiles.grandMasteryDagger
                }
            };

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            grandMasterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinGMMaterial");
            grandMasterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinGMMaterial");
            grandMasterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinGMMaterial");
            grandMasterySkin.rendererInfos[3].defaultMaterial = Modules.Materials.CreateHopooMaterial("assassinGMMaterial");

            //Skins.AddCSSSkinChangeResponse(grandMasterySkin, Skins.assassinCSSEffect.GRANDMASTERY);

            skins.Add(grandMasterySkin);

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