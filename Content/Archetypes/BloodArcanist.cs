using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.UnitLogic.FactLogic;
using MagicTime.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityModManagerNet;

namespace MagicTime.Archetypes
{
    internal static class BloodArcanist
    {
        private static BlueprintArchetype blood_arcanist_archetype;
        private static List<BlueprintFeatureReference> bloodlines;
        private static BlueprintFeatureSelection bloodline_selection;

        public static void Create()
        {
            blood_arcanist_archetype = Helpers.CreateBlueprint<BlueprintArchetype>("BloodArcanistArchetype", bp =>
            {
                bp.LocalizedName = Helpers.CreateString("BloodArcanistArchetype.Name", "Blood Arcanist");
                bp.LocalizedDescription = Helpers.CreateString("BloodArcanistArchetype.Description", "Though most arcanists possess only a " +
                    "rudimentary innate arcane gift, the blood arcanist has the full power of a bloodline to draw upon.");
            });

            var ttt_bloodlines_fix = false;
            if (UnityModManager.FindMod("TabletopTweaks") != null)
            {
                var dd_spellbook = DB.GetSelection("Dragon Disciple Spellbook Selection");
                var ttt_dd_sage_sorc_ref = TTTBloodlinesFix.Bloodlines.GetBlueprint<BlueprintFeature>("dd026e4d-fd5a-4f18-a79a-24a1bbaf8546")
                    .ToReference<BlueprintFeatureReference>();
                ttt_bloodlines_fix = dd_spellbook.m_AllFeatures.Contains(ttt_dd_sage_sorc_ref);
            }

            if (!ttt_bloodlines_fix)
            {
                TTTBloodlinesFix.Bloodlines.PatchBloodlineRestrictions();
            }

            CreateBloodlines();

            bloodline_selection = Helpers.CreateFeatureSelection("BloodArcanistBloodlineSelection", "Blood Arcanist Bloodline", "A blood arcanist " +
                "selects one bloodline from those available through the sorcerer bloodline class feature. The blood arcanist gains the bloodline " +
                "arcana and bloodline powers of that bloodline, treating her arcanist level as her sorcerer level. The blood arcanist does not gain " +
                "the class skill, bonus feats, or bonus spells from her bloodline.", null, true, FeatureGroup.BloodLine);
            bloodline_selection.m_AllFeatures = bloodlines.ToArray();
            bloodline_selection.Group2 = FeatureGroup.None;
            bloodline_selection.NoSelectionIfFeature();

            blood_arcanist_archetype.RemoveFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, DB.GetSelection("Arcanist Exploits").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(3, DB.GetSelection("Arcanist Exploits").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(9, DB.GetSelection("Arcanist Exploits").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(15, DB.GetSelection("Arcanist Exploits").ToReference<BlueprintFeatureSelectionReference>()),
                Helpers.CreateLevelEntry(20, DB.GetFeature("Arcanist Capstone").ToReference<BlueprintFeatureReference>())
            };

            blood_arcanist_archetype.AddFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, bloodline_selection)
            };

            DB.GetClass("Arcanist Class").AddArchetype(blood_arcanist_archetype);

            FixBloodlineAscendance();
        }

