using System.ComponentModel;
using FlatSharp.Attributes;
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace pkNX.Structures.FlatBuffers;

[FlatBufferStruct, TypeConverter(typeof(ExpandableObjectConverter))]
public class Vector2f
{
    [FlatBufferItem(0)] public float X { get; set; }
    [FlatBufferItem(1)] public float Y { get; set; }
}
