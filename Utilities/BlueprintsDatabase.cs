using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using static Kingmaker.Blueprints.ResourcesLibrary;

namespace MagicTime.Utilities
{
    static class BlueprintsDatabase
    {
        public static readonly BlueprintParametrizedFeature SpellFocus = TryGetBlueprint<BlueprintParametrizedFeature>("16fa59cc9a72a6043b566b49184f53fe");

        public static readonly BlueprintFeatureSelection FeatSelection = TryGetBlueprint<BlueprintFeatureSelection>("247a4068296e8be42890143f451b4b45");
        public static readonly BlueprintFeatureSelection FighterFeatSelection = TryGetBlueprint<BlueprintFeatureSelection>("41c8486641f7d6d4283ca9dae4147a9f");
        public static readonly BlueprintFeatureSelection MythicFeatSelection = TryGetBlueprint<BlueprintFeatureSelection>("9ee0f6745f555484299b0a1563b99d81");
        public static readonly BlueprintFeatureSelection MythicAbilitySelection = TryGetBlueprint<BlueprintFeatureSelection>("ba0e5a900b775be4a99702f1ed08914d");
        public static readonly BlueprintFeatureSelection MythicExtraAbility = TryGetBlueprint<BlueprintFeatureSelection>("8a6a511c55e67d04db328cc49aaad2b8");
        public static readonly BlueprintFeatureSelection CombatTrick = TryGetBlueprint<BlueprintFeatureSelection>("c5158a6622d0b694a99efb1d0025d2c1");
        public static readonly BlueprintFeatureSelection MagusArcanaSelection = TryGetBlueprint<BlueprintFeatureSelection>("e9dc4dfc73eaaf94aae27e0ed6cc9ada");
        public static readonly BlueprintFeatureSelection ESArcanaSelection = TryGetBlueprint<BlueprintFeatureSelection>("d4b54d9db4932454ab2899f931c2042c");
        public static readonly BlueprintFeatureSelection HexcrafterArcanaSelection = TryGetBlueprint<BlueprintFeatureSelection>("ad6b9cecb5286d841a66e23cea3ef7bf");
        public static readonly BlueprintFeatureSelection WizardFeatSelection = TryGetBlueprint<BlueprintFeatureSelection>("8c3102c2ff3b69444b139a98521a4899");

        public static readonly BlueprintCharacterClass Arcanist = TryGetBlueprint<BlueprintCharacterClass>("52dbfd8505e22f84fad8d702611f60b7");
        public static readonly BlueprintCharacterClass Magus = TryGetBlueprint<BlueprintCharacterClass>("45a4607686d96a1498891b3286121780");
        public static readonly BlueprintCharacterClass Wizard = TryGetBlueprint<BlueprintCharacterClass>("ba34257984f4c41408ce1dc2004e342e");
        public static readonly BlueprintCharacterClass Cleric = TryGetBlueprint<BlueprintCharacterClass>("67819271767a9dd4fbfd4ae700befea0");

        public static readonly BlueprintArchetype SinMage = TryGetBlueprint<BlueprintArchetype>("55a8ce15e30d71547a44c69bb2e8a84f");

