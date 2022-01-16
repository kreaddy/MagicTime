using HarmonyLib;
using ModKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace MagicTime
{
    internal static class Main
    {
        public static bool Enabled;

        public static UnityModManager.ModEntry Mod;

        public static Settings SettingsContainer = new Settings();

        // Annoying workarounds for static constructors messing up harmony.
        public static bool static_constructor_uiutilitytexts_safe = false;

        public static void Log(string msg)
        {
            Mod.Logger.Log(msg);
        }

        public static void OnSaveGUI(UnityModManager.ModEntry obj)
        {
            SettingsContainer.Save();
        }

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Mod = modEntry;
            SettingsContainer.Load();
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            var harmony = new Harmony(modEntry.Info.Id);
            Starion.BPExtender.EXData.Initialize();
            Resources.Initializer();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry obj)
        {
            UI.AutoWidth();
            using (UI.VerticalScope())
            {
                UI.Label("You need to restart the game for changes to take effect!".yellow().bold().size(20));
                UI.Space(15);
                for (int i = 0; i < SettingsContainer.groups.Count; i++)
                {
                    var key = SettingsContainer.groups.Keys.ElementAt(i);
                    var group = SettingsContainer.groups[key];
                    UI.Div(0, 30);
                    if (i == 0)
                    {
                        UI.Toggle(group.name.pink().bold().size(15), ref group.enabled);
                        UI.Label(group.description);
                    }
                    if (i > 0 && group.settings != null)
                    {
                        UI.HStack(null, 0, () =>
                        {
                            UI.Toggle(group.name.pink().bold().size(15), ref group.enabled);
                        });
                        foreach (var setting in group.settings.Values)
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
                        }                            
                    }
                }
            }
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        public class Setting
        {
            public string description;
            public bool enabled;
            public bool homebrew;
            public string id;
            public string name;
            public Setting(string g_id, string g_name, bool g_enabled, string desc, bool bad_bubble = false)
            {
                id = g_id;
                name = g_name;
                enabled = g_enabled;
                description = desc;
                homebrew = bad_bubble;
            }
        }

        public class SettingGroup
        {
            public string description;
            public bool enabled;
            public string id;
            public string name;
            public Setting[] content;
            [JsonIgnore]
            public Dictionary<string, Setting> settings = new Dictionary<string, Setting>();
            public SettingGroup(string g_id, string g_name, bool g_enabled, string desc = null, JArray g_content = null)
            {
                id = g_id;
                name = g_name;
                enabled = g_enabled;
                description = desc;
                if (g_content != null)
                {
                    foreach (var item in g_content)
                    {
                        var i_id = item["id"].ToString();
                        settings.Add(i_id, new Setting(i_id, item["name"].ToString(), (bool)item["enabled"],
                            item["description"].ToString(), (bool)item["homebrew"]));
                    }
                }
            }
            public void PrepareSerialization()
            {
                content = settings.Values.ToArray();
            }
        }
        public class Settings : UnityModManager.ModSettings
        {
            public Dictionary<string, SettingGroup> groups = new Dictionary<string, SettingGroup>();
            public string mod_path;
            private JArray raw_data;
            public void Load()
            {
                mod_path = Path.Combine(Mod.Path, "Settings.json");
                var serialize_settings = new JsonSerializerSettings()
                {
                    CheckAdditionalContent = true,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented
                };
                var serializer = JsonSerializer.Create(serialize_settings);

                CreateOrUpdateSettingsFile(serializer);
                DeserializeSettings(serializer);
                CreateSettingGroups();

                raw_data.Clear();
            }

            public void Save()
            {
                var serialize_settings = new JsonSerializerSettings()
                {
                    CheckAdditionalContent = true,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented

                };
                var serializer = JsonSerializer.Create(serialize_settings);
                var groups_to_serialize = new List<SettingGroup>();
                foreach (var group in groups)
                {
                    group.Value.PrepareSerialization();
                    groups_to_serialize.Add(group.Value);
                }
                using (StreamWriter file = new StreamWriter(mod_path))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    serializer.Serialize(writer, groups_to_serialize.ToArray());
                }
            }
            private void CreateOrUpdateSettingsFile(JsonSerializer serializer)
            {
                if (!File.Exists(mod_path))
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MagicTime.DefaultSettings.json"))
                    using (FileStream file = File.Create(mod_path))
                    {
                        stream.CopyTo(file);
                    }
                }
                else
                {
                    JArray default_settings;
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MagicTime.DefaultSettings.json"))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        default_settings = JArray.Parse(reader.ReadToEnd());
                    }
                    using (StreamReader reader = File.OpenText(mod_path))
                    {
                        var user_settings = JArray.Parse(reader.ReadToEnd());
                        default_settings.Merge(user_settings, new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Merge
                        });
                    }
                    using (StreamWriter file = new StreamWriter(mod_path))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        serializer.Serialize(writer, default_settings);
                    }
                }
            }

            private void CreateSettingGroups()
            {
                foreach (var item in raw_data)
                {
                    var id = item["id"].ToString();
                    groups.Add(id, new SettingGroup(id, item["name"].ToString(), (bool)item["enabled"],
                        item["description"].ToString(), item["content"].ToObject<JArray>()));
                }
            }

            private void DeserializeSettings(JsonSerializer serializer)
            {
                using (StreamReader file = File.OpenText(mod_path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    raw_data = serializer.Deserialize<JArray>(reader);
                }
            }
        }
    }
}