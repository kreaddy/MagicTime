using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.Utility;
using MagicTime.Utilities;

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
            staff_wand.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 11);
            Main.AddNewDiscovery(staff_wand);
        }
    }

    [HarmonyPatch(typeof(AbilityData), "GetParamsFromItem")]
    internal class AbilityData_GetParamsFromItem_StaffLikeWandPatch
    {
        private static void Postfix(ref AbilityParams __result, AbilityData __instance, ItemEntity itemEntity)
        {
            if (__result == null || itemEntity == null || itemEntity.Blueprint == null || __instance.Caster == null ||
                (itemEntity.Blueprint is BlueprintItemEquipmentUsable) == false)
            {
                return;
            }
            if ((itemEntity.Blueprint as BlueprintItemEquipmentUsable).Type != UsableItemType.Wand)
            {
                return;
            }
            MagicTime.Main.Log("not null");
            if (__instance.Caster.HasFact(StaffLikeWand.staff_wand))
            {
                if (__instance.Caster.GetSpellbook(DB.GetClass("Wizard Class")).CasterLevel > __result.CasterLevel)
                {
                    __result.CasterLevel = __instance.Caster.GetSpellbook(DB.GetClass("Wizard Class")).CasterLevel;
                }
                __result.DC = __instance.Caster.Stats.Intelligence.Bonus + itemEntity.GetSpellLevel() + 10;
            }
        }
    }
}