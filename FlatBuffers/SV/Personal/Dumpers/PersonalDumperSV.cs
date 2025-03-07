using System.Diagnostics;

namespace pkNX.Structures.FlatBuffers.SV;

public class PersonalDumperSV
{
    public const bool HasAbilities = true;

    public required IReadOnlyList<string> Abilities { private get; init; }
    public required IReadOnlyList<string> Types { private get; init; }
    public required IReadOnlyList<string> Items { private get; init; }
    public required IReadOnlyList<string> Colors { private get; init; }
    public required IReadOnlyList<string> EggGroups { private get; init; }
    public required IReadOnlyList<string> ExpGroups { private get; init; }
    public required IReadOnlyList<string> Moves { protected get; init; }
    public required IReadOnlyList<string> Species { private get; init; }
    public required IReadOnlyList<string> ZukanA { private get; init; }
    public required IReadOnlyList<string> ZukanB { private get; init; }

    public static ushort[] TMIndexes => PersonalInfo9SV.TMIndexes;

    private static readonly string[] AbilitySuffix = [" (1)", " (2)", " (H)"];

    public IReadOnlyList<List<string>> MoveSpeciesLearn { get; private set; } = [];

    public readonly PersonalDumperSettings Settings = new();

    public List<string> Dump(PersonalTable9SV table)
    {
        var lines = new List<string>();
        var ml = new List<string>[Moves.Count];
        for (int i = 0; i < ml.Length; i++)
            ml[i] = [];
        MoveSpeciesLearn = ml;

        for (ushort species = 0; species <= table.MaxSpeciesID; species++)
        {
            var pi = table[species];
            var specInternal = SpeciesConverterSV.GetInternal9(species);
            for (byte form = 0; form < pi.FormCount; form++)
                AddDump(lines, table, specInternal, species, form);
        }
        return lines;
    }

    private string GetSpeciesName(ushort internalIndex) => Species[internalIndex];

    public void AddDump(List<string> lines, PersonalTable9SV table, ushort speciesInternal, ushort species, byte form)
    {
        var index = table.GetFormIndex(species, form);
        var entry = table[index];
        string name = GetSpeciesName(speciesInternal);
        if (form != 0)
            name += $"-{form}";
        if (entry.FB.Dex is { } dex)
            name += $" #{dex.Index:000}";
        if (entry.FB.KitakamiDex != 0)
            name += $" K#{entry.FB.KitakamiDex:000}";
        if (entry.FB.BlueberryDex != 0)
            name += $" B#{entry.FB.BlueberryDex:000}";
        AddDump(lines, entry, index, name, speciesInternal, form);
    }

    private void AddDump(List<string> lines, PersonalInfo9SV pi, int entry, string name, ushort speciesInternal, byte form)
    {
        if (pi is { IsPresentInGame: false })
            return;

        var specName = GetSpeciesName(speciesInternal);
        var specCode = pi.FormCount > 1 ? $"{specName}-{form}" : $"{specName}";

        if (Settings.Stats)
            AddPersonalLines(lines, pi, entry, name, specCode);
        if (Settings.Learn)
            AddLearnsets(pi.FB, lines, specCode);
        if (Settings.Evo)
            AddEvolutions(pi.FB, lines);
        if (Settings.Dex)
            AddZukan(lines, entry);

        lines.Add("");
    }

    private void AddZukan(List<string> lines, int entry)
    {
        if (entry >= Species.Count)
            return;
        lines.Add(ZukanA[entry].Replace("\\n", " "));
        lines.Add(ZukanB[entry].Replace("\\n", " "));
    }

    private void AddLearnsets(PersonalInfo fb, List<string> lines, string specCode)
    {
        var learn = fb.Learnset;
        lines.Add("Level Up Moves:");
        foreach (var x in learn)
        {
            var move = x.Move;
            var level = x.Level;
            if (level == -3)
                level = 0; // remap for canonical purposes
            lines.Add($"- [{level:00}] {Moves[move]}");
            MoveSpeciesLearn[move].Add(specCode);
        }

        if (TMIndexes.Length != 0)
        {
            var tmMoveIDs = fb.TechnicalMachine;
            if (tmMoveIDs.Count != 0)
            {
                lines.Add("TM Learn:");
                foreach (var move in tmMoveIDs.OrderBy(z => Array.IndexOf(TMIndexes, z)))
                {
                    var tmID = Array.IndexOf(TMIndexes, move);
                    if (tmID < 0)
                        continue;
                    lines.Add($"- [TM{tmID:000}] {Moves[move]}");
                    MoveSpeciesLearn[move].Add(specCode);
                }
            }
        }

        {
            var egg = fb.EggMoves;
            if (egg.Count != 0)
            {
                lines.Add("Egg Moves:");
                foreach (var move in egg)
                {
                    lines.Add($"- {Moves[move]}");
                    MoveSpeciesLearn[move].Add(specCode);
                }
            }
        }
        {
            var tmMoveIDs = fb.ReminderMoves;
            if (tmMoveIDs.Count != 0)
            {
                lines.Add("Reminder:");
                foreach (var move in tmMoveIDs)
                {
                    lines.Add($"- {Moves[move]}");
                    MoveSpeciesLearn[move].Add(specCode);
                }
            }
        }
    }

