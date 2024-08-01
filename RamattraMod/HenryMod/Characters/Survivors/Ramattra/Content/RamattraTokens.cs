using System;
using RamattraMod.Modules;
using RamattraMod.Survivors.Ramattra.Achievements;

namespace RamattraMod.Survivors.Ramattra
{
    public static class RamattraTokens
    {
        public static void Init()
        {
            AddRamattraTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddRamattraTokens()
        {
            #region Ramattra
            string prefix = RamattraSurvivor.RAMMY_PREFIX;

            string desc = "Acolyte is a monk determined to find peace, using any means possible." + Environment.NewLine + Environment.NewLine
             + "< ! > Void Staff rapidly shoots slow moving projectiles, so make sure to lead your shots." + Environment.NewLine + Environment.NewLine
             + "< ! > Restoration can be used to save you and your allies from critical damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Form Shift should be used when close to enemies, as swapping to Nemesis form makes you a melee survivor." + Environment.NewLine + Environment.NewLine
             + "< ! > Mandatory Unity can pull enemies from the sky, making them easy to hit with your Void Staff." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, off to maintain peace elsewhere.";
            string outroFailure = "..and so he vanished, never to know peace again.";

            Language.Add(prefix + "NAME", "Acolyte");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "The Chosen One");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Steadfast Faith");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_STAFF_NAME", "Void Staff");
            Language.Add(prefix + "PRIMARY_STAFF_DESCRIPTION", Tokens.agilePrefix + $" Rapidly shoot from your staff for <style=cIsDamage>{100f * RamattraStaticValues.voidShardDamageCoef}% damage</style>.");
            
            Language.Add(prefix + "PRIMARY_PUNCH_NAME", "Spiritual Punch");
            Language.Add(prefix + "PRIMARY_PUNCH_DESCRIPTION", Tokens.agilePrefix + $" Punch in front of you for <style=cIsDamage>{100f * RamattraStaticValues.pummelDamageCoef}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_RESTORE_NAME", "Restoration");
            Language.Add(prefix + "SECONDARY_RESTORE_DESCRIPTION", Tokens.agilePrefix + $" Protect you and your allies within 20m, giving 40% maximum health & shield as barrier.");
            
            Language.Add(prefix + "SECONDARY_BLOCK_NAME", "Block");
            Language.Add(prefix + "SECONDARY_BLOCK_DESCRIPTION", $"Block in front of you, reducing incoming damage by 40%.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_SHIFT_NAME", "Form Shift");
            Language.Add(prefix + "UTILITY_SHIFT_DESCRIPTION", "Shift between Omnic and Nemesis forms.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_VORTEX_NAME", "Mandatory Unity");
            Language.Add(prefix + "SPECIAL_VORTEX_DESCRIPTION", Tokens.agilePrefix + $" Throw a vortex, pulling enemies to the ground and slowing them, dealing <style=cIsDamage>{100f}%</style> damage per second.");
            
            Language.Add(prefix + "SPECIAL_SWARM_NAME", "Persistent Swarm");
            Language.Add(prefix + "SPECIAL_SWARM_DESCRIPTION", Tokens.agilePrefix + $" Summon a void swarm for 3 seconds, dealing <style=cIsDamage>{RamattraStaticValues.annihilationDamageCoef * 5 * 100f}%</style> damage per second in a 40m radius. Victims in the radius sustain the void, extending the duration for up to 20 seconds.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(RamattraMasteryAchievement.identifier), "Acolyte: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(RamattraMasteryAchievement.identifier), "As Acolyte, beat the game or obliterate on Monsoon.");
            #endregion
            #endregion
        }
    }
}
