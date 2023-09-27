using BepInEx;
using AssassinMod.Modules.Survivors;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using R2API.Utils;
using RoR2.Skills;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

//delete after rendering avatar
/*using AssassinMod.Items;
using System.Reflection;
using System.Linq;*/

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace AssassinMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class AssassinPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.HasteReapr.AssassinMod";
        public const string MODNAME = "AssassinMod";
        public const string MODVERSION = "1.3.2";

        public const string DEVELOPER_PREFIX = "HASTEREAPR";

        public static AssassinPlugin instance;

        public static bool emoteAPILoaded = false;
        public static bool scepterStandaloneLoaded = false;

        //delete this after rendering the avatar
        //public List<ItemBase> Items = new List<ItemBase>();
        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            //Modules.SoundBanks.Init();

            emoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("MetrosexualFruitcake-CustomEmotesAPI-1.10.2");
            scepterStandaloneLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");

            // survivor initialization
            new AssassinPlr().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();

            EmoteApiCompat();

            //This section automatically scans the project for all items
            /*var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);
                }
            }*/
        }

        /*public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }*/

        private void Hook()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;

            On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;

            EmotesAPI.CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if(newAnimation != "none")
            {
                if(mapper.transform.name == "Assassin_Emote_Skeleton")
                {
                    //mapper.transform.parent.Find("Knife.L").gameObject.SetActive(false);
                    //mapper.transform.parent.Find("Knife.R").gameObject.SetActive(false);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_L").gameObject.SetActive(false);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_R").gameObject.SetActive(false);
                }
            }
            else
            {
                if (mapper.transform.name == "Assassin_Emote_Skeleton")
                {
                    //mapper.transform.parent.Find("Knife.L").gameObject.SetActive(true);
                    //mapper.transform.parent.Find("Knife.R").gameObject.SetActive(true);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_L").gameObject.SetActive(true);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_R").gameObject.SetActive(true);
                }
            }
        }

        private void EmoteApiCompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "AssassinSurvivorBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("Assassin_Emote_Skeleton.prefab");
                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele, jank: true);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1f;
                    }
                }
            };
        }

        private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
            orig(self, damageReport);
            //"com.HasteReapr.AssassinMod_AssassinBody_0(Clone)"
            if (self.baseNameToken == DEVELOPER_PREFIX + "_ASSASSIN_BODY_NAME")
            {
                if(self.healthComponent.combinedHealth <= (self.healthComponent.fullCombinedHealth) * 0.6f)
                {
                    self.AddTimedBuff(Modules.Buffs.madGodBuff, 3);
                }
            };

            //if the victim is hit by any of the poison stuff
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, Modules.Projectiles.poisonDmgType))
            {
                damageReport.victimBody.AddTimedBuff(Modules.Buffs.poisonDebuff, 10f);

                //MonoBehaviour poisonEff = new Modules.Buffs.AssassinPoisonController();
                damageReport.victimBody.gameObject.AddComponent<Modules.Buffs.AssassinPoisonController>();
                //obj.victimBody.gameObject.GetComponent<Modules.Buffs.AssassinPoisonController>().totalDamage = obj.damageInfo.damage * 5;
                damageReport.victimBody.gameObject.GetComponent<Modules.Buffs.AssassinPoisonController>().totalDamage = Util.OnHitProcDamage(damageReport.damageInfo.damage, damageReport.attackerBody.damage, 5f);
            }

            //if the victim is hit by the smokebomb AOE apply stun
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, Modules.Projectiles.smokeDmgType))
            {
                //damageReport.victimBody.AddTimedBuff(RoR2Content.Buffs., 2);
                RoR2.SetStateOnHurt.SetStunOnObject(damageReport.victimBody.gameObject, 2.5f);
            }

            //if the victim is hit by backstab damage type
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, Modules.Projectiles.backStabDmg))
            { //then checks if its a backstab
                if(BackstabManager.IsBackstab(damageReport.attackerBody.characterDirection.forward, damageReport.victimBody))
                {
                    float curHP = damageReport.victimBody.healthComponent.combinedHealth;
                    float maxHP = damageReport.victimBody.healthComponent.fullCombinedHealth;
                    float dmg = 0;

                    if (Modules.Config.BackstabInsta.Value || !damageReport.victimBody.isBoss)
                    {
                        //if we arent a boss, then do the damage thing
                        //just making sure we dont overflow & do negative damage
                        if ((curHP + maxHP) * 6 <= int.MaxValue)
                            dmg = (curHP + maxHP) * 6;
                        else
                            dmg = int.MaxValue;
                    } 
                    else
                    {
                        if (RoR2.Util.CheckRoll(Modules.Config.BackstabChance.Value, damageReport.attackerMaster))
                        {
                            //just making sure we dont overflow & do negative damage
                            if ((curHP + maxHP) * 6 <= int.MaxValue)
                                dmg = (curHP + maxHP) * 6;
                            else
                                dmg = int.MaxValue;
                        }
                        else
                            dmg = damageReport.victimBody.healthComponent.fullHealth * 0.25f + damageReport.victimBody.healthComponent.adaptiveArmorValue;
                    }

                    DamageInfo dmgInfo = new DamageInfo()
                    {
                        attacker = damageReport.attackerBody.gameObject,
                        crit = false,
                        damage = dmg,
                        damageColorIndex = DamageColorIndex.Bleed,
                        force = Vector3.zero,
                        procCoefficient = 0,
                        damageType = DamageType.Generic,
                        position = damageReport.victimBody.corePosition,
                        inflictor = damageReport.attackerBody.gameObject
                    };
                    damageReport.victimBody.healthComponent.TakeDamage(dmgInfo);
                    damageReport.attackerBody.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, 2.5f);

                    //if (RoR2.Util.CheckRoll(Modules.Config.RechargeChance.Value, damageReport.attackerMaster))
                        damageReport.attackerBody.skillLocator.special.rechargeStopwatch = 0f;
                }
            }

            //if (BackstabManager.IsBackstab(damageReport.attackerBody.corePosition, damageReport.victimBody)) Chat.AddMessage("Is a succesful backstab.");
        }

        /*private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            //if the victim is hit by any of the poison stuff
            if(DamageAPI.HasModdedDamageType(obj.damageInfo, Modules.Projectiles.poisonDmgType))
            {
                obj.victimBody.AddTimedBuff(Modules.Buffs.poisonDebuff, 10f);

                //MonoBehaviour poisonEff = new Modules.Buffs.AssassinPoisonController();
                obj.victimBody.gameObject.AddComponent<Modules.Buffs.AssassinPoisonController>();
                //obj.victimBody.gameObject.GetComponent<Modules.Buffs.AssassinPoisonController>().totalDamage = obj.damageInfo.damage * 5;
                obj.victimBody.gameObject.GetComponent<Modules.Buffs.AssassinPoisonController>().totalDamage = Util.OnHitProcDamage(obj.damageInfo.damage, obj.attackerBody.damage, 5f);
            }

            //if the victim is hit by the smokebomb AOE apply stun
            if(DamageAPI.HasModdedDamageType(obj.damageInfo, Modules.Projectiles.smokeDmgType))
            {
                obj.victimBody.AddTimedBuff(RoR2Content.Buffs.Nullified, 2);
            }

            //if its backstab then just set the damage to 6x the full hp
            if (DamageAPI.HasModdedDamageType(obj.damageInfo, Modules.Projectiles.backStabDmg))
            {
                if(BackstabManager.IsBackstab(obj.attackerBody.characterDirection, obj.victimBody))
                {
                    float curHP = obj.victimBody.healthComponent.combinedHealth;
                    float maxHP = obj.victimBody.healthComponent.fullCombinedHealth;

                    //just making sure we dont overflow & do negative damage
                    if ((curHP + maxHP) * 6 <= int.MaxValue)
                        obj.damageInfo.damage = (curHP + maxHP) * 6;
                    else
                        obj.damageInfo.damage = int.MaxValue;
                }
            }

            //if the victim has poison do 10% more damage
            if (obj.victimBody.HasBuff(Modules.Buffs.poisonDebuff))
            {
                obj.damageInfo.damage *= 1.1f;
            }
        }*/

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(Modules.Buffs.madGodBuff))
                {
                    self.damage *= 1 + (0.075f * self.GetBuffCount(Modules.Buffs.madGodBuff));
                    self.attackSpeed *= 1 + (0.075f * self.GetBuffCount(Modules.Buffs.madGodBuff));
                }

                if (self.HasBuff(Modules.Buffs.hardcoreDrugsBuff))
                {
                    self.damage *= 1.05f;
                    self.attackSpeed *= 1.05f;
                    self.maxHealth *= 1.05f;
                    self.moveSpeed *= 1.05f;
                    self.regen *= 1.05f;
                }
            }
        }

        public static void SetupScepterStandalone(string bodyName, SkillDef scepterSkill, SkillSlot skillSlot, int skillIndex)
        {
            if (scepterStandaloneLoaded) SetupScepterStandaloneInternal(bodyName, scepterSkill, skillSlot, skillIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupScepterStandaloneInternal(string bodyName, SkillDef scepterSkill, SkillSlot skillSlot, int skillIndex)
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSkill, bodyName, skillSlot, skillIndex);
        }
    }
}