    private void AddEvolutions(PersonalInfo fb, List<string> lines)
    {
        var evo = fb.Evolutions;
        if (evo.Count == 0)
            return;

        foreach (var z in evo)
        {
            if (z.Reserved3 != 0 || z.Reserved4 != 0 || z.Reserved5 != 0)
                throw new Exception("Reserved fields not 0");

            var method = (EvolutionType)z.Method;
            string arg = GetArgTypeDisplayValue(method, z.Argument);
            var line = $"Evolves into {GetSpeciesName(z.SpeciesInternal)}-{z.Form} @ lv{z.Level} ({method}) [{arg}]";
            lines.Add(line);
        }
    }

    private string GetArgTypeDisplayValue(EvolutionType type, ushort value)
    {
        if (type.IsPlibUseItemType())
        {
            var item = Plib9.PlibToItem[value];
            return Items[item];
        }
        var argType = type.GetArgType();
        return GetArgTypeDisplayValue(argType, value);
    }

    private string GetArgTypeDisplayValue(EvolutionTypeArgumentType argType, ushort value) => argType switch
    {
        EvolutionTypeArgumentType.Level => value.ToString(),
        EvolutionTypeArgumentType.NoArg => value.ToString(),
        EvolutionTypeArgumentType.Items => Items[value],
        EvolutionTypeArgumentType.Moves => Moves[value],
        EvolutionTypeArgumentType.Species => GetSpeciesName(value),
        EvolutionTypeArgumentType.Type => Types[value],
        EvolutionTypeArgumentType.Stat => value.ToString(),
        EvolutionTypeArgumentType.Version => value.ToString(),
        _ => throw new ArgumentOutOfRangeException(nameof(argType), argType, null),
    };

    private void AddPersonalLines(List<string> lines, IPersonalInfo pi, int entry, string name, string specCode)
    {
        Debug.WriteLine($"Dumping {specCode}");
        lines.Add("======");
        lines.Add($"{entry:000} - {name} (Stage: {pi.EvoStage})");
        lines.Add("======");
        if (pi is IPersonalMisc_SWSH { IsPresentInGame: false })
            lines.Add("Present: No");
        lines.Add($"Base Stats: {pi.HP}.{pi.ATK}.{pi.DEF}.{pi.SPA}.{pi.SPD}.{pi.SPE} (BST: {pi.GetBaseStatTotal()})");
        lines.Add($"EV Yield: {pi.EV_HP}.{pi.EV_ATK}.{pi.EV_DEF}.{pi.EV_SPA}.{pi.EV_SPD}.{pi.EV_SPE}");
        lines.Add($"Gender Ratio: {pi.Gender}");
        lines.Add($"Catch Rate: {pi.CatchRate}");

        if (HasAbilities)
        {
            var abils = new int[pi.GetNumAbilities()];
            pi.GetAbilities(abils);
            var msg = string.Join(" | ", abils.Select((z, j) => Abilities[z] + AbilitySuffix[j]));
            lines.Add($"Abilities: {msg}");
        }

        lines.Add(string.Format(pi.Type1 != pi.Type2
            ? "Type: {0} / {1}"
            : "Type: {0}", Types[(int)pi.Type1], Types[(int)pi.Type2]));

        lines.Add($"EXP Group: {ExpGroups[pi.EXPGrowth]}");
        lines.Add(string.Format(pi.EggGroup1 != pi.EggGroup2
            ? "Egg Group: {0} / {1}"
            : "Egg Group: {0}", EggGroups[pi.EggGroup1], EggGroups[pi.EggGroup2]));
        lines.Add($"Height: {(decimal)pi.Height / 100:00.00}m, Weight: {(decimal)pi.Weight / 10:000.0}kg, Color: {Colors[pi.Color]}");
    }
}

public static class Plib9
{
    public static bool IsPlibUseItemType(this EvolutionType method) => method switch
    {
        EvolutionType.UseItem => true,
        EvolutionType.UseItemMale => true,
        EvolutionType.UseItemFemale => true,
        EvolutionType.LevelUpHeldItemDay => true,
        EvolutionType.LevelUpHeldItemNight => true,
        //EvolutionType.UseItemWormhole => true,
        //EvolutionType.UseItemFullMoon => true,
        _ => false,
    };

    // plib_item_conversion_array
    // inverted {a,b} => {b,a} so we can get the item ID from plib evo arg.

