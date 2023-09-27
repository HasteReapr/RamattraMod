using R2API;
using System;

namespace AssassinMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Assassin
            string prefix = AssassinPlugin.DEVELOPER_PREFIX + "_ASSASSIN_BODY_";

            string desc = "The assassin fights at medium range and uses poison to damage their enemies over time.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Daggers are great for dealing with singular targets." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Poisons are great to deal with crowds of enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Teleporting can be tricky to aim, though it arcs so aim it a bit above where you actually want to land." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > War Cry is great to use when you activate the teleporter" + Environment.NewLine + Environment.NewLine;

            string TheLorax = "Once revered as an alchemical prodigy, the Assassin was destined to leave an indelible mark on history.They displayed an unparalleled affinity for brewing potions that could do anything one could ask for. Under the mentorship of esteemed alchemists, they honed their craft and earned a reputation as the most capable alchemist there was.However over time, they became fueled by greed, something the alchemical society heavily frowned on. This led to them abandoning the society that mentored them.\n\n" +
                              "Their immense greed and a hatred for the alchemical society they grew up in led the Assassin down a dark path they could have never imagined. Consumed by greed and the desire to be famous, they turned their expertise to a darker purpose. They spent months brewing their deadliest potion, a neurotoxin so deadly it drained the life of any who inhaled the fumes. Knowing they couldn’t survive this, they made an anti-venom and infused their cloak with it.\n\n" +
                              "They gained notoriety as a relentless and elusive bounty hunter. Their services were sought by criminals and warlords alike, their poison becoming their signature assassination style, effectively serving as a calling card.They harnessed their brewing skills to make other potions, ones that could move their molecular structure around, and others that could make their body disappear in the blink of an eye.\n\n" +
                              "Iterating on their previous poison, they had come up with something much more virulent.This new poison was able to deliver all of its toxins almost instantly, with the side effect that it would linger in the air.This allowed them to kill entire rooms of people with a single potion.\n\n" +
                              "The Assassin’s notoriety gained the attention of a mysterious figure who came to them offering a bounty they couldn’t refuse. They were tasked with killing the captain of the Safe Travels.Upon landing on Petrichor V they felt a powerful presence, one that they had the desire to kill.";


            string outro = "..and so they left, to claim their highest bounty.";
            string outroFailure = "..and so they vanished, with a not a trace of smoke to be seen.";

            LanguageAPI.Add(prefix + "NAME", "Assassin");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Silent Poisonous Professional");
            LanguageAPI.Add(prefix + "LORE", TheLorax);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Keywords
            string WarcryAdd = "\n<style=cIsUtility>While under the influence of War Cry, </style>";
            LanguageAPI.Add("KEYWORD_DAGGER_WC", "<style=cKeywordName>Inspired</style>" + WarcryAdd + "throw 4 daggers.");
            LanguageAPI.Add("KEYWORD_POISON_WC", "<style=cKeywordName>Inspired</style>" + WarcryAdd + "throw a clustered potion with weaker children.");
            LanguageAPI.Add("KEYWORD_VENOM_WC", "<style=cKeywordName>Inspired</style>" + WarcryAdd + "throw 2 potions in a loose fan");
            LanguageAPI.Add("KEYWORD_TELEPORT_WC", "<style=cKeywordName>Inspired</style>" + WarcryAdd + "throw the teleport potion faster, and explode in a poisonous cloud upon teleporting.");
            LanguageAPI.Add("KEYWORD_ROLL_WC", "<style=cKeywordName>Inspired</style>" + WarcryAdd + "rolling applies the warbanner effect.");
            #endregion

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Exalted");
            LanguageAPI.Add(prefix + "GRAND_MASTERY_SKIN_NAME", "Divine");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Mad God's Rage");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Getting hit below <style=cIsHealth>60% maximum health</style> will increase your attack speed and damage by <style=cIsDamage>7.5% per stack</style>, for <style=cIsUtility>3 seconds.</style>");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_DAGGER_NAME", "Dagger");
            LanguageAPI.Add(prefix + "PRIMARY_DAGGER_DESCRIPTION", Helpers.agilePrefix + $"Throw 2 daggers for <style=cIsDamage>{135f}% damage</style>.");
            LanguageAPI.Add(prefix + "PRIMARY_DAGGER_DESCRIPTION_SCEPTER", Helpers.agilePrefix + $"Throw 3 daggers for <style=cIsDamage>{150f}% damage</style> each. <style=cIsUtility>While under the influence of War Cry, throw 5 daggers.</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_POISON_NAME", "Poison");
            LanguageAPI.Add(prefix + "SECONDARY_POISON_DESCRIPTION", Helpers.agilePrefix + $"Throw a poisonous potion for <style=cIsDamage>{250f}% damage</style> plus an additional <style=cIsDamage>{500f}% damage over time.</style>");
            LanguageAPI.Add(prefix + "SECONDARY_POISON_DESCRIPTION_SCEPTER", Helpers.agilePrefix + $"Throw a clustered poisonous potion for <style=cIsDamage>{250f}% damage</style> plus an additional <style=cIsDamage>{500f}% damage over time, each.</style> \n\n<style=cKeywordName>While under the influence of War Cry, throw a recursively clustered potion, with weaker children.</style>");
            #endregion

            #region AltSecondary
            LanguageAPI.Add(prefix + "SECONDARY_POISON_NAME_ALT", "Virulent Venom");
            LanguageAPI.Add(prefix + "SECONDARY_POISON_DESCRIPTION_ALT", Helpers.agilePrefix + $"Throw a lingering potion for <style=cIsDamage>{750f}% damage.</style> <style=cIsUtility>Toxins linger for 5 seconds.</style> <style=cDeath>Does not inflict poison.</style>");
            LanguageAPI.Add(prefix + "SCEPTER_SECONDARY_POISON_DESCRIPTION_ALT", Helpers.agilePrefix + $"Throw 2 lingering potions in a loose fan for <style=cIsDamage>{750f}% damage.</style> <style=cIsUtility>Toxins linger for 5 seconds.</style> <style=cDeath>Does not inflict poison.</style>");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_CLOAK_NAME", "Teleport");
            LanguageAPI.Add(prefix + "UTILITY_CLOAK_DESCRIPTION", "Throw a potion towards your cursor <style=cIsUtility>teleporting you to where it lands,</style> then throw a potion onto the ground, <style=cIsUtility>exploding into a cloud of smoke, gaining the Cloak Speed buff.</style>");
            #endregion

            #region AltUtility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Cloaking Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll forward, becoming invisible and gaining a small movespeed boost.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_POISON_SPAM_NAME", "War Cry");
            LanguageAPI.Add(prefix + "SPECIAL_POISON_SPAM_DESCRIPTION", $"<style=cIsUtility>Buff yourself, and all of your abilities for 5 seconds.</style>");// <style=cIsUtility>Throw 3 knives, a recursive poison, and more potent smoke bomb</style> during the duration.");
            LanguageAPI.Add(prefix + "SPECIAL_POISON_SPAM_DESCRIPTION_SCEPTER", $"<style=cIsUtility>Buff yourself, and all of your abilities, and stats by 5%, for 5 seconds.</style>");// <style=cIsUtility>Throw 3 knives, a recursive poison, and more potent smoke bomb</style> during the duration.");
            #endregion
            
            #region AltSpecial
            LanguageAPI.Add(prefix + "SPECIAL_BACKSTAB_NAME", "Spinal Tap");
            LanguageAPI.Add(prefix + "SPECIAL_BACKSTAB_DESCRIPTION", $"Stab your enemies for <style=cIsDamage>{750f}% damage. </style><style=cIsUtility>When stabbing a lesser enemy from behind,</style> <style=cIsHealth>instantly kill your target.</style> On stabbing a boss from behind, have a <style=cIsUtility>{Config.BackstabChance.Value}%</style> to <style=cIsHealth>instantly kill your target.</style> If this fails, deal <style=cIsDamage>20% of the targets health.</style>");// <style=cIsUtility>Throw 3 knives, a recursive poison, and more potent smoke bomb</style> during the duration.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Assassin: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Assassin, beat the game or obliterate on Monsoon or higher.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Assassin: Mastery");

            LanguageAPI.Add(prefix + "GRAND_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Assassin: Grand Mastery");
            LanguageAPI.Add(prefix + "GRAND_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Assassin, beat the game or obliterate on Typhoon or higher.");
            LanguageAPI.Add(prefix + "GRAND_MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Assassin: Grand Mastery");
            #endregion
            #endregion
        }
    }
}