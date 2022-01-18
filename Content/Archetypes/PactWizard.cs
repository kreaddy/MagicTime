using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using MagicTime.Utilities;
using Starion.BPExtender.AbilityResource;
using Starion.BPExtender.UnitFact;
using System.Collections.Generic;
using System.Linq;

namespace MagicTime.Archetypes
{
    internal static class PactWizard
    {
        private static BlueprintArchetype pact_wizard_archetype;
        public static BlueprintFeatureSelection patron_selection;
        public static BlueprintFeature spontaneous_conversion;
        private static BlueprintFeatureSelection curse_selection;
        public static BlueprintAbilityResource pact_wizard_reroll_resource;
        public static BlueprintBuff double_rolls_buff;
        private static BlueprintFeature double_rolls_feature;
        private static BlueprintFeature lv15feature;
        public static BlueprintFeature auto_success;
        public static string[] wizardSpellbooks = new string[]
        {
            "5a38c9ac8607890409fcb8f6342da6f4", "5785f40e7b1bfc94ea078e7156aa9711", "97cd3941ce333ce46ae09436287ed699",
            "74b87962a97d56c4583979216631eb4c", "05b105ddee654db4fb1547ba48ffa160", "9e4b96d7b02f8c8498964aeee6eaef9b",
            "cbc30bcc7b8adec48a53a6540f5596ae", "58b15cc36ceda8942a7a29aafa755452"
        };

        public static void Create()
        {
            pact_wizard_archetype = Helpers.CreateBlueprint<BlueprintArchetype>("PactWizardArchetype", bp =>
            {
                bp.LocalizedName = Helpers.CreateString("PactWizardArchetype.Name", "Pact Wizard");
                bp.LocalizedDescription = Helpers.CreateString("PactWizardArchetype.Description", "While the art of wizardry is usually a scholar’s " +
                    "pursuit, there are those who seek mastery of arcane power without tedious study and monotonous research. Motivated by foolish " +
                    "ambition, such individuals turn to the greatest enigmas of the cosmos in the hopes of attaining greater power. Though few " +
                    "successfully attract the attention of these forces, those who do receive phenomenal arcane power for their efforts, but become " +
                    "the dutiful playthings and servants of the forces with which they consort.");
            });

            CreatePatronFeature();
            CreateSpontaneousConversion();
            CreateCurseFeature();
            CreateDoubleRolls();
            CreateBonusToRolls();
            CreateAutoSuccess();

            pact_wizard_archetype.RemoveFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, DB.GetFeature("Scribe Scrolls").ToReference<BlueprintFeatureReference>()),
                Helpers.CreateLevelEntry(5, DB.GetSelection("Wizard Feat Selection").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(10, DB.GetSelection("Wizard Feat Selection").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(15, DB.GetSelection("Wizard Feat Selection").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(20, DB.GetSelection("Wizard Feat Selection").ToReference<BlueprintFeatureSelectionReference>())
            };

            pact_wizard_archetype.AddFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, patron_selection, spontaneous_conversion),
                Helpers.CreateLevelEntry(5, curse_selection),
                Helpers.CreateLevelEntry(10, double_rolls_feature),
                Helpers.CreateLevelEntry(15, lv15feature),
                Helpers.CreateLevelEntry(20, auto_success)
            };

            var newUIGroups = new UIGroup[]
            {
                Helpers.CreateUIGroup(curse_selection, double_rolls_feature, lv15feature, auto_success)
            };
            DB.GetClass("Wizard Class").Progression.UIGroups = DB.GetClass("Wizard Class").Progression.UIGroups.Concat(newUIGroups).ToArray();

            DB.GetClass("Wizard Class").AddArchetype(pact_wizard_archetype);
        }

        public static void CreatePatronFeature()
        {
            var patron_list = DB.GetSelection("Witch Patron Selection").m_AllFeatures;
            patron_selection = Helpers.CreateFeatureSelection("PactWizardPatronSelection", "Patron", "At 1st level, a pact wizard must select a " +
                "patron. This functions like the witch class ability of the same name, except the pact wizard automatically adds his patron’s " +
                "spells to his spellbook instead of to his familiar.", null, false, FeatureGroup.WitchPatron);
            patron_selection.m_AllFeatures = patron_list;

            // Patch the patrons to progress with the wizard class.
            foreach (var reference in patron_list)
            {
                var patron = (BlueprintProgression)reference.Get();
                patron.m_Classes = patron.m_Classes.Concat(new[] { Helpers.CreateClassBasedProgression(DB.GetClass("Wizard Class"), 0) }).ToArray();
                patron.m_Archetypes = patron.m_Archetypes.Concat(new[] { Helpers.CreateArchetypeBasedProgression(pact_wizard_archetype, 0) }).ToArray();

                var container = new SpellListContainer();

                foreach (var entry in patron.LevelEntries)
                {
                    var feature = (BlueprintFeature)entry.Features.First();
                    var component = entry.Features.Select(f => f.GetComponent<AddKnownSpell>()).First();
                    feature.CreateAddSpellKnown(DB.GetClass("Wizard Class"), pact_wizard_archetype, component.Spell, component.SpellLevel);
                    if (!container.spell_list.ContainsKey(component.Spell))
                    {
                        container.spell_list.Add(component.Spell, entry.Level);
                    }
                }
                
                patron.CreateComponents(container);
            }
        }