    public static readonly Dictionary<ushort, ushort> PlibToItem = new()
    {
        { 0001, 0080 }, // Sun Stone
        { 0002, 0081 }, // Moon Stone
        { 0003, 0082 }, // Fire Stone
        { 0004, 0083 }, // Thunder Stone
        { 0005, 0084 }, // Water Stone
        { 0006, 0085 }, // Leaf Stone
        { 0007, 0107 }, // Shiny Stone
        { 0008, 0108 }, // Dusk Stone
        { 0009, 0110 }, // Oval Stone
        { 0010, 1779 }, // Griseous Core
        { 0011, 0000 },
        { 0012, 0000 },
        { 0013, 0000 },
        { 0014, 0000 },
        { 0015, 0229 }, // Everstone
        { 0016, 0236 }, // Light Ball
        { 0017, 0000 },
        { 0018, 0000 },
        { 0019, 0280 }, // Destiny Knot
        { 0020, 0289 }, // Power Bracer
        { 0021, 0290 }, // Power Belt
        { 0022, 0291 }, // Power Lens
        { 0023, 0292 }, // Power Band
        { 0024, 0293 }, // Power Anklet
        { 0025, 0294 }, // Power Weight
        { 0026, 0298 }, // Flame Plate
        { 0027, 0299 }, // Splash Plate
        { 0028, 0300 }, // Zap Plate
        { 0029, 0301 }, // Meadow Plate
        { 0030, 0302 }, // Icicle Plate
        { 0031, 0303 }, // Fist Plate
        { 0032, 0304 }, // Toxic Plate
        { 0033, 0305 }, // Earth Plate
        { 0034, 0306 }, // Sky Plate
        { 0035, 0307 }, // Mind Plate
        { 0036, 0308 }, // Insect Plate
        { 0037, 0309 }, // Stone Plate
        { 0038, 0310 }, // Spooky Plate
        { 0039, 0311 }, // Draco Plate
        { 0040, 0312 }, // Dread Plate
        { 0041, 0313 }, // Iron Plate
        { 0042, 0000 },
        { 0043, 0000 },
        { 0044, 0000 },
        { 0045, 0000 },
        { 0046, 0000 },
        { 0047, 0000 },
        { 0048, 0000 },
        { 0049, 0326 }, // Razor Claw
        { 0050, 0327 }, // Razor Fang
        { 0051, 0644 }, // Pixie Plate
        { 0052, 0849 }, // Ice Stone
        { 0053, 0000 },
        { 0054, 0000 },
        { 0055, 0000 },
        { 0056, 0000 },
        { 0057, 0000 },
        { 0058, 0000 },
        { 0059, 0000 },
        { 0060, 0000 },
        { 0061, 0000 },
        { 0062, 0000 },
        { 0063, 0000 },
        { 0064, 0000 },
        { 0065, 0000 },
        { 0066, 0000 },
        { 0067, 0000 },
        { 0068, 0000 },
        { 0069, 0000 },
        { 0070, 1103 }, // Rusted Sword
        { 0071, 1104 }, // Rusted Shield
        { 0072, 1109 }, // Strawberry Sweet
        { 0073, 1110 }, // Love Sweet
        { 0074, 1111 }, // Berry Sweet
        { 0075, 1112 }, // Clover Sweet
        { 0076, 1113 }, // Flower Sweet
        { 0077, 1114 }, // Star Sweet
        { 0078, 1115 }, // Ribbon Sweet
        { 0079, 1116 }, // Sweet Apple
        { 0080, 1117 }, // Tart Apple
        { 0081, 1253 }, // Cracked Pot
        { 0082, 1254 }, // Chipped Pot
        { 0083, 1582 }, // Galarica Cuff
        { 0084, 1592 }, // Galarica Wreath
        { 0085, 2344 }, // Auspicious Armor
        { 0086, 1861 }, // Malicious Armor
        { 0087, 2345 }, // Leader’s Crest
        { 0088, 1857 }, // Scroll of Darkness
        { 0089, 1858 }, // Scroll of Waters
        { 0090, 0000 },
        { 0091, 0000 },
        { 0092, 0218 }, // Soothe Bell
        { 0093, 0109 }, // Dawn Stone
        { 0094, 2403 }, // Unremarkable Teacup
        { 0095, 2404 }, // Masterpiece Teacup
        { 0096, 2402 }, // Syrupy Apple
        { 0111, 0537 }, // Prism Scale
        { 0112, 0325 }, // Reaper Cloth
        { 0113, 0252 }, // Upgrade
        { 0114, 0324 }, // Dubious Disc
        { 0115, 0322 }, // Electirizer
        { 0116, 0323 }, // Magmarizer
        { 0117, 0321 }, // Protector
        { 0118, 0235 }, // Dragon Scale
        { 0119, 2482 }, // Metal Alloy
    };
}
