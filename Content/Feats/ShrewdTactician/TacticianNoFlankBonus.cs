using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using MagicTime.Utilities;

namespace MagicTime.Feats.Mechanics
{
    [AllowedOn(typeof(BlueprintFeature), false)]
    [TypeId("2076f1b6-71b8-499d-b80a-abf3ff6d3334")]
    public class TacticianNoFlankBonus : UnitFactComponentDelegate, ITargetRulebookHandler<RuleCalculateAttackBonus>,
        IRulebookHandler<RuleCalculateAttackBonus>, ISubscriber, ITargetRulebookSubscriber
    {
        public void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
        {
            if (!Owner.CombatState.IsFlanked || !evt.Weapon.Blueprint.IsMelee)
            {
                return;
            }
            bool flag = evt.Initiator.State.Features.SoloTactics;
            int bonus = -2;
            {
                foreach (UnitEntityData unitEntityData in Owner.CombatState.EngagedBy)
                {
                    flag = (unitEntityData.Descriptor.HasFact(BlueprintsDatabase.Outflank) && unitEntityData != evt.Initiator);
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (flag) { bonus -= 2; }
            evt.AddModifier(bonus, base.Fact, ModifierDescriptor.UntypedStackable);
        }

        public void OnEventDidTrigger(RuleCalculateAttackBonus evt)
        {
        }
    }
}