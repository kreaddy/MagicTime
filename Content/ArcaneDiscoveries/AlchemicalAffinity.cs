using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using MagicTime.Utilities;
using System.Linq;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class Main
    {
        public static BlueprintFeatureSelection ad_selection;

        public static void Create()
        {
            ad_selection = Helpers.CreateFeatureSelection(
                "ADArcaneDiscoverySelection",
                "Arcane Discovery",
                "A wizard can learn an arcane discovery in place of a regular feat or wizard bonus feat.", null, true, FeatureGroup.WizardFeat);
            ad_selection.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 1);
            ad_selection.CreateFeatureTags(FeatureTag.Magic);
            Helpers.AddNewWizardFeat(ad_selection);
        }

        public static void AddNewDiscovery(BlueprintFeature discovery)
        {
            var list = ad_selection.m_AllFeatures.ToList();
            list.Add(discovery.ToReference<BlueprintFeatureReference>());
            ad_selection.m_AllFeatures = list.ToArray();
        }
    }

    internal static class AlchemicalAffinity
    {
        private static BlueprintFeature aa_feature;

        public static void Create()
        {
            aa_feature = Helpers.CreateFeature(
                "ADAlchemicalAffinity",
                "Alchemical Affinity",
                "Having studied alongside alchemists, you’ve learned to use their methodologies to enhance your spellcraft.\n" +
                "Benefit: Whenever you cast a spell that appears on both the wizard and alchemist spell lists, you treat your caster level as 1 " +
                "higher than normal and the save DC of such spells increases by 1.",
                null, DB.GetAbility("Targeted Bomb Admixture").Icon);
            aa_feature.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 5);
            aa_feature.CreateGenericComponent<Mechanics.AlchemicalAffinityLogic>();
            Main.AddNewDiscovery(aa_feature);
        }
    }
}

namespace MagicTime.ArcaneDiscoveries.Mechanics
{
    [TypeId("82d3fbf6-7401-402b-a1f3-87224efeec4b")]
    [AllowedOn(typeof(BlueprintFeature), false)]
    public class AlchemicalAffinityLogic : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAbilityParams>,
        IRulebookHandler<RuleCalculateAbilityParams>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            if (Owner == null || evt.AbilityData == null) { return; }
            if (evt.AbilityData.IsInSpellList(DB.GetSpellList("Wizard Spells")) &&
                evt.AbilityData.IsInSpellList(DB.GetSpellList("Alchemist Spells")))
            {
                evt.AddBonusCasterLevel(1);
                evt.AddBonusDC(1);
            }
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt)
        { }
    }
}