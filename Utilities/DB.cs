using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MagicTime.Utilities
{
    internal static class DB
    {
        private static Dictionary<string, string> repo;

        public static T GetBP<T>(string id) where T : BlueprintScriptableObject
        {
            if (repo == null) { BuildRepo(); }
            return ResourcesLibrary.TryGetBlueprint<T>(BlueprintGuid.Parse(repo[id]));
        }

        public static BlueprintAbility GetAbility(string id)
        {
            return GetBP<BlueprintAbility>(id);
        }

        public static BlueprintAbilityResource GetAbilityResource(string id)
        {
            return GetBP<BlueprintAbilityResource>(id);
        }

        public static BlueprintArchetype GetArchetype(string id)
        {
            return GetBP<BlueprintArchetype>(id);
        }

        public static BlueprintCharacterClass GetClass(string id)
        {
            return GetBP<BlueprintCharacterClass>(id);
        }

        public static BlueprintFeature GetFeature(string id)
        {
            return GetBP<BlueprintFeature>(id);
        }

        public static BlueprintProgression GetProgression(string id)
        {
            return GetBP<BlueprintProgression>(id);
        }

        public static BlueprintFeatureSelection GetSelection(string id)
        {
            return GetBP<BlueprintFeatureSelection>(id);
        }

        public static BlueprintBuff GetBuff(string id)
        {
            return GetBP<BlueprintBuff>(id);
        }

        public static BlueprintSpellList GetSpellList(string id)
        {
            return GetBP<BlueprintSpellList>(id);
        }

        public static BlueprintSpellbook GetSpellbook(string id)
        {
            return GetBP<BlueprintSpellbook>(id);
        }

        private static void BuildRepo()
        {
            var serializer = new JsonSerializer();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MagicTime.Utilities.BlueprintDatabase.json"))
            using (StreamReader stream_reader = new StreamReader(stream))
            using (JsonTextReader reader = new JsonTextReader(stream_reader))
            {
                repo = serializer.Deserialize<Dictionary<string, string>>(reader);
            }
        }
    }
}