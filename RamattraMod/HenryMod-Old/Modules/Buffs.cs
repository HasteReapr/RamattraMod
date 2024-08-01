using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace RamattraMod.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef vortexDebuff;

        internal static void RegisterBuffs()
        {
            vortexDebuff = AddNewBuff("Grounded",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite, 
                Color.white, 
                false, 
                false);

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
    }
}