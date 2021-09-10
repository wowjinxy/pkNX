using System.ComponentModel;
using FlatSharp.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
#nullable disable

namespace pkNX.Structures.FlatBuffers
{
    // Gates, Elevators?
    [FlatBufferTable, TypeConverter(typeof(ExpandableObjectConverter))]
    public class PlacementZone8_F01Holder
    {
        [FlatBufferItem(00)] public PlacementZone8_F01 Field_00 { get; set; }
    }

    [FlatBufferTable, TypeConverter(typeof(ExpandableObjectConverter))]
    public class PlacementZone8_F01
    {
        [FlatBufferItem(00)] public PlacementZoneMetaTripleXYZ8 Field_00 { get; set; }
        [FlatBufferItem(01)] public string Model { get; set; }
        // 2 empty table?
        [FlatBufferItem(03)] public float Field_03 { get; set; }
        [FlatBufferItem(04)] public float Field_04 { get; set; }
        // 5 empty table?
        // 6 empty table?
        [FlatBufferItem(11)] public PlacementZoneDeepY8 Unknown { get; set; } // maybe? or signed int
        [FlatBufferItem(12)] public byte Number { get; set; }
        // 13 empty object?
        // 14 object [?,object-empty]
    }
}
