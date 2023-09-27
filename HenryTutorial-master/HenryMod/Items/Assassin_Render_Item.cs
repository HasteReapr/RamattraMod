/*using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static AssassinMod.Modules.Assets;
using static R2API.ItemAPI;

namespace AssassinMod.Items
{
    internal class Assassin_Render_Item : ItemBase
    {
        public override string ItemName => "AssasinRender";

        public override string ItemLangTokenName => "ASSASSIN_RENDER";

        public override string ItemPickupDesc => "sample";

        public override string ItemFullDescription => "sample";

        public override string ItemLore => "sample";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => mainAssetBundle.LoadAsset<GameObject>("mdlAssassinAvatar_Render.prefab");

        public override Sprite ItemIcon => mainAssetBundle.LoadAsset<Sprite>("texMainSkin");

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
        }
    }
}
*/
// unused, was for getting the avatar render