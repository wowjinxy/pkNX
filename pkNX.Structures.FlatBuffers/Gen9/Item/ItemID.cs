using FlatSharp.Attributes;
// ReSharper disable UnusedMember.Global

namespace pkNX.Structures.FlatBuffers;

[FlatBufferEnum(typeof(int))]
public enum ItemID
{
    ITEMID_NONE = 0,
    ITEMID_MASUTAABOORU = 1,
    ITEMID_HAIPAABOORU = 2,
    ITEMID_SUUPAABOORU = 3,
    ITEMID_MONSUTAABOORU = 4,
    ITEMID_SAFARIBOORU = 5,
    ITEMID_NETTOBOORU = 6,
    ITEMID_DAIBUBOORU = 7,
    ITEMID_NESUTOBOORU = 8,
    ITEMID_RIPIITOBOORU = 9,
    ITEMID_TAIMAABOORU = 10,
    ITEMID_GOOZYASUBOORU = 11,
    ITEMID_PUREMIABOORU = 12,
    ITEMID_DAAKUBOORU = 13,
    ITEMID_HIIRUBOORU = 14,
    ITEMID_KUIKKUBOORU = 15,
    ITEMID_KIZUGUSURI = 17,
    ITEMID_DOKUKESI = 18,
    ITEMID_YAKEDONAOSI = 19,
    ITEMID_KOORINAOSI = 20,
    ITEMID_NEMUKEZAMASI = 21,
    ITEMID_MAHINAOSI = 22,
    ITEMID_KAIHUKUNOKUSURI = 23,
    ITEMID_MANTANNOKUSURI = 24,
    ITEMID_SUGOIKIZUGUSURI = 25,
    ITEMID_IIKIZUGUSURI = 26,
    ITEMID_NANDEMONAOSI = 27,
    ITEMID_GENKINOKAKERA = 28,
    ITEMID_GENKINOKATAMARI = 29,
    ITEMID_OISIIMIZU = 30,
    ITEMID_SAIKOSOODA = 31,
    ITEMID_MIKKUSUORE = 32,
    ITEMID_MOOMOOMIRUKU = 33,
    ITEMID_TIKARANOKONA = 34,
    ITEMID_TIKARANONEKKO = 35,
    ITEMID_BANNOUGONA = 36,
    ITEMID_HUKKATUSOU = 37,
    ITEMID_PIIPIIEIDO = 38,
    ITEMID_PIIPIIRIKABAA = 39,
    ITEMID_PIIPIIEIDAA = 40,
    ITEMID_PIIPIIMAKKUSU = 41,
    ITEMID_MAKKUSUAPPU = 45,
    ITEMID_TAURIN = 46,
    ITEMID_BUROMUHEKISIN = 47,
    ITEMID_INDOMETASIN = 48,
    ITEMID_RIZOTIUMU = 49,
    ITEMID_HUSIGINAAME = 50,
    ITEMID_POINTOAPPU = 51,
    ITEMID_KITOSAN = 52,
    ITEMID_POINTOMAKKUSU = 53,
    ITEMID_EFEKUTOGAADO = 55,
    ITEMID_KURITHIKATTO = 56,
    ITEMID_PURASUPAWAA = 57,
    ITEMID_DHIFENDAA = 58,
    ITEMID_SUPIIDAA = 59,
    ITEMID_YOKUATAARU = 60,
    ITEMID_SUPESYARUAPPU = 61,
    ITEMID_SUPESYARUGAADO = 62,
    ITEMID_PIPPININGYOU = 63,
    ITEMID_TAIYOUNOISI = 80,
    ITEMID_TUKINOISI = 81,
    ITEMID_HONOONOISI = 82,
    ITEMID_KAMINARINOISI = 83,
    ITEMID_MIZUNOISI = 84,
    ITEMID_RIIHUNOISI = 85,
    ITEMID_TIISANAKINOKO = 86,
    ITEMID_OOKINAKINOKO = 87,
    ITEMID_SINZYU = 88,
    ITEMID_OOKINASINZYU = 89,
    ITEMID_HOSINOSUNA = 90,
    ITEMID_HOSINOKAKERA = 91,
    ITEMID_KINNOTAMA = 92,
    ITEMID_AMAIMITU = 94,
    ITEMID_KITYOUNAHONE = 106,
    ITEMID_HIKARINOISI = 107,
    ITEMID_YAMINOISI = 108,
    ITEMID_MEZAMEISI = 109,
    ITEMID_MANMARUISI = 110,
    ITEMID_HAKKINDAMA = 112,
    ITEMID_KONGOUDAMA = 135,
    ITEMID_SIRATAMA = 136,
    ITEMID_KURABONOMI = 149,
    ITEMID_KAGONOMI = 150,
    ITEMID_MOMONNOMI = 151,
    ITEMID_TIIGONOMI = 152,
    ITEMID_NANASINOMI = 153,
    ITEMID_HIMERINOMI = 154,
    ITEMID_ORENNOMI = 155,
    ITEMID_KIINOMI = 156,
    ITEMID_RAMUNOMI = 157,
    ITEMID_OBONNOMI = 158,
    ITEMID_FIRANOMI = 159,
    ITEMID_UINOMI = 160,
    ITEMID_MAGONOMI = 161,
    ITEMID_BANZINOMI = 162,
    ITEMID_IANOMI = 163,
    ITEMID_ZAROKUNOMI = 169,
    ITEMID_NEKOBUNOMI = 170,
    ITEMID_TAPORUNOMI = 171,
    ITEMID_ROMENOMI = 172,
    ITEMID_UBUNOMI = 173,
    ITEMID_MATOMANOMI = 174,
    ITEMID_OKKANOMI = 184,
    ITEMID_ITOKENOMI = 185,
    ITEMID_SOKUNONOMI = 186,
    ITEMID_RINDONOMI = 187,
    ITEMID_YATHENOMI = 188,
    ITEMID_YOPUNOMI = 189,
    ITEMID_BIAANOMI = 190,
    ITEMID_SYUKANOMI = 191,
    ITEMID_BAKOUNOMI = 192,
    ITEMID_UTANNOMI = 193,
    ITEMID_TANGANOMI = 194,
    ITEMID_YOROGINOMI = 195,
    ITEMID_KASIBUNOMI = 196,
    ITEMID_HABANNOMI = 197,
    ITEMID_NAMONOMI = 198,
    ITEMID_RIRIBANOMI = 199,
    ITEMID_HOZUNOMI = 200,
    ITEMID_TIIRANOMI = 201,
    ITEMID_RYUGANOMI = 202,
    ITEMID_KAMURANOMI = 203,
    ITEMID_YATAPINOMI = 204,
    ITEMID_ZUANOMI = 205,
    ITEMID_SANNOMI = 206,
    ITEMID_SUTAANOMI = 207,
    ITEMID_NAZONOMI = 208,
    ITEMID_MIKURUNOMI = 209,
    ITEMID_IBANNOMI = 210,
    ITEMID_ZYAPONOMI = 211,
    ITEMID_RENBUNOMI = 212,
    ITEMID_HIKARINOKONA = 213,
    ITEMID_SIROIHAABU = 214,
    ITEMID_SENSEINOTUME = 217,
    ITEMID_YASURAGINOSUZU = 218,
    ITEMID_MENTARUHAABU = 219,
    ITEMID_KODAWARIHATIMAKI = 220,
    ITEMID_OUZYANOSIRUSI = 221,
    ITEMID_GINNOKONA = 222,
    ITEMID_OMAMORIKOBAN = 223,
    ITEMID_KEMURIDAMA = 228,
    ITEMID_KAWARAZUNOISI = 229,
    ITEMID_KIAINOHATIMAKI = 230,
    ITEMID_SIAWASETAMAGO = 231,
    ITEMID_PINTORENZU = 232,
    ITEMID_METARUKOOTO = 233,
    ITEMID_TABENOKOSI = 234,
    ITEMID_DENKIDAMA = 236,
    ITEMID_YAWARAKAISUNA = 237,
    ITEMID_KATAIISI = 238,
    ITEMID_KISEKINOTANE = 239,
    ITEMID_KUROIMEGANE = 240,
    ITEMID_KUROOBI = 241,
    ITEMID_ZISYAKU = 242,
    ITEMID_SINPINOSIZUKU = 243,
    ITEMID_SURUDOIKUTIBASI = 244,
    ITEMID_DOKUBARI = 245,
    ITEMID_TOKENAIKOORI = 246,
    ITEMID_NOROINOOHUDA = 247,
    ITEMID_MAGATTASUPUUN = 248,
    ITEMID_MOKUTAN = 249,
    ITEMID_RYUUNOKIBA = 250,
    ITEMID_SIRUKUNOSUKAAHU = 251,
    ITEMID_KAIGARANOSUZU = 253,
    ITEMID_KOUKAKURENZU = 265,
    ITEMID_TIKARANOHATIMAKI = 266,
    ITEMID_MONOSIRIMEGANE = 267,
    ITEMID_TATUZINNOOBI = 268,
    ITEMID_HIKARINONENDO = 269,
    ITEMID_INOTINOTAMA = 270,
    ITEMID_PAWAHURUHAABU = 271,
    ITEMID_DOKUDOKUDAMA = 272,
    ITEMID_KAENDAMA = 273,
    ITEMID_KIAINOTASUKI = 275,
    ITEMID_FOOKASURENZU = 276,
    ITEMID_METORONOOMU = 277,
    ITEMID_KUROITEKKYUU = 278,
    ITEMID_KOUKOUNOSIPPO = 279,
    ITEMID_AKAIITO = 280,
    ITEMID_KUROIHEDORO = 281,
    ITEMID_TUMETAIIWA = 282,
    ITEMID_SARASARAIWA = 283,
    ITEMID_ATUIIWA = 284,
    ITEMID_SIMETTAIWA = 285,
    ITEMID_NEBARINOKAGIDUME = 286,
    ITEMID_KODAWARISUKAAHU = 287,
    ITEMID_KUTTUKIBARI = 288,
    ITEMID_PAWAARISUTO = 289,
    ITEMID_PAWAABERUTO = 290,
    ITEMID_PAWAARENZU = 291,
    ITEMID_PAWAABANDO = 292,
    ITEMID_PAWAAANKURU = 293,
    ITEMID_PAWAAUEITO = 294,
    ITEMID_KIREINANUKEGARA = 295,
    ITEMID_OOKINANEKKO = 296,
    ITEMID_KODAWARIMEGANE = 297,
    ITEMID_HINOTAMAPUREETO = 298,
    ITEMID_SIZUKUPUREETO = 299,
    ITEMID_IKAZUTIPUREETO = 300,
    ITEMID_MIDORINOPUREETO = 301,
    ITEMID_TURARANOPUREETO = 302,
    ITEMID_KOBUSINOPUREETO = 303,
    ITEMID_MOUDOKUPUREETO = 304,
    ITEMID_DAITINOPUREETO = 305,
    ITEMID_AOZORAPUREETO = 306,
    ITEMID_HUSIGINOPUREETO = 307,
    ITEMID_TAMAMUSIPUREETO = 308,
    ITEMID_GANSEKIPUREETO = 309,
    ITEMID_MONONOKEPUREETO = 310,
    ITEMID_RYUUNOPUREETO = 311,
    ITEMID_KOWAMOTEPUREETO = 312,
    ITEMID_KOUTETUPUREETO = 313,
    ITEMID_SURUDOITUME = 326,
    ITEMID_WAZAMASIN01 = 328,
    ITEMID_WAZAMASIN02 = 329,
    ITEMID_WAZAMASIN03 = 330,
    ITEMID_WAZAMASIN04 = 331,
    ITEMID_WAZAMASIN05 = 332,
    ITEMID_WAZAMASIN06 = 333,
    ITEMID_WAZAMASIN07 = 334,
    ITEMID_WAZAMASIN08 = 335,
    ITEMID_WAZAMASIN09 = 336,
    ITEMID_WAZAMASIN10 = 337,
    ITEMID_WAZAMASIN11 = 338,
    ITEMID_WAZAMASIN12 = 339,
    ITEMID_WAZAMASIN13 = 340,
    ITEMID_WAZAMASIN14 = 341,
    ITEMID_WAZAMASIN15 = 342,
    ITEMID_WAZAMASIN16 = 343,
    ITEMID_WAZAMASIN17 = 344,
    ITEMID_WAZAMASIN18 = 345,
    ITEMID_WAZAMASIN19 = 346,
    ITEMID_WAZAMASIN20 = 347,
    ITEMID_WAZAMASIN21 = 348,
    ITEMID_WAZAMASIN22 = 349,
    ITEMID_WAZAMASIN23 = 350,
    ITEMID_WAZAMASIN24 = 351,
    ITEMID_WAZAMASIN25 = 352,
    ITEMID_WAZAMASIN26 = 353,
    ITEMID_WAZAMASIN27 = 354,
    ITEMID_WAZAMASIN28 = 355,
    ITEMID_WAZAMASIN29 = 356,
    ITEMID_WAZAMASIN30 = 357,
    ITEMID_WAZAMASIN31 = 358,
    ITEMID_WAZAMASIN32 = 359,
    ITEMID_WAZAMASIN33 = 360,
    ITEMID_WAZAMASIN34 = 361,
    ITEMID_WAZAMASIN35 = 362,
    ITEMID_WAZAMASIN36 = 363,
    ITEMID_WAZAMASIN37 = 364,
    ITEMID_WAZAMASIN38 = 365,
    ITEMID_WAZAMASIN39 = 366,
    ITEMID_WAZAMASIN40 = 367,
    ITEMID_WAZAMASIN41 = 368,
    ITEMID_WAZAMASIN42 = 369,
    ITEMID_WAZAMASIN43 = 370,
    ITEMID_WAZAMASIN44 = 371,
    ITEMID_WAZAMASIN45 = 372,
    ITEMID_WAZAMASIN46 = 373,
    ITEMID_WAZAMASIN47 = 374,
    ITEMID_WAZAMASIN48 = 375,
    ITEMID_WAZAMASIN49 = 376,
    ITEMID_WAZAMASIN50 = 377,
    ITEMID_WAZAMASIN51 = 378,
    ITEMID_WAZAMASIN52 = 379,
    ITEMID_WAZAMASIN53 = 380,
    ITEMID_WAZAMASIN54 = 381,
    ITEMID_WAZAMASIN55 = 382,
    ITEMID_WAZAMASIN56 = 383,
    ITEMID_WAZAMASIN57 = 384,
    ITEMID_WAZAMASIN58 = 385,
    ITEMID_WAZAMASIN59 = 386,
    ITEMID_WAZAMASIN60 = 387,
    ITEMID_WAZAMASIN61 = 388,
    ITEMID_WAZAMASIN62 = 389,
    ITEMID_WAZAMASIN63 = 390,
    ITEMID_WAZAMASIN64 = 391,
    ITEMID_WAZAMASIN65 = 392,
    ITEMID_WAZAMASIN66 = 393,
    ITEMID_WAZAMASIN67 = 394,
    ITEMID_WAZAMASIN68 = 395,
    ITEMID_WAZAMASIN69 = 396,
    ITEMID_WAZAMASIN70 = 397,
    ITEMID_WAZAMASIN71 = 398,
    ITEMID_WAZAMASIN72 = 399,
    ITEMID_WAZAMASIN73 = 400,
    ITEMID_WAZAMASIN74 = 401,
    ITEMID_WAZAMASIN75 = 402,
    ITEMID_WAZAMASIN76 = 403,
    ITEMID_WAZAMASIN77 = 404,
    ITEMID_WAZAMASIN78 = 405,
    ITEMID_WAZAMASIN79 = 406,
    ITEMID_WAZAMASIN80 = 407,
    ITEMID_WAZAMASIN81 = 408,
    ITEMID_WAZAMASIN82 = 409,
    ITEMID_WAZAMASIN83 = 410,
    ITEMID_WAZAMASIN84 = 411,
    ITEMID_WAZAMASIN85 = 412,
    ITEMID_WAZAMASIN86 = 413,
    ITEMID_WAZAMASIN87 = 414,
    ITEMID_WAZAMASIN88 = 415,
    ITEMID_WAZAMASIN89 = 416,
    ITEMID_WAZAMASIN90 = 417,
    ITEMID_WAZAMASIN91 = 418,
    ITEMID_WAZAMASIN92 = 419,
    ITEMID_SUPIIDOBOORU = 492,
    ITEMID_REBERUBOORU = 493,
    ITEMID_RUAABOORU = 494,
    ITEMID_HEBIIBOORU = 495,
    ITEMID_RABURABUBOORU = 496,
    ITEMID_HURENDOBOORU = 497,
    ITEMID_MUUNBOORU = 498,
    ITEMID_KONPEBOORU = 499,
    ITEMID_SINKANOKISEKI = 538,
    ITEMID_KARUISI = 539,
    ITEMID_GOTUGOTUMETTO = 540,
    ITEMID_HUUSEN = 541,
    ITEMID_REDDOKAADO = 542,
    ITEMID_NERAINOMATO = 543,
    ITEMID_SIMETUKEBANDO = 544,
    ITEMID_KYUUKON = 545,
    ITEMID_ZYUUDENTI = 546,
    ITEMID_DASSYUTUBOTAN = 547,
    ITEMID_NOOMARUZYUERU = 564,
    ITEMID_TAIRYOKUNOHANE = 565,
    ITEMID_KINRYOKUNOHANE = 566,
    ITEMID_TEIKOUNOHANE = 567,
    ITEMID_TIRYOKUNOHANE = 568,
    ITEMID_SEISINNOHANE = 569,
    ITEMID_SYUNPATUNOHANE = 570,
    ITEMID_KIREINAHANE = 571,
    ITEMID_DORIIMUBOORU = 576,
    ITEMID_KAORUKINOKO = 580,
    ITEMID_DEKAIKINNOTAMA = 581,
    ITEMID_ODANGOSINZYU = 582,
    ITEMID_SUISEINOKAKERA = 583,
    ITEMID_WAZAMASIN93 = 618,
    ITEMID_WAZAMASIN94 = 619,
    ITEMID_WAZAMASIN95 = 620,
    ITEMID_HIKARUOMAMORI = 632,
    ITEMID_UTUSIKAGAMI = 638,
    ITEMID_ZYAKUTENHOKEN = 639,
    ITEMID_TOTUGEKITYOKKI = 640,
    ITEMID_SEIREIPUREETO = 644,
    ITEMID_TOKUSEIKAPUSERU = 645,
    ITEMID_HIKARIGOKE = 648,
    ITEMID_YUKIDAMA = 649,
    ITEMID_BOUZINGOOGURU = 650,
    ITEMID_ROZERUNOMI = 686,
    ITEMID_AKKINOMI = 687,
    ITEMID_TARAPUNOMI = 688,
    ITEMID_WAZAMASIN96 = 690,
    ITEMID_WAZAMASIN97 = 691,
    ITEMID_WAZAMASIN98 = 692,
    ITEMID_WAZAMASIN99 = 693,
    ITEMID_TANKENKOKOROE = 703,
    ITEMID_IMASIMENOTUBO = 765,
    ITEMID_GINNOOUKAN = 795,
    ITEMID_KINNOOUKAN = 796,
    ITEMID_BIBIRIDAMA = 846,
    ITEMID_KOORINOISI = 849,
    ITEMID_URUTORABOORU = 851,
    ITEMID_KURENAINOMITU = 853,
    ITEMID_YAMABUKINOMITU = 854,
    ITEMID_USUMOMONOMITU = 855,
    ITEMID_MURASAKINOMITU = 856,
    ITEMID_GURANDOKOOTO = 879,
    ITEMID_BOUGOPATTO = 880,
    ITEMID_EREKISIIDO = 881,
    ITEMID_SAIKOSIIDO = 882,
    ITEMID_MISUTOSIIDO = 883,
    ITEMID_GURASUSIIDO = 884,
    ITEMID_KUTITATURUGI = 1103,
    ITEMID_KUTITATATE = 1104,
    ITEMID_AMAAIRINGO = 1116,
    ITEMID_SUPPAIRINGO = 1117,
    ITEMID_NODOAME = 1118,
    ITEMID_DASSYUTUPAKKU = 1119,
    ITEMID_ATUZOKOBUUTU = 1120,
    ITEMID_KARABURIHOKEN = 1121,
    ITEMID_RUUMUSAABISU = 1122,
    ITEMID_BANNOUGASA = 1123,
    ITEMID_KEIKENTIAME_1 = 1124,
    ITEMID_KEIKENTIAME_2 = 1125,
    ITEMID_KEIKENTIAME_3 = 1126,
    ITEMID_KEIKENTIAME_4 = 1127,
    ITEMID_KEIKENTIAME_5 = 1128,
    ITEMID_SAMISIGARIMINTO = 1231,
    ITEMID_IZIPPARIMINTO = 1232,
    ITEMID_YANTYAMINTO = 1233,
    ITEMID_YUKANMINTO = 1234,
    ITEMID_ZUBUTOIMINTO = 1235,
    ITEMID_WANPAKUMINTO = 1236,
    ITEMID_NOUTENKIMINTO = 1237,
    ITEMID_NONKIMINTO = 1238,
    ITEMID_HIKAEMEMINTO = 1239,
    ITEMID_OTTORIMINTO = 1240,
    ITEMID_UKKARIMINTO = 1241,
    ITEMID_REISEIMINTO = 1242,
    ITEMID_ODAYAKAMINTO = 1243,
    ITEMID_OTONASIIMINTO = 1244,
    ITEMID_SINTYOUMINTO = 1245,
    ITEMID_NAMAIKIMINTO = 1246,
    ITEMID_OKUBYOUMINTO = 1247,
    ITEMID_SEKKTIMINTO = 1248,
    ITEMID_YOUKIMINTO = 1249,
    ITEMID_MUJYAKIMINTO = 1250,
    ITEMID_MAZIMEMINTO = 1251,
    ITEMID_WARETAPOTTO = 1253,
    ITEMID_KAKETAPOTTO = 1254,
    ITEMID_ROTOMUNOKATAROGU = 1278,
    ITEMID_GARANATUBURESU = 1582,
    ITEMID_KIDUNANODADUNA = 1590,
    ITEMID_KIDUNANODADUNA_KAIZYO = 1591,
    ITEMID_GARANATURIISU = 1592,
    ITEMID_TOKUSEIPATTI = 1606,
    ITEMID_DAIKONGOUDAMA = 1777,
    ITEMID_DAISIRATAMA = 1778,
    ITEMID_DAIHAKKINDAMA = 1779,
    ITEMID_SUMAHOROTOMU = 1829,
    ITEMID_SANDOUICCHI = 1830,
    ITEMID_DENSETSUBOORUA = 1831,
    ITEMID_DENSETSUBOORUB = 1832,
    ITEMID_TAKARABOORU = 1833,
    ITEMID_GENSYOA = 1834,
    ITEMID_GENSYOB = 1835,
    ITEMID_HIROTTASAIFU = 1836,
    ITEMID_CHIISANATAKENOKO = 1842,
    ITEMID_OOKINATAKENOKO = 1843,
    ITEMID_AKUNOKAKEJIKU = 1857,
    ITEMID_MIZUNOKAKEJIKU = 1858,
    ITEMID_NOROINOYOROI = 1861,
    ITEMID_CHENJAA1 = 1862,
    ITEMID_CHENJAA2 = 1863,
    ITEMID_CHENJAA3 = 1864,
    ITEMID_CHENJAA4 = 1865,
    ITEMID_CHENJAA5 = 1866,
    ITEMID_CHENJAA6 = 1867,
    ITEMID_CHENJAA7 = 1868,
    ITEMID_CHENJAA8 = 1869,
    ITEMID_CHENJAA9 = 1870,
    ITEMID_CHENJAA10 = 1871,
    ITEMID_CHENJAA11 = 1872,
    ITEMID_CHENJAA12 = 1873,
    ITEMID_CHENJAA13 = 1874,
    ITEMID_CHENJAA14 = 1875,
    ITEMID_CHENJAA15 = 1876,
    ITEMID_CHENJAA16 = 1877,
    ITEMID_CHENJAA17 = 1878,
    ITEMID_CHENJAA18 = 1879,
    ITEMID_BUUSUTOENAJII = 1880,
    ITEMID_SENTOUBAFFA1 = 1881,
    ITEMID_SENTOUBAFFA2 = 1882,
    ITEMID_SENTOUBAFFA3 = 1883,
    ITEMID_SENTOUBAFFA4 = 1884,
    ITEMID_SENTOUBAFFA5 = 1885,
    ITEMID_SENTOUBAFFA6 = 1886,
    ITEMID_MAYONEEZU = 1889,
    ITEMID_KECHAPPU = 1890,
    ITEMID_MASUTAADO = 1891,
    ITEMID_BATAA = 1892,
    ITEMID_PIINATTSUBATAA = 1893,
    ITEMID_CHIRISOOSU = 1894,
    ITEMID_SORUTO = 1895,
    ITEMID_PEPPAA = 1896,
    ITEMID_YOOGURUTO = 1897,
    ITEMID_HOIPPUKURIIMU = 1898,
    ITEMID_KURIIMUCHIIZU = 1899,
    ITEMID_BERIIJAMU = 1900,
    ITEMID_MAAMAREEDO = 1901,
    ITEMID_ORIIBUOIRU = 1902,
    ITEMID_BINEGAA = 1903,
    ITEMID_TAKARASUPAISU1 = 1904,
    ITEMID_TAKARASUPAISU2 = 1905,
    ITEMID_TAKARASUPAISU3 = 1906,
    ITEMID_TAKARASUPAISU4 = 1907,
    ITEMID_TAKARASUPAISU5 = 1908,
    ITEMID_RETASUPAKKU = 1909,
    ITEMID_TOMATOSURAISU = 1910,
    ITEMID_KATTOMINITOMATO = 1911,
    ITEMID_KYUURISURAISU = 1912,
    ITEMID_PIKURUSUSURAISU = 1913,
    ITEMID_TAMANEGISURAISU = 1914,
    ITEMID_REDDOAARII = 1915,
    ITEMID_PIIMANSURAISU = 1916,
    ITEMID_AKAPAPURIKASURAISU = 1917,
    ITEMID_KIPAPURIKASURAISU = 1918,
    ITEMID_ABOKADO = 1919,
    ITEMID_YAKIBEEKON = 1920,
    ITEMID_HAMUSURAISU = 1921,
    ITEMID_NAMAHAMU = 1922,
    ITEMID_YAKICHORISOO = 1923,
    ITEMID_BOIRUSOOSEEJI = 1924,
    ITEMID_HANBAAGU = 1925,
    ITEMID_KANISUTIKKU = 1926,
    ITEMID_SUMOOKUKIRIMI = 1927,
    ITEMID_KIRIMIFURAI = 1928,
    ITEMID_SURAISUEGGU = 1929,
    ITEMID_TORUTIIJA = 1930,
    ITEMID_TOOFU = 1931,
    ITEMID_RAISU = 1932,
    ITEMID_NUUDORU = 1933,
    ITEMID_POTETOSARADA = 1934,
    ITEMID_SURAISUCHIIZU = 1935,
    ITEMID_BANANASURAISU = 1936,
    ITEMID_ICHIGOSURAISU = 1937,
    ITEMID_WAGIRIRINGO = 1938,
    ITEMID_WAGIRIKIUI = 1939,
    ITEMID_KATTOPAIN = 1940,
    ITEMID_HARAPEENYO = 1941,
    ITEMID_HOOSURADISSHU = 1942,
    ITEMID_KAREEPAUDAA = 1943,
    ITEMID_WASABISOOSU = 1944,
    ITEMID_KURESON = 1945,
    ITEMID_BAJIRU = 1946,
    ITEMID_SOZAI01 = 1956,
    ITEMID_SOZAI02 = 1957,
    ITEMID_SOZAI03 = 1958,
    ITEMID_SOZAI04 = 1959,
    ITEMID_SOZAI05 = 1960,
    ITEMID_SOZAI06 = 1961,
    ITEMID_SOZAI07 = 1962,
    ITEMID_SOZAI08 = 1963,
    ITEMID_SOZAI09 = 1964,
    ITEMID_SOZAI10 = 1965,
    ITEMID_SOZAI11 = 1966,
    ITEMID_SOZAI12 = 1967,
    ITEMID_SOZAI13 = 1968,
    ITEMID_SOZAI14 = 1969,
    ITEMID_SOZAI15 = 1970,
    ITEMID_SOZAI16 = 1971,
    ITEMID_SOZAI17 = 1972,
    ITEMID_SOZAI18 = 1973,
    ITEMID_SOZAI19 = 1974,
    ITEMID_SOZAI20 = 1975,
    ITEMID_SOZAI21 = 1976,
    ITEMID_SOZAI22 = 1977,
    ITEMID_SOZAI23 = 1978,
    ITEMID_SOZAI24 = 1979,
    ITEMID_SOZAI25 = 1980,
    ITEMID_SOZAI26 = 1981,
    ITEMID_SOZAI27 = 1982,
    ITEMID_SOZAI28 = 1983,
    ITEMID_SOZAI29 = 1984,
    ITEMID_SOZAI30 = 1985,
    ITEMID_SOZAI31 = 1986,
    ITEMID_SOZAI32 = 1987,
    ITEMID_SOZAI33 = 1988,
    ITEMID_SOZAI34 = 1989,
    ITEMID_SOZAI35 = 1990,
    ITEMID_SOZAI36 = 1991,
    ITEMID_SOZAI37 = 1992,
    ITEMID_SOZAI38 = 1993,
    ITEMID_SOZAI39 = 1994,
    ITEMID_SOZAI40 = 1995,
    ITEMID_SOZAI41 = 1996,
    ITEMID_SOZAI42 = 1997,
    ITEMID_SOZAI43 = 1998,
    ITEMID_SOZAI44 = 1999,
    ITEMID_SOZAI45 = 2000,
    ITEMID_SOZAI46 = 2001,
    ITEMID_SOZAI47 = 2002,
    ITEMID_SOZAI48 = 2003,
    ITEMID_SOZAI49 = 2004,
    ITEMID_SOZAI50 = 2005,
    ITEMID_SOZAI51 = 2006,
    ITEMID_SOZAI52 = 2007,
    ITEMID_SOZAI53 = 2008,
    ITEMID_SOZAI54 = 2009,
    ITEMID_SOZAI55 = 2010,
    ITEMID_SOZAI56 = 2011,
    ITEMID_SOZAI57 = 2012,
    ITEMID_SOZAI58 = 2013,
    ITEMID_SOZAI59 = 2014,
    ITEMID_SOZAI60 = 2015,
    ITEMID_SOZAI61 = 2016,
    ITEMID_SOZAI62 = 2017,
    ITEMID_SOZAI63 = 2018,
    ITEMID_SOZAI64 = 2019,
    ITEMID_SOZAI65 = 2020,
    ITEMID_SOZAI66 = 2021,
    ITEMID_SOZAI67 = 2022,
    ITEMID_SOZAI68 = 2023,
    ITEMID_SOZAI69 = 2024,
    ITEMID_SOZAI70 = 2025,
    ITEMID_SOZAI71 = 2026,
    ITEMID_SOZAI72 = 2027,
    ITEMID_SOZAI73 = 2028,
    ITEMID_SOZAI74 = 2029,
    ITEMID_SOZAI75 = 2030,
    ITEMID_SOZAI76 = 2031,
    ITEMID_SOZAI77 = 2032,
    ITEMID_SOZAI78 = 2033,
    ITEMID_SOZAI79 = 2034,
    ITEMID_SOZAI80 = 2035,
    ITEMID_SOZAI81 = 2036,
    ITEMID_SOZAI82 = 2037,
    ITEMID_SOZAI83 = 2038,
    ITEMID_SOZAI84 = 2039,
    ITEMID_SOZAI85 = 2040,
    ITEMID_SOZAI86 = 2041,
    ITEMID_SOZAI87 = 2042,
    ITEMID_SOZAI88 = 2043,
    ITEMID_SOZAI89 = 2044,
    ITEMID_SOZAI90 = 2045,
    ITEMID_SOZAI91 = 2046,
    ITEMID_SOZAI92 = 2047,
    ITEMID_SOZAI93 = 2048,
    ITEMID_SOZAI94 = 2049,
    ITEMID_SOZAI95 = 2050,
    ITEMID_SOZAI96 = 2051,
    ITEMID_SOZAI97 = 2052,
    ITEMID_SOZAI98 = 2053,
    ITEMID_SOZAI99 = 2054,
    ITEMID_SOZAI100 = 2055,
    ITEMID_SOZAI101 = 2056,
    ITEMID_SOZAI102 = 2057,
    ITEMID_SOZAI103 = 2058,
    ITEMID_SOZAI104 = 2059,
    ITEMID_SOZAI105 = 2060,
    ITEMID_SOZAI106 = 2061,
    ITEMID_SOZAI107 = 2062,
    ITEMID_SOZAI108 = 2063,
    ITEMID_SOZAI109 = 2064,
    ITEMID_SOZAI110 = 2065,
    ITEMID_SOZAI111 = 2066,
    ITEMID_SOZAI112 = 2067,
    ITEMID_SOZAI113 = 2068,
    ITEMID_SOZAI114 = 2069,
    ITEMID_SOZAI115 = 2070,
    ITEMID_SOZAI116 = 2071,
    ITEMID_SOZAI117 = 2072,
    ITEMID_SOZAI118 = 2073,
    ITEMID_SOZAI119 = 2074,
    ITEMID_SOZAI120 = 2075,
    ITEMID_SOZAI121 = 2076,
    ITEMID_SOZAI122 = 2077,
    ITEMID_SOZAI123 = 2078,
    ITEMID_SOZAI124 = 2079,
    ITEMID_SOZAI125 = 2080,
    ITEMID_SOZAI126 = 2081,
    ITEMID_SOZAI127 = 2082,
    ITEMID_SOZAI128 = 2083,
    ITEMID_SOZAI129 = 2084,
    ITEMID_SOZAI130 = 2085,
    ITEMID_SOZAI131 = 2086,
    ITEMID_SOZAI132 = 2087,
    ITEMID_SOZAI133 = 2088,
    ITEMID_SOZAI134 = 2089,
    ITEMID_SOZAI135 = 2090,
    ITEMID_SOZAI136 = 2091,
    ITEMID_SOZAI137 = 2092,
    ITEMID_SOZAI138 = 2093,
    ITEMID_SOZAI139 = 2094,
    ITEMID_SOZAI140 = 2095,
    ITEMID_SOZAI141 = 2096,
    ITEMID_SOZAI142 = 2097,
    ITEMID_SOZAI143 = 2098,
    ITEMID_SOZAI144 = 2099,
    ITEMID_SOZAI148 = 2103,
    ITEMID_SOZAI149 = 2104,
    ITEMID_SOZAI150 = 2105,
    ITEMID_SOZAI151 = 2106,
    ITEMID_SOZAI152 = 2107,
    ITEMID_SOZAI153 = 2108,
    ITEMID_SOZAI154 = 2109,
    ITEMID_SOZAI155 = 2110,
    ITEMID_SOZAI156 = 2111,
    ITEMID_SOZAI157 = 2112,
    ITEMID_SOZAI158 = 2113,
    ITEMID_SOZAI159 = 2114,
    ITEMID_SOZAI160 = 2115,
    ITEMID_SOZAI161 = 2116,
    ITEMID_SOZAI162 = 2117,
    ITEMID_SOZAI163 = 2118,
    ITEMID_SOZAI164 = 2119,
    ITEMID_SOZAI165 = 2120,
    ITEMID_SOZAI166 = 2121,
    ITEMID_SOZAI167 = 2122,
    ITEMID_SOZAI168 = 2123,
    ITEMID_SOZAI171 = 2126,
    ITEMID_SOZAI172 = 2127,
    ITEMID_SOZAI173 = 2128,
    ITEMID_SOZAI174 = 2129,
    ITEMID_SOZAI175 = 2130,
    ITEMID_SOZAI176 = 2131,
    ITEMID_SOZAI177 = 2132,
    ITEMID_SOZAI178 = 2133,
    ITEMID_SOZAI179 = 2134,
    ITEMID_SOZAI180 = 2135,
    ITEMID_SOZAI181 = 2136,
    ITEMID_SOZAI182 = 2137,
    ITEMID_SOZAI201 = 2156,
    ITEMID_SOZAI202 = 2157,
    ITEMID_SOZAI203 = 2158,
    ITEMID_SOZAI204 = 2159,
    ITEMID_WAZAMASIN100 = 2160,
    ITEMID_WAZAMASIN101 = 2161,
    ITEMID_WAZAMASIN102 = 2162,
    ITEMID_WAZAMASIN103 = 2163,
    ITEMID_WAZAMASIN104 = 2164,
    ITEMID_WAZAMASIN105 = 2165,
    ITEMID_WAZAMASIN106 = 2166,
    ITEMID_WAZAMASIN107 = 2167,
    ITEMID_WAZAMASIN108 = 2168,
    ITEMID_WAZAMASIN109 = 2169,
    ITEMID_WAZAMASIN110 = 2170,
    ITEMID_WAZAMASIN111 = 2171,
    ITEMID_WAZAMASIN112 = 2172,
    ITEMID_WAZAMASIN113 = 2173,
    ITEMID_WAZAMASIN114 = 2174,
    ITEMID_WAZAMASIN115 = 2175,
    ITEMID_WAZAMASIN116 = 2176,
    ITEMID_WAZAMASIN117 = 2177,
    ITEMID_WAZAMASIN118 = 2178,
    ITEMID_WAZAMASIN119 = 2179,
    ITEMID_WAZAMASIN120 = 2180,
    ITEMID_WAZAMASIN121 = 2181,
    ITEMID_WAZAMASIN122 = 2182,
    ITEMID_WAZAMASIN123 = 2183,
    ITEMID_WAZAMASIN124 = 2184,
    ITEMID_WAZAMASIN125 = 2185,
    ITEMID_WAZAMASIN126 = 2186,
    ITEMID_WAZAMASIN127 = 2187,
    ITEMID_WAZAMASIN128 = 2188,
    ITEMID_WAZAMASIN129 = 2189,
    ITEMID_WAZAMASIN130 = 2190,
    ITEMID_WAZAMASIN131 = 2191,
    ITEMID_WAZAMASIN132 = 2192,
    ITEMID_WAZAMASIN133 = 2193,
    ITEMID_WAZAMASIN134 = 2194,
    ITEMID_WAZAMASIN135 = 2195,
    ITEMID_WAZAMASIN136 = 2196,
    ITEMID_WAZAMASIN137 = 2197,
    ITEMID_WAZAMASIN138 = 2198,
    ITEMID_WAZAMASIN139 = 2199,
    ITEMID_WAZAMASIN140 = 2200,
    ITEMID_WAZAMASIN141 = 2201,
    ITEMID_WAZAMASIN142 = 2202,
    ITEMID_WAZAMASIN143 = 2203,
    ITEMID_WAZAMASIN144 = 2204,
    ITEMID_WAZAMASIN145 = 2205,
    ITEMID_WAZAMASIN146 = 2206,
    ITEMID_WAZAMASIN147 = 2207,
    ITEMID_WAZAMASIN148 = 2208,
    ITEMID_WAZAMASIN149 = 2209,
    ITEMID_WAZAMASIN150 = 2210,
    ITEMID_WAZAMASIN151 = 2211,
    ITEMID_WAZAMASIN152 = 2212,
    ITEMID_WAZAMASIN153 = 2213,
    ITEMID_WAZAMASIN154 = 2214,
    ITEMID_WAZAMASIN155 = 2215,
    ITEMID_WAZAMASIN156 = 2216,
    ITEMID_WAZAMASIN157 = 2217,
    ITEMID_WAZAMASIN158 = 2218,
    ITEMID_WAZAMASIN159 = 2219,
    ITEMID_WAZAMASIN160 = 2220,
    ITEMID_WAZAMASIN161 = 2221,
    ITEMID_WAZAMASIN162 = 2222,
    ITEMID_WAZAMASIN163 = 2223,
    ITEMID_WAZAMASIN164 = 2224,
    ITEMID_WAZAMASIN165 = 2225,
    ITEMID_WAZAMASIN166 = 2226,
    ITEMID_WAZAMASIN167 = 2227,
    ITEMID_WAZAMASIN168 = 2228,
    ITEMID_WAZAMASIN169 = 2229,
    ITEMID_WAZAMASIN170 = 2230,
    ITEMID_WAZAMASIN171 = 2231,
    ITEMID_PIKUNIKKUSETTO = 2311,
    ITEMID_SUITOU1 = 2313,
    ITEMID_SUITOU2 = 2314,
    ITEMID_SUITOU3 = 2315,
    ITEMID_SUITOU4 = 2316,
    ITEMID_SUITOU5 = 2317,
    ITEMID_KOPPU1 = 2318,
    ITEMID_KOPPU2 = 2319,
    ITEMID_KOPPU3 = 2320,
    ITEMID_KOPPU4 = 2321,
    ITEMID_KOPPU5 = 2322,
    ITEMID_TEEBURUKABAA1 = 2323,
    ITEMID_TEEBURUKABAA2 = 2324,
    ITEMID_TEEBURUKABAA3 = 2325,
    ITEMID_TEEBURUKABAA4 = 2326,
    ITEMID_TEEBURUKABAA5 = 2327,
    ITEMID_PIKUNIKKUBOORU1 = 2329,
    ITEMID_PIKUNIKKUBOORU2 = 2330,
    ITEMID_PIKUNIKKUBOORU3 = 2331,
    ITEMID_PIKUNIKKUBOORU4 = 2332,
    ITEMID_PIKUNIKKUBOORU5 = 2333,
    ITEMID_RANCHIPIKKU1 = 2334,
    ITEMID_RANCHIPIKKU2 = 2335,
    ITEMID_RANCHIPIKKU3 = 2336,
    ITEMID_RANCHIPIKKU4 = 2337,
    ITEMID_RANCHIPIKKU5 = 2338,
    ITEMID_RANCHIPIKKU6 = 2339,
    ITEMID_RANCHIPIKKU7 = 2340,
    ITEMID_RANCHIPIKKU8 = 2341,
    ITEMID_RANCHIPIKKU9 = 2342,
    ITEMID_IWAINOYOROI = 2344,
    ITEMID_KASIRANOAKASI = 2345,
    ITEMID_SUITOU6 = 2348,
    ITEMID_SUITOU7 = 2349,
    ITEMID_SUITOU8 = 2350,
    ITEMID_SUITOU9 = 2351,
    ITEMID_SUITOU10 = 2352,
    ITEMID_SUITOU11 = 2353,
    ITEMID_SUITOU12 = 2354,
    ITEMID_KOPPU6 = 2355,
    ITEMID_KOPPU7 = 2356,
    ITEMID_KOPPU8 = 2357,
    ITEMID_KOPPU9 = 2358,
    ITEMID_KOPPU10 = 2359,
    ITEMID_KOPPU11 = 2360,
    ITEMID_KOPPU12 = 2361,
    ITEMID_KOPPU13 = 2362,
    ITEMID_KOPPU14 = 2363,
    ITEMID_KOPPU15 = 2364,
    ITEMID_PIKUNIKKUBOORU6 = 2365,
    ITEMID_TEEBURUKABAA6 = 2366,
    ITEMID_TEEBURUKABAA7 = 2367,
    ITEMID_TEEBURUKABAA8 = 2368,
    ITEMID_TEEBURUKABAA9 = 2369,
    ITEMID_TEEBURUKABAA10 = 2370,
    ITEMID_TEEBURUKABAA11 = 2371,
    ITEMID_TEEBURUKABAA12 = 2372,
    ITEMID_TEEBURUKABAA13 = 2373,
    ITEMID_TEEBURUKABAA14 = 2374,
    ITEMID_TEEBURUKABAA15 = 2375,
    ITEMID_TEEBURUKABAA16 = 2376,
    ITEMID_TEEBURUKABAA17 = 2377,
    ITEMID_TEEBURUKABAA18 = 2378,
    ITEMID_TEEBURUKABAA19 = 2379,
    ITEMID_TEEBURUKABAA20 = 2380,
    ITEMID_SUITOU13 = 2381,
    ITEMID_SUITOU14 = 2382,
    ITEMID_KOPPU16 = 2383,
    ITEMID_KOPPU17 = 2384,
    ITEMID_RANCHIPIKKU10 = 2385,
    ITEMID_RANCHIPIKKU11 = 2386,
    ITEMID_RANCHIPIKKU12 = 2387,
    ITEMID_RANCHIPIKKU13 = 2388,
    ITEMID_RANCHIPIKKU14 = 2389,
    ITEMID_RANCHIPIKKU15 = 2390,
    ITEMID_RANCHIPIKKU16 = 2391,
    ITEMID_RANCHIPIKKU17 = 2392,
    ITEMID_RANCHIPIKKU18 = 2393,
    ITEMID_RANCHIPIKKU19 = 2394,
    ITEMID_SARA2 = 2396,
}
