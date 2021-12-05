using Kingmaker.Blueprints.Classes;
using Kingmaker.Utility;
using MagicTime.Utilities;
using System.Linq;

namespace MagicTime.Archetypes
{
    internal static class Warsighted
    {
        public static void Create()
        {
            var warsighted_archetype = Helpers.CreateBlueprint<BlueprintArchetype>("WarsightedArchetype", bp =>
            {
                bp.LocalizedName = Helpers.CreateString("WarsightedArchetype.Name", "Warsighted");
                bp.LocalizedDescription = Helpers.CreateString("WarsightedArchetype.Description", "A warsighted’s unique gifts are not in " +
                    "strange magical revelations, but in her ability to adapt in the midst of a battle with new fighting techniques.");
            });

            var bonus_feat_selection = Helpers.CreateFeatureSelection("WarsightedBonusFeat", "Bonus Combat Feat", "At 1st, 7th, 11th and 15th " +
                "level, a warsighted learns an additional feat belonging to the combat feats category. She must still meet the prerequisites " +
                "for the feat.", null, DB.GetSelection("Fighter Feat Selection").Icon);
            bonus_feat_selection.m_AllFeatures = DB.GetSelection("Fighter Feat Selection").AllFeatures;

            warsighted_archetype.RemoveFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, DB.GetFeature("Revelation Selection")),
                Helpers.CreateLevelEntry(7, DB.GetFeature("Revelation Selection")),
                Helpers.CreateLevelEntry(11, DB.GetFeature("Revelation Selection")),
                Helpers.CreateLevelEntry(15, DB.GetFeature("Revelation Selection"))
            };

            warsighted_archetype.AddFeatures = new LevelEntry[] {
                Helpers.CreateLevelEntry(1, bonus_feat_selection),
                Helpers.CreateLevelEntry(7, bonus_feat_selection),
                Helpers.CreateLevelEntry(11, bonus_feat_selection),
                Helpers.CreateLevelEntry(15, bonus_feat_selection)
            };

            var newUIGroups = new UIGroup[]
            {
                Helpers.CreateUIGroup(bonus_feat_selection)
            };

            DB.GetClass("Oracle Class").Progression.UIGroups = DB.GetClass("Oracle Class").Progression.UIGroups.Concat(newUIGroups).ToArray();

            DB.GetClass("Oracle Class").AddArchetype(warsighted_archetype);
        }
    }
}