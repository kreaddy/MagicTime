﻿using Kingmaker.Blueprints;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityModManagerNet;

namespace MagicTime
{
    internal static class Resources
    {
        private static readonly Dictionary<BlueprintGuid, SimpleBlueprint> new_bps = new Dictionary<BlueprintGuid, SimpleBlueprint>();
        private static JObject data;

        public static void Initializer()
        {
            var file = File.OpenText(UnityModManager.modsPath + @"/MagicTime/Guids.json");
            data = JObject.Parse(file.ReadToEnd());
            file.Close();
            file.Dispose();
        }

        public static BlueprintGuid AddAsset(string asset_name, SimpleBlueprint bp)
        {
            var result = new BlueprintGuid(Guid.Parse((string)data.SelectToken(asset_name)));
            if (ResourcesLibrary.BlueprintsCache.m_LoadedBlueprints.ContainsKey(result))
            {
                Main.Log("Duplicate Guid: " + result.ToString() + "-" + asset_name);
                Main.Log("Original Guid: " + ResourcesLibrary.BlueprintsCache.m_LoadedBlueprints[result].Blueprint.name);
                result = new BlueprintGuid(Guid.NewGuid());
                Main.Log("Autogenerated Guid: " + result.ToString());
            }
            new_bps[result] = bp;
            ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(result, bp);
            bp.OnEnable();
            return result;
        }

        public static void Cleanup()
        {
            data = null;
        }

        public static void UpdateDatabase(string guid, string name)
        {
            var text = File.ReadAllText(UnityModManager.modsPath + @"/MagicTime/Guids.json");
            text = text.Replace('}', ',');
            text = text.Insert(text.Length, "\"" + name + "\": \"" + guid + "\"\n}");
            File.WriteAllText(UnityModManager.modsPath + @"/MagicTime/Guids.json", text);
            Cleanup();
            Initializer();
        }
    }
}