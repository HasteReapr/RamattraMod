using RamattraMod.Survivors.Ramattra.Achievements;
using RoR2;
using UnityEngine;

namespace RamattraMod.Survivors.Ramattra
{
    public static class RamattraUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                RamattraMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(RamattraMasteryAchievement.identifier),
                RamattraSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
