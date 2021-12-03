using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityModManagerNet;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ModKit;

namespace MagicTime
{
    static class Main
    {
        public class SettingGroup
        {
            public string id;
            public string name;
            public bool enabled;
            public string description;
            public List<Setting> content;

            public SettingGroup(string g_id, string g_name, bool g_enabled, string desc = null, JToken data = null)
            {
                id = g_id;
                name = g_name;
                enabled = g_enabled;
                description = desc;
                if (data != null)
                {
                    content = new List<Setting>();
                    foreach (var element in data.ToArray())
                    {
                        content.Add(new Setting(element["id"].ToString(), element["name"].ToString(), (bool)element["enabled"],
                            element["description"].ToString(), (bool)element["homebrew"]));
                    }
                }
            }
        }

        public class Setting
        {
            public string id;
            public string name;
            public bool enabled;
            public string description;
            public bool homebrew;

            public Setting(string g_id, string g_name, bool g_enabled, string desc, bool bad_bubble = false)
            {
                id = g_id;
                name = g_name;
                enabled = g_enabled;
                description = desc;
                homebrew = bad_bubble;
            }
        }

        public class Settings : UnityModManager.ModSettings
        {
            public JObject[] data;

            public SettingGroup[] groups;

            public bool no_homebrew = false;
            public List<string> archetypes = new List<string>();
            public List<string> discoveries = new List<string>();
            public List<string> feats = new List<string>();
            public List<string> arcanas = new List<string>();
            public List<string> mythics = new List<string>();

            public void Save()
            {
                if (File.Exists(Path.Combine(Mod.Path, "Settings.json.backup")))
                {
                    File.Delete(Path.Combine(Mod.Path, "Settings.json.backup"));
                }
                File.Copy(Path.Combine(Mod.Path, "Settings.json"), Path.Combine(Mod.Path, "Settings.json.backup"));
                var serialize_settings = new JsonSerializerSettings()
                {
                    CheckAdditionalContent = true,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented
                };
                var serializer = JsonSerializer.Create(serialize_settings);
                using (StreamWriter file = new StreamWriter(Path.Combine(Mod.Path, "Settings.json")))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    serializer.Serialize(writer, groups);
                }
            }

            public void Load()
            {
                var serialize_settings = new JsonSerializerSettings()
                {
                    CheckAdditionalContent = true,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented
                };
                var serializer = JsonSerializer.Create(serialize_settings);
                using (StreamReader file = File.OpenText(Path.Combine(Mod.Path, "Settings.json")))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    data = serializer.Deserialize<JObject[]>(reader);
                }
                var list = new List<SettingGroup>();
                for (int i = 0; i < data.Length; i++)
                {
                    list.Add(new SettingGroup(data[i]["id"].ToString(), data[i]["name"].ToString(), (bool)data[i]["enabled"],
                        data[i]["description"].ToString(), data[i]["content"]));
                };
                groups = list.ToArray();
                PopulateContainer();
            }

            private void PopulateContainer()
            {
                foreach (var group in groups)
                {
                    switch (group.id)
                    {
                        case "disable_homebrew":
                            no_homebrew = group.enabled;
                            break;
                        case "archetypes":
                            if (group.enabled)
                            {
                                foreach (var setting in group.content)
                                {
                                    if (setting.enabled && !(setting.homebrew && no_homebrew)) { archetypes.Add(setting.id); }
                                }
                            }
                            break;
                        case "a_discoveries":
                            if (group.enabled)
                            {
                                foreach (var setting in group.content)
                                {
                                    if (setting.enabled && !(setting.homebrew && no_homebrew)) { discoveries.Add(setting.id); }
                                }
                            }
                            break;
                        case "feats":
                            if (group.enabled)
                            {
                                foreach (var setting in group.content)
                                {
                                    if (setting.enabled && !(setting.homebrew && no_homebrew)) { feats.Add(setting.id); }
                                }
                            }
                            break;
                        case "magus_arcana":
                            if (group.enabled)
                            {
                                foreach (var setting in group.content)
                                {
                                    if (setting.enabled && !(setting.homebrew && no_homebrew)) { arcanas.Add(setting.id); }
                                }
                            }
                            break;
                        case "mythic":
                            if (group.enabled)
                            {
                                foreach (var setting in group.content)
                                {
                                    if (setting.enabled && !(setting.homebrew && no_homebrew)) { mythics.Add(setting.id); }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public static Settings SettingsContainer = new Settings();
        public static bool Enabled;
        public static UnityModManager.ModEntry Mod;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Mod = modEntry;
            SettingsContainer.Load();
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            var harmony = new Harmony(modEntry.Info.Id);
            Resources.Initializer();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry obj)
        {
            var data = SettingsContainer.data;
            UI.AutoWidth();
            UI.Div(0, 15);
            using (UI.VerticalScope())
            {
                UI.Label("You need to restart the game for changes to take effect!".yellow().bold().size(20));
                UI.Space(15);
                for (int i = 0; i < SettingsContainer.groups.Length; i++)
                {
                    var group = SettingsContainer.groups[i];
                    UI.Toggle(group.name.pink().bold().size(15), ref group.enabled);
                    if (group.description != null)
                    {
                        UI.Label(group.description);
                        UI.Space(10);
                    }
                    if (group.content != null)
                    {
                        foreach (var setting in group.content)
                        {
                            if (setting.homebrew)
                            {
                                UI.Toggle(setting.name.bold().green(), ref setting.enabled);
                            }
                            else
                            {
                                UI.Toggle(setting.name.bold(), ref setting.enabled);
                            }
                            UI.Label(setting.description);
                            UI.Space(10);
                        }
                    }
                }
            }
        }

        public static void OnSaveGUI(UnityModManager.ModEntry obj)
        {
            SettingsContainer.Save();
        }

        public static void Log(string msg)
        {
            Mod.Logger.Log(msg);
        }
    }
}