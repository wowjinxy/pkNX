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
    [FlatBufferTable, TypeConverter(typeof(ExpandableObjectConverter))]
    public class PlacementZone8_F13Holder
    {
        [FlatBufferItem(00)] public PlacementZone8_F13 Field_00 { get; set; }
    }

    [FlatBufferTable, TypeConverter(typeof(ExpandableObjectConverter))]
    public class PlacementZone8_F13
    {
        [FlatBufferItem(00)] public PlacementZoneMetaTripleXYZ8 Field_00 { get; set; }
        [FlatBufferItem(01)] public ulong Hash_01 { get; set; }
        [FlatBufferItem(02)] public ulong Hash_02 { get; set; }
        [FlatBufferItem(03)] public uint Field_03 { get; set; }
        [FlatBufferItem(04)] public ulong Hash_04 { get; set; }
        [FlatBufferItem(05)] public uint Field_05 { get; set; }
        [FlatBufferItem(06)] public uint Field_06 { get; set; }
        // 7
        [FlatBufferItem(08)] public byte Byte_08 { get; set; }
        [FlatBufferItem(09)] public ulong Hash_09 { get; set; }
    }
}