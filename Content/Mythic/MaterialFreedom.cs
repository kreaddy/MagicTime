using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using MagicTime.Utilities;
using Starion.BPExtender.UnitFact;
using System.Collections.Generic;
using System.Linq;

namespace MagicTime.Mythic
{
    internal static class MaterialFreedom
    {
        public static void Create()
        {
            var jade = Helpers.CreateFeature("MaterialFreedomJade", "Material Freedom (Jade)", "You can ignore the need for jade in spell components.",
                null, DB.GetItem("Jade").Icon);
            jade.CreateComponent<MaterialFreedomComponent>(c => c.Item = DB.GetItem("Jade").ToReference<BlueprintItemReference>());

            var diamond_dust = Helpers.CreateFeature("MaterialFreedomDiamondDust", "Material Freedom (Diamond Dust)", "You can ignore the need " +
                "for diamond dust in spell components.", null, DB.GetItem("Diamond Dust").Icon);
            diamond_dust.CreateComponent<MaterialFreedomComponent>(c => c.Item = DB.GetItem("Diamond Dust").ToReference<BlueprintItemReference>());

            var diamond = Helpers.CreateFeature("MaterialFreedomDiamond", "Material Freedom (Diamond)", "You can ignore the need " +
                "for diamond in spell components.", null, DB.GetItem("Diamond").Icon);
            diamond.CreateComponent<MaterialFreedomComponent>(c => c.Item = DB.GetItem("Diamond").ToReference<BlueprintItemReference>());

            var dinosaur_bone = Helpers.CreateFeature("MaterialFreedomDinosaurBone", "Material Freedom (Dinosaur Bone)", "You can ignore the need " +
                "for dinosaur bone in spell components.", null, DB.GetItem("Dinosaur Bone").Icon);
            dinosaur_bone.CreateComponent<MaterialFreedomComponent>(c => c.Item = DB.GetItem("Dinosaur Bone").ToReference<BlueprintItemReference>());

            var selection = Helpers.CreateFeatureSelection("MaterialFreedomSelection", "Material Freedom", "Your mythic powers allows you to eschew " +
                "the material components of your spells.\nBenefit: Choose a specific material component: you now ignore it entirely when casting " +
                "a spell that normally requires it. You may choose this ability multiple times.");
            selection.m_AllFeatures = new BlueprintFeatureReference[]
            {
                diamond.ToReference<BlueprintFeatureReference>(),
                diamond_dust.ToReference<BlueprintFeatureReference>(),
                dinosaur_bone.ToReference<BlueprintFeatureReference>(),
                jade.ToReference<BlueprintFeatureReference>()
            };

            Helpers.AddNewMythicAbility(selection);

            PatchAssembly();
        }

        private static void PatchAssembly()
        {
            var harmony = new Harmony(Main.Mod.Info.Id);
            var method = AccessTools.Method(typeof(AbilityData), "get_HasEnoughMaterialComponent");
            var patch = AccessTools.Method(typeof(MaterialFreedom_AbilityData), "MF_get_HasEnoughMaterialComponent");
            harmony.Patch(method, postfix: new HarmonyMethod(patch));
            method = AccessTools.Method(typeof(AbilityData), "SpendMaterialComponent");
            patch = AccessTools.Method(typeof(MaterialFreedom_AbilityData), "MF_SpendMaterialComponent");
            harmony.Patch(method, prefix: new HarmonyMethod(patch));
        }

        private class MaterialFreedomUnitPart : OldStyleUnitPart
        {
            private readonly List<IgnoredComponent> ItemList = new List<IgnoredComponent>();

            public void AddEntry(BlueprintItemReference item, EntityFact source_fact)
            {
                ItemList.Add(new IgnoredComponent { Item = item, SourceFact = source_fact });
            }

            public void RemoveEntry(EntityFact source_fact)
            {
                ItemList.RemoveAll(e => e.SourceFact == source_fact);
                if (!ItemList.Any())
                {
                    RemoveSelf();
                }
            }

            public bool HasItem(BlueprintItemReference item)
            {
                var entry = ItemList.FirstOrDefault(e => e.Item.guid == item.guid);
                return entry != null;
            }
        }

        private class IgnoredComponent
        {
            public BlueprintItemReference Item;
            public EntityFact SourceFact;
        }

        [TypeId("e2755d66-d9c0-446b-aff0-e2327d91e177")]
        private class MaterialFreedomComponent : UnitFactComponentDelegate
        {
            public override void OnTurnOn()
            {
                Owner.Ensure<MaterialFreedomUnitPart>().AddEntry(Item, Fact);
            }

            public override void OnTurnOff()
            {
                Owner.Ensure<MaterialFreedomUnitPart>().RemoveEntry(Fact);
            }

            public BlueprintItemReference Item;
        }

        private static class MaterialFreedom_AbilityData
        {
            private static bool MF_SpendMaterialComponent(AbilityData __instance)
            {
                if (!__instance.RequireMaterialComponent) { return true; }
                var part = __instance.Caster?.Get<MaterialFreedomUnitPart>();
                if (part != null && part.HasItem(__instance.Blueprint.MaterialComponent.m_Item))
                {
                    return false;
                }
                return true;
            }

            private static void MF_get_HasEnoughMaterialComponent(AbilityData __instance, ref bool __result)
            {
                if (__result || !__instance.RequireMaterialComponent) { return; }
                var part = __instance.Caster?.Get<MaterialFreedomUnitPart>();
                if (part != null && part.HasItem(__instance.Blueprint.MaterialComponent.m_Item))
                {
                    __result = true;
                }
            }
        }
    }
}