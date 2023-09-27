using RoR2;
using System;
using UnityEngine;

namespace AssassinMod.Modules.Achievements
{
    internal class MasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_MASTERY";
        //the name of the sprite in your bundle
        public override string AchievementDescToken => AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override string AchievementSpriteName => "texMasterySkin";
        //the token of your character's unlock achievement if you have one
        public override string PrerequisiteUnlockableIdentifier => AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_MASTERYUNLOCKABLE_UNLOCKABLE_NAME";

        public override string RequiredCharacterBody => "AssassinSurvivorBody";
        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }

    internal class GrandMasteryAchievement : BaseMasteryUnlockable
    {
        public override string AchievementTokenPrefix => AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_GRAND_MASTERY";
        //the name of the sprite in your bundle
        public override string AchievementDescToken => AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_GRAND_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override string AchievementSpriteName => "texGrandMasterySkin";
        //the token of your character's unlock achievement if you have one
        public override string PrerequisiteUnlockableIdentifier => AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_GRAND_MASTERYUNLOCKABLE_UNLOCKABLE_NAME";

        public override string RequiredCharacterBody => "AssassinSurvivorBody";
        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3.5f;
    }
}