        public static readonly BlueprintFeature ChannelPositive = TryGetBlueprint<BlueprintFeature>("8c769102f3996684fb6e09a2c4e7e5b9");
        public static readonly BlueprintFeature ChannelNegative = TryGetBlueprint<BlueprintFeature>("dab5255d809f77c4395afc2b713e9cd6");
        public static readonly BlueprintFeature KnowledgeDomainAllowed = TryGetBlueprint<BlueprintFeature>("443d44b3e0ea84046a9bf304c82a0425");
        public static readonly BlueprintFeature MagicDomainAllowed = TryGetBlueprint<BlueprintFeature>("08a5686378a87b64399d329ba4ef71b8");
        public static readonly BlueprintFeature ChaosDomainAllowed = TryGetBlueprint<BlueprintFeature>("8c7d778bc39fec642befc1435b00f613");
        public static readonly BlueprintFeature TravelDomainAllowed = TryGetBlueprint<BlueprintFeature>("c008853fe044bd442ae8bd22260592b7");
        public static readonly BlueprintFeature DarknessDomainAllowed = TryGetBlueprint<BlueprintFeature>("6d8e7accdd882e949a63021af5cde4b8");
        public static readonly BlueprintFeature MythicIgnoreAlignment = TryGetBlueprint<BlueprintFeature>("24e78475f0a243e1a810452d14d0a1bd");
        public static readonly BlueprintFeature ConsumeSpellsFeature = TryGetBlueprint<BlueprintFeature>("69cfb4ab0d9812249b924b8f23d6d19f");
        public static readonly BlueprintFeature PotentMagic = TryGetBlueprint<BlueprintFeature>("995110cc948d5164a820403a9e903151");
        public static readonly BlueprintFeature ArcaneReservoirFeature = TryGetBlueprint<BlueprintFeature>("55db1859bd72fd04f9bd3fe1f10e4cbb");
        public static readonly BlueprintFeature ArcanistExploitSelection = TryGetBlueprint<BlueprintFeature>("b8bf3d5023f2d8c428fdf6438cecaea7");
        public static readonly BlueprintFeature ArcanistGreaterExploits = TryGetBlueprint<BlueprintFeature>("c7536b93f17c70d4fa3a8cf9aa76bfb7");
        public static readonly BlueprintFeature ArcanistCapstone = TryGetBlueprint<BlueprintFeature>("261270d064148224fb982590b7a65414");
        public static readonly BlueprintFeature FamiliarLizard = TryGetBlueprint<BlueprintFeature>("61aeb92c176193e48b0c9c50294ab290");
        public static readonly BlueprintFeature FamiliarViper = TryGetBlueprint<BlueprintFeature>("3c0b706c526e0654b8af90ded235a089");
        public static readonly BlueprintFeature FamiliarTarantula = TryGetBlueprint<BlueprintFeature>("689b16790354c4c4c9b0f671f68d85fc");
        public static readonly BlueprintFeature FamiliarCentipede = TryGetBlueprint<BlueprintFeature>("791d888c3f87da042a0a4d0f5c43641c");
        public static readonly BlueprintFeature SpellPenetration = TryGetBlueprint<BlueprintFeature>("ee7dc126939e4d9438357fbd5980d459");
        public static readonly BlueprintFeature GreaterSpellPenetration = TryGetBlueprint<BlueprintFeature>("1978c3f91cfbbc24b9c9b0d017f4beec");
        public static readonly BlueprintFeature MythicSpellPenetration = TryGetBlueprint<BlueprintFeature>("51b6b22ff184eef46a675449e837365d");
        public static readonly BlueprintFeature MetamagicBolster = TryGetBlueprint<BlueprintFeature>("fbf5d9ce931f47f3a0c818b3f8ef8414");
        public static readonly BlueprintFeature MetamagicEmpower = TryGetBlueprint<BlueprintFeature>("a1de1e4f92195b442adb946f0e2b9d4e");
        public static readonly BlueprintFeature MetamagicExtend = TryGetBlueprint<BlueprintFeature>("f180e72e4a9cbaa4da8be9bc958132ef");
        public static readonly BlueprintFeature MetamagicMaximize = TryGetBlueprint<BlueprintFeature>("7f2b282626862e345935bbea5e66424b");
        public static readonly BlueprintFeature MetamagicPersistent = TryGetBlueprint<BlueprintFeature>("cd26b9fa3f734461a0fcedc81cafaaac");
        public static readonly BlueprintFeature MetamagicQuicken = TryGetBlueprint<BlueprintFeature>("ef7ece7bb5bb66a41b256976b27f424e");
        public static readonly BlueprintFeature MetamagicReach = TryGetBlueprint<BlueprintFeature>("46fad72f54a33dc4692d3b62eca7bb78");
        public static readonly BlueprintFeature MetamagicSelective = TryGetBlueprint<BlueprintFeature>("85f3340093d144dd944fff9a9adfd2f2");
        public static readonly BlueprintFeature FullReservoir = TryGetBlueprint<BlueprintFeature>("b3f5fd67399a2a54ea4ddd97206b4c82");
        public static readonly BlueprintFeature Alertness = TryGetBlueprint<BlueprintFeature>("1c04fe9a13a22bc499ffac03e6f79153");
        public static readonly BlueprintFeature CombatReflexes = TryGetBlueprint<BlueprintFeature>("0f8939ae6f220984e8fb568abbdfba95");
        public static readonly BlueprintFeature Outflank = TryGetBlueprint<BlueprintFeature>("422dab7309e1ad343935f33a4d6e9f11");
        public static readonly BlueprintFeature PoisonImmunity = TryGetBlueprint<BlueprintFeature>("7e3f3228be49cce49bda37f7901bf246");
        public static readonly BlueprintFeature SubtypeDemon = TryGetBlueprint<BlueprintFeature>("dc960a234d365cb4f905bdc5937e623a");
        public static readonly BlueprintFeature SubtypeDemodand = TryGetBlueprint<BlueprintFeature>("0d112671041420340b5ce7e9ab7b4320");
        public static readonly BlueprintFeature WarriorPriest = TryGetBlueprint<BlueprintFeature>("b9bee4e4e15573546b76a8d942ce914b");
        public static readonly BlueprintFeature PowerfulChange = TryGetBlueprint<BlueprintFeature>("5e01e267021bffe4e99ebee3fdc872d1");
        public static readonly BlueprintFeature OppAbjuration = TryGetBlueprint<BlueprintFeature>("7f8c1b838ff2d2e4f971b42ccdfa0bfd");
        public static readonly BlueprintFeature OppDivination = TryGetBlueprint<BlueprintFeature>("09595544116fe5349953f939aeba7611");
        public static readonly BlueprintFeature OppConjuration = TryGetBlueprint<BlueprintFeature>("ca4a0d68c0408d74bb83ade784ebeb0d");
        public static readonly BlueprintFeature OppEnchantment = TryGetBlueprint<BlueprintFeature>("875fff6feb84f5240bf4375cb497e395");
        public static readonly BlueprintFeature OppEvocation = TryGetBlueprint<BlueprintFeature>("c3724cfbe98875f4a9f6d1aabd4011a6");
        public static readonly BlueprintFeature OppIllusion = TryGetBlueprint<BlueprintFeature>("6750ead44c0c034428c6509c68110375");
        public static readonly BlueprintFeature OppNecromancy = TryGetBlueprint<BlueprintFeature>("a9bb3dcb2e8d44a49ac36c393c114bd9");
        public static readonly BlueprintFeature OppTransmutation = TryGetBlueprint<BlueprintFeature>("fc519612a3c604446888bb345bca5234");
        public static readonly BlueprintFeature HumanSkilled = TryGetBlueprint<BlueprintFeature>("3adf9274a210b164cb68f472dc1e4544");
        public static readonly BlueprintFeature ClericProficiencies = TryGetBlueprint<BlueprintFeature>("8c971173613282844888dc20d572cfc9");
        public static readonly BlueprintFeature ProficiencyClub = TryGetBlueprint<BlueprintFeature>("2c343b1606bc68248891bd53d38a3d18");
        public static readonly BlueprintFeature ProficiencyDagger = TryGetBlueprint<BlueprintFeature>("b776c19291928cf4184d4dc65f09f3a6");
        public static readonly BlueprintFeature ProficiencyLightMace = TryGetBlueprint<BlueprintFeature>("d0a788c77b0eae948944fa424125c120");
        public static readonly BlueprintFeature ProficiencyHeavyMace = TryGetBlueprint<BlueprintFeature>("3f18330d717ea0148b496ee8cc291a60");
        public static readonly BlueprintFeature ProficiencyQuarterstaff = TryGetBlueprint<BlueprintFeature>("aed4f88b52ae0fb468895f90da854ad4");
        public static readonly BlueprintFeature ProficiencyLightCrossbow = TryGetBlueprint<BlueprintFeature>("3d96feeabdfb1ff45b63af77b545813f");
        public static readonly BlueprintFeature ProficiencyLightArmor = TryGetBlueprint<BlueprintFeature>("6d3728d4e9c9898458fe5e9532951132");
        public static readonly BlueprintFeature ProficiencyShield = TryGetBlueprint<BlueprintFeature>("cb8686e7357a68c42bdd9d4e65334633");
        public static readonly BlueprintFeature Diehard = TryGetBlueprint<BlueprintFeature>("86669ce8759f9d7478565db69b8c19ad");
        public static readonly BlueprintFeature BarbarianDR = TryGetBlueprint<BlueprintFeature>("cffb5cddefab30140ac133699d52a8f8");

