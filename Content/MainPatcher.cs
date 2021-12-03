using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System.Collections.Generic;

namespace MagicTime
{
    class MainPatcher
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        public static class BlueprintPatcher
        {
            static bool Initialized;

            [HarmonyPriority(Priority.LowerThanNormal)]
            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                if (Main.SettingsContainer.archetypes.Contains("flagellant")) { Archetypes.Flagellant.Create(); }
                if (Main.SettingsContainer.discoveries.Count > 0) { ArcaneDiscoveries.Main.Create(); }
                if (Main.SettingsContainer.discoveries.Contains("alch_affinity")) { ArcaneDiscoveries.AlchemicalAffinity.Create(); }
                if (Main.SettingsContainer.discoveries.Contains("forest_blessing")) { ArcaneDiscoveries.ForestBlessing.Create(); }
                if (Main.SettingsContainer.discoveries.Contains("suck_it_bft")) { ArcaneDiscoveries.Idealize.Create(); }
                if (Main.SettingsContainer.discoveries.Contains("muscle_wizard")) { ArcaneDiscoveries.KnowledgeIsPower.Create(); }
                if (Main.SettingsContainer.discoveries.Contains("opp_research")) { ArcaneDiscoveries.OppositionResearch.Create(); }
                if (Main.SettingsContainer.discoveries.Contains("staff_wand")) { ArcaneDiscoveries.StaffLikeWand.Create(); }
                if (Main.SettingsContainer.feats.Contains("shrewd_tactician")) { Feats.ShrewdTactician.Create(); }
                if (Main.SettingsContainer.arcanas.Contains("law_circle")) { MagusArcana.CircleOfOrder.Create(); }
                if (Main.SettingsContainer.mythics.Contains("warrior_priest")) { Mythic.WarriorPriest.Create(); }
                if (Main.SettingsContainer.mythics.Contains("mythic_poison")) { Mythic.MythicPoisons.Create(); }

                Resources.Cleanup();
            }
        }
    }
}
