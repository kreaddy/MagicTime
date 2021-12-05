using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic;
using MagicTime.Utilities;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class OppositionResearch
    {
        private static BlueprintFeature opp_abjuration;
        private static BlueprintFeature opp_divination;
        private static BlueprintFeature opp_conjuration;
        private static BlueprintFeature opp_enchantment;
        private static BlueprintFeature opp_evocation;
        private static BlueprintFeature opp_illusion;
        private static BlueprintFeature opp_necromancy;
        private static BlueprintFeature opp_transmutation;

        private static BlueprintFeatureSelection opp_research_feature;

        public static void Create()
        {
            var logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Abjuration };
            opp_abjuration = Helpers.CreateFeature(
                "ORAbjuration",
                "Opposition Research — Abjuration",
                "You no longer need to expend two slots in order to prepare a spell from the Abjuration school.",
                null, DB.GetFeature("Opposition School Abjuration").Icon);
            opp_abjuration.CreateComponents(logic);
            opp_abjuration.CreateFeatureRestriction(DB.GetFeature("Opposition School Abjuration"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Divination };
            opp_divination = Helpers.CreateFeature(
                "ORDivination",
                "Opposition Research — Divination",
                "You no longer need to expend two slots in order to prepare a spell from the Divination school.",
                null, DB.GetFeature("Opposition School Divination").Icon);
            opp_divination.CreateComponents(logic);
            opp_divination.CreateFeatureRestriction(DB.GetFeature("Opposition School Divination"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Conjuration };
            opp_conjuration = Helpers.CreateFeature(
                "ORConjuration",
                "Opposition Research — Conjuration",
                "You no longer need to expend two slots in order to prepare a spell from the Conjuration school.",
                null, DB.GetFeature("Opposition School Conjuration").Icon);
            opp_conjuration.CreateComponents(logic);
            opp_conjuration.CreateFeatureRestriction(DB.GetFeature("Opposition School Conjuration"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Enchantment };
            opp_enchantment = Helpers.CreateFeature(
                "OREnchantment",
                "Opposition Research — Enchantment",
                "You no longer need to expend two slots in order to prepare a spell from the Enchantment school.",
                null, DB.GetFeature("Opposition School Enchantment").Icon);
            opp_enchantment.CreateComponents(logic);
            opp_enchantment.CreateFeatureRestriction(DB.GetFeature("Opposition School Enchantment"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Evocation };
            opp_evocation = Helpers.CreateFeature(
                "OREvocation",
                "Opposition Research — Evocation",
                "You no longer need to expend two slots in order to prepare a spell from the Illusion school.",
                null, DB.GetFeature("Opposition School Evocation").Icon);
            opp_evocation.CreateComponents(logic);
            opp_evocation.CreateFeatureRestriction(DB.GetFeature("Opposition School Evocation"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Illusion };
            opp_illusion = Helpers.CreateFeature(
                "ORIllusion",
                "Opposition Research — Illusion",
                "You no longer need to expend two slots in order to prepare a spell from the Illusion school.",
                null, DB.GetFeature("Opposition School Illusion").Icon);
            opp_illusion.CreateComponents(logic);
            opp_illusion.CreateFeatureRestriction(DB.GetFeature("Opposition School Illusion"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Necromancy };
            opp_necromancy = Helpers.CreateFeature(
                "ORNecromancy",
                "Opposition Research — Necromancy",
                "You no longer need to expend two slots in order to prepare a spell from the Necromancy school.",
                null, DB.GetFeature("Opposition School Necromancy").Icon);
            opp_necromancy.CreateComponents(logic);
            opp_necromancy.CreateFeatureRestriction(DB.GetFeature("Opposition School Necromancy"));

            logic = new Mechanics.ClearOppositionSchool() { school = SpellSchool.Transmutation };
            opp_transmutation = Helpers.CreateFeature(
                "ORTransmutation",
                "Opposition Research — Transmutation",
                "You no longer need to expend two slots in order to prepare a spell from the Transmutation school.",
                null, DB.GetFeature("Opposition School Transmutation").Icon);
            opp_transmutation.CreateComponents(logic);
            opp_transmutation.CreateFeatureRestriction(DB.GetFeature("Opposition School Transmutation"));

            opp_research_feature = Helpers.CreateFeatureSelection(
                "ORSchoolSelection",
                "Opposition Research",
                "Select one Wizard opposition school; preparing spells of this school now only requires one spell slot of the appropriate level " +
                "instead of two.");
            opp_research_feature.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 9);
            opp_research_feature.CreateArchetypeBan(DB.GetArchetype("Sin Mage"));
            opp_research_feature.m_AllFeatures = new BlueprintFeatureReference[]
            {
                opp_abjuration.ToReference<BlueprintFeatureReference>(),
                opp_divination.ToReference<BlueprintFeatureReference>(),
                opp_conjuration.ToReference<BlueprintFeatureReference>(),
                opp_enchantment.ToReference<BlueprintFeatureReference>(),
                opp_evocation.ToReference<BlueprintFeatureReference>(),
                opp_illusion.ToReference<BlueprintFeatureReference>(),
                opp_necromancy.ToReference<BlueprintFeatureReference>(),
                opp_transmutation.ToReference<BlueprintFeatureReference>()
            };

            Main.AddNewDiscovery(opp_research_feature);
        }
    }
}

namespace MagicTime.ArcaneDiscoveries.Mechanics
{
    public class ClearOppositionSchool : UnitFactComponentDelegate
    {
        public SpellSchool school;

        public override void OnActivate()
        {
            foreach (Spellbook spellbook in Owner.Spellbooks)
            {
                foreach (SpellSchool item in spellbook.OppositionSchools)
                {
                    if (item == school)
                    {
                        spellbook.ExOppositionSchools.Add(item);
                        spellbook.OppositionSchools.Remove(item);
                    }
                }
            }
            base.OnActivate();
        }

        public override void OnDeactivate()
        {
            foreach (Spellbook spellbook in Owner.Spellbooks)
            {
                foreach (SpellSchool item in spellbook.ExOppositionSchools)
                {
                    if (item == school)
                    {
                        spellbook.ExOppositionSchools.Remove(item);
                        spellbook.OppositionSchools.Add(item);
                    }
                }
            }
            base.OnDeactivate();
        }
    }
}