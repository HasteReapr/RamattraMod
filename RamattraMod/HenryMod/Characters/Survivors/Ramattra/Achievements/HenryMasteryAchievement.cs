using RoR2;
using RamattraMod.Modules.Achievements;

namespace RamattraMod.Survivors.Ramattra.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class RamattraMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = RamattraSurvivor.RAMMY_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = RamattraSurvivor.RAMMY_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => RamattraSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}