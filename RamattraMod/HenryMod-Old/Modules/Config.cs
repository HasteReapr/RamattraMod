using BepInEx.Configuration;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace RamattraMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteJuggle { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteCss { get; private set; }
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteSit { get; private set; }

        public static ConfigEntry<bool> BackstabInsta { get; private set; }
        public static ConfigEntry<float> BackstabChance { get; private set; }
        public static ConfigEntry<float> RechargeChance { get; private set; }

        public static void ReadConfig()
        {
            KeybindEmoteJuggle = RammyPlugin.instance.Config.Bind("Keybinds", "Emote - Juggle", new KeyboardShortcut(KeyCode.Alpha1), "Button to play this emote.");
            KeybindEmoteCss = RammyPlugin.instance.Config.Bind("Keybinds", "Emote - Pose", new KeyboardShortcut(KeyCode.Alpha2), "Button to play this emote.");
            KeybindEmoteSit = RammyPlugin.instance.Config.Bind("Keybinds", "Emote - Sit", new KeyboardShortcut(KeyCode.Alpha3), "Button to play this emote.");
            BackstabInsta = RammyPlugin.instance.Config.Bind("Skills", "Spinal Tap Always Instakill", false, "Toggles whether or not Spinal Tap instantly kills bosses. If false will have a percent chance to instakill, which can be affected by clover.");
            BackstabChance = RammyPlugin.instance.Config.Bind("Skills", "Spinal Tap Instakill Chance", 2f, "The percent chance for Spinal Tap to instakill bosses.");
            BackstabChance = RammyPlugin.instance.Config.Bind("Skills", "Spinal Tap Recharge Chance", 10f, "The percent chance for Spinal Tap to instantly recharge on a succesful backstab..");
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return RammyPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}