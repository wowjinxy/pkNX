using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FlatSharp;
using pkNX.Containers;
using pkNX.Game;
using pkNX.Structures;
using pkNX.Structures.FlatBuffers;
using pkNX.Structures.FlatBuffers.SV;
using pkNX.Structures.FlatBuffers.SV.Trinity;
using EvolutionType = pkNX.Structures.EvolutionType;
using GameVersion = pkNX.Structures.GameVersion;
using Move = pkNX.Structures.Move;
using Schema = pkNX.Structures.FlatBuffers.Reflection;
using Species = pkNX.Structures.Species;

// ReSharper disable StringLiteralTypo

namespace pkNX.WinForms;

public class GameDumperSV(GameManagerSV rom)
{
    public string DumpFolder
    {
        get
        {
            var parent = Directory.GetParent(rom.PathRomFS) ?? throw new DirectoryNotFoundException($"Unable to find parent directory of {rom.PathRomFS}");
            return Path.Combine(parent.FullName, "Dump");
        }
    }

    private const string DumpArchive = "archive";
    private const string DumpArchiveExtracted = TrinityPakExtractor.DumpArchiveExtracted;

    /// <summary>
    /// Message string language names/folders which contain the text localizations.
    /// </summary>
    private static readonly string[] LanguageEnglishNames =
    [
        "JPN",
        "JPN_KANJI",
        "English",
        "French",
        "Italian",
        "German",
        "Spanish",
        "Korean",
        "Simp_Chinese",
        "Trad_Chinese",
    ];

    private string[] GetCommonText(string name, string lang, TextConfig cfg)
    {
        var data = rom.GetPackedFile($"message/dat/{lang}/common/{name}.dat");
        return new TextFile(data, cfg).Lines;
    }

    private AHTB GetCommonAHTB(string name, string lang)
    {
        var data = rom.GetPackedFile($"message/dat/{lang}/common/{name}.tbl");
        return new AHTB(data);
    }

    private string GetPath(string path)
    {
        Directory.CreateDirectory(DumpFolder);
        var result = Path.Combine(DumpFolder, path);
        var parent = Directory.GetParent(result) ?? throw new DirectoryNotFoundException($"Unable to get parent directory of {result}");
        Directory.CreateDirectory(parent.FullName); // double check :(
        return result;
    }

    private string GetPath(string parent, string path)
    {
        Directory.CreateDirectory(DumpFolder);
        var result = Path.Combine(DumpFolder, parent, path);
        var parent2 = Directory.GetParent(result) ?? throw new DirectoryNotFoundException($"Unable to get parent directory of {result}");
        Directory.CreateDirectory(parent2.FullName); // double check :(
        return result;
    }

    public void DumpAHTB()
    {
        var arcPath = GetPath(DumpArchive, DumpArchiveExtracted);
        if (!Directory.Exists(arcPath))
        {
            WinFormsUtil.Alert("Archive not extracted yet. Please extract the archive first.");
            return;
        }
        var files = Directory.EnumerateFiles(arcPath, "*.ahtb", SearchOption.AllDirectories);

        var result = new HashSet<string>();
        var list = new List<string>();
        foreach (var f in files)
        {
            try
            {
                var bytes = File.ReadAllBytes(f);
                var tbl = new AHTB(bytes);
                var summaries = tbl.Summary;
                foreach (var t in tbl.ShortSummary)
                    result.Add(t);
                var fn = Path.GetFileName(f);
                list.Add(fn);
                list.AddRange(summaries);
            }
            catch
            {
            }
        }

        var outname = GetPath("ahtb.txt");
        var outname2 = GetPath("ahtblist.txt");
        File.WriteAllLines(outname, result);
        File.WriteAllLines(outname2, list);
    }

    public void DumpStrings()
    {
        var textConfig = new TextConfig(GameVersion.SV);
        GetStrings(textConfig, "script");
        GetStrings(textConfig, "common");
    }

    private void GetStrings(TextConfig textConfig, string type)
    {
        var arcPath = Path.Combine(GetPath(DumpArchive, DumpArchiveExtracted), "arc");
        foreach (var lang in LanguageEnglishNames)
            RipLanguage(textConfig, type, lang, arcPath);
    }

    private void RipLanguage(TextConfig textConfig, string type, string lang, string arcPath)
    {
        var prefix = $"messagedat{lang}";
        const string suffix = ".trpak";
        var pattern = $"{prefix}*{suffix}";
        List<(string File, string[] Lines)> text = [];
        List<(string File, string[] Lines)> full = [];
        var folders = Directory.EnumerateDirectories(arcPath, pattern, SearchOption.TopDirectoryOnly);
        foreach (var folder in folders)
        {
            // Get the file name out from the pattern we specified above.
            var name = Path.GetFileName(folder).AsSpan();
            var textFile = name.Slice(prefix.Length, name.Length - prefix.Length - suffix.Length);
            if (!textFile.StartsWith(type))
                continue;
            var fn = Path.ChangeExtension(textFile[type.Length..].ToString(), null);
            var (shortText, fullText) = GetTextTuple(textConfig, lang, folder, type, fn);
            text.Add(new(fn, shortText));
            full.Add(new(fn, fullText));
        }

        DumpTotalFiles(type, lang, text, full);
    }

    private (string[] Text, string[] Full) GetTextTuple(TextConfig textConfig, string lang, string dumped, string folder, string file)
    {
        var files = Directory.GetFiles(dumped);
        if (files.Length != 2)
        {
            if (files.Length == 1 && !files[0].EndsWith("ahtb"))
            {
                var dat = files.Single(z => !z.EndsWith("ahtb"));
                var dest = Path.Combine(lang, folder, $"{file}.txt");
                var pathLines = GetPath("text", dest);
                var textData = File.ReadAllBytes(dat);
                var lines = TextFile.GetStrings(textData, textConfig)!;
                File.WriteAllLines(pathLines, lines);
                return (lines, lines);
            }
            return (Array.Empty<string>(), Array.Empty<string>());
        }

        {
            var dat = files.Single(z => !z.EndsWith("ahtb"));
            var ahtb = files.Single(z => z.EndsWith("ahtb"));

            var dest = Path.Combine(lang, folder, $"{file}.txt");
            var pathLines = GetPath("text", dest);
            var textData = File.ReadAllBytes(dat);
            var lines = TextFile.GetStrings(textData, textConfig)!;
            File.WriteAllLines(pathLines, lines);

            var ahtbData = File.ReadAllBytes(ahtb);
            var table = new AHTB(ahtbData);
            var detailed = new string[table.Count];
            for (int i = 0; i < lines.Length; i++)
            {
                var entry = table.Entries[i];
                var hash = entry.Hash;
                var name = entry.Name;
                var line = lines[i];
                detailed[i] = $"{i:000}\t{hash:X16}\t{name}\t{line}";
            }

            var pathAHTB = GetPath("textFull", dest);
            File.WriteAllLines(pathAHTB, detailed);
            return (lines, detailed);
        }
    }

    private void DumpTotalFiles(string type, string lang,
        IReadOnlyList<(string File, string[] Lines)> text,
        IReadOnlyList<(string File, string[] Lines)> full)
    {
        var outText = GetPath("text", Path.Combine(lang, $"{type}.txt"));
        using var swt = File.CreateText(outText);
        var outFull = GetPath("text", Path.Combine(lang, $"{type}Full.txt"));
        using var swf = File.CreateText(outFull);
        for (var i = 0; i < text.Count; i++)
        {
            static void WriteFileHeader(string file, TextWriter s)
            {
                s.WriteLine("~~~~~~~~~~~~~~~");
                s.WriteLine($"Text File : {file}.dat");
                s.WriteLine("~~~~~~~~~~~~~~~");
            }

            var (fn, lines) = full[i];
            WriteFileHeader(fn, swf);
            foreach (var line in lines)
                swf.WriteLine(line);

            (fn, lines) = text[i];
            WriteFileHeader(fn, swt);
            foreach (var line in lines)
                swt.WriteLine(line);
        }
    }

    public void DumpPersonal()
    {
        const string personalPath = "avalon/data/personal_array.bin";
        Dump<PersonalTable, PersonalInfo>(personalPath, z => z.Table);

        var perbin = rom.GetPackedFile(personalPath);
        var pt = new PersonalTable9SV(new FakeContainer([perbin]));

        var bin = pt.Write();
        var path2 = GetPath("pkhex", "personal_sv");
        File.WriteAllBytes(path2, bin);

        var learnsets = SerializeLearnsetPickle(pt);
        File.WriteAllBytes(GetPath("pkhex", "lvlmove_sv.pkl"), MiniUtil.PackMini(learnsets, "sv"));
        var egg = SerializeU16Pickle(pt, z => z.EggMoves);
        File.WriteAllBytes(GetPath("pkhex", "eggmove_sv.pkl"), MiniUtil.PackMini(egg, "sv"));
        var remind = SerializeU16Pickle(pt, z => z.ReminderMoves);
        File.WriteAllBytes(GetPath("pkhex", "reminder_sv.pkl"), MiniUtil.PackMini(remind, "sv"));
        var evos = SerializeEvolutionPickle(pt);
        File.WriteAllBytes(GetPath("pkhex", "evos_sv.pkl"), MiniUtil.PackMini(evos, "sv"));

        List<(ushort Internal, ushort National)> map = [];
        for (ushort i = 0; i <= (ushort)DevID.DEV_MATCHA2; i++)
        {
            var pi = pt[i];
            var info = pi.FB.Info;
            map.Add((info.SpeciesInternal, info.SpeciesNational));
        }
        File.WriteAllText(GetPath("pkhex", "national dex.txt"), string.Join(Environment.NewLine, map.Select(z => $"{z.Internal},{z.National}")));

        foreach (var lang in LanguageEnglishNames)
            RipPersonal(pt, lang);
    }

    private void RipPersonal(PersonalTable9SV pt, string lang)
    {
        var cfg = new TextConfig(rom.Game);
        string[] GetText(string name) => GetCommonText(name, lang, cfg);
        var specNames = GetText("monsname");
        var moveNames = GetText("wazaname");
        var abilNames = GetText("tokusei");
        var itemNames = GetText("itemname");
        var zukanA = GetText("zukan_comment_A");
        var zukanB = GetText("zukan_comment_B");
        var pd = new PersonalDumperSV
        {
            Species = specNames,
            Moves = moveNames,
            Abilities = abilNames,
            Items = itemNames,
            ZukanA = zukanA,
            ZukanB = zukanB,

            Types = GetText("typename"),
            Colors = Enum.GetNames<PokeColor>(),
            EggGroups = Enum.GetNames<EggGroup>(),
            ExpGroups = Enum.GetNames<EXPGroup>(),
        };

        var lines = pd.Dump(pt);
        File.WriteAllLines(GetPath(lang, "personal.txt"), lines);

        var moveLines = pd.MoveSpeciesLearn
            .Select((z, i) => $"{i:000}\t{moveNames[i]}\t{string.Join(", ", z.Distinct())}");
        File.WriteAllLines(GetPath(lang, "MovePerPokemon.txt"), moveLines);

        var dexOrder = pt.Table
            .Where(z => z.FB.Dex != null)
            .OrderBy(z => z.FB.Dex!.Index)
            .GroupBy(z => z.FB.Info.SpeciesNational)
            .Select(z => z.First());

        var pc = dexOrder.Select(z => $"{z.FB.Dex!.Index:000}\t{specNames[z.FB.Info.SpeciesInternal]}");
        File.WriteAllText(GetPath(lang, "dex.txt"), string.Join(Environment.NewLine, pc));

        var foreign = pt.Table
            .Where(z => z.FB.Dex == null && z.IsPresentInGame)
            .GroupBy(z => z.FB.Info.SpeciesNational)
            .Select(z => z.First());

        var fl = foreign
            .Select(z => $"{z.FB.Info.SpeciesNational:000}\t{specNames[z.FB.Info.SpeciesInternal]}");
        File.WriteAllText(GetPath(lang, "foreign.txt"), string.Join(Environment.NewLine, fl));

        File.WriteAllLines(GetPath(lang, "species.txt"), SpeciesConverterSV.GetRearrangedAsNational(specNames));
        File.WriteAllLines(GetPath(lang, "abilities.txt"), abilNames);
        File.WriteAllLines(GetPath(lang, "items.txt"), itemNames);
        File.WriteAllLines(GetPath(lang, "moves.txt"), moveNames);
        File.WriteAllLines(GetPath(lang, "ZukanA.txt"), zukanA);
        File.WriteAllLines(GetPath(lang, "ZukanB.txt"), zukanB);

        var TMNames = PersonalDumperSV.TMIndexes.Select((z, i) => $"TM{i:000}\t{moveNames[z]}").Skip(1); // !~TM000
        File.WriteAllText(GetPath(lang, "tm.txt"), string.Join(Environment.NewLine, TMNames));
    }

