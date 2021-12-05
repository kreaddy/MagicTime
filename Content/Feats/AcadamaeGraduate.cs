using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using MagicTime.Utilities;

namespace MagicTime.Feats
{
    internal static class AcadamaeGraduate
    {
        public static BlueprintFeature graduate_feature;

        public static void Create()
        {
            graduate_feature = Helpers.CreateFeature("AcadamaeGraduate", "Acadamae Graduate", "Whenever you cast a prepared arcane spell from " +
                "the Conjuration school with the Summoning descriptor that takes a full-round action to cast, reduce the casting time to a " +
                "standard action. Casting a spell in this way is taxing and requires a Fortitude save (DC 15 + spell level) to resist becoming " +
                "fatigued for 1 minute.", "f_graduate", null, false);
            graduate_feature.CreateFeatureTags(Kingmaker.Blueprints.Classes.Selection.FeatureTag.Magic);
            graduate_feature.CreateFeatureRestrictionInv(DB.GetFeature("Opposition School Conjuration"));
            graduate_feature.CreateGenericComponent<Mechanics.AcadamaeGraduateFatigue>();
            Helpers.AddNewWizardFeat(graduate_feature);
        }
    }

    [HarmonyPatch(typeof(AbilityData), "get_RequireFullRoundAction")]
    internal class AbilityData_get_RequireFullRoundAction_AcadamaePatch
    {
        private static bool Postfix(bool result, AbilityData __instance)
        {
            if (result == true && __instance.Blueprint.IsSpell && !__instance.IsSpontaneous && __instance.Caster != null
                && __instance.SpellSource == SpellSource.Arcane && __instance.Blueprint.School == SpellSchool.Conjuration &&
                __instance.Blueprint.SpellDescriptor.HasFlag(SpellDescriptor.Summoning) &&
                __instance.Caster.GetFeature(AcadamaeGraduate.graduate_feature) != null)
            {
                return false;
            }
            return result;
        }
    }
}

namespace MagicTime.Feats.Mechanics
{
    [TypeId("fe0cb74b-36b9-498b-8317-b68685910df0")]
    [AllowedOn(typeof(BlueprintFeature), false)]
    public class AcadamaeGraduateFatigue : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCastSpell>, IRulebookHandler<RuleCastSpell>,
        ISubscriber, IInitiatorRulebookSubscriber
    {
        public void OnEventDidTrigger(RuleCastSpell evt)
        {
            if (evt.Spell != null && !evt.Spell.IsSpontaneous && evt.Success && evt.Context.SpellSchool == SpellSchool.Conjuration &&
                evt.Context.SpellDescriptor.HasFlag(SpellDescriptor.Summoning))
            {
                var result = GameHelper.CheckSkillResult(evt.Initiator, StatType.SaveFortitude, 15 + evt.Spell.SpellLevel);
                if (!result)
                {
                    evt.Initiator.AddBuff(DB.GetBuff("Fatigued Buff"), evt.Initiator, System.TimeSpan.FromMinutes(1));
                }
            }
        }

        public void OnEventAboutToTrigger(RuleCastSpell evt)
        {
        }
    }
}