        public static void CreateSpontaneousConversion()
        {
            spontaneous_conversion = Helpers.CreateFeature("PactWizardConversion", "Spontaneous Spell Conversion", "A pact wizard can expend any " +
                "prepared spell that isn’t a spell prepared using the additional spell slot the wizard receives from his arcane school in order to " +
                "spontaneously cast one of his patron’s spells of the same level or lower.");
        }

        public static void CreateCurseFeature()
        {
            var oracle_selection = DB.GetSelection("Curse Selection");
            curse_selection = Helpers.CreateFeatureSelection("PactWizardCurseSelection", "Great Power, Greater Expense", "As a pact wizard grows in " +
                "power, his choice of patron begins to affect his physical body.\nAt 5th level, the pact wizard chooses one oracle curse, using half " +
                "his character level as his effective oracle level when determining the effects of this curse.\nIf an oracle curse would add spells " +
                "to the oracle’s list of spells known, the pact wizard instead add those spells to the wizard’s spell list as well as to his " +
                "spellbook.", oracle_selection.Icon, false, FeatureGroup.OracleCurse);
            curse_selection.m_AllFeatures = oracle_selection.m_AllFeatures;

            // Patch the curses giving spells so they add them to the spellbook.
            foreach (var reference in oracle_selection.m_AllFeatures)
            {
                var curse = (BlueprintProgression)reference.Get();

                var container = new SpellListContainer();

                foreach (var entry in curse.LevelEntries)
                {
                    var feature = (BlueprintFeature)entry.Features.First();
                    var component = entry.Features.Select(f => f.GetComponent<AddKnownSpell>()).First();
                    if (component != null)
                    {
                        feature.CreateAddSpellKnown(DB.GetClass("Wizard Class"), pact_wizard_archetype, component.Spell, component.SpellLevel);
                        container.spell_list.Add(component.Spell, entry.Level);
                    }
                }

                curse.CreateComponents(container);
            }
        }

        public static void CreateDoubleRolls()
        {
            pact_wizard_reroll_resource = Helpers.CreateBlueprint<BlueprintAbilityResource>("PactWizardDoubleRollsResource", bp =>
            {
                bp.CreateExtraData();
                bp.SetIncreasedWithStat(StatType.Intelligence, 3, true);
            });

            double_rolls_buff = Helpers.CreateBuff("PactWizardDoubleRollsBuff", "Great Power, Greater Expense", "", null, null,
                BlueprintBuff.Flags.HiddenInUi);
            double_rolls_buff.CreateGenericComponent<Mechanics.PactWizardRollD20>();

            var double_rolls_ab = Helpers.CreateActivatableAbility("PactWizardDoubleRollsAb", "Great Power, Greater Expense", "At 10th level, the " +
                "pact wizard can invoke his patron’s power to roll twice and take the better result when attempting any caster level check, " +
                "concentration check, initiative check, or saving throw. He can activate this ability as a free action before attempting the " +
                "checks. He can use this ability a number of times per day equal to 3 + half his Intelligence modifier.", null, double_rolls_buff);
            double_rolls_ab.m_Icon = DB.GetAbility("Foresight Ability").Icon;
            double_rolls_ab.CreateAbilityResourceLogic(pact_wizard_reroll_resource, ActivatableAbilityResourceLogic.ResourceSpendType.Never);

            double_rolls_feature = Helpers.CreateFeature("PactWizardDoubleRollsFeature", double_rolls_ab.Name, double_rolls_ab.Description, null,
                DB.GetAbility("Foresight Ability").Icon);
            double_rolls_feature.HideInCharacterSheetAndLevelUp = true;
            double_rolls_feature.CreateAddAbilityResource(pact_wizard_reroll_resource, 0);
            double_rolls_feature.CreateAddFacts(double_rolls_ab);
        }

