﻿using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using RamattraMod;
using UnityEngine;

namespace RamattraMod.Modules
{
    internal static class Skills
    {
        #region genericskills
        /// <summary>
        /// Create 4 GenericSkills for primary, secondary, utility, and special, and create skillfamilies for them
        /// </summary>
        /// <param name="targetPrefab">Body prefab to add GenericSkills</param>
        /// <param name="destroyExisting">Destroy any existing GenericSkills on the body prefab so you can replace them?</param>
        public static void CreateSkillFamilies(GameObject targetPrefab, bool destroyExisting = true) => CreateSkillFamilies(targetPrefab, destroyExisting, SkillSlot.Primary, SkillSlot.Secondary, SkillSlot.Utility, SkillSlot.Special);
        /// <summary>
        /// Destroy existing GenericSkills, create in order the GenericSkills for the skillslots desired, and create skillfamilies for them.
        /// </summary>
        /// <param name="targetPrefab">Body prefab to add GenericSkills</param>
        /// <param name="slots">Order of slots to add to the body prefab. Input SkillSlot.None to create a GenericSkill on the prefab outside of usual 4. For example, mul-t's selectable second primary, a selectable passive like acrid, etc</param>
        public static void CreateSkillFamilies(GameObject targetPrefab, params SkillSlot[] slots) => CreateSkillFamilies(targetPrefab, true, slots);

        /// <summary>
        /// Create in order the GenericSkills for the skillslots desired, and create skillfamilies for them.
        /// </summary>
        /// <param name="targetPrefab">Body prefab to add GenericSkills</param>
        /// <param name="destroyExisting">Destroy any existing GenericSkills on the body prefab so you can replace them?</param>
        /// <param name="slots">Order of slots to add to the body prefab. <para>Input SkillSlot.None to create a GenericSkill on the prefab outside of usual 4. These will be called GenericSkill1, with the number being its order on the loadout screen</para><para>For example, mul-t's selectable second primary, a selectable passive like acrid, etc</para></param>
        public static void CreateSkillFamilies(GameObject targetPrefab, bool destroyExisting, params SkillSlot[] slots)
        {
            //should this even be a thing here
            if (destroyExisting)
            {
                foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>())
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            SkillLocator skillLocator = targetPrefab.GetComponent<SkillLocator>();

            for (int i = 0; i < slots.Length; i++)
            {
                switch (slots[i])
                {
                    case SkillSlot.Primary:
                        skillLocator.primary = CreateGenericSkillWithSkillFamily(targetPrefab, "Primary");
                        break;
                    case SkillSlot.Secondary:
                        skillLocator.secondary = CreateGenericSkillWithSkillFamily(targetPrefab, "Secondary");
                        break;
                    case SkillSlot.Utility:
                        skillLocator.utility = CreateGenericSkillWithSkillFamily(targetPrefab, "Utility");
                        break;
                    case SkillSlot.Special:
                        skillLocator.special = CreateGenericSkillWithSkillFamily(targetPrefab, "Special");
                        break;
                    case SkillSlot.None:
                        CreateGenericSkillWithSkillFamily(targetPrefab, "GenericSkill" + i+1);
                        break;
                }
            }

        }

        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string familyName, bool hidden = false)
        {
            GenericSkill skill = targetPrefab.AddComponent<GenericSkill>();
            skill.skillName = familyName;
            skill.hideInCharacterSelect = hidden;

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = targetPrefab.name + familyName + "Family";
            newFamily.variants = new SkillFamily.Variant[0];

            skill._skillFamily = newFamily;

            RamattraMod.Modules.Content.AddSkillFamily(newFamily);
            return skill;
        }
        #endregion

        #region skillfamilies

