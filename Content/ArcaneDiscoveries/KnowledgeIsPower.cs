using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using MagicTime.Utilities;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class KnowledgeIsPower
    {
        private static BlueprintFeature int_to_cm_feature;

        public static void Create()
        {
            int_to_cm_feature = Helpers.CreateFeature(
                "KnowledgeIsPower",
                "Knowledge Is Power",
                "Your understanding of physical forces gives you power over them. You add your Intelligence modifier on combat maneuver checks " +
                "and to your CMD.");
            int_to_cm_feature.CreateGenericComponent<Mechanics.KnowledgeIsPowerLogic>();
            Main.AddNewDiscovery(int_to_cm_feature);
        }
    }
}

namespace MagicTime.ArcaneDiscoveries.Mechanics
{
    [AllowedOn(typeof(BlueprintFeature), false)]
    [TypeId("58b1eb1a-efac-480e-8358-89d9be52da80")]
    public class KnowledgeIsPowerLogic : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateBaseCMD>,
        IRulebookHandler<RuleCalculateBaseCMD>, IInitiatorRulebookHandler<RuleCalculateBaseCMB>, IRulebookHandler<RuleCalculateBaseCMB>,
        ISubscriber, IInitiatorRulebookSubscriber
    {
        public void OnEventAboutToTrigger(RuleCalculateBaseCMD evt)
        {
            if (evt.Initiator == Owner)
            {
                evt.AddModifier(Owner.Stats.Intelligence.Bonus, Fact, Kingmaker.Enums.ModifierDescriptor.UntypedStackable);
            }
        }

        public void OnEventDidTrigger(RuleCalculateBaseCMD evt)
        {
        }

        public void OnEventAboutToTrigger(RuleCalculateBaseCMB evt)
        {
            if (evt.Initiator == Owner)
            {
                evt.AddModifier(Owner.Stats.Intelligence.Bonus, Fact, Kingmaker.Enums.ModifierDescriptor.UntypedStackable);
            }
        }

        public void OnEventDidTrigger(RuleCalculateBaseCMB evt)
        {
        }
    }
}