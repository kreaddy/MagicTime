using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints;

namespace MagicTime
{
    static class MainPatcher
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

                var no_hb = Main.SettingsContainer.groups["disable_homebrew"].enabled;

                if (Main.SettingsContainer.groups["a_discoveries"].enabled)
                {
                    ArcaneDiscoveries.Main.Create();
                    if (DiscoveryON("alch_affinity")) { ArcaneDiscoveries.AlchemicalAffinity.Create(); }
                    if (DiscoveryON("forest_blessing")) { ArcaneDiscoveries.ForestBlessing.Create(); }
                    if (DiscoveryON("suck_it_bft")) { ArcaneDiscoveries.Idealize.Create(); }
                    if (DiscoveryON("muscle_wizard")) { ArcaneDiscoveries.KnowledgeIsPower.Create(); }
                    if (DiscoveryON("opp_research")) { ArcaneDiscoveries.OppositionResearch.Create(); }
                    if (DiscoveryON("staff_wand")) { ArcaneDiscoveries.StaffLikeWand.Create(); }
                    if (DiscoveryON("metamagic_insight") && !no_hb) { ArcaneDiscoveries.MetamagicInsight.Create(); }
                }

                if (Main.SettingsContainer.groups["feats"].enabled)
                {
                    if (FeatON("shrewd_tactician")) { Feats.ShrewdTactician.Create(); }
                    if (FeatON("acadamae")) { Feats.AcadamaeGraduate.Create(); }
                    if (FeatON("meta_intensified")) { Feats.MetamagicIntensfied.Create(); }
                    if (FeatON("meta_dazing")) { Feats.MetamagicDazing.Create(); }
                }

                if (Main.SettingsContainer.groups["magus_arcana"].enabled)
                {
                    if (ArcanaON("law_circle")) { MagusArcana.CircleOfOrder.Create(); }
                }

                if (Main.SettingsContainer.groups["mythic"].enabled)
                {
                    if (MythicON("warrior_priest")) { Mythic.WarriorPriest.Create(); }
                    if (MythicON("mythic_poison") && !no_hb && !WorldcrawlLoaded()) { Mythic.MythicPoisons.Create(); }
                    if (MythicON("material_freedom") && !no_hb) { Mythic.MaterialFreedom.Create(); }
                }

                if (Main.SettingsContainer.groups["archetypes"].enabled)
                {
                    if (ArchetypeON("flagellant") && !no_hb) { Archetypes.Flagellant.Create(); }
                    if (ArchetypeON("warsighted")) { Archetypes.Warsighted.Create(); }
                    if (ArchetypeON("blood_arcanist")) { Archetypes.BloodArcanist.Create(); }
                    if (ArchetypeON("pact_wizard")) { Archetypes.PactWizard.Create(); }
                }

                Resources.Cleanup();
            }
        }

        private static bool ArchetypeON(string id)
        {
            return Main.SettingsContainer.groups["archetypes"].settings[id].enabled;
        }

        private static bool DiscoveryON(string id)
        {
            return Main.SettingsContainer.groups["a_discoveries"].settings[id].enabled;
        }

        private static bool FeatON(string id)
        {
            return Main.SettingsContainer.groups["feats"].settings[id].enabled;
        }

        private static bool ArcanaON(string id)
        {
            return Main.SettingsContainer.groups["magus_arcana"].settings[id].enabled;
        }

        private static bool MythicON(string id)
        {
            return Main.SettingsContainer.groups["mythic"].settings[id].enabled;
        }

		private static bool WorldcrawlLoaded()
		{
            var ar = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>("3ae2ff4b51ad5bde3436ffac822611c1");
			if (ar == null)
            {
                return false;
            }
            Main.Log("Mythic Poisons disabled because Worldcrawl is present.");
            return true;
		}
    }
}