    private static byte[][] SerializeU16Pickle(PersonalTable9SV pt, Func<PersonalInfo, IList<ushort>> sel)
    {
        var t = pt.Table;
        var result = new byte[t.Length][];
        for (int i = 0; i < t.Length; i++)
        {
            var p = t[i].FB;
            if (!p.IsPresentInGame)
                result[i] = [];
            else
                result[i] = Write(sel(p));
        }
        return result;

        static byte[] Write(IList<ushort> moves)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            foreach (var m in moves.Order()) // just in case
                bw.Write(m);
            return ms.ToArray();
        }
    }

    private static byte[][] SerializeEvolutionPickle(PersonalTable9SV pt)
    {
        var t = pt.Table;
        var result = new byte[t.Length][];
        for (int i = 0; i < t.Length; i++)
            result[i] = GetPickle(t[i]);
        return result;

        static byte[] GetPickle(PersonalInfo9SV e)
        {
            if (!e.IsPresentInGame)
                return [];
            return Write(e.FB.Info.SpeciesNational, e.FB.Evolutions);
        }

        static byte[] Write(int species, IEnumerable<PersonalInfoEvolution> evos)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            var list = evos.ToArray();
            if (species == (int)Species.Crabrawler)
                Array.Reverse(list); // put the levelup evo last.

            foreach (var m in list) // just in case
            {
                var method = ((EvolutionType)m.Method);
                if (method == EvolutionType.Hisui)
                    continue; // skip: no evolution

                // ReSharper disable RedundantCast
                bw.Write((byte)method);
                bool levelRequired = method.IsLevelUpRequired();
                bw.Write(levelRequired);
                bw.Write(GetArg(method, m.Argument));
                bw.Write(SpeciesConverterSV.GetNational9(m.SpeciesInternal));
                bw.Write((byte)m.Form);
                bw.Write((byte)m.Level);
                // ReSharper restore RedundantCast

                static ushort GetArg(EvolutionType type, ushort arg)
                {
                    if (type.IsPlibUseItemType())
                        return Plib9.PlibToItem[arg];
                    return arg;
                }
            }
            return ms.ToArray();
        }
    }

    private static byte[][] SerializeLearnsetPickle(PersonalTable9SV pt)
    {
        var t = pt.Table;
        var result = new byte[t.Length][];
        for (int i = 0; i < t.Length; i++)
        {
            var p = t[i];
            var learn = p.FB.Learnset;
            if (i == (int)Species.Zorua && p.Form == 1)
                learn = Insert(learn, 28, Move.Spite);
            else if (i == (int)Species.Larvesta && p.Form == 0)
                learn = Insert(learn, 24, Move.BugBite);
            result[i] = Write(learn);
        }
        return result;

        static List<PersonalInfoMove> Insert(IList<PersonalInfoMove> learn, sbyte level, Move move)
        {
            var list = learn.ToList();
            list.Add(new PersonalInfoMove { Level = level, Move = (ushort)move });
            return [.. list.OrderBy(z => z.Level)];
        }

        static byte[] Write(IList<PersonalInfoMove> fbLearnset)
        {
            using var ms = new MemoryStream(4 + (4 * fbLearnset.Count));
            using var bw = new BinaryWriter(ms);
            foreach (var m in fbLearnset)
            {
                bw.Write(m.Move);
                var lvl = m.Level == -3 ? 0 : m.Level;
                bw.Write((ushort)lvl);
            }
            bw.Write(-1);
            return ms.ToArray();
        }
    }

    public void DumpMisc()
    {
        DumpItemMachine();
        DumpBalloon();
        DumpBattle();
        DumpEncount();
        DumpDLC();
        DumpGymRewards();
        DumpGrow();
        DumpAudio();
        DumpAjito();
        DumpField();
        DumpJunk();
        DumpRaid();
        DumpItems();
        DumpFieldReturn();
        DumpGem();
        DumpPokeDex();
        DumpUI();
        DumpUniquePath();
        DumpNetBattle();
        DumpTrades();
        DumpShopDress();
        DumpClubroom();
    }

    private void DumpGrow()
    {
        // FlatBuffer with a single field: uint[101][8]
        // (8 inlined fields, with each having 101 fields). Same exp growth curve as prior games.
        var growTable = rom.GetPackedFile("avalon/data/growTable.bin").AsSpan();
        for (int i = 0; i < 8; i++)
        {
            // Just read the inline structs as bytes out to individual files.
            const int len = 101;
            const int size = len * sizeof(uint);
            var data = growTable.Slice(0x10 + (i * size), size).ToArray();
            File.WriteAllBytes(GetPath("raw", Path.Combine("avalon", "data", "growTable", $"grow_{i:00}.bin")), data);
        }
    }

    private void DumpGymRewards()
    {
        Dump<PopupFixTableArray, PopupFixTable>("world/data/gym/gym_denki_popup_fix/gym_denki_popup_fix_array.bfbs", z => z.Table);
        Dump<PopupPosTableArray, PopupPosTable>("world/data/gym/gym_denki_popup_pos/gym_denki_popup_pos_array.bfbs", z => z.Table);
        //Dump<ENDLESSBORDERArray, ENDLESSBORDER>("world/data/gym/gym_esper_reward_endless/gym_esper_reward_endless_array.bfbs", z => z.Table);
        //Dump<EXERCISECOURSESArray, EXERCISECOURSES>("world/data/gym/gym_esper_reward_exercise/gym_esper_reward_exercise_array.bfbs", z => z.Table);
        Dump<GymKooriCourseTableArray, GymKooriCourseTable>("world/data/gym/gym_koori_course/gym_koori_course_array.bfbs", z => z.Table);
        DumpJson<GymKooriCoursePokemonTable>("world/data/gym/gym_koori_course_pokemon/gym_koori_course_pokemon_base.bfbs");
        Dump<PokeTableArray, PokeTable>("world/data/gym/gym_kusa_poke/gym_kusa_poke_array.bfbs", z => z.Table);
        Dump<Structures.FlatBuffers.SV.Gym.RewardTableArray, Structures.FlatBuffers.SV.Gym.RewardTable>("world/data/gym/gym_kusa_reward/gym_kusa_reward_array.bfbs", z => z.Table);
        Dump<FixTableArray, FixTable>("world/data/gym/gym_mizu_seri_fix/gym_mizu_seri_fix_array.bfbs", z => z.Table);
        Dump<SeriItemTableArray, SeriItemTable>("world/data/gym/gym_mizu_seri_item/gym_mizu_seri_item_array.bfbs", z => z.Table);
        Dump<SeriNpcTableArray, SeriNpcTable>("world/data/gym/gym_mizu_seri_npc/gym_mizu_seri_npc_array.bfbs", z => z.Table);
        Dump<SeriVenueTableArray, SeriVenueTable>("world/data/gym/gym_mizu_seri_venue/gym_mizu_seri_venue_array.bfbs", z => z.Table);
        DumpJson<GymMushiData>("world/data/gym/gym_mushi_data/gym_mushi_data.bfbs");
        Dump<GymMushiRewardArray, GymMushiReward>("world/data/gym/gym_mushi_reward/gym_mushi_reward_array.bfbs", z => z.Table);
    }

    private void DumpShopDress()
    {
        Dump<DressupStylePresetDataArray, DressupStylePresetData>("world/data/ui/dressup_preset/dressup_preset_data/dressup_preset_data_array.bfbs", z => z.Table);
        Dump<DressupStyleDataArray, DressupStyleData>("world/data/ui/dressup_style/dressup_style_data/dressup_style_data_array.bfbs", z => z.Table);
        Dump<DressupCategoryDataArray, DressupCategoryData>("world/data/ui/shop/dressup_shop/dressup_category_data/dressup_category_data_array.bfbs", z => z.Table);
        Dump<DressupItemDataArray, DressupItemData>("world/data/ui/shop/dressup_shop/dressup_item_data/dressup_item_data_array.bfbs", z => z.Table);
        Dump<DressupShopDataArray, DressupShopData>("world/data/ui/shop/dressup_shop/dressup_shop_data/dressup_shop_data_array.bfbs", z => z.Table);
        Dump<BufDataArray, BufData>("world/data/ui/shop/shop_restaurant/restaurant_bufdata/restaurant_bufdata_array.bfbs", z => z.Table);
        Dump<RestaurantMenuDataArray, RestaurantMenuData>("world/data/ui/shop/shop_restaurant/restaurant_menudata/restaurant_menudata_array.bfbs", z => z.Table);
        Dump<RestaurantShopDataArray, RestaurantShopData>("world/data/ui/shop/shop_restaurant/restaurant_shopdata/restaurant_shopdata_array.bfbs", z => z.Table);
        Dump<ShopWazamachineDataArray, ShopWazamachineData>("world/data/ui/shop/shop_wazamachine/shop_wazamachine_data/shop_wazamachine_data_array.bfbs", z => z.Table);
    }

    private void DumpField()
    {
        DumpJson<CoinSymbolManager>("world/data/field/coin_symbol/coin_symbol_manager/coin_symbol_manager_data.bfbs");

        DumpJson<GemSymbolSetting>("world/data/field/fixed_symbol/gem_symbol_setting/data.bfbs");
        DumpJson<FixedSymbolManager>("world/data/field/fixed_symbol/fixed_symbol_manager/fixed_symbol_manager_data.bfbs");
        Dump<GemSymbolLotteryTableArray, GemSymbolLotteryTable>("world/data/field/fixed_symbol/gem_symbol_lottery_table/gem_symbol_lottery_table_array.bfbs", z => z.Table);
        Dump<FixedSymbolTableArray, FixedSymbolTable>("world/data/field/fixed_symbol/fixed_symbol_table/fixed_symbol_table_array.bfbs", z => z.Table);

        Dump<ExclusionGroupArray, ExclusionGroup>("world/data/field/exclusion_area/exclusion_group_table/exclusion_group_table_array.bfbs", z => z.Table);

        DumpSel<FixedSymbolTableArray, PokeDataSymbol>("world/data/field/fixed_symbol/fixed_symbol_table/fixed_symbol_table_array.bfbs",
            z => z.Table.Select(x => x.Symbol), "sym");
        DumpSel<FixedSymbolTableArray, FixedSymbolAI>("world/data/field/fixed_symbol/fixed_symbol_table/fixed_symbol_table_array.bfbs",
        z => z.Table.Select(x => x.PokeAI), "ai");
        DumpSel<FixedSymbolTableArray, FixedSymbolGeneration>("world/data/field/fixed_symbol/fixed_symbol_table/fixed_symbol_table_array.bfbs",
        z => z.Table.Select(x => x.PokeGeneration), "gen");
    }

    private void DumpAjito()
    {
        Dump<AjitoCommonLevelArray, AjitoCommonLevel>("world/data/ajito/AjitoCommonLevel/AjitoCommonLevel_array.bfbs", z => z.Table);
        Dump<AjitoPokemonArray, AjitoPokemon>("world/data/ajito/AjitoPokemon/AjitoPokemon_array.bfbs", z => z.Table);
        Dump<AjitoUnitArray, AjitoUnit>("world/data/ajito/AjitoUnit/AjitoUnit_array.bfbs", z => z.Table);
        Dump<DanCarBattleDataArray, DanCarBattleData>("world/data/danbattle/boss/dan_car_battle_data/dan_car_battle_data_array.bfbs", z => z.Table);
    }

    private void DumpEncount()
    {
        Dump<TreeShakePokemonArray, TreeShakePokemon>("world/data/event/treeshake/treeshake_pokemon/treeshake_pokemon_array.bfbs", z => z.Table);
        Dump<PointDataArray, PointData>("world/data/encount/point_data/point_data/encount_data_atlantis.bfbs", z => z.Table);
        Dump<PointDataArray, PointData>("world/data/encount/point_data/point_data/encount_data_100000.bfbs", z => z.Table);
        Dump<PointDataArray, PointData>("world/data/encount/point_data/point_data/encount_data_su1.bfbs", z => z.Table);
        Dump<PointDataArray, PointData>("world/data/encount/point_data/point_data/encount_data_su2.bfbs", z => z.Table);
      //Dump<PointDataArray, PointData>("world/data/encount/point_data/point_data/encount_data_lc.bfbs", z => z.Table); -- no points (null array)
        Dump<OutbreakPointArray, OutbreakPointData>("world/data/encount/point_data/outbreak_point_data/outbreak_point_main.bfbs", z => z.Table);
        Dump<OutbreakPointArray, OutbreakPointData>("world/data/encount/point_data/outbreak_point_data/outbreak_point_su1.bfbs", z => z.Table);
        Dump<OutbreakPointArray, OutbreakPointData>("world/data/encount/point_data/outbreak_point_data/outbreak_point_su2.bfbs", z => z.Table);
        Dump<EncountPokeDataArray, EncountPokeData>("world/data/encount/pokedata/pokedata/pokedata_array.bfbs", z => z.Table);
        Dump<EncountPokeDataArray, EncountPokeData>("world/data/encount/pokedata/pokedata_su1/pokedata_su1_array.bfbs", z => z.Table);
        Dump<EncountPokeDataArray, EncountPokeData>("world/data/encount/pokedata/pokedata_su2/pokedata_su2_array.bfbs", z => z.Table);
        Dump<EncountPokeDataArray, EncountPokeData>("world/data/encount/pokedata/pokedata_lc/pokedata_lc_array.bfbs", z => z.Table);
        DumpJson<SettingData>("world/data/encount/setting/setting/data.bfbs");
        Dump<RaidDifficultyLotteryTableArray, RaidDifficultyLotteryTable>("world/data/encount/setting/raid_difficulty_lottery/raid_difficulty_lottery_array.bfbs", z => z.Table);
        DumpJson<RaidGemSetting>("world/data/encount/setting/raid_gem_setting/raid_gem_setting.bfbs");
        DumpJson<RaidGemSetting>("world/data/encount/setting/raid_gem_setting/su1_raid_gem_setting.bfbs");
        DumpJson<RaidGemSetting>("world/data/encount/setting/raid_gem_setting/su2_raid_gem_setting.bfbs");
        DumpJson<OutbreakSetting>("world/data/encount/setting/outbreak_setting/data.bfbs");
        Dump<FieldDungeonAreaArray, FieldDungeonArea>("world/data/field/area/field_dungeon_area/field_dungeon_area_array.bfbs", z => z.Table);
        Dump<FieldInsideAreaArray, FieldInsideArea>("world/data/field/area/field_inside_area/field_inside_area_array.bfbs", z => z.Table);
        Dump<FieldLocationArray, FieldLocation>("world/data/field/area/field_location/field_location_array.bfbs", z => z.Table);
        Dump<FieldMainAreaArray, FieldMainArea>("world/data/field/area/field_main_area/field_main_area_array.bfbs", z => z.Table);
        Dump<FieldSubAreaArray, FieldSubArea>("world/data/field/area/field_sub_area/field_sub_area_array.bfbs", z => z.Table);
        Dump<FieldDungeonAreaArray, FieldDungeonArea>("world/data/field/area_su1/field_dungeon_area_su1/field_dungeon_area_su1_array.bfbs", z => z.Table);
        Dump<FieldInsideAreaArray, FieldInsideArea>("world/data/field/area_su1/field_inside_area_su1/field_inside_area_su1_array.bfbs", z => z.Table);
        Dump<FieldLocationArray, FieldLocation>("world/data/field/area_su1/field_location_su1/field_location_su1_array.bfbs", z => z.Table);
        Dump<FieldMainAreaArray, FieldMainArea>("world/data/field/area_su1/field_main_area_su1/field_main_area_su1_array.bfbs", z => z.Table);
        Dump<FieldSubAreaArray, FieldSubArea>("world/data/field/area_su1/field_sub_area_su1/field_sub_area_su1_array.bfbs", z => z.Table);
        Dump<FieldDungeonAreaArray, FieldDungeonArea>("world/data/field/area_su2/field_dungeon_area_su2/field_dungeon_area_su2_array.bfbs", z => z.Table);
        Dump<FieldInsideAreaArray, FieldInsideArea>("world/data/field/area_su2/field_inside_area_su2/field_inside_area_su2_array.bfbs", z => z.Table);
        //Dump<FieldLocationArray, FieldLocation>("world/data/field/area_su2/field_location_su2/field_location_su2_array.bfbs", z => z.Table);
        Dump<FieldMainAreaArray, FieldMainArea>("world/data/field/area_su2/field_main_area_su2/field_main_area_su2_array.bfbs", z => z.Table);
        Dump<FieldSubAreaArray, FieldSubArea>("world/data/field/area_su2/field_sub_area_su2/field_sub_area_su2_array.bfbs", z => z.Table);

        const string gifts = "world/data/event/event_add_pokemon/eventAddPokemon/eventAddPokemon_array.bfbs";
        Dump<EventAddPokemonArray, EventAddPokemon>(gifts, z => z.Table);
        DumpSel<EventAddPokemonArray, PokeDataFull>(gifts, z => z.Table.Select(x => x.PokeData));

        for (int i = 13; i <= 37; i++)
            DumpJson<LegendPokeTable>($"world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_{i:000}_legend_poke.bfbs");

        Dump<ReleasePokeTableArray, ReleasePokeTable>("world/data/event/s2_sub_012_legend_poke/release_poke/release_poke_array.bfbs", z => z.Table);
    }

    private void DumpDLC()
    {
        Dump<DLCItemGroupArray, DLCItemGroup>("world/data/ui/dlc_item_data/dlc_item_group/dlc_item_group_array.bfbs", z => z.Table);
    }

    private void DumpJunk()
    {
        Dump<MapChangeParametersArray, MapChangeParameters>("world/data/field/map_change_common/map_change_common_array.bfbs", z => z.Table);
        Dump<Structures.FlatBuffers.SV.EventSub012.RewardTableArray, Structures.FlatBuffers.SV.EventSub012.RewardTable>("world/data/event/sub_012/sub_012_reward/sub_012_reward_array.bfbs", z => z.Table);
        DumpJson<FieldCamera>("world/data/field/playables/field_camera/battleData.bfbs");
        DumpJson<FieldCamera>("world/data/field/playables/field_camera/data.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/cam_t01_i01_01.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/interior_02.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/interior_03.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/interior_04.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/interior_05.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/sample.bfbs");
        DumpJson<FieldInteriorCamera>("world/data/field/playables/field_interior_camera/sample.bfbs");
        DumpJson<FieldPlayer>("world/data/field/playables/field_player/data.bfbs");
        DumpJson<FieldRide>("world/data/field/playables/field_ride/data.bfbs");
        Dump<AreaFlyFlagArray, AreaFlyFlag>("world/data/field/fly_area/area_fly_flag/area_fly_flag_array.bfbs", z => z.Table);
        Dump<EventSkyFlyArray, EventSkyFly>("world/data/ui/map_icon_data/ui_map_icon_data/ui_map_icon_data_array.bfbs", z => z.Table);
        Dump<NpcTrafficGenerateTableArray, NpcTrafficGenerateTable>("world/data/event/npc_traffic_generate/npc_traffic_generate_array.bfbs", z => z.Table);
        Dump<PopupWindowTableArray, PopupWindowTable>("world/data/event/field_popup/event_field_popup/event_field_popup_array.bfbs", z => z.Table);
        DumpJson<PicnicSystemData>("world/data/picnic/picnic_system/default_system_data.bfbs");
        DumpJson<PicnicTablesetData>("world/data/picnic/picnic_tableset_data/common.bfbs");
        DumpJson<PicnicWagonData>("world/data/picnic/picnic_wagon_data/common.bfbs");
        DumpJson<PicnicPokemon>("world/data/picnic/pokemon/picnic_pokemon/RIDE.bfbs");
        Dump<ObjectGenerationRangeArray, ObjectGenerationRange>("world/data/system/object_generation_range/object_generation_range_array.bfbs", z => z.Table);
        Dump<BattleBgmSelectTableArray, BattleBgmSelectTable>("world/data/ui/battle_bgm_select/battle_bgm_select_data/battle_bgm_select_data_array.bfbs", z => z.Table);
        Dump<DanQuestClearArray, DanQuestClear>("world/data/ui/quest_clear/dan_quest_clear/dan_quest_clear_array.bfbs", z => z.Table);
        Dump<GymQuestClearArray, GymQuestClear>("world/data/ui/quest_clear/gym_quest_clear/gym_quest_clear_array.bfbs", z => z.Table);
        Dump<NushiQuestClearArray, NushiQuestClear>("world/data/ui/quest_clear/nushi_quest_clear/nushi_quest_clear_array.bfbs", z => z.Table);
        Dump<SystemBgArray, SystemBg>("world/data/ui/system_bg/system_bg/system_bg_array.bfbs", z => z.Table);
        Dump<TipsDataArray, TipsData>("world/data/ui/tips/tips_data/tips_data_array.bfbs", z => z.Table);
        Dump<NpcDestinationDataTableArray, NpcDestinationDataTable>("world/data/ui/ymap/yamp_npc_navigation_data/yamp_npc_navigation_data_array.bfbs", z => z.Table);
        Dump<DestinationDataTableArray, DestinationDataTable>("world/data/ui/ymap/ymap_destination_data/ymap_destination_data_array.bfbs", z => z.Table);
        Dump<DestinationDataTableArray, DestinationDataTable>("world/data/ui/ymap/ymap_destination_dlc1_data/ymap_destination_dlc1_data_array.bfbs", z => z.Table);
        Dump<DestinationDataTableArray, DestinationDataTable>("world/data/ui/ymap/ymap_destination_dlc2_data/ymap_destination_dlc2_data_array.bfbs", z => z.Table);
        Dump<PlaceNameDataTableArray, PlaceNameDataTable>("world/data/ui/ymap/ymap_place_name_data/ymap_place_name_data_array.bfbs", z => z.Table);
        Dump<PlaceNameDataTableArray, PlaceNameDataTable>("world/data/ui/ymap/ymap_place_name_dlc1_data/ymap_place_name_dlc1_data_array.bfbs", z => z.Table);
        Dump<PlaceNameDataTableArray, PlaceNameDataTable>("world/data/ui/ymap/ymap_place_name_dlc2_data/ymap_place_name_dlc2_data_array.bfbs", z => z.Table);
    }

    public void DumpBalloon()
    {
        Dump<RewardActionTableArray, RewardActionTable>("world/data/oniballoon/reward_action_param/reward_action_param_array.bfbs", z => z.Table);
        Dump<ResultDemoTableArray, ResultDemoTable>("world/data/oniballoon/result_demo_param/result_demo_param_array.bfbs", z => z.Table);
        //Dump<PlaceNameDataTableArray, PlaceNameDataTable>("?? "FirstRewardParamArray" bfbs", z => z.Table);
        Dump<Structures.FlatBuffers.SV.Balloon.RewardTableArray, Structures.FlatBuffers.SV.Balloon.RewardTable> ("world/data/oniballoon/reward_level_param/reward_level_param_array.bfbs", z => z.Table);
        Dump<OjamaPokemonUniqueTableArray, OjamaPokemonUniqueTable>("world/data/oniballoon/ojama_pokemon_unique_param/ojama_pokemon_unique_param_array.bfbs", z => z.Table);
        Dump<GameBaseParamTableArray, GameBaseParamTable>("world/data/oniballoon/game_base_param/game_base_param_array.bfbs", z => z.Table);
        Dump<OjamaPokemonCommonTableArray, OjamaPokemonCommonTable>("world/data/oniballoon/ojama_pokemon_common_param/ojama_pokemon_common_param_array.bfbs", z => z.Table);
        Dump<ScoreBaseTableArray, ScoreBaseTable>("world/data/oniballoon/score_base_param/score_base_param_array.bfbs", z => z.Table);
    }

    public void DumpEncounters()
    {
        const string language = "English";

        var cfg = new TextConfig(GameVersion.SV);
        var ahtb = GetCommonAHTB("place_name", language);
        var place_names = GetCommonText("place_name", language, cfg);
        var nameDict = EncounterDumperSV.GetInternalStringLookup(place_names, ahtb);

        var config = new EncounterDumpConfigSV
        {
            PlaceNameMap = nameDict,
            SpecNamesInternal = GetCommonText("monsname", language, cfg),
            MoveNames = GetCommonText("wazaname", language, cfg),
            Path = GetPath("encounters"),
        };
        EncounterDumperSV.Dump(rom, config);
    }

    public void DumpRaid()
    {
        Dump<DeliveryRaidFixedRewardItemArray, DeliveryRaidFixedRewardItem>("world/data/raid/delivery_fixed_reward_item/delivery_fixed_reward_item_array.bfbs", z => z.Table);
        Dump<DeliveryRaidLotteryRewardItemArray, DeliveryRaidLotteryRewardItem>("world/data/raid/delivery_lottery_reward_item/delivery_lottery_reward_item_array.bfbs", z => z.Table);
        Dump<DeliveryRaidEnemyTableArray, DeliveryRaidEnemyTable>("world/data/raid/delivery_raid_enemy/delivery_raid_enemy_array.bfbs", z => z.Table);
        Dump<DeliveryRaidPriorityArray, DeliveryRaidPriority>("world/data/raid/delivery_raid_priority/delivery_raid_priority_array.bfbs", z => z.Table);
        Dump<RaidFixedRewardItemArray, RaidFixedRewardItem>("world/data/raid/raid_fixed_reward_item/raid_fixed_reward_item_array.bfbs", z => z.Table);
        Dump<RaidLotteryRewardItemArray, RaidLotteryRewardItem>("world/data/raid/raid_lottery_reward_item/raid_lottery_reward_item_array.bfbs", z => z.Table);
        Dump<RaidRewardSlotArray, RaidRewardSlot>("world/data/raid/raid_lottery_reward_slot/raid_lottery_reward_slot_array.bfbs", z => z.Table);
        Dump<RaidPokeScaleDataArray, RaidPokeScaleData>("world/data/raid/raid_poke_scale/raid_poke_scale_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_01/raid_trainer_01_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_02/raid_trainer_02_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_03/raid_trainer_03_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_04/raid_trainer_04_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_05/raid_trainer_05_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_06/raid_trainer_06_array.bfbs", z => z.Table);
        Dump<RaidTrainerArray, RaidTrainer>("world/data/raid/raid_trainer_07/raid_trainer_07_array.bfbs", z => z.Table);
        DumpJson<RaidGemItemRewardBoost>("world/data/raid/raid_gem_item_reward_boost/data.bfbs");

        var mainRaids = new[]
        {
            "world/data/raid/raid_enemy_01/raid_enemy_01_array.bfbs",
            "world/data/raid/raid_enemy_02/raid_enemy_02_array.bfbs",
            "world/data/raid/raid_enemy_03/raid_enemy_03_array.bfbs",
            "world/data/raid/raid_enemy_04/raid_enemy_04_array.bfbs",
            "world/data/raid/raid_enemy_05/raid_enemy_05_array.bfbs",
            "world/data/raid/raid_enemy_06/raid_enemy_06_array.bfbs",
        };
        var su1Raids = new[]
        {
            "world/data/raid/su1_raid_enemy_01/su1_raid_enemy_01_array.bfbs",
            "world/data/raid/su1_raid_enemy_02/su1_raid_enemy_02_array.bfbs",
            "world/data/raid/su1_raid_enemy_03/su1_raid_enemy_03_array.bfbs",
            "world/data/raid/su1_raid_enemy_04/su1_raid_enemy_04_array.bfbs",
            "world/data/raid/su1_raid_enemy_05/su1_raid_enemy_05_array.bfbs",
            "world/data/raid/su1_raid_enemy_06/su1_raid_enemy_06_array.bfbs",
        };
        var su2Raids = new[]
        {
            "world/data/raid/su2_raid_enemy_01/su2_raid_enemy_01_array.bfbs",
            "world/data/raid/su2_raid_enemy_02/su2_raid_enemy_02_array.bfbs",
            "world/data/raid/su2_raid_enemy_03/su2_raid_enemy_03_array.bfbs",
            "world/data/raid/su2_raid_enemy_04/su2_raid_enemy_04_array.bfbs",
            "world/data/raid/su2_raid_enemy_05/su2_raid_enemy_05_array.bfbs",
            "world/data/raid/su2_raid_enemy_06/su2_raid_enemy_06_array.bfbs",
        };
        foreach (var f in (string[])[.. mainRaids, .. su1Raids, .. su2Raids])
        {
            Dump<RaidEnemyTableArray, RaidEnemyTable>(f, z => z.Table);
            static IEnumerable<PokeDataBattle> sel(RaidEnemyTableArray z) => z.Table.Select(x => x.Info.BossPokePara);
            DumpSel<RaidEnemyTableArray, PokeDataBattle>(f, sel);
        }

        var outPath = GetPath("pkhex");
        TeraRaidRipper.DumpRaids(rom, mainRaids, outPath, "raidTotal.txt", "encounter_gem_paldea.pkl");
        TeraRaidRipper.DumpRaids(rom, su1Raids, outPath, "raidTotalKitakami.txt", "encounter_gem_kitakami.pkl");
        TeraRaidRipper.DumpRaids(rom, su2Raids, outPath, "raidTotalBlueberry.txt", "encounter_gem_blueberry.pkl");
    }

    public void DumpMoves()
    {
        const string wazaPath = "avalon/data/waza_array.bin";
        Dump<WazaTable, Waza>(wazaPath, z => z.Table);
        var table = Get<WazaTable>(wazaPath);
        var outPath = GetPath("pkhex");

        var pp = table.Table.Select(z => z.PP).Select(z => $"{z:00}");
        File.WriteAllText(Path.Combine(outPath, "move_pp.txt"), string.Join(',', pp));

        var types = table.Table.Select(z => z.Type).Select(z => $"{z:00}");
        File.WriteAllText(Path.Combine(outPath, "move_type.txt"), string.Join(',', types));
    }

    public void DumpAbilities()
    {
        const string tokuseiPath = "avalon/data/tokusei_array.bin";
        Dump<TokuseiTable, Tokusei>(tokuseiPath, z => z.Table);
    }

    private void DumpItemMachine()
    {
        Dump<ItemMachineItemTableArray, ItemMachineItemTable>("world/data/ui/item_machine/item_table/item_table_array.bfbs", z => z.Table);
        Dump<ItemMachineLotteryTableArray, ItemMachineLotteryTable>("world/data/ui/item_machine/lottery_rate/lottery_rate_array.bfbs", z => z.Table);
        Dump<ItemMachineSpecialTableArray, ItemMachineSpecialTable>("world/data/ui/item_machine/special_item_table/special_item_table_array.bfbs", z => z.Table);
        Dump<EffectTableArray, EffectTable>("world/data/ui/item_machine/effect_table/effect_table_array.bfbs", z => z.Table);
        Dump<MaterialBaseParamArray, MaterialBaseParam>("world/data/ui/item_machine/base_param/base_param_array.bfbs", z => z.Table);
    }

    private void DumpItems()
    {
        Dump<RummagingItemDataTableArray, RummagingItemDataTable>("world/data/item/rummagingItemDataTable/rummagingItemDataTable_array.bfbs", z => z.Table);
        Dump<ItemPointTypeBiomeTableArray, ItemPointTypeBiomeTable>("world/data/item/itemPointTypeBiomeTable/itemPointTypeBiomeTable_array.bfbs", z => z.Table);
        Dump<HiddenItemBiomeTableArray, HiddenItemBiomeTable>("world/data/item/hiddenItemBiomeTable/hiddenItemBiomeTable_array.bfbs", z => z.Table);
        Dump<HiddenItemBiomeTableArray, HiddenItemBiomeTable>("world/data/item/hiddenItemBiomeTable_su1/hiddenItemBiomeTable_su1_array.bfbs", z => z.Table);
        Dump<HiddenItemBiomeTableArray, HiddenItemBiomeTable>("world/data/item/hiddenItemBiomeTable_su2/hiddenItemBiomeTable_su2_array.bfbs", z => z.Table);
        Dump<HiddenItemBiomeTableArray, HiddenItemBiomeTable>("world/data/item/hiddenItemBiomeTable_lc/hiddenItemBiomeTable_lc_array.bfbs", z => z.Table);
        Dump<DropItemDataArray, DropItemData>("world/data/item/dropitemdata/dropitemdata_array.bfbs", z => z.Table);
        Dump<ItemDataArray, ItemData>("world/data/item/itemdata/itemdata_array.bfbs", z => z.Table);
        Dump<HiddenItemDataTableArray, HiddenItemDataTable>("world/data/item/hiddenItemDataTable/hiddenItemDataTable_array.bfbs", z => z.Table);
        Dump<HiddenItemDataTableArray, HiddenItemDataTable>("world/data/item/hiddenItemDataTable_su1/hiddenItemDataTable_su1_array.bfbs", z => z.Table);
        Dump<HiddenItemDataTableArray, HiddenItemDataTable>("world/data/item/hiddenItemDataTable_su2/hiddenItemDataTable_su2_array.bfbs", z => z.Table);
        Dump<HiddenItemDataTableArray, HiddenItemDataTable>("world/data/item/hiddenItemDataTable_lc/hiddenItemDataTable_lc_array.bfbs", z => z.Table);
        Dump<MonohiroiItemArray, MonohiroiItem>("world/data/item/monohiroiItemData/monohiroiItemData_array.bfbs", z => z.Table);
    }

    private void DumpAudio()
    {
        DumpJson<EnvPokeVoiceLotterySetting>("audio/fb/env_poke_voice/env_poke_voice_lottery_settings/env_poke_voice_lottery_settings_data.bfbs");
        Dump<BGMEventArray, BGMEvent>("audio/fb/bgm/bgm_event/bgm_event_array.bfbs", z => z.Table);
    }

    private void DumpUniquePath()
    {
        Dump<ConditionSimpleAutoBattleHecklerAreaArray, ConditionSimpleAutoBattleHecklerArea>("world/data/pokemon/ai_action/trigger/condition_simple_auto_battle_heckler_area/condition_simple_auto_battle_heckler_area_array.bfbs", z => z.Table);
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/partner_movement_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/partner_pokemon_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/pokemon_collision_unique_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/pokemon_common_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/pokemon_contact_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/pokemon_species_behavior_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/pokemon_species_table.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/pokemon_stain_unique_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/wild_pokemon_group_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/wild_pokemon_single_data.bfbs");
        DumpJson<PokemonUniquePathData>("world/data/pokemon/pokemon_unique_path_data/wild_pokemon_special_data.bfbs");
        Dump<PokeObjArray, PokeObj>("world/data/pokeobj/pokeobj_param/pokeobj_param_array.bfbs", z => z.Table);
    }

    private void DumpFieldReturn()
    {
        Dump<FieldAutomaticReturnTableArray, FieldAutomaticReturnTable>("world/data/field/area_range/field_auto_return_data/field_auto_return_data_array.bfbs", z => z.Table);
        Dump<FieldAutoReturnPosTableArray, FieldAutoReturnPosTable>("world/data/field/area_range/field_auto_return_pos/field_auto_return_pos_array.bfbs", z => z.Table);
        Dump<FieldOutOfRangeTableArray, FieldOutOfRangeTable>("world/data/field/area_range/field_out_of_range_data/field_out_of_range_data_array.bfbs", z => z.Table);
    }

    private void DumpGem()
    {
        Dump<GemSettingRootTableArray, GemSettingRootTable>("world/data/gem/gem/gem_array.bfbs", z => z.Table);
    }

    private void DumpBattle()
    {
        Dump<ItemTableArray, ItemTable>("world/data/battle/plib_item_conversion/plib_item_conversion_array.bfbs", z => z.Table);
        Dump<PokeExceptionTableArray, PokeExceptionTable>("world/data/battle/pokeExceptionTable/pokeExceptionTable_array.bfbs", z => z.Table);
        DumpJson<BattleConfig>("world/data/battle/battleconfig/battleconfig.bfbs");
        Dump<EventBattlePokemonArray, EventBattlePokemon>("world/data/battle/eventBattlePokemon/eventBattlePokemon_array.bfbs", z => z.Table);
        Dump<BattleEffectTableArray, BattleEffectTable>("world/data/battle/battleeffecttable/battleeffecttable_array.bfbs", z => z.Table);

        DumpSel<EventBattlePokemonArray, PokeDataEventBattle>("world/data/battle/eventBattlePokemon/eventBattlePokemon_array.bfbs",
            z => z.Table.Select(x => x.PokeData));
    }

    private void DumpPokeDex()
    {
        Dump<BlacklistArray, Blacklist>("world/data/ui/pokedex/blacklist/blacklist_array.bfbs", z => z.Table);
        Dump<BlacklistArray, Blacklist>("world/data/ui/pokedex/blacklist_dlc1/blacklist_dlc1_array.bfbs", z => z.Table);
        Dump<BlacklistArray, Blacklist>("world/data/ui/pokedex/blacklist_dlc2/blacklist_dlc2_array.bfbs", z => z.Table);

        Dump<MemoPokeTableArray, MemoPokeTable>("world/data/ui/pokedex/memo_poke_data/memo_poke_data_array.bfbs", z => z.Table);
        Dump<MemoPokeTableArray, MemoPokeTable>("world/data/ui/pokedex/memo_poke_data_dlc1/memo_poke_data_dlc1_array.bfbs", z => z.Table);
        Dump<MemoPokeTableArray, MemoPokeTable>("world/data/ui/pokedex/memo_poke_data_dlc2/memo_poke_data_dlc2_array.bfbs", z => z.Table);

        Dump<RewardDataArray, RewardData>("world/data/ui/pokedex/reward_data/reward_data_array.bfbs", z => z.Table);
        Dump<RewardDataArray, RewardData>("world/data/ui/pokedex/reward_data_dlc1/reward_data_dlc1_array.bfbs", z => z.Table);
        Dump<RewardDataArray, RewardData>("world/data/ui/pokedex/reward_data_dlc2/reward_data_dlc2_array.bfbs", z => z.Table);

        DumpX<DistributionRootArray, DistributionRoot, DistributionData>("world/data/ui/pokedex/distribution_data/distribution_data_array.bfbs", z => z.Table, z => z.Table);
        DumpX<DistributionRootArray, DistributionRoot, DistributionData>("world/data/ui/pokedex/distribution_data_dlc1/distribution_data_dlc1_array.bfbs", z => z.Table, z => z.Table);
        DumpX<DistributionRootArray, DistributionRoot, DistributionData>("world/data/ui/pokedex/distribution_data_dlc2/distribution_data_dlc2_array.bfbs", z => z.Table, z => z.Table);
    }

    private void DumpUI()
    {
        Dump<ShopDataArray, ShopData>("world/data/ui/shop/shop_data/shop_data_array.bfbs", z => z.Table);
        Dump<RibbonDataArray, RibbonData>("world/data/ui/status/ribbon/ribbon_array.bfbs", z => z.Table);

        Dump<FriendlyShopDataArray, FriendlyShopData>("world/data/ui/shop/friendlyshop/friendlyshop_data/friendlyshop_data_array.bfbs", z => z.Table);
        Dump<LineupDataArray, LineupData>("world/data/ui/shop/friendlyshop/friendlyshop_lineup_data/friendlyshop_lineup_data_array.bfbs", z => z.Table);
        Dump<SchoolMapDataArray, SchoolMapData>("world/data/ui/schoolmap/schoolmap_data/schoolmap_data_array.bfbs", z => z.Table);
    }

    private void DumpNetBattle()
    {
        Dump<NetBattleRuleDataArray, NetBattleRuleData>("world/data/ui/net_battle/net_battle_rule_data/net_battle_rule_data_array.bfbs", z => z.Table);
        Dump<NetBattleRuleParamArray, NetBattleRuleParam>("world/data/ui/net_battle/net_battle_rule_param/net_battle_rule_param_array.bfbs", z => z.Table);
    }

    public void DumpTrainers()
    {
        Dump<TrDataMainArray, TrDataMain>("world/data/trainer/trdata/trdata_array.bfbs", z => z.Table);
        Dump<TrainerEnvArray, TrainerEnv>("world/data/trainer/trenv/trenv_array.bfbs", z => z.Table);
        Dump<TrainerTypeArray, TrainerType>("world/data/trainer/trtype/trtype_array.bfbs", z => z.Table);
        DumpX<TrDataMainArray, TrDataMain, PokeDataBattle>("world/data/trainer/trdata/trdata_array.bfbs", z => z.Table, z => [z.Poke1, z.Poke2, z.Poke3, z.Poke4, z.Poke5, z.Poke6]);
    }

    public void DumpCooking()
    {
        const string fpc = "world/data/cooking/FoodPowerCombo/FoodPowerCombo_array.bfbs";
        const string ipc = "world/data/cooking/Ingredient/IngredientData/IngredientData_array.bfbs";
        DumpJson<CookingCommonData>("world/data/cooking/CookingCommonData/cooking_common_data.bfbs");
        Dump<FoodPowerComboArray, FoodPowerCombo>(fpc, z => z.Table);
        Dump<IngredientDataArray, IngredientData>(ipc, z => z.Table);
        Dump<IngredientDishDataArray, IngredientDishData>("world/data/cooking/Ingredient/IngredientDishData/IngredientDishData_array.bfbs", z => z.Table);
        Dump<RecipeDataArray, RecipeData>("world/data/cooking/Recipe/RecipeData/RecipeData_array.bfbs", z => z.Table);
        Dump<SeasoningDataArray, SeasoningData>("world/data/cooking/Seasoning/SeasoningData/SeasoningData_array.bfbs", z => z.Table);
        Dump<TakaraSpicePowerTableArray, TakaraSpicePowerTable>("world/data/cooking/TakaraSpicePowerTable/TakaraSpicePowerTable_array.bfbs", z => z.Table);

        DumpSel<FoodPowerComboArray, FoodPowerComboParam>(fpc, z => z.Table.Select(x => x.Param));
        DumpSel<FoodPowerComboArray, FoodPowerParam>     (fpc, z => z.Table.Select(x => x.Param.FoodPower), "effect");
        DumpSel<FoodPowerComboArray, FoodPokeTypeParam>  (fpc, z => z.Table.Select(x => x.Param.PokeTypePower), "type");

        DumpSel<IngredientDataArray, IngredientParam>  (ipc, z => z.Table.Select(x => x.Param), "flavor");
        DumpSel<IngredientDataArray, FoodPowerParam>   (ipc, z => z.Table.Select(x => x.Power), "effect");
        DumpSel<IngredientDataArray, FoodPokeTypeParam>(ipc, z => z.Table.Select(x => x.PokeTypePower), "type");

        // Combining ingredients and seasonings fills a weight table, and the game picks boosts "randomly" from the weights.
        // Double Herba seems like it guarantees the first effect boost is Shiny, and heavily favors Marks due to Herba boost.
        // Herba boosts heavily bias all stats, so the ingredients and seasoning weights get watered down to effectively be random type favored.
        // 2 effect (power) boosts, 1 type boost.
        // Sandwich stars determine level?

        // Visualize the FoodPowerCombo data -- seems like it's just stupid random sandwiches for laughs.
        var config = new TextConfig(GameVersion.SV);
        foreach (var lang in LanguageEnglishNames)
        {
            var types = GetCommonText("typename", lang, config);
            var data = rom.GetPackedFile("world/data/cooking/FoodPowerCombo/FoodPowerCombo_array.bin");
            var fb = FlatBufferConverter.DeserializeFrom<FoodPowerComboArray>(data);
            var table = fb.Table;
            var path = GetPath(lang, "foodCombo.txt");
            using var ts = File.CreateText(path);
            foreach (var entry in table)
            {
                var param = entry.Param;
                // Write the ingredients in a single line.
                if (param.Ingredient(0) != IngredientType.NONE) ts.Write($"{param.Ingredient(0)}, ");
                if (param.Ingredient(1) != IngredientType.NONE) ts.Write($"{param.Ingredient(1)}, ");
                if (param.Ingredient(2) != IngredientType.NONE) ts.Write($"{param.Ingredient(2)}, ");
                if (param.Ingredient(3) != IngredientType.NONE) ts.Write($"{param.Ingredient(3)}, ");
                if (param.Ingredient(4) != IngredientType.NONE) ts.Write($"{param.Ingredient(4)}, ");
                if (param.Ingredient(5) != IngredientType.NONE) ts.Write($"{param.Ingredient(5)}, ");
                ts.WriteLine();
                // Write the seasonings in a single line.
                if (param.Seasoning(0) != SeasoningType.NONE) ts.Write($"{param.Seasoning(0)}, ");
                if (param.Seasoning(1) != SeasoningType.NONE) ts.Write($"{param.Seasoning(1)}, ");
                if (param.Seasoning(2) != SeasoningType.NONE) ts.Write($"{param.Seasoning(2)}, ");
                if (param.Seasoning(3) != SeasoningType.NONE) ts.Write($"{param.Seasoning(3)}, ");
                ts.WriteLine();

                var boost = param.FoodPower;
                // For each property in the object, write a line if non-zero.
                if (boost.EGG != 0) ts.WriteLine($"Egg: {boost.EGG}");
                if (boost.CAPTURE != 0) ts.WriteLine($"Capture: {boost.CAPTURE}");
                if (boost.EXP != 0) ts.WriteLine($"EXP: {boost.EXP}");
                if (boost.LOSTPROPERTY != 0) ts.WriteLine($"Drops: {boost.LOSTPROPERTY}");
                if (boost.RAID != 0) ts.WriteLine($"Raid: {boost.RAID}");
                if (boost.ANOTHERNAME != 0) ts.WriteLine($"Mark: {boost.ANOTHERNAME}");
                if (boost.RARE != 0) ts.WriteLine($"Shiny: {boost.RARE}");
                if (boost.GIGANT != 0) ts.WriteLine($"Jumbo: {boost.GIGANT}");
                if (boost.MIINIMUM != 0) ts.WriteLine($"Tiny: {boost.MIINIMUM}");
                if (boost.ENCOUNT != 0) ts.WriteLine($"Encounter: {boost.ENCOUNT}");

                // Write a type if non-zero.
                var type = param.PokeTypePower;
                for (int i = 0; i <= (int)MoveType.Fairy; i++)
                {
                    var amount = type.GetBoostFromIndex(i);
                    if (amount != 0)
                        ts.WriteLine($"{types[i]}: {amount}");
                }
                ts.WriteLine();
            }
        }
    }

    private void DumpTrades()
    {
        Dump<EventTradeListArray, EventTradeList>("world/data/event/eventTradeList/eventTradeList_array.bfbs", z => z.Table);
        Dump<EventTradePokemonArray, EventTradePokemon>("world/data/event/eventTradePokemon/eventTradePokemon_array.bfbs", z => z.Table);
        DumpSel<EventTradePokemonArray, PokeDataTrade>("world/data/event/eventTradePokemon/eventTradePokemon_array.bfbs",
            z => z.Table.Select(x => x.PokeData));
    }

    private void DumpClubroom()
    {
        Dump<MissionInfosArray, MissionInfos>("world/data/club/MissionInfo/MissionInfo_array.bfbs", z => z.Table);
        Dump<MissionSettingsArray, MissionSettings>("world/data/club/MissionSettings/MissionSettings_array.bfbs", z => z.Table);
        Dump<ClubRoomBoardInfoArray, ClubRoomBoardInfo>("world/data/club/ClubRoomBoardInfo/ClubRoomBoardInfo_array.bfbs", z => z.Table);
        Dump<ClubNpcRewardListArray, ClubNpcRewardList>("world/data/club/ClubNpcRewardList/ClubNpcRewardList_array.bfbs", z => z.Table);
        DumpJson<MetamonBlockInfo>("world/data/club/MetamonBlockInfo/metamonBlockInfoData.bfbs");
        Dump<ClubRoomNpcPairTableArray, ClubRoomNpcPairTable>("world/data/club/ClubRoomNpcPairTable/ClubRoomNpcPairTable_array.bfbs", z => z.Table);
    }

    private void Dump<TTable, TEntry>(ulong hash, Func<TTable, IList<TEntry>> sel)
        where TTable : class, IFlatBufferSerializable<TTable>
        where TEntry : notnull
    {
        var flat = Get<TTable>(hash);
        var path = GetPath(hash.ToString("X16"));
        Dump(sel, flat, path);
    }

    private void Dump<TTable, TEntry>(string f, Func<TTable, IList<TEntry>> sel)
        where TTable : class, IFlatBufferSerializable<TTable>
        where TEntry : notnull
    {
        var flat = Get<TTable>(f);
        var path = GetPath("raw", f.Replace('/', Path.DirectorySeparatorChar));
        Dump(sel, flat, path);
    }

    private static void Dump<TTable, TEntry>(Func<TTable, IList<TEntry>> sel, TTable flat, string path)
        where TTable : class, IFlatBufferSerializable<TTable> where TEntry : notnull
    {
        DumpJson(flat, path);
        var table = sel(flat);
        var dump = TableUtil.GetTable(table);

        var fileName = Path.ChangeExtension(path, ".txt");
        File.WriteAllText(fileName, dump);
    }

    private TTable Get<TTable>(ulong hash)
        where TTable : class, IFlatBufferSerializable<TTable>
    {
        var bin = rom.GetPackedFile(hash);
        return Get<TTable>(bin);
    }

    private TTable Get<TTable>(string f)
        where TTable : class, IFlatBufferSerializable<TTable>
    {
        var bin = rom.GetPackedFile(f.Replace("bfbs", "bin"));
        return Get<TTable>(bin);
    }

    private static TTable Get<TTable>(Memory<byte> bin) where TTable : class, IFlatBufferSerializable<TTable> => FlatBufferConverter.DeserializeFrom<TTable>(bin);

    private void DumpSel<TTable, TEntry>(string f, Func<TTable, IEnumerable<TEntry>> sel, string prefix = "sel")
        where TTable : class, IFlatBufferSerializable<TTable>
        where TEntry : notnull
    {
        var bin = rom.GetPackedFile(f.Replace("bfbs", "bin"));
        var path = GetPath("raw", f.Replace('/', Path.DirectorySeparatorChar));
        DumpSel(sel, prefix, bin, path);
    }

    private static void DumpSel<TTable, TEntry>(Func<TTable, IEnumerable<TEntry>> sel, string prefix, Memory<byte> bin, string path)
        where TTable : class, IFlatBufferSerializable<TTable> where TEntry : notnull
    {
        var flat = FlatBufferConverter.DeserializeFrom<TTable>(bin);
        var table = sel(flat);
        var dump = TableUtil.GetTable(table);

        var fileName = Path.ChangeExtension(path, $".{prefix}.txt");
        File.WriteAllText(fileName, dump);
    }

    private void DumpX<TTable, TEntry, TSub>(string f, Func<TTable, IList<TEntry>> sel, Func<TEntry, IList<TSub>> sel2)
        where TTable : class, IFlatBufferSerializable<TTable>
        where TEntry : class
        where TSub : notnull
    {
        var bin = rom.GetPackedFile(f.Replace("bfbs", "bin"));
        var flat = FlatBufferConverter.DeserializeFrom<TTable>(bin);

        var path = GetPath("raw", f.Replace('/', Path.DirectorySeparatorChar));
        DumpJson(flat, path);

        var arr = sel(flat);
        for (int i = 0; i < arr.Count; i++)
        {
            var entry = sel2(arr[i]);
            var dump = TableUtil.GetTable(entry);

            var folder = Path.GetDirectoryName(path);
            var fileName = Path.Combine(folder!, $"{typeof(TEntry).Name}_{i}.txt");
            File.WriteAllText(fileName, dump);
        }
    }

    private void DumpJson<T>(string f) where T : class, IFlatBufferSerializable<T>
    {
        var bin = rom.GetPackedFile(f.Replace("bfbs", "bin"));
        var flat = FlatBufferConverter.DeserializeFrom<T>(bin);

        var path = GetPath("raw", f.Replace('/', Path.DirectorySeparatorChar));
        DumpJson(flat, path);
    }

    private static void DumpJson<T>(T flat, string filePath) where T : class
    {
        var opt = new System.Text.Json.JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
        var json = System.Text.Json.JsonSerializer.Serialize(flat, opt);

        var fileName = Path.ChangeExtension(filePath, ".json");
        File.WriteAllText(fileName, json);
    }

    public void DumpSpecific()
    {
        var files = new[]
        {
            "world/data/pokemon/ai/pokemon_species_behavior_data/0025_00_00.bfbs",
            "world/data/pokemon/ai/pokemon_species_table/0025_00_00.bfbs",
            "world/data/pokemon/collision/pokemon_collision_unique_data/0025_00_00.bfbs",
            "world/data/pokemon/partner/partner_movement_data/0025_00_00.bfbs",
            "world/data/pokemon/partner/partner_pokemon_data/0025_00_00.bfbs",
            "world/data/pokemon/pokemon_common_data/0025_00_00.bfbs",
            "world/data/pokemon/pokemon_contact_data/0025_00_00.bfbs",
            "world/data/pokemon/stain/pokemon_stain_unique_data/0025_00_00.bfbs",
            "world/data/pokemon/wild/wild_pokemon_group_data/0025_00_00.bfbs",
            "world/data/pokemon/wild/wild_pokemon_single_data/0025_00_00.bfbs",
            "world/data/pokemon/wild/wild_pokemon_special_data/0025_00_00.bfbs",

            "world/data/ui/item_machine/item_table/item_table_array.bfbs",
            "world/data/ui/item_machine/lottery_rate/lottery_rate_array.bfbs",
            "world/data/ui/item_machine/special_item_table/special_item_table_array.bfbs",
            "world/data/ui/item_machine/effect_table/effect_table_array.bfbs",
            "world/data/ui/item_machine/base_param/base_param_array.bfbs",

            // Delivery Outbreak
            "world/data/encount/delivery_outbreak/delivery_outbreak_pokedata/delivery_outbreak_pokedata_array.bfbs",
            "world/data/encount/delivery_outbreak/delivery_outbreak_zone_main/delivery_outbreak_zone_main_array.bfbs",
            "world/data/encount/delivery_outbreak/delivery_outbreak_zone_su1/delivery_outbreak_zone_su1_array.bfbs",
            "world/data/encount/delivery_outbreak/delivery_outbreak_zone_su2/delivery_outbreak_zone_su2_array.bfbs",

            "world/data/oniballoon/reward_action_param/reward_action_param_array.bfbs",
            "world/data/oniballoon/result_demo_param/result_demo_param_array.bfbs",
            //"?? "FirstRewardParamArray" bfbs",
            "world/data/oniballoon/reward_level_param/reward_level_param_array.bfbs",
            "world/data/oniballoon/ojama_pokemon_unique_param/ojama_pokemon_unique_param_array.bfbs",
            "world/data/oniballoon/game_base_param/game_base_param_array.bfbs",
            "world/data/oniballoon/ojama_pokemon_common_param/ojama_pokemon_common_param_array.bfbs",
            "world/data/oniballoon/score_base_param/score_base_param_array.bfbs",

            "world/data/ui/dlc_item_data/dlc_item_group/dlc_item_group_array.bfbs",
            "world/data/fieldwork/fieldwork_bear_poke/fieldwork_bear_poke_array.bfbs",

            "world/data/gym/gym_denki_popup_fix/gym_denki_popup_fix_array.bfbs",
            "world/data/gym/gym_denki_popup_pos/gym_denki_popup_pos_array.bfbs",
            "world/data/gym/gym_esper_reward_endless/gym_esper_reward_endless_array.bfbs",
            "world/data/gym/gym_esper_reward_exercise/gym_esper_reward_exercise_array.bfbs",
            "world/data/gym/gym_koori_course/gym_koori_course_array.bfbs",
            "world/data/gym/gym_koori_course_pokemon/gym_koori_course_pokemon_base.bfbs",
            "world/data/gym/gym_kusa_poke/gym_kusa_poke_array.bfbs",
            "world/data/gym/gym_kusa_reward/gym_kusa_reward_array.bfbs",
            "world/data/gym/gym_mizu_seri_fix/gym_mizu_seri_fix_array.bfbs",
            "world/data/gym/gym_mizu_seri_item/gym_mizu_seri_item_array.bfbs",
            "world/data/gym/gym_mizu_seri_npc/gym_mizu_seri_npc_array.bfbs",
            "world/data/gym/gym_mizu_seri_venue/gym_mizu_seri_venue_array.bfbs",
            "world/data/gym/gym_mushi_data/gym_mushi_data.bfbs",
            "world/data/gym/gym_mushi_reward/gym_mushi_reward_array.bfbs",

            "audio/fb/env_poke_voice/env_poke_voice_lottery_settings/env_poke_voice_lottery_settings_data.bfbs",
            "audio/fb/bgm/bgm_event/bgm_event_array.bfbs",
            "world/data/battle/plib_item_conversion/plib_item_conversion_array.bfbs",
            "world/data/battle/pokeExceptionTable/pokeExceptionTable_array.bfbs",
            "world/data/pokemon/ai_action/trigger/condition_simple_auto_battle_heckler_area/condition_simple_auto_battle_heckler_area_array.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/partner_movement_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/partner_pokemon_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/pokemon_collision_unique_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/pokemon_common_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/pokemon_contact_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/pokemon_species_behavior_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/pokemon_species_table.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/pokemon_stain_unique_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/wild_pokemon_group_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/wild_pokemon_single_data.bfbs",
            "world/data/pokemon/pokemon_unique_path_data/wild_pokemon_special_data.bfbs",
            "world/data/pokeobj/pokeobj_param/pokeobj_param_array.bfbs",
            // Trainers
            "world/data/trainer/trdata/trdata_array.bfbs",
            "world/data/trainer/trenv/trenv_array.bfbs",
            "world/data/trainer/trtype/trtype_array.bfbs",
            // Encounters
            "world/data/encount/point_data/point_data/encount_data_atlantis.bfbs",
            "world/data/encount/point_data/point_data/encount_data_100000.bfbs",
            "world/data/encount/point_data/point_data/encount_data_su1.bfbs",
            "world/data/encount/point_data/point_data/encount_data_su2.bfbs",
            "world/data/encount/point_data/point_data/encount_data_lc.bfbs",
            "world/data/encount/point_data/outbreak_point_data/outbreak_point_main.bfbs",
            "world/data/encount/point_data/outbreak_point_data/outbreak_point_su1.bfbs",
            "world/data/encount/point_data/outbreak_point_data/outbreak_point_su2.bfbs",
            "world/data/encount/pokedata/pokedata/pokedata_array.bfbs",
            "world/data/encount/pokedata/pokedata_su1/pokedata_su1_array.bfbs",
            "world/data/encount/pokedata/pokedata_su2/pokedata_su2_array.bfbs",
            "world/data/encount/pokedata/pokedata_lc/pokedata_lc_array.bfbs",
            "world/data/encount/setting/setting/data.bfbs",
            "world/data/encount/setting/raid_difficulty_lottery/raid_difficulty_lottery_array.bfbs",
            "world/data/encount/setting/raid_gem_setting/raid_gem_setting.bfbs",
            "world/data/encount/setting/raid_gem_setting/su1_raid_gem_setting.bfbs",
            "world/data/encount/setting/raid_gem_setting/su2_raid_gem_setting.bfbs",
            "world/data/encount/setting/outbreak_setting/data.bfbs",
            "world/data/event/treeshake/treeshake_pokemon/treeshake_pokemon_array.bfbs",
            "world/data/event/npc_management/npc_management_s1_sub_003_always/npc_management_s1_sub_003_always_array.bfbs",
            // Field
            "world/data/field/area/field_dungeon_area/field_dungeon_area_array.bfbs",
            "world/data/field/area/field_inside_area/field_inside_area_array.bfbs",
            "world/data/field/area/field_location/field_location_array.bfbs",
            "world/data/field/area/field_main_area/field_main_area_array.bfbs",
            "world/data/field/area/field_sub_area/field_sub_area_array.bfbs",
            "world/data/field/area_su1/field_dungeon_area_su1/field_dungeon_area_su1_array.bfbs",
            "world/data/field/area_su1/field_inside_area_su1/field_inside_area_su1_array.bfbs",
            "world/data/field/area_su1/field_location_su1/field_location_su1_array.bfbs",
            "world/data/field/area_su1/field_main_area_su1/field_main_area_su1_array.bfbs",
            "world/data/field/area_su1/field_sub_area_su1/field_sub_area_su1_array.bfbs",
            "world/data/field/area_su2/field_dungeon_area_su2/field_dungeon_area_su2_array.bfbs",
            "world/data/field/area_su2/field_inside_area_su2/field_inside_area_su2_array.bfbs",
            //"world/data/field/area_su2/field_location_su2/field_location_su2_array.bfbs", DOES NOT EXIST.
            "world/data/field/area_su2/field_main_area_su2/field_main_area_su2_array.bfbs",
            "world/data/field/area_su2/field_sub_area_su2/field_sub_area_su2_array.bfbs",
            "world/data/field/fixed_symbol/fixed_symbol_table/fixed_symbol_table_array.bfbs",
            // Event Trade
            "world/data/event/eventTradeList/eventTradeList_array.bfbs",
            "world/data/event/eventTradePokemon/eventTradePokemon_array.bfbs",
            // Battle
            "world/data/battle/eventBattlePokemon/eventBattlePokemon_array.bfbs",
            "world/data/battle/battleconfig/battleconfig.bfbs",
            "world/data/battle/battleeffecttable/battleeffecttable_array.bfbs",
            // Cooking
            "world/data/cooking/CookingCommonData/cooking_common_data.bfbs",
            "world/data/cooking/FoodPowerCombo/FoodPowerCombo_array.bfbs",
            "world/data/cooking/Ingredient/IngredientData/IngredientData_array.bfbs",
            "world/data/cooking/Ingredient/IngredientDishData/IngredientDishData_array.bfbs",
            "world/data/cooking/Recipe/RecipeData/RecipeData_array.bfbs",
            "world/data/cooking/Seasoning/SeasoningData/SeasoningData_array.bfbs",
            "world/data/cooking/TakaraSpicePowerTable/TakaraSpicePowerTable_array.bfbs",
            // pokedex
            "world/data/ui/pokedex/blacklist/blacklist_array.bfbs",
            "world/data/ui/pokedex/distribution_data/distribution_data_array.bfbs",
            "world/data/ui/pokedex/memo_poke_data/memo_poke_data_array.bfbs",
            "world/data/ui/pokedex/reward_data/reward_data_array.bfbs",

            "world/data/ui/pokedex/blacklist_dlc1/blacklist_dlc1_array.bfbs",
            "world/data/ui/pokedex/distribution_data_dlc1/distribution_data_dlc1_array.bfbs",
            "world/data/ui/pokedex/memo_poke_data_dlc1/memo_poke_data_dlc1_array.bfbs",
            "world/data/ui/pokedex/reward_data_dlc1/reward_data_dlc1_array.bfbs",

            "world/data/ui/pokedex/blacklist_dlc2/blacklist_dlc2_array.bfbs",
            "world/data/ui/pokedex/distribution_data_dlc2/distribution_data_dlc2_array.bfbs",
            "world/data/ui/pokedex/memo_poke_data_dlc2/memo_poke_data_dlc2_array.bfbs",
            "world/data/ui/pokedex/reward_data_dlc2/reward_data_dlc2_array.bfbs",

            // UI
            "world/data/ui/shop/shop_data/shop_data_array.bfbs",
            "world/data/ui/status/ribbon/ribbon_array.bfbs",

            // Net
            "world/data/ui/net_battle/net_battle_rule_data/net_battle_rule_data_array.bfbs",
            "world/data/ui/net_battle/net_battle_rule_param/net_battle_rule_param_array.bfbs",

            // UI
            "world/data/ui/map_icon_data/on_cursor_data_mission/on_cursor_data_mission_array.bfbs",
            "world/data/ui/map_icon_data/on_cursor_data_point/on_cursor_data_point_array.bfbs",
            "world/data/ui/map_icon_data/on_cursor_data_town/on_cursor_data_town_array.bfbs",
            "world/data/ui/map_icon_data/town_shop_tag/town_shop_tag_array.bfbs",
            "world/data/ui/map_icon_data/ui_map_icon_data/ui_map_icon_data_array.bfbs",

            // Gem
            "world/data/gem/gem/gem_array.bfbs",

            // Item
            "world/data/item/dropitemdata/dropitemdata_array.bfbs",
            "world/data/item/hiddenItemBiomeTable/hiddenItemBiomeTable_array.bfbs",
            "world/data/item/hiddenItemDataTable/hiddenItemDataTable_array.bfbs",
            "world/data/item/itemPointTypeBiomeTable/itemPointTypeBiomeTable_array.bfbs",
            "world/data/item/itemdata/itemdata_array.bfbs",
            "world/data/item/monohiroiItemData/monohiroiItemData_array.bfbs",
            "world/data/item/rummagingItemDataTable/rummagingItemDataTable_array.bfbs",

            // Raid
            "world/data/raid/delivery_fixed_reward_item/delivery_fixed_reward_item_array.bfbs",
            "world/data/raid/delivery_lottery_reward_item/delivery_lottery_reward_item_array.bfbs",
            "world/data/raid/delivery_raid_enemy/delivery_raid_enemy_array.bfbs",
            "world/data/raid/delivery_raid_priority/delivery_raid_priority_array.bfbs",
            "world/data/raid/raid_enemy_01/raid_enemy_01_array.bfbs",
            "world/data/raid/raid_enemy_02/raid_enemy_02_array.bfbs",
            "world/data/raid/raid_enemy_03/raid_enemy_03_array.bfbs",
            "world/data/raid/raid_enemy_04/raid_enemy_04_array.bfbs",
            "world/data/raid/raid_enemy_05/raid_enemy_05_array.bfbs",
            "world/data/raid/raid_enemy_06/raid_enemy_06_array.bfbs",
            "world/data/raid/raid_fixed_reward_item/raid_fixed_reward_item_array.bfbs",
            "world/data/raid/raid_lottery_reward_item/raid_lottery_reward_item_array.bfbs",
            "world/data/raid/raid_lottery_reward_slot/raid_lottery_reward_slot_array.bfbs",
            "world/data/raid/raid_poke_scale/raid_poke_scale_array.bfbs",
            "world/data/raid/raid_trainer_01/raid_trainer_01_array.bfbs",
            "world/data/raid/raid_trainer_02/raid_trainer_02_array.bfbs",
            "world/data/raid/raid_trainer_03/raid_trainer_03_array.bfbs",
            "world/data/raid/raid_gem_item_reward_boost/data.bfbs",
            "world/data/raid/su1_raid_enemy_01/su1_raid_enemy_01_array.bfbs",
            "world/data/raid/su1_raid_enemy_02/su1_raid_enemy_02_array.bfbs",
            "world/data/raid/su1_raid_enemy_03/su1_raid_enemy_03_array.bfbs",
            "world/data/raid/su1_raid_enemy_04/su1_raid_enemy_04_array.bfbs",
            "world/data/raid/su1_raid_enemy_05/su1_raid_enemy_05_array.bfbs",
            "world/data/raid/su1_raid_enemy_06/su1_raid_enemy_06_array.bfbs",
            "world/data/raid/su2_raid_enemy_01/su2_raid_enemy_01_array.bfbs",
            "world/data/raid/su2_raid_enemy_02/su2_raid_enemy_02_array.bfbs",
            "world/data/raid/su2_raid_enemy_03/su2_raid_enemy_03_array.bfbs",
            "world/data/raid/su2_raid_enemy_04/su2_raid_enemy_04_array.bfbs",
            "world/data/raid/su2_raid_enemy_05/su2_raid_enemy_05_array.bfbs",
            "world/data/raid/su2_raid_enemy_06/su2_raid_enemy_06_array.bfbs",

            // Coin
            "world/data/field/coin_symbol/coin_symbol_manager/coin_symbol_manager_data.bfbs",

            "world/data/field/fixed_symbol/gem_symbol_lottery_table/gem_symbol_lottery_table_array.bfbs",
            "world/data/field/fixed_symbol/gem_symbol_setting/data.bfbs",
            "world/data/field/exclusion_area/exclusion_group_table/exclusion_group_table_array.bfbs",
            "world/data/field/fixed_symbol/fixed_symbol_manager/fixed_symbol_manager_data.bfbs",

            "world/data/event/event_add_pokemon/eventAddPokemon/eventAddPokemon_array.bfbs",
            "world/data/battle/wazaeffecttable/wazaeffecttable_array.bfbs",
            "world/data/ui/dressup_preset/dressup_preset_data/dressup_preset_data_array.bfbs",
            "world/data/ui/dressup_style/dressup_style_data/dressup_style_data_array.bfbs",
            "world/data/ui/pokeicon/poke_icon_general_scale/poke_icon_general_scale_array.bfbs",
            "world/data/ui/shop/dressup_shop/dressup_category_data/dressup_category_data_array.bfbs",
            "world/data/ui/shop/dressup_shop/dressup_item_data/dressup_item_data_array.bfbs",
            "world/data/ui/shop/dressup_shop/dressup_shop_data/dressup_shop_data_array.bfbs",
            "world/data/ui/shop/shop_restaurant/restaurant_bufdata/restaurant_bufdata_array.bfbs",
            "world/data/ui/shop/shop_restaurant/restaurant_menudata/restaurant_menudata_array.bfbs",
            "world/data/ui/shop/shop_restaurant/restaurant_shopdata/restaurant_shopdata_array.bfbs",
            "world/data/ui/shop/shop_wazamachine/shop_wazamachine_data/shop_wazamachine_data_array.bfbs",
            "world/data/ajito/AjitoCommonLevel/AjitoCommonLevel_array.bfbs",
            "world/data/ajito/AjitoPokemon/AjitoPokemon_array.bfbs",
            "world/data/ajito/AjitoUnit/AjitoUnit_array.bfbs",
            "world/data/danbattle/boss/dan_car_battle_data/dan_car_battle_data_array.bfbs",

            "world/data/field/area_range/field_auto_return_data/field_auto_return_data_array.bfbs",
            "world/data/field/area_range/field_auto_return_pos/field_auto_return_pos_array.bfbs",
            "world/data/field/area_range/field_out_of_range_data/field_out_of_range_data_array.bfbs",

            "world/data/field/map_change_common/map_change_common_array.bfbs",
            "world/data/event/sub_012/sub_012_reward/sub_012_reward_array.bfbs",

            "world/data/field/playables/field_camera/battleData.bfbs",
            "world/data/field/playables/field_camera/data.bfbs",
            "world/data/field/playables/field_interior_camera/cam_t01_i01_01.bfbs",
            "world/data/field/playables/field_interior_camera/interior_02.bfbs",
            "world/data/field/playables/field_interior_camera/interior_03.bfbs",
            "world/data/field/playables/field_interior_camera/interior_04.bfbs",
            "world/data/field/playables/field_interior_camera/interior_05.bfbs",
            "world/data/field/playables/field_interior_camera/sample.bfbs",
            "world/data/field/playables/field_interior_camera/sample.bfbs",
            "world/data/field/playables/field_player/data.bfbs",
            "world/data/field/playables/field_ride/data.bfbs",
            "world/data/field/fly_area/area_fly_flag/area_fly_flag_array.bfbs",
            "world/data/event/npc_traffic_generate/npc_traffic_generate_array.bfbs",
            "world/data/event/field_popup/event_field_popup/event_field_popup_array.bfbs",
            "world/data/picnic/picnic_system/default_system_data.bfbs",
            "world/data/picnic/picnic_tableset_data/common.bfbs",
            "world/data/picnic/picnic_wagon_data/common.bfbs",
            "world/data/picnic/pokemon/picnic_pokemon/RIDE.bfbs",
            "world/data/system/object_generation_range/object_generation_range_array.bfbs",
            "world/data/ui/battle_bgm_select/battle_bgm_select_data/battle_bgm_select_data_array.bfbs",
            "world/data/ui/quest_clear/dan_quest_clear/dan_quest_clear_array.bfbs",
            "world/data/ui/quest_clear/gym_quest_clear/gym_quest_clear_array.bfbs",
            "world/data/ui/quest_clear/nushi_quest_clear/nushi_quest_clear_array.bfbs",
            "world/data/ui/system_bg/system_bg/system_bg_array.bfbs",
            "world/data/ui/tips/tips_data/tips_data_array.bfbs",
            "world/data/ui/ymap/yamp_npc_navigation_data/yamp_npc_navigation_data_array.bfbs",
            "world/data/ui/ymap/ymap_destination_data/ymap_destination_data_array.bfbs",
            "world/data/ui/ymap/ymap_destination_dlc1_data/ymap_destination_dlc1_data_array.bfbs",
            "world/data/ui/ymap/ymap_destination_dlc2_data/ymap_destination_dlc2_data_array.bfbs",
            "world/data/ui/ymap/ymap_place_name_data/ymap_place_name_data_array.bfbs",

            "world/data/ui/schoolmap/schoolmap_data/schoolmap_data_array.bfbs",
            "world/data/ui/shop/friendlyshop/friendlyshop_data/friendlyshop_data_array.bfbs",
            "world/data/ui/shop/friendlyshop/friendlyshop_lineup_data/friendlyshop_lineup_data_array.bfbs",

            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_013_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_014_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_015_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_016_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_017_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_018_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_019_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_020_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_021_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_022_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_023_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_024_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_025_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_026_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_027_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_028_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_029_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_030_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_031_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_032_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_033_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_034_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_035_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_036_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/legend_poke/s2_sub_037_legend_poke.bfbs",
            "world/data/event/s2_sub_012_legend_poke/release_poke/release_poke_array.bfbs",
            "world/data/event/sub_012/sub_012_reward/sub_012_reward_array.bfbs",
            "world/data/event/s2_sub_012_legend_poke/release_config/default.bfbs",

            "world/data/event/s1_sub_013/s1_sub_013_chair_reward/s1_sub_013_chair_reward_array.bfbs",
            "world/data/event/s1_sub_013/s1_sub_013_complete_reward/s1_sub_013_complete_reward_array.bfbs",
            "world/data/event/s1_sub_013/s1_sub_013_food_reward/s1_sub_013_food_reward_array.bfbs",

            "world/data/bb_challenge/fairy_challenge_questionInfos/fairy_challenge_questionInfos_array.bfbs",
            "world/data/bb_challenge/fairy_challenge_targetInfos/fairy_challenge_targetInfos_array.bfbs",
            "world/data/bb_challenge/hagane_challenge_course/hagane_challenge_course_array.bfbs",
            "world/data/bb_challenge/honoo_challenge_npc/honoo_challenge_npc_array.bfbs",

            "world/data/ui/schoolmap/bb_schoolmap_data/bb_schoolmap_data_array.bfbs",

            "world/data/club/ClubNpcRandomRewardList/ClubNpcRandomRewardList_array.bfbs",
            "world/data/club/ClubNpcRewardList/ClubNpcRewardList_array.bfbs",
            "world/data/club/ClubNpcTradeList/ClubNpcTradeList_array.bfbs",
            "world/data/club/ClubPairNpcFaceInfo/ClubPairNpcFaceInfo_array.bfbs",
            "world/data/club/ClubRoomNpcPairTable/ClubRoomNpcPairTable_array.bfbs",
            "world/data/club/MetamonBlockInfo/metamonBlockInfoData.bfbs",
            "world/data/club/MissionInfo/MissionInfo_array.bfbs",
            "world/data/club/Quiz/quiz_poke_data_easy/quiz_poke_data_easy_array.bfbs",
            "world/data/club/Quiz/quiz_poke_data_normal/quiz_poke_data_normal_array.bfbs",
            "world/data/club/Quiz/quiz_poke_data_hard/quiz_poke_data_hard_array.bfbs",
            "world/data/ui/clubroom_bgm_select/clubroom_bgm_select_data/clubroom_bgm_select_data_array.bfbs",
            "world/data/club/ClubRoomBoardInfo/ClubRoomBoardInfo_array.bfbs",
            "world/data/club/MissionSettings/MissionSettings_array.bfbs",

            "world/data/field/synchro/synchro_data/synchro_data_array.bfbs",

            "world/data/event/field_npc/event_field_npc_message/event_field_npc_message_condition_flagwork/event_field_npc_message_condition_flagwork_array.bfbs",
        };
        foreach (var f in files)
        {
            DumpPackedFile(f);
            DumpPackedFile(Path.ChangeExtension(f, ".bin"));
        }
    }

    private void DumpPackedFile(string f)
    {
        var path = GetPath("raw", f.Replace('/', Path.DirectorySeparatorChar));
        var dir = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var data = rom.GetPackedFile(f);
        File.WriteAllBytes(path, data);

        var ext = Path.GetExtension(f);
        if (ext == ".bfbs")
        {
            var settings = new Schema.SchemaDumpSettings();
            var schema = Schema.SchemaDump.HandleReflection(data, path, dir, settings);
            DumpJson(schema, Path.ChangeExtension(path, ".schema.json"));
        }
    }

    public void DumpHashReflectionBFBS()
    {
        ulong[] files =
        [
            0x6B3C7A393C4806BB,
            0xABA6B518253AEEEB,
            0xCC942F5FDA362FF3,
            0x0D39C3AA4AD7816F,
            0x08C2BDF7ABC185AF,
            0x977AF7D3AB1F5E3D,
            0xDB862D9BC4E09F63,
            0xDB0A4CEB9BE77E1E,
            0x2C432AF938EDCF99,
        ];
        foreach (var f in files)
        {
            if (!rom.HasFile(f))
                continue;

            var path = GetPath("hashFile", f.ToString("X16"));
            var dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var data = rom.GetPackedFile(f);
            File.WriteAllBytes(path, data);
            try
            {
                var settings = new Schema.SchemaDumpSettings();
                var schema = Schema.SchemaDump.HandleReflection(data, path, dir, settings);
                DumpJson(schema, Path.ChangeExtension(path, ".schema.json"));
            }
            catch
            {
            }
        }
    }

    public void DumpArchives()
    {
        TrinityPakExtractor.DumpArchives(rom.PathRomFS, GetPath(DumpArchive));
        Debug.WriteLine("Loaded");
    }

    public void DumpDistributionRaids(string path)
    {
        TeraRaidRipper.DumpDistributionRaids(rom, path);
    }

    public void DumpDeliveryOutbreaks(string path)
    {
        var dump = GetPath("encounters");
        MassOutbreakRipper.DumpDeliveryOutbreaks(rom, path, dump);
    }

    public void DumpKitakamiSpawnPoints()
    {
        var dump = GetPath("kitakami");
        if (!Directory.Exists(dump))
            Directory.CreateDirectory(dump);

        // Main
        var dataRegular = rom.GetPackedFile("world/data/encount/point_data/point_data/encount_data_su1.bin");
        var pointsRegular = FlatBufferConverter.DeserializeFrom<PointDataArray>(dataRegular).Table;
        using var file = new StreamWriter(Path.Combine(dump, "original_encount.txt"));
        foreach (var point in pointsRegular)
            file.WriteLine($"{point.Position.X:R},{point.Position.Z:R}");

        // Outbreak
        var dataOutbreak = rom.GetPackedFile("world/data/encount/point_data/outbreak_point_data/outbreak_point_su1.bin");
        var pointsOutbreak = FlatBufferConverter.DeserializeFrom<OutbreakPointArray>(dataOutbreak).Table;
        using var fileOutbreak = new StreamWriter(Path.Combine(dump, "original_outbreak.txt"));
        foreach (var point in pointsOutbreak)
            fileOutbreak.WriteLine($"{point.Position.X:R},{point.Position.Z:R}");
    }
}
