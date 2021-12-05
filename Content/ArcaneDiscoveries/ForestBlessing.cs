using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using MagicTime.Utilities;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class ForestBlessing
    {
        private static BlueprintFeature fblessing_feature;

        public static void Create()
        {
            fblessing_feature = Helpers.CreateFeature(
                "ADForestBlessing",
                "Forest's Blessing",
                "You cast any spells that appear on both the wizard and druid spell lists at +1 caster level and with +1 to the save DC.",
                "ad_forest_blessing");
            fblessing_feature.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 5);
            fblessing_feature.CreateGenericComponent<Mechanics.ForestBlessingLogic>();
            Main.AddNewDiscovery(fblessing_feature);
        }
    }
}

namespace MagicTime.ArcaneDiscoveries.Mechanics
{
    [TypeId("d0dd852f-aad0-403f-a211-0b76c092cc0d")]
    [AllowedOn(typeof(BlueprintFeature), false)]
    public class ForestBlessingLogic : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAbilityParams>,
        IRulebookHandler<RuleCalculateAbilityParams>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
        {
            if (Owner == null || evt.AbilityData == null) { return; }
            if (evt.AbilityData.IsInSpellList(DB.GetSpellList("Wizard Spells")) && evt.AbilityData.IsInSpellList(DB.GetSpellList("Druid Spells")))
            {
                evt.AddBonusCasterLevel(1);
                evt.AddBonusDC(1);
            }
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt)
        { }
    }
}