        public static readonly BlueprintFeatureReplaceSpellbook SinMagicLust = TryGetBlueprint<BlueprintFeatureReplaceSpellbook>("e1ebc61a71c55054991863a5f6f6d2c2");
        public static readonly BlueprintFeatureReplaceSpellbook SinMagicPride = TryGetBlueprint<BlueprintFeatureReplaceSpellbook>("aa271e69902044b47a8e62c4e58a9dcb");

        public static readonly BlueprintSpellbook ArcanistSpellbook = TryGetBlueprint<BlueprintSpellbook>("33903fe5c4abeaa45bc249adb9d98848");
        public static readonly BlueprintSpellbook ClericSpellbook = TryGetBlueprint<BlueprintSpellbook>("4673d19a0cf2fab4f885cc4d1353da33");

        public static readonly BlueprintAbility ArcaneReservoirBase = TryGetBlueprint<BlueprintAbility>("91295893ae9fdfb4b8936a93eff019df");
        public static readonly BlueprintAbility AssassinCreatePoison = TryGetBlueprint<BlueprintAbility>("46660d0da7797124aa221818778edc9d");
        public static readonly BlueprintAbility ShieldOfLaw = TryGetBlueprint<BlueprintAbility>("73e7728808865094b8892613ddfaf7f5");
        public static readonly BlueprintAbility TargetedBombAdm = TryGetBlueprint<BlueprintAbility>("24afb2c948c731440a3aaf5411904c89");

