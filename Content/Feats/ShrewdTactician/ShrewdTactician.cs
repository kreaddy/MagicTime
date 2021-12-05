using Kingmaker.Blueprints.Classes;
using MagicTime.Utilities;

namespace MagicTime.Feats
{
    static class ShrewdTactician
    {
        private static BlueprintFeature shrewd_tactician;

        public static void Create()
        {
            shrewd_tactician = Helpers.CreateFeature(
                "ShrewdTactician",
                "Shrewd Tactician",
                "Your dealings with pirates, thieves, and assassins have taught you to be exceedingly careful.\nBenefit: Opponents do not gain " +
                "a +2 bonus on attack rolls for flanking you, although they can still sneak attack you.", null, null, false);
            shrewd_tactician.CreateFeatureRestriction(DB.GetFeature("Alertness"));
            shrewd_tactician.CreateFeatureRestriction(DB.GetFeature("Combat Reflexes"));
            shrewd_tactician.CreateGenericComponent<Mechanics.TacticianNoFlankBonus>();
            shrewd_tactician.Groups = new FeatureGroup[] { FeatureGroup.Feat, FeatureGroup.CombatFeat };
            Helpers.AddNewFeat(shrewd_tactician);
            Helpers.AddNewFighterFeat(shrewd_tactician);
            Helpers.AddNewCombatTrick(shrewd_tactician);
        }
    }
}