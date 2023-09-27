using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AssassinMod.Modules.Survivors;
using SkinChangeResponse = RoR2.CharacterSelectSurvivorPreviewDisplayController.SkinChangeResponse;

namespace AssassinMod.Modules
{
    internal static class Skins
    {
        public static List<GameObject> allGameObjectActivations = new List<GameObject>();

        private static CharacterSelectSurvivorPreviewDisplayController assassinCSSPreviewCTRL;
        private static SkinChangeResponse[] defaultResponses;
        public enum assassinCSSEffect
        {
            DEFAULT,
            MASTERY,
            GRANDMASTERY
        }

        #region tools
        //this is stolen from paladin bc i dont know what im doing
        public static void AddCSSSkinChangeResponse(SkinDef def, assassinCSSEffect assassinCSS)
        {
            SkinChangeResponse skinChangeResponse = defaultResponses[(int)assassinCSS];
            skinChangeResponse.triggerSkin = def;

            SkinChangeResponse[] addedSkinchange = new SkinChangeResponse[] {
                skinChangeResponse
            };

            assassinCSSPreviewCTRL.skinChangeResponses = assassinCSSPreviewCTRL.skinChangeResponses.Concat(addedSkinchange).ToArray();
        }

        //also stolen from paladin, disecting it line by line has kind of helped me understand it
        public static SkinDef.GameObjectActivation[] getActivations(params GameObject[] activatedObjects)
        {

            List<SkinDef.GameObjectActivation> GameObjectActivations = new List<SkinDef.GameObjectActivation>();

            for (int i = 0; i < allGameObjectActivations.Count; i++)
            {

                bool activate = activatedObjects.Contains(allGameObjectActivations[i]);
                //activate |= Config.spookyArms.Value && (i == 1 || i == 2);

                GameObjectActivations.Add(new SkinDef.GameObjectActivation
                {
                    gameObject = allGameObjectActivations[i],
                    shouldActivate = activate
                });
            }

            return GameObjectActivations.ToArray();
        }


        internal static SkinDef CreateSkinDef(string skinName, Sprite skinIcon, CharacterModel.RendererInfo[] defaultRendererInfos, GameObject root, UnlockableDef unlockableDef = null)
        {
            SkinDefInfo skinDefInfo = new SkinDefInfo
            {
                BaseSkins = Array.Empty<SkinDef>(),
                GameObjectActivations = new SkinDef.GameObjectActivation[0],
                Icon = skinIcon,
                MeshReplacements = new SkinDef.MeshReplacement[0],
                MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
                Name = skinName,
                NameToken = skinName,
                ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
                RendererInfos = new CharacterModel.RendererInfo[defaultRendererInfos.Length],
                RootObject = root,
                UnlockableDef = unlockableDef
            };

            On.RoR2.SkinDef.Awake += DoNothing;

            SkinDef skinDef = ScriptableObject.CreateInstance<RoR2.SkinDef>();
            skinDef.baseSkins = skinDefInfo.BaseSkins;
            skinDef.icon = skinDefInfo.Icon;
            skinDef.unlockableDef = skinDefInfo.UnlockableDef;
            skinDef.rootObject = skinDefInfo.RootObject;
            defaultRendererInfos.CopyTo(skinDefInfo.RendererInfos, 0);
            skinDef.rendererInfos = skinDefInfo.RendererInfos;
            skinDef.gameObjectActivations = skinDefInfo.GameObjectActivations;
            skinDef.meshReplacements = skinDefInfo.MeshReplacements;
            skinDef.projectileGhostReplacements = skinDefInfo.ProjectileGhostReplacements;
            skinDef.minionSkinReplacements = skinDefInfo.MinionSkinReplacements;
            skinDef.nameToken = skinDefInfo.NameToken;
            skinDef.name = skinDefInfo.Name;

            On.RoR2.SkinDef.Awake -= DoNothing;

            return skinDef;
        }

        private static void DoNothing(On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self)
        {
        }

        internal struct SkinDefInfo
        {
            internal SkinDef[] BaseSkins;
            internal Sprite Icon;
            internal string NameToken;
            internal UnlockableDef UnlockableDef;
            internal GameObject RootObject;
            internal CharacterModel.RendererInfo[] RendererInfos;
            internal SkinDef.MeshReplacement[] MeshReplacements;
            internal SkinDef.GameObjectActivation[] GameObjectActivations;
            internal SkinDef.ProjectileGhostReplacement[] ProjectileGhostReplacements;
            internal SkinDef.MinionSkinReplacement[] MinionSkinReplacements;
            internal string Name;
        }

        private static CharacterModel.RendererInfo[] getRendererMaterials(CharacterModel.RendererInfo[] defaultRenderers, params Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            for (int i = 0; i < newRendererInfos.Length; i++)
            {
                try
                {
                    newRendererInfos[i].defaultMaterial = materials[i];
                }
                catch
                {
                    Log.Error("error adding skin rendererinfo material. make sure you're not passing in too many");
                }
            }

            return newRendererInfos;
        }
        /// <summary>
        /// pass in strings for mesh assets in your bundle. pass the same amount and order based on your rendererinfos, filling with null as needed
        /// <code>
        /// myskindef.meshReplacements = Modules.Skins.getMeshReplacements(defaultRenderers,
        ///    "meshHenrySword",
        ///    null,
        ///    "meshHenry");
        /// </code>
        /// </summary>
        /// <param name="defaultRendererInfos">your skindef's rendererinfos to access the renderers</param>
        /// <param name="meshes">name of the mesh assets in your project</param>
        /// <returns></returns>
        internal static SkinDef.MeshReplacement[] getMeshReplacements(CharacterModel.RendererInfo[] defaultRendererInfos, params string[] meshes)
        {

            List<SkinDef.MeshReplacement> meshReplacements = new List<SkinDef.MeshReplacement>();

            for (int i = 0; i < defaultRendererInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(meshes[i]))
                    continue;

                meshReplacements.Add(
                new SkinDef.MeshReplacement
                {
                    renderer = defaultRendererInfos[i].renderer,
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>(meshes[i])
                });
            }

            return meshReplacements.ToArray();
        }
        #endregion

        #region Skinerize
        public static Dictionary<uint, string> SkinIdices = new Dictionary<uint, string>();
        public static List<SkinDef> skinDefs = new List<SkinDef>();

        public enum AssassinSkin
        {
            NONE = -1,
            DEFAULT,
            MASTERY,
            GRANDMASTERY
        }

        public static bool isAssassinCurrentSkin(CharacterBody charBody, string skin)
        {
            return charBody.baseNameToken == "AssassinSurvivorBody" && SkinIdices.ContainsKey(charBody.skinIndex) && SkinIdices[charBody.skinIndex] == skin;
        }

        public static bool isAssassinCurrentSkin(CharacterBody charBody, AssassinSkin skin)
        {
            return isAssassinCurrentSkin(charBody, GetSkinID(skin));
        }

        public static string GetSkinID(AssassinSkin skin)
        {
            switch (skin)
            {
                default:
                case AssassinSkin.DEFAULT:
                    return AssassinPlr.ASSASSIN_PREFIX + "DEFAULT_SKIN_NAME";
                case AssassinSkin.MASTERY:
                    return AssassinPlr.ASSASSIN_PREFIX + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME";
                case AssassinSkin.GRANDMASTERY:
                    return AssassinPlr.ASSASSIN_PREFIX + "GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME";
            }
        }
        #endregion
    }
}