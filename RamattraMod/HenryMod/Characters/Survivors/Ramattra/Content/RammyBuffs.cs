using RoR2;
using UnityEngine;

namespace RamattraMod.Survivors.Ramattra
{
    public static class RammyBuffs
    {
        // Debuff applied by Ravenous Vortex.
        public static BuffDef vortexDebuff;
        public static BuffDef blockBuff;

        public static void Init(AssetBundle assetBundle)
        {
            vortexDebuff = Modules.Content.CreateAndAddBuff("Grounded",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                true);

            blockBuff = Modules.Content.CreateAndAddBuff("Ramattra Blocking",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.grey,
                false,
                false);
        }
    }
}
