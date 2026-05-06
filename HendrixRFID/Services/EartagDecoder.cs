namespace HendrixRFID.Services;

public class EartagDecoder
{
    private static readonly Dictionary<int, char> BreedMap = new()
    {
        { 1,  'X' }, // Danish Hybrid
        { 2,  'Y' }, // Yorkshire
        { 3,  'Z' }, // ZigZag
        { 4,  'D' }, // DG Duroc
        { 5,  'L' }, // Landrace
        { 6,  'P' }, // Pietrain → Maxter
        { 7,  'G' }, // Castrate (Galt gris)
        { 8,  'S' }, // Slaughterpig
        { 9,  'A' }, // Magnus (Hypor Duroc)
        { 10, 'K' }, // Kanto (Different Hypor Duroc)
        { 11, 'W' }, // Large White
        { 12, 'R' }, // Landrace (Hypor)
        { 13, 'H' }, // Hypor Libra (WR / RW)
        { 14, 'F' }, // Combination pig (WL / LW)
        { 15, 'N' }, // Combination pig (RY / YR)
    };

    // Partition => (companyBits, itemBits)
    private static readonly Dictionary<int, (int CompanyBits, int ItemBits)> PartitionMap = new()
    {
        { 0, (40, 4)  },
        { 1, (37, 7)  },
        { 2, (34, 10) },
        { 3, (30, 14) },
        { 4, (27, 17) },
        { 5, (24, 20) }, // <= Hendrix bruger denne 
        { 6, (20, 24) },
    };

    /// <summary>
    /// Dekoder en EPC hex-streng til et læseligt PigId.
    /// Eksempel: "3074257BF7194E4000003039" => "R6230112345"
    /// Returnerer null hvis formatet er ukendt.
    /// </summary>
    public string? Decode(string epcHex)
    {
        if (string.IsNullOrEmpty(epcHex) || epcHex.Length < 24)
            return null;

        string binStr = HexToBinary(epcHex);
        if (binStr.Length < 96)
            return null;

        // Læs partition dynamisk fra bits 11-13
        int partition = Convert.ToInt32(binStr.Substring(11, 3), 2);

        if (!PartitionMap.TryGetValue(partition, out var layout))
            return null;

        // Bit 14 er altid starten på company prefix
        int companyStart = 14;
        int itemStart    = companyStart + layout.CompanyBits;
        int serialStart  = itemStart + layout.ItemBits;

        // Tjek at vi har nok bits
        if (serialStart + 38 > binStr.Length)
            return null;

        // Udtræk de tre felter
        string itemRefBin   = binStr.Substring(itemStart, layout.ItemBits);
        string serialNumBin = binStr.Substring(serialStart, 38);

        uint  itemRef   = Convert.ToUInt32(itemRefBin, 2);
        ulong serialNum = Convert.ToUInt64(serialNumBin, 2);

        int   breedInt     = (int)(itemRef / 100);
        int   majorCounter = (int)(itemRef % 100);
        ulong farmNumber   = serialNum / 100_000;
        ulong minorCounter = serialNum % 100_000;

        if (!BreedMap.TryGetValue(breedInt, out char breedLetter))
            return null;

        // Formater som BFFFCCSSSSS — matcher PDF og fysisk øremærke
        return $"{breedLetter}{farmNumber:D3}{majorCounter:D2}{minorCounter:D5}";
    }

    private static string HexToBinary(string hex)
    {
        return string.Concat(hex.Select(c =>
            Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
        ));
    }
}