using BepInEx;
using RamattraMod.Modules.Survivors;
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
namespace RamattraMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MetrosexualFruitcake-CustomEmotesAPI-1.10.2", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class RammyPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.HasteReapr.RammyMod";
        public const string MODNAME = "RammyMod";
        public const string MODVERSION = "0.0.1";

        public const string DEVELOPER_PREFIX = "HASTEREAPR";

        public static RammyPlugin instance;

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
            new RamattraPlr().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();

            //EmoteApiCompat();

        }

        private void Hook()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;

            On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;

            //EmotesAPI.CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
        }

        /*private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if(newAnimation != "none")
            {
                if(mapper.transform.name == "rogue_emote_skeleton" || mapper.transform.name == "rogue_emote_skeleton_tiny")
                {
                    //mapper.transform.parent.Find("Knife.L").gameObject.SetActive(false);
                    //mapper.transform.parent.Find("Knife.R").gameObject.SetActive(false);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_L").gameObject.SetActive(false);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_R").gameObject.SetActive(false);
                }
            }
            else
            {
                if (mapper.transform.name == "rogue_emote_skeleton" || mapper.transform.name == "rogue_emote_skeleton_tiny")
                {
                    //mapper.transform.parent.Find("Knife.L").gameObject.SetActive(true);
                    //mapper.transform.parent.Find("Knife.R").gameObject.SetActive(true);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_L").gameObject.SetActive(true);
                    mapper.transform.parent.GetComponent<ChildLocator>().FindChild("Knife_R").gameObject.SetActive(true);
                }
            }
        }*/

        private void EmoteApiCompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "AssassinSurvivorBody")
                    {
                        string assassin_skele = "rogue_emote_skeleton.prefab";
                        System.Random rnd = new System.Random();
                        if (rnd.Next(0, 100) <= 1) {
                            assassin_skele = "rogue_emote_skeleton_tiny.prefab";
                            Log.Info("Little guy :3");
                        }
                        
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>(assassin_skele);
                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele, jank: true);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1f;
                    }
                }
            };
        }

        private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, Modules.Projectiles.vortexDmgType))
            {
                self.AddTimedBuff(Modules.Buffs.vortexDebuff, 0.5f);
            };

            if (self.HasBuff(Modules.Buffs.vortexDebuff))
            {
                if (self.GetComponent<Modules.Misc.VortexGrounding>() == null) self.gameObject.AddComponent<Modules.Misc.VortexGrounding>().body = self;
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(Modules.Buffs.vortexDebuff))
                {
                    self.moveSpeed *= 0.2f;
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