using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace AssassinMod.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;

        internal static BuffDef poisonDebuff;
        //internal static DotController.DotIndex poisonDoT;

        internal static BuffDef assassinDrugsBuff;
        internal static BuffDef hardcoreDrugsBuff;
        internal static BuffDef madGodBuff;

        internal static void RegisterBuffs()
        {
            armorBuff = AddNewBuff("HenryArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, 
                Color.white, 
                false, 
                false);

            poisonDebuff = AddNewBuff("AssassinPoison",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, //change this to the poison debuff icon at some point
                Color.green,
                true,
                true);

            assassinDrugsBuff = AddNewBuff("AssassinDrugsBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, //make the thing do the thing as above
                Color.red,
                false,
                false);

            hardcoreDrugsBuff = AddNewBuff("hardcoreDrugsBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, //make the thing do the thing as above
                Color.yellow,
                false,
                false);

            madGodBuff = AddNewBuff("MadGodBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, //make the thing do the thing as above
                Color.black,
                true,
                false
                );

            //RegisterDoTs();
        }

        /*private static void RegisterDoTs()
        {
            var poisonDef = new DotController.DotDef
            {
                interval = 0.1f,
                damageCoefficient = 0.02f,
                damageColorIndex = DamageColorIndex.Poison,
                associatedBuff = poisonDebuff,
                terminalTimedBuffDuration = 5f
            };

            //DotAPI.CustomDotBehaviour poisonBehaviour = AddCustomDotBehaviour;

            //poisonDoT = DotAPI.RegisterDotDef(poisonDef, poisonBehaviour);
        }*/

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }

        public class AssassinPoisonController : MonoBehaviour
        {
            public float interval = 0.1f;
            public float timer;
            public float totalDamage = 5;
            public float damageCoefficient = 0.01f;
            public float duration = 10f;
            public HealthComponent victimHealthComponent;
            public CharacterBody attackerBody;
            public DamageInfo info;
            //ticks 10 times a second for 1% of the total damage.

            private void Start()
            {
                attackerBody = GetComponent<CharacterBody>();
                //totalDamage = attackerBody.damage;
                victimHealthComponent = attackerBody.GetComponent<HealthComponent>();
                var inventory = attackerBody.inventory;
                if (inventory)
                {
                    //have some things affect how much damage the poison does
                }
            }

            private void FixedUpdate()
            {
                timer += Time.fixedDeltaTime;
                duration -= Time.fixedDeltaTime;
                if(timer >= interval && duration >= 0)
                {
                    if (victimHealthComponent)
                    {
                        info = new DamageInfo()
                        {
                            attacker = gameObject,
                            crit = false,
                            damage = damageCoefficient * totalDamage,// * victimBody.GetBuffCount(poisonDebuff), //this should be 1% of the total damage since it tics 10 times/second for 10 seconds.
                            damageColorIndex = DamageColorIndex.Heal,
                            force = Vector3.zero,
                            procCoefficient = 0.005f, //0.5% chance,
                            damageType = DamageType.PoisonOnHit,
                            position = victimHealthComponent.body.corePosition,
                            dotIndex = DotController.DotIndex.None,
                            inflictor = gameObject
                        };
                        victimHealthComponent.TakeDamage(info);
                    }
                    timer = 0f;
                }

                if(duration < 0f)
                {
                    Destroy(this);
                }
            }
        }
    }
}