        public static readonly BlueprintAbilityAreaEffect VolcanicStorm = TryGetBlueprint<BlueprintAbilityAreaEffect>("1d649d8859b25024888966ba1cc291d1");

        public static readonly BlueprintBuff ArcaneReservoirResourceBuff = TryGetBlueprint<BlueprintBuff>("1dd776b7b27dcd54ab3cedbbaf440cf3");
        public static readonly BlueprintBuff ArcaneReservoirDCBuff = TryGetBlueprint<BlueprintBuff>("db4b91a8a297c4247b13cfb6ea228bf3");
        public static readonly BlueprintBuff ArcanistShadowVeilBuff = TryGetBlueprint<BlueprintBuff>("5ceedff361efd4c4eb8e8369c13b03ea");
        public static readonly BlueprintBuff AreshkagalFireBuff4 = TryGetBlueprint<BlueprintBuff>("199088612b7e4958bba5a594f3b88237");
        public static readonly BlueprintBuff AreshkagalAcidBuff4 = TryGetBlueprint<BlueprintBuff>("e543322e27bf4773b208df5179d64b34");
        public static readonly BlueprintBuff DragonIIIBlack = TryGetBlueprint<BlueprintBuff>("c231e0cf7c203644d81e665d6115ae69");
        public static readonly BlueprintBuff DragonIIIBlue = TryGetBlueprint<BlueprintBuff>("a4993affb4c4ad6429eca6daeb7b86a8");
        public static readonly BlueprintBuff DragonIIIBrass = TryGetBlueprint<BlueprintBuff>("8acd6ac6f89c73b4180191eb63768009");
        public static readonly BlueprintBuff DragonIIIBronze = TryGetBlueprint<BlueprintBuff>("1d3d388fd7b740842bde43dfb0aa56bb");
        public static readonly BlueprintBuff DragonIIICopper = TryGetBlueprint<BlueprintBuff>("c0e8f767f87ac354495865ce3dc3ee46");
        public static readonly BlueprintBuff DragonIIIGold = TryGetBlueprint<BlueprintBuff>("ec6ad3612c4f0e340b54a11a0e78793d");
        public static readonly BlueprintBuff DragonIIIGreen = TryGetBlueprint<BlueprintBuff>("2d294863adf81f944a7558f7ae248448");
        public static readonly BlueprintBuff DragonIIIRed = TryGetBlueprint<BlueprintBuff>("782d09044e895fa44b9b6d9cca3a52b5");
        public static readonly BlueprintBuff DragonIIISilver = TryGetBlueprint<BlueprintBuff>("80babfb32011f384ea865d768857da79");
        public static readonly BlueprintBuff DragonIIIWhite = TryGetBlueprint<BlueprintBuff>("8dae421e48035a044a4b1a7b9208c5db");
        public static readonly BlueprintBuff DemonIVBalor = TryGetBlueprint<BlueprintBuff>("e1c5725668f48df48a9676d26aa57fbf");
        public static readonly BlueprintBuff DemonIVGallu = TryGetBlueprint<BlueprintBuff>("051c8dea7acf6aa41b8b1c1f65cda421");
        public static readonly BlueprintBuff DemonIVMarilith = TryGetBlueprint<BlueprintBuff>("f048ee68bc72da447970025667a77b12");
        public static readonly BlueprintBuff DemonIVVavakia = TryGetBlueprint<BlueprintBuff>("13c8e843d01eef5479efcd6a9adac432");
        public static readonly BlueprintBuff CorruptedBloodBuff = TryGetBlueprint<BlueprintBuff>("1419d2e2eee432849b0a596e82b9e0a2");

        public static readonly BlueprintAbilityResource ArcaneReservoirRes = TryGetBlueprint<BlueprintAbilityResource>("cac948cbbe79b55459459dd6a8fe44ce");
        public static readonly BlueprintAbilityResource MagusArcanePool = TryGetBlueprint<BlueprintAbilityResource>("effc3e386331f864e9e06d19dc218b37");

        public static readonly BlueprintRace Human = TryGetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");

        public static readonly BlueprintSpellList WizardList = TryGetBlueprint<BlueprintSpellList>("ba0401fdeb4062f40a7aa95b6f07fe89");
        public static readonly BlueprintSpellList AlchemistList = TryGetBlueprint<BlueprintSpellList>("f60d0cd93edc65c42ad31e34a905fb2f");
        public static readonly BlueprintSpellList DruidList = TryGetBlueprint<BlueprintSpellList>("bad8638d40639d04fa2f80a1cac67d6b");
    }
}