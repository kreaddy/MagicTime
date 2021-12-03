using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.Utility;
using MagicTime.Utilities;
using static MagicTime.Utilities.BlueprintsDatabase;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class StaffLikeWand
    {
        public static BlueprintFeature staff_wand;

        public static void Create()
        {
            staff_wand = Helpers.CreateFeature(
                "ADStaffLikeWand",
                "Staff-Like Wand",
                "Your research has unlocked a new power in conjunction with using a wand.\nYou use your own Intelligence score and relevant feats " +
                "to set the DC for saves against spells you cast from a wand, and you can use your caster level when activating the power of a wand " +
                "if it’s higher than the caster level of the wand.",
                "ad_stafflike_wand");
            staff_wand.CreateClassLevelRestriction(Wizard, 11);
            Main.AddNewDiscovery(staff_wand);
        }
    }

    [HarmonyPatch(typeof(AbilityData), "GetParamsFromItem")]
    internal class AbilityData_GetParamsFromItem_StaffLikeWandPatch
    {
        private static AbilityParams Postfix(AbilityParams itemParams, AbilityData __instance, [NotNull] ItemEntity itemEntity)
        {
            if (itemEntity.Blueprint != null && (itemEntity.Blueprint as BlueprintItemEquipmentUsable).Type != UsableItemType.Wand)
            {
                return itemParams;
            }
            if (__instance.Caster.HasFact(StaffLikeWand.staff_wand))
            {
                if (__instance.Caster.GetSpellbook(Wizard).CasterLevel > itemParams.CasterLevel)
                {
                    itemParams.CasterLevel = __instance.Caster.GetSpellbook(Wizard).CasterLevel;
                }
                itemParams.DC = __instance.Caster.Stats.Intelligence.Bonus + itemEntity.GetSpellLevel() + 10;
            }
            return itemParams;
        }
    }
}