        //everything calls this
        public static void AddSkillToFamily(SkillFamily skillFamily, SkillDef skillDef, UnlockableDef unlockableDef = null)
        {
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = unlockableDef,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        public static void AddSkillsToFamily(SkillFamily skillFamily, params SkillDef[] skillDefs)
        {
            foreach (SkillDef skillDef in skillDefs)
            {
                AddSkillToFamily(skillFamily, skillDef);
            }
        }

        public static void AddPrimarySkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().primary.skillFamily, skillDefs);
        }
        public static void AddSecondarySkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().secondary.skillFamily, skillDefs);
        }
        public static void AddUtilitySkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().utility.skillFamily, skillDefs);
        }
        public static void AddSpecialSkills(GameObject targetPrefab, params SkillDef[] skillDefs)
        {
            AddSkillsToFamily(targetPrefab.GetComponent<SkillLocator>().special.skillFamily, skillDefs);
        }

        /// <summary>
        /// pass in an amount of unlockables equal to or less than skill variants, null for skills that aren't locked
        /// <code>
        /// AddUnlockablesToFamily(skillLocator.primary, null, skill2UnlockableDef, null, skill4UnlockableDef);
        /// </code>
        /// </summary>
        public static void AddUnlockablesToFamily(SkillFamily skillFamily, params UnlockableDef[] unlockableDefs)
        {
            for (int i = 0; i < unlockableDefs.Length; i++)
            {
                SkillFamily.Variant variant = skillFamily.variants[i];
                variant.unlockableDef = unlockableDefs[i];
                skillFamily.variants[i] = variant;
            }
        }
        #endregion

        #region skilldefs
        public static SkillDef CreateSkillDef(SkillDefInfo skillDefInfo)
        {
            return CreateSkillDef<SkillDef>(skillDefInfo);
        }

        public static T CreateSkillDef<T>(SkillDefInfo skillDefInfo) where T : SkillDef
        {
            //pass in a type for a custom skilldef, e.g. HuntressTrackingSkillDef
            T skillDef = ScriptableObject.CreateInstance<T>();

            skillDef.skillName = skillDefInfo.skillName;
            (skillDef as ScriptableObject).name = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;

            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;

            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;

            skillDef.dontAllowPastMaxStocks = skillDefInfo.dontAllowPastMaxStocks;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;

            RamattraMod.Modules.Content.AddSkillDef(skillDef);


            return skillDef;
        }
        #endregion skilldefs
    }

    /// <summary>
    /// class for easily creating skilldefs with default values, and with a field for UnlockableDef
    /// </summary>
    internal class SkillDefInfo
    {
        public string skillName;
        public string skillNameToken;
        public string skillDescriptionToken;
        public string[] keywordTokens = new string[0];
        public Sprite skillIcon;

        public SerializableEntityStateType activationState;
        public string activationStateMachineName;
        public InterruptPriority interruptPriority;

        public int baseMaxStock = 1;
        public float baseRechargeInterval;

        public int rechargeStock = 1;
        public int requiredStock = 1;
        public int stockToConsume = 1;

        public bool resetCooldownTimerOnUse = false;
        public bool fullRestockOnAssign = true;
        public bool dontAllowPastMaxStocks = false;
        public bool beginSkillCooldownOnSkillEnd = false;
        public bool mustKeyPress = false;

        public bool isCombatSkill = true;
        public bool canceledFromSprinting = false;
        public bool cancelSprintingOnActivation = true;
        public bool forceSprintDuringState = false;

        #region constructors
        public SkillDefInfo() { }
        /// <summary>
        /// Creates a skilldef for a typical primary.
        /// <para>combat skill, cooldown: 0, required stock: 0, InterruptPriority: Any</para>
        /// </summary>
        public SkillDefInfo(string skillName,
                            string skillNameToken,
                            string skillDescriptionToken,
                            Sprite skillIcon,

                            SerializableEntityStateType activationState,
                            string activationStateMachineName = "Weapon",
                            bool agile = false)
        {
            this.skillName = skillName;
            this.skillNameToken = skillNameToken;
            this.skillDescriptionToken = skillDescriptionToken;
            this.skillIcon = skillIcon;

            this.activationState = activationState;
            this.activationStateMachineName = activationStateMachineName;

            this.cancelSprintingOnActivation = !agile;

            if (agile) this.keywordTokens = new string[] { "KEYWORD_AGILE" };

            this.interruptPriority = InterruptPriority.Any;
            this.isCombatSkill = true;
            this.baseRechargeInterval = 0;

            this.requiredStock = 0;
            this.stockToConsume = 0;

        }
        #endregion construction complete
    }
}