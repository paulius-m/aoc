using Tools;
using System.Numerics;

namespace Days.Y2021.Day16;


public class Solution : ISolution<byte[]>
{

    public async Task<byte[]> LoadInput()
    {
        var hex = (await File.ReadAllLinesAsync(this.GetInputFile("input"))).First();

        var big = BigInteger.Parse(hex, System.Globalization.NumberStyles.HexNumber);

        return big.ToByteArray().Reverse().ToArray();
    }

    public object Part1(byte[] input)
    {
        var r = new BitReader(input);

        var packet = ReadPacket(r);
        return Fold(packet);

        static long Fold(Packet p)
        {
            return p switch
            {
                OperatorPacket cp => cp.Version + cp.SubPackets.Sum(Fold),
                Literal l => l.Version
            };
        }
    }

    public object Part2(byte[] input)
    {
        var r = new BitReader(input);

        var packet = ReadPacket(r);
        return Fold(packet);

        static long Fold(Packet sp)
        {
            return sp switch
            {
                OperatorPacket cp => cp.Type switch
                {
                    PacketType.Sum => cp.SubPackets.Sum(Fold),
                    PacketType.Product => cp.SubPackets.Product(Fold),
                    PacketType.Minimum => cp.SubPackets.Min(Fold),
                    PacketType.Maximum => cp.SubPackets.Max(Fold),
                    PacketType.GreaterThan => Fold(cp.SubPackets[0]) > Fold(cp.SubPackets[1]) ? 1L : 0L,
                    PacketType.LessThan => Fold(cp.SubPackets[0]) < Fold(cp.SubPackets[1]) ? 1L : 0L,
                    PacketType.EqualTo => Fold(cp.SubPackets[0]) == Fold(cp.SubPackets[1]) ? 1L : 0L,
                    _ => throw new NotImplementedException(),
                },
                Literal l => l.Value
            };
        }
    }

    private static Packet ReadPacket(BitReader r)
    {
        var v = r.ReadBits(3);
        var t = (PacketType)r.ReadBits(3);

        if (t is PacketType.Literal)
        {
            return new Literal(GetLiteralValue(r), v, t);
        }

        return new OperatorPacket(GetSubPackets(r).ToArray(), v, t);
    }

    private static long GetLiteralValue(BitReader r)
    {
        var literal = 0L;
        long b;
        do
        {
            b = r.ReadBits(1);
            literal <<= 4;
            literal |= r.ReadBits(4);
        } while (b is not 0);
        return literal;
    }

    private static IEnumerable<Packet> GetSubPackets(BitReader r)
    {
        if (r.ReadBits(1) is 0)
        {
            var tlib = r.ReadBits(15);
            var end = r.CurrentPointer + tlib;
            while (r.CurrentPointer < end)
            {
                yield return ReadPacket(r);
            }
        }
        else
        {
            var nosp = r.ReadBits(11);
            for (var j = 0; j < nosp; j++)
            {
                yield return ReadPacket(r);
            }
        }
    }

    public record Packet(long Version, PacketType Type);
    public record class OperatorPacket(Packet[] SubPackets, long Version, PacketType Type) : Packet(Version, Type);
    public record class Literal(long Value, long Version, PacketType Type) : Packet(Version, Type);

    public enum PacketType
    {
        Sum = 0,
        Product = 1,
        Minimum = 2,
        Maximum = 3,
        Literal = 4,
        GreaterThan = 5,
        LessThan = 6,
        EqualTo = 7,
    }
}