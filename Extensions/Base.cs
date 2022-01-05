using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem;
using System.Collections.Generic;

namespace Starion.BPExtender
{
    public static class Mechanics
    {
        public static BlueprintFeature FavoriteMetamagicIntensified;
    }

    public class BlueprintExtraDataContainer
    {
        public Dictionary<string, AbilityResource.Container> ResourceContainer;
        public Dictionary<string, UnitFact.Container> UnitFactContainer;

        public BlueprintExtraDataContainer()
        {
            ResourceContainer = new Dictionary<string, AbilityResource.Container>();
            UnitFactContainer = new Dictionary<string, UnitFact.Container>();
        }
    }

    public static class EXData
    {
        public static BlueprintExtraDataContainer Data;

        public static void Initialize()
        {
            Data = new BlueprintExtraDataContainer();
        }

        public static List<UnitFact.Container> FetchUnitFactContainers(EntityFactsManager manager)
        {
            var result = new List<UnitFact.Container>();
            UnitFact.Container container;
            foreach (var fact in manager.m_Facts)
            {
                container = null;
                if (Data.UnitFactContainer.ContainsKey(fact.Blueprint.AssetGuid.m_Guid.ToString()))
                {
                    container = Data.UnitFactContainer[fact.Blueprint.AssetGuid.m_Guid.ToString()];
                }

                if (container != null)
                {
                    result.Add(container);
                }
            }
            return result;
        }
    }
}