        private static void CreateBloodlines()
        {
            bloodlines = new List<BlueprintFeatureReference>();

            var abyssal = Helpers.Clone(DB.GetProgression("Sorc Blood Abyssal"), "BloodArcanistAbyssalProgression");
            MakeClassAndArchetypeRef(abyssal);
            RemoveSpells(abyssal);
            bloodlines.Add(abyssal.ToReference<BlueprintFeatureReference>());

            var arcane = Helpers.Clone(DB.GetProgression("Sorc Blood Arcane"), "BloodArcanistArcaneProgression");
            MakeClassAndArchetypeRef(arcane);
            RemoveSpells(arcane);
            bloodlines.Add(arcane.ToReference<BlueprintFeatureReference>());

            var celestial = Helpers.Clone(DB.GetProgression("Sorc Blood Celestial"), "BloodArcanistCelestialProgression");
            MakeClassAndArchetypeRef(celestial);
            RemoveSpells(celestial);
            bloodlines.Add(celestial.ToReference<BlueprintFeatureReference>());

            var dblack = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Black"), "BloodArcanistDraconicBlackProgression");
            MakeClassAndArchetypeRef(dblack);
            RemoveSpells(dblack);
            bloodlines.Add(dblack.ToReference<BlueprintFeatureReference>());

            var dblue = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Blue"), "BloodArcanistDraconicBlueProgression");
            MakeClassAndArchetypeRef(dblue);
            RemoveSpells(dblue);
            bloodlines.Add(dblue.ToReference<BlueprintFeatureReference>());

            var dbrass = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Brass"), "BloodArcanistDraconicBrassProgression");
            MakeClassAndArchetypeRef(dbrass);
            RemoveSpells(dbrass);
            bloodlines.Add(dbrass.ToReference<BlueprintFeatureReference>());

            var dbronze = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Bronze"), "BloodArcanistDraconicBronzeProgression");
            MakeClassAndArchetypeRef(dbronze);
            RemoveSpells(dbronze);
            bloodlines.Add(dbronze.ToReference<BlueprintFeatureReference>());

            var dcopper = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Copper"), "BloodArcanistDraconicCopperProgression");
            MakeClassAndArchetypeRef(dcopper);
            RemoveSpells(dcopper);
            bloodlines.Add(dcopper.ToReference<BlueprintFeatureReference>());

            var dgold = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Gold"), "BloodArcanistDraconicGoldProgression");
            MakeClassAndArchetypeRef(dgold);
            RemoveSpells(dgold);
            bloodlines.Add(dgold.ToReference<BlueprintFeatureReference>());

            var dgreen = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Green"), "BloodArcanistDraconicGreenProgression");
            MakeClassAndArchetypeRef(dgreen);
            RemoveSpells(dgreen);
            bloodlines.Add(dgreen.ToReference<BlueprintFeatureReference>());

            var dred = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Red"), "BloodArcanistDraconicRedProgression");
            MakeClassAndArchetypeRef(dred);
            RemoveSpells(dred);
            bloodlines.Add(dred.ToReference<BlueprintFeatureReference>());

            var dsilver = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic Silver"), "BloodArcanistDraconicSilverProgression");
            MakeClassAndArchetypeRef(dsilver);
            RemoveSpells(dsilver);
            bloodlines.Add(dsilver.ToReference<BlueprintFeatureReference>());

            var dwhite = Helpers.Clone(DB.GetProgression("Sorc Blood Draconic White"), "BloodArcanistDraconicWhiteProgression");
            MakeClassAndArchetypeRef(dwhite);
            RemoveSpells(dwhite);
            bloodlines.Add(dwhite.ToReference<BlueprintFeatureReference>());

            var eleair = Helpers.Clone(DB.GetProgression("Sorc Blood Elemental Air"), "BloodArcanistElementalAirProgression");
            MakeClassAndArchetypeRef(eleair);
            RemoveSpells(eleair);
            bloodlines.Add(eleair.ToReference<BlueprintFeatureReference>());

            var eleearth = Helpers.Clone(DB.GetProgression("Sorc Blood Elemental Earth"), "BloodArcanistElementalEarthProgression");
            MakeClassAndArchetypeRef(eleearth);
            RemoveSpells(eleearth);
            bloodlines.Add(eleearth.ToReference<BlueprintFeatureReference>());

            var elefire = Helpers.Clone(DB.GetProgression("Sorc Blood Elemental Fire"), "BloodArcanistElementalFireProgression");
            MakeClassAndArchetypeRef(elefire);
            RemoveSpells(elefire);
            bloodlines.Add(elefire.ToReference<BlueprintFeatureReference>());

            var elewater = Helpers.Clone(DB.GetProgression("Sorc Blood Elemental Water"), "BloodArcanistElementalWaterProgression");
            MakeClassAndArchetypeRef(elewater);
            RemoveSpells(elewater);
            bloodlines.Add(elewater.ToReference<BlueprintFeatureReference>());

            var fey = Helpers.Clone(DB.GetProgression("Sorc Blood Fey"), "BloodArcanistFeyProgression");
            MakeClassAndArchetypeRef(fey);
            RemoveSpells(fey);
            bloodlines.Add(fey.ToReference<BlueprintFeatureReference>());

            var infernal = Helpers.Clone(DB.GetProgression("Sorc Blood Infernal"), "BloodArcanistInfernalProgression");
            MakeClassAndArchetypeRef(infernal);
            RemoveSpells(infernal);
            bloodlines.Add(infernal.ToReference<BlueprintFeatureReference>());

            var serpent = Helpers.Clone(DB.GetProgression("Sorc Blood Serpentine"), "BloodArcanistSerpentineProgression");
            MakeClassAndArchetypeRef(serpent);
            RemoveSpells(serpent);
            bloodlines.Add(serpent.ToReference<BlueprintFeatureReference>());

            var undead = Helpers.Clone(DB.GetProgression("Sorc Blood Undead"), "BloodArcanistUndeadProgression");
            MakeClassAndArchetypeRef(undead);
            RemoveSpells(undead);
            bloodlines.Add(undead.ToReference<BlueprintFeatureReference>());
        }

        private static void FixBloodlineAscendance()
        {
            var ba = DB.GetSelection("Bloodline Ascendance");
            var ttt_fix = ba.GetComponent<PrerequisiteFeaturesFromList>();
            if (ttt_fix != null)
            {
                var list = ttt_fix.m_Features.ToList();
                list.Add(bloodline_selection.ToReference<BlueprintFeatureReference>());
                ttt_fix.m_Features = list.ToArray();
            }
            else
            {
                ba.CreateFeatureRestriction(bloodline_selection, Prerequisite.GroupType.Any);
            }
            bloodline_selection.IsPrerequisiteFor = new List<BlueprintFeatureReference>();
            bloodline_selection.IsPrerequisiteFor.Add(ba.ToReference<BlueprintFeatureReference>());
        }

        private static void MakeClassAndArchetypeRef(BlueprintProgression bloodline)
        {
            bloodline.m_Classes = new BlueprintProgression.ClassWithLevel[]
{
                    new BlueprintProgression.ClassWithLevel()
                    {
                        m_Class = DB.GetClass("Arcanist Class").ToReference<BlueprintCharacterClassReference>(),
                        AdditionalLevel = 0
                    }
};
            bloodline.m_Archetypes = new BlueprintProgression.ArchetypeWithLevel[]
            {
                    new BlueprintProgression.ArchetypeWithLevel()
                    {
                        m_Archetype = blood_arcanist_archetype.ToReference<BlueprintArchetypeReference>(),
                        AdditionalLevel = 0
                    }
            };
        }

        private static void RemoveSpells(BlueprintProgression bloodline)
        {
            var new_entries = new List<LevelEntry>();
            foreach (var entry in bloodline.LevelEntries)
            {
                new_entries.Add(Helpers.CreateLevelEntryByList(entry.Level, entry.m_Features.Where(feature =>
                !feature.Get().HasLogic<AddKnownSpell>() && !feature.Get().HasLogic<AddClassSkill>() &&
                !feature.Get().AssetGuid.ToString().Equals("19ab16dba857d1a4ba617074f203f975")).ToList()));
            }
            bloodline.LevelEntries = new_entries.ToArray();
        }
    }
}