        public static void CreateBonusToRolls()
        {
            var lv15buff = Helpers.CreateBuff("PactWizardLv15Buff", "Great Power, Greater Expense", "", null, null, BlueprintBuff.Flags.HiddenInUi);
            lv15buff.CreateAddStatBonusContext(StatType.Initiative, ModifierDescriptor.Insight, AbilityRankType.StatBonus);
            lv15buff.CreateAddStatBonusContext(StatType.SaveFortitude, ModifierDescriptor.Insight, AbilityRankType.StatBonus);
            lv15buff.CreateAddStatBonusContext(StatType.SaveReflex, ModifierDescriptor.Insight, AbilityRankType.StatBonus);
            lv15buff.CreateAddStatBonusContext(StatType.SaveWill, ModifierDescriptor.Insight, AbilityRankType.StatBonus);
            lv15buff.CreateConcentrationBonus(0, false, ContextValueType.Rank, AbilityRankType.StatBonus);
            lv15buff.CreateSpellPenBonus(0, false, ContextValueType.Rank, AbilityRankType.StatBonus);
            lv15buff.CreateDispelBonus(ContextValueType.Rank, AbilityRankType.StatBonus);
            lv15buff.CreateContextRankConfigStat(AbilityRankType.StatBonus, StatType.Intelligence);

            lv15feature = Helpers.CreateFeature("PactWizardLv15Feature", "Great Power, Greater Expense", "At 15th level, when the pact " +
                "wizard invokes his patron’s power to roll twice on a check, he adds his Intelligence bonus to the result as an insight bonus. " +
                "Furthermore, when he applies metamagic feats to any spells he learned via his patron or curse, he treats that spell’s final " +
                "effective level as 1 lower (to a minimum level equal to the spell’s original level).", null, double_rolls_feature.Icon);
            lv15feature.CreateBuffExtraEffects(double_rolls_buff, lv15buff);
            lv15feature.CreateGenericComponent<ReduceMetamagicCostForSpellFromFeatureGroups>(c =>
            {
                c.group = FeatureGroup.WitchPatron;
                c.group2 = FeatureGroup.OracleCurse;
                c.value = 1;
            });
        }

        public static void CreateAutoSuccess()
        {
            var auto_success_buff = Helpers.CreateBuff("PactWizardAutoSuccessBuff", "Great Power, Greater Expense", "", null, null,
                BlueprintBuff.Flags.HiddenInUi);
            auto_success_buff.CreateExtraData();
            auto_success_buff.GetExtraData().dispel_success_on_20 = true;
            auto_success_buff.GetExtraData().spell_pen_success_on_20 = true;

            auto_success = Helpers.CreateFeature("PactWizardAutoSuccess", "Great Power, Greater Expense", "At 20th level, whenever the pact " +
                "wizard invokes his patron’s power to roll twice on a check and his result is a natural 20, he automatically succeeds, " +
                "regardless of whether or not a check of that type would normally allow an automatic success.", null, double_rolls_feature.Icon);
            auto_success.CreateBuffExtraEffects(double_rolls_buff, auto_success_buff);
        }
    }

    [HarmonyPatch(typeof(Spellbook), "GetSpontaneousConversionSpells", typeof(AbilityData))]
    internal class Spellbook_GetSpontaneousConversionSpells_PactWizardPatch
    {
        [HarmonyPostfix]
        private static void PactWizard_GetSpontaneousConversionSpells(AbilityData spell, ref IEnumerable<BlueprintAbility> __result, Spellbook __instance)
        {
            if (spell == null) { return; }
            var spellLevel = __instance.GetSpellLevel(spell);
            if (spellLevel <= 0) { return; }
            if (PactWizard.wizardSpellbooks.Contains(__instance.Blueprint.AssetGuid.ToString()) &&
                __instance.Owner.HasFact(PactWizard.spontaneous_conversion))
            {
                if (spell.SpellSlot.Type != SpellSlotType.Common) { return; }
                var patron = (BlueprintProgression)__instance.Owner.Progression.Selections[PactWizard.patron_selection].GetSelections(1).First();
                var list = new List<BlueprintAbility>();
                foreach (var entry in patron.LevelEntries)
                {
                    if (entry.Level > __instance.Owner.Progression.GetClassLevel(DB.GetClass("Wizard Class")))
                    {
                        continue;
                    }
                    var components = entry.Features.Select(f => f.GetComponent<AddKnownSpell>());
                    foreach (var comp in components)
                    {
                        if (comp.SpellLevel <= spellLevel)
                        {
                            list.Add(comp.m_Spell.Get());
                        }
                    }
                }
                __result = list.ToArray();
            }
        }
    }
}

namespace MagicTime.Archetypes.Mechanics
{
    [AllowedOn(typeof(BlueprintBuff), false)]
    [TypeId("36908850-ef36-4936-8d20-b67a9e6b4095")]
    public class PactWizardRollD20 : UnitBuffComponentDelegate, IInitiatorRulebookHandler<RuleRollD20>, IRulebookHandler<RuleRollD20>,
    ISubscriber, IInitiatorRulebookSubscriber
    {
        private bool CanApply(RuleRollD20 evt)
        {
            var prev = Rulebook.CurrentContext.PreviousEvent;
            if ((prev is RuleInitiativeRoll || prev is RuleSpellResistanceCheck || prev is RuleDispelMagic || prev is RuleSavingThrow ||
                prev is RuleCheckConcentration) && evt.Initiator == Owner)
            {
                return true;
            }
            return false;
        }

        public void OnEventAboutToTrigger(RuleRollD20 evt)
        {
            if (!CanApply(evt)) { return; }
            evt.AddReroll(1, true, Buff);
        }

        public void OnEventDidTrigger(RuleRollD20 evt)
        {
            if (!CanApply(evt)) { return; }
            Owner.Resources.Spend(PactWizard.pact_wizard_reroll_resource, 1);
            if (Owner.Resources.GetResourceAmount(PactWizard.pact_wizard_reroll_resource) == 0)
            {
                Owner.RemoveFact(Buff);
            }
        }
    }
}