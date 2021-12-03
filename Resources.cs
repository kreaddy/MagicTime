using JetBrains.Annotations;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityModManagerNet;
using Kingmaker.Blueprints;

namespace MagicTime
{
    static class Resources
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
                Main.Log("Duplicate Guid: " + result.ToString() + "-" + asset_name + ". Autogenerating...");
                result = new BlueprintGuid(Guid.NewGuid());
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
    }
}