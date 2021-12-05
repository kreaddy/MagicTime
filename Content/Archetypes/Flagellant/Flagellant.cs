using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using MagicTime.Utilities;
using System.Linq;

namespace MagicTime.Archetypes
{
    internal static class Flagellant
    {
        private static BlueprintFeature focused_feature;

        private static BlueprintFeature proficiencies;

        private static BlueprintBuff mortified_casting_buff_dc;
        private static BlueprintBuff mortified_casting_buff_lv;
        private static BlueprintAbility mortified_casting_ab_dc;
        private static BlueprintAbility mortified_casting_ab_cl;
        private static BlueprintAbility mortified_casting_ab_base;
        private static BlueprintFeature mortified_casting_feature;

        private static BlueprintFeature deadened_flesh;

        private static BlueprintBuff g_mortified_casting_buff_dc;
        private static BlueprintBuff g_mortified_casting_buff_lv;
        private static BlueprintAbility g_mortified_casting_ab_dc;
        private static BlueprintAbility g_mortified_casting_ab_cl;
        private static BlueprintAbility g_mortified_casting_ab_base;
        public static BlueprintFeature g_mortified_casting_feature;

        public static void Create()
        {
            var flagellant_archetype = Helpers.CreateBlueprint<BlueprintArchetype>("FlagellantArchetype", bp =>
            {
                bp.LocalizedName = Helpers.CreateString("FlagellantArchetype.Name", "Flagellant");
                bp.LocalizedDescription = Helpers.CreateString("FlagellantArchetype.Description", "A flagellant believes pain and suffering lead to " +
                    "the realization of the divine. A common method of flagellation is to whip a leather strap across her shoulders while praying. " +
                    "Other flagellants scour their limbs with blades. Their wounds proclaim their devotion, and the self-inflicted agony focuses " +
                    "their belief.");
                bp.AddSkillPoints = 2;
            });

            CreateFocused();
            CreateProficiencies();
            CreateMortifiedSpellcasting();
            CreateDeadenedFlesh();
            CreateGreaterMortifiedSpellcasting();

            flagellant_archetype.RemoveFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, DB.GetFeature("Cleric Proficiencies"))
            };

            flagellant_archetype.AddFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, proficiencies, focused_feature, mortified_casting_feature),
                Helpers.CreateLevelEntry(2, DB.GetFeature("Diehard")),
                Helpers.CreateLevelEntry(7, deadened_flesh),
                Helpers.CreateLevelEntry(10, deadened_flesh),
                Helpers.CreateLevelEntry(11, g_mortified_casting_feature),
                Helpers.CreateLevelEntry(13, deadened_flesh),
                Helpers.CreateLevelEntry(16, deadened_flesh),
                Helpers.CreateLevelEntry(19, deadened_flesh)
            };

            var newUIGroups = new UIGroup[]
            {
                Helpers.CreateUIGroup(deadened_flesh),
                Helpers.CreateUIGroup(mortified_casting_feature, g_mortified_casting_feature)
            };
            DB.GetClass("Cleric Class").Progression.UIGroups = DB.GetClass("Cleric Class").Progression.UIGroups.Concat(newUIGroups).ToArray();

            DB.GetClass("Cleric Class").AddArchetype(flagellant_archetype);
        }

        public static void CreateFocused()
        {
            focused_feature = Helpers.CreateFeature("FLFocused", "Focused", "A flagellant gains 2 additional skill points per level.",
                null, DB.GetFeature("Human Skilled").Icon);
        }

        public static void CreateProficiencies()
        {
            proficiencies = Helpers.CreateFeature("FLProficiencies", "Flagellant Proficiencies", "Flagellants are proficient with light armor and " +
                "shields and with the following weapons: club, dagger, heavy mace, light mace, quarterstaff and light crossbows.",
                null, DB.GetFeature("Cleric Proficiencies").m_Icon);
            proficiencies.CreateAddFacts(DB.GetFeature("Prof Club"), DB.GetFeature("Prof Dagger"), DB.GetFeature("Prof Light Mace"),
                DB.GetFeature("Prof Heavy Mace"), DB.GetFeature("Prof Staff"), DB.GetFeature("Prof Light Xbow"), DB.GetFeature("Prof Shield"),
                DB.GetFeature("Prof Light Armor"));
        }

        public static void CreateMortifiedSpellcasting()
        {
            mortified_casting_buff_dc = Helpers.CreateBuff("FLMortifiedBuffDC", "Mortified Spellcasting",
                "The next spell you cast this round will have its DC increased by 1 but you will take damage equal to its level upon casting it.",
                "fl_mortified", DB.GetBuff("Corrupted Blood Buff").FxOnStart, BlueprintBuff.Flags.RemoveOnRest);
            mortified_casting_buff_dc.CreateAbilityUseTrigger(true, DB.GetSpellbook("Cleric Spellbook"), true, new ContextActionRemoveSelf());
            mortified_casting_buff_dc.CreateIncreaseSpellbookDC(1, DB.GetSpellbook("Cleric Spellbook").ToReference<BlueprintSpellbookReference>());
            mortified_casting_buff_dc.CreateGenericComponent<Mechanics.MortifiedCastingDamage>();

            mortified_casting_buff_lv = Helpers.CreateBuff("FLMortifiedBuffLv", "Mortified Spellcasting",
                "Your caster level is increased by 1 for the next spell you cast but you will take damage equal to its spell level upon casting it.",
                "fl_mortified", DB.GetBuff("Corrupted Blood Buff").FxOnStart, BlueprintBuff.Flags.RemoveOnRest);
            mortified_casting_buff_lv.CreateAbilityUseTrigger(true, DB.GetSpellbook("Cleric Spellbook"), true, new ContextActionRemoveSelf());
            mortified_casting_buff_lv.CreateIncreaseSpellbookCL(1, DB.GetSpellbook("Cleric Spellbook").ToReference<BlueprintSpellbookReference>());
            mortified_casting_buff_lv.CreateGenericComponent<Mechanics.MortifiedCastingDamage>();

            mortified_casting_ab_dc = Helpers.CreateAbility("FLMortifiedAbDC", "Mortified Casting — Increase DC",
                "Increase the save DC of your next spell by 1.", "fl_mortified", "1 round", "", UnitCommand.CommandType.Swift);
            mortified_casting_ab_dc.SetSupernaturalSelf();
            mortified_casting_ab_dc.CreateAbilityEffectRunAction(Helpers.CreateContextActionBuff(mortified_casting_buff_dc, false, true,
                Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            mortified_casting_ab_cl = Helpers.CreateAbility("FLMortifiedAbCL", "Mortified Casting — Increase CL",
                "Increase the caster level of your next spell by 1.", "fl_mortified", "1 round", "", UnitCommand.CommandType.Swift);
            mortified_casting_ab_cl.SetSupernaturalSelf();
            mortified_casting_ab_cl.CreateAbilityEffectRunAction(Helpers.CreateContextActionBuff(mortified_casting_buff_lv, false, true,
                Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            mortified_casting_ab_base = Helpers.CreateAbility("FLMortifiedAbBase", "Mortified Casting", "As a swift action, a flagellant may " +
                "inflict 1 damage on herself per level of a spell she wants to increase in power. If she does so, she can cast a spell in the same " +
                "round as if she is 1 level higher or she can increase the spell’s saving throw DC by 1.\nIf her remaining hit points are lower " +
                "than the spell level the spell fizzles out.",
                "fl_mortified", "", "", UnitCommand.CommandType.Swift);
            mortified_casting_ab_base.CreateVariants(mortified_casting_ab_dc, mortified_casting_ab_cl);
            mortified_casting_ab_base.SetParentOf(mortified_casting_ab_dc, mortified_casting_ab_cl);

            mortified_casting_feature = Helpers.CreateFeature("FLMortifiedFeature", "Mortified Casting", "A flagellant powers spellcasting through " +
                "her own blood.\nAs a swift action, she may inflict 1 damage on herself per level of a spell she wants to increase in power. If she " +
                "does so, she can cast a spell in the same round as if she is 1 level higher or she can increase the spell’s saving throw DC by 1.",
                "fl_mortified");
            mortified_casting_feature.HideInCharacterSheetAndLevelUp = true;
            mortified_casting_feature.CreateAddFacts(mortified_casting_ab_base);
        }

        public static void CreateDeadenedFlesh()
        {
            deadened_flesh = Helpers.CreateFeature("FLDeadenedFlesh", "Deadened Flesh", "At 7th level, the flagellant becomes so used to physical " +
                "injury that she gains DR 1/—. At 10th level, and every 3 cleric levels thereafter (13th, 16th, and 19th level), this damage " +
                "reduction rises by 1 point.", null, DB.GetFeature("Barbarian DR").Icon);
            deadened_flesh.Ranks = 5;
            deadened_flesh.ReapplyOnLevelUp = true;
            deadened_flesh.CreateContextRankConfig(ContextRankBaseValueType.FeatureListRanks, ContextRankProgression.AsIs, 1);
            deadened_flesh.GetComponent<ContextRankConfig>().m_FeatureList = new BlueprintFeatureReference[]
                {
                    deadened_flesh.ToReference<BlueprintFeatureReference>()
                };
            deadened_flesh.CreateAddDRUntyped(1, ContextValueType.Rank);
        }

        public static void CreateGreaterMortifiedSpellcasting()
        {
            g_mortified_casting_buff_dc = Helpers.CreateBuff("FLGMortifiedBuffDC", "Greater Mortified Spellcasting",
                "The next spell you cast this round will have its DC increased by 2 but you will take damage equal to twice its level upon " +
                "casting it.",
                "fl_mortified", DB.GetBuff("Corrupted Blood Buff").FxOnStart, BlueprintBuff.Flags.RemoveOnRest);
            g_mortified_casting_buff_dc.CreateAbilityUseTrigger(true, DB.GetSpellbook("Cleric Spellbook"), true, new ContextActionRemoveSelf());
            g_mortified_casting_buff_dc.CreateIncreaseSpellbookDC(2, DB.GetSpellbook("Cleric Spellbook").ToReference<BlueprintSpellbookReference>());
            g_mortified_casting_buff_dc.CreateGenericComponent<Mechanics.MortifiedCastingDamage>();

            g_mortified_casting_buff_lv = Helpers.CreateBuff("FLGMortifiedBuffLv", "Greater Mortified Spellcasting",
                "Your caster level is increased by 2 for the next spell you cast but you will take damage equal to twice its spell level upon " +
                "casting it.",
                "fl_mortified", DB.GetBuff("Corrupted Blood Buff").FxOnStart, BlueprintBuff.Flags.RemoveOnRest);
            g_mortified_casting_buff_lv.CreateAbilityUseTrigger(true, DB.GetSpellbook("Cleric Spellbook"), true, new ContextActionRemoveSelf());
            g_mortified_casting_buff_lv.CreateIncreaseSpellbookCL(2, DB.GetSpellbook("Cleric Spellbook").ToReference<BlueprintSpellbookReference>());
            g_mortified_casting_buff_lv.CreateGenericComponent<Mechanics.MortifiedCastingDamage>();

            g_mortified_casting_ab_dc = Helpers.CreateAbility("FLGMortifiedAbDC", "Greater Mortified Casting — Increase DC",
                "Increase the save DC of your next spell by 2.", "fl_mortified", "1 round", "", UnitCommand.CommandType.Swift);
            g_mortified_casting_ab_dc.SetSupernaturalSelf();
            g_mortified_casting_ab_dc.CreateAbilityEffectRunAction(Helpers.CreateContextActionBuff(g_mortified_casting_buff_dc, false, true,
                Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            g_mortified_casting_ab_cl = Helpers.CreateAbility("FLGMortifiedAbCL", "Greater Mortified Casting — Increase CL",
                "Increase the caster level of your next spell by 2.", "fl_mortified", "1 round", "", UnitCommand.CommandType.Swift);
            g_mortified_casting_ab_cl.SetSupernaturalSelf();
            g_mortified_casting_ab_cl.CreateAbilityEffectRunAction(Helpers.CreateContextActionBuff(g_mortified_casting_buff_lv, false, true,
                Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            g_mortified_casting_ab_base = Helpers.CreateAbility("FLGMortifiedAbBase", "Greater Mortified Casting", "As a swift action, a flagellant may " +
                "inflict 2 damage on herself per level of a spell she wants to increase in power. If she does so, she can cast a spell in the same " +
                "round as if she is 2 level higher or she can increase the spell’s saving throw DC by 2.\nIf her remaining hit points are lower " +
                "than the spell level the spell fizzles out.",
                "fl_mortified", "", "", UnitCommand.CommandType.Swift);
            g_mortified_casting_ab_base.CreateVariants(g_mortified_casting_ab_dc, g_mortified_casting_ab_cl);
            g_mortified_casting_ab_base.SetParentOf(g_mortified_casting_ab_dc, g_mortified_casting_ab_cl);

            g_mortified_casting_feature = Helpers.CreateFeature("FLGMortifiedFeature", "Mortified Casting", "At 11th level a flagellant can " +
                "increase the caster level or save DC of her next spell by 2 instead of 1 but also deals twice the damage to herself.",
                "fl_mortified");
            g_mortified_casting_feature.HideInCharacterSheetAndLevelUp = true;
            g_mortified_casting_feature.CreateAddFacts(g_mortified_casting_ab_base);
            g_mortified_casting_feature.CreateRemoveFact(mortified_casting_ab_base);
        }
    }
}