using BepInEx;
using RamattraMod.Survivors.Ramattra;
using RamattraMod.Survivors.Ramattra.Content;
using RamattraMod.Survivors.Ramattra.Components;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using Unity;
using UnityEngine;

using static R2API.DamageAPI;
using R2API;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace RamattraMod
{
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class RammyPlugin : BaseUnityPlugin
    {
        // if you do not change this, you are giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.HasteReapr.RammyMod";
        public const string MODNAME = "RammyMod";
        public const string MODVERSION = "0.0.1";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "HASTEREAPR";

        public static RammyPlugin instance;

        BodyIndex ramIndex = BodyCatalog.FindBodyIndex("RamattraBody");

        void Awake()
        {
            //used for multiplayer testing, delete before public release
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };

            instance = this;

            //easy to use logger
            Log.Init(Logger);

            // used when you want to properly set up language folders
            Modules.Language.Init();

            // character initialization
            new RamattraSurvivor().Initialize();

            Hook();

            // make a content pack and add it. this has to be last
            new Modules.ContentPacks().Initialize();
        }

        private void Hook()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if(self.body.bodyIndex == ramIndex)
            {
                if (self.GetComponent<RamattraNemesisController>().isBlocking && self.body.HasBuff(RammyBuffs.blockBuff))
                {
                    damageInfo.damage *= 1 - (RamattraStaticValues.blockReductionPercent * self.body.GetBuffCount(RammyBuffs.blockBuff));
                }
            }

            orig(self, damageInfo);
        }

        private void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, RammyProjectiles.vortexDmgType))
            {
                self.AddTimedBuff(RammyBuffs.vortexDebuff, 0.5f);
            };

            if (self.HasBuff(RammyBuffs.vortexDebuff))
            {
                if (self.GetComponent<VortexGrounding>() == null) self.gameObject.AddComponent<VortexGrounding>().body = self;
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(RammyBuffs.vortexDebuff))
                {
                    self.moveSpeed *= 0.2f;
                }

                if (self.bodyIndex == ramIndex)
                {
                    if (self.GetComponent<RamattraNemesisController>().isNem)
                    {
                        self.moveSpeed *= 1.4f;
                    }
                    if (self.GetComponent<RamattraNemesisController>().isBlocking)
                    {
                        self.moveSpeed *= 0.6f;
                    }
                }
            }
        }
    }
}
