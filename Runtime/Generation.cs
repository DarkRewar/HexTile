using System.Collections.Generic;
using System.Linq;

namespace Lignus.HexTile
{
    public static class Generation
    {
        public static List<Hex> Parallelogram(Hex min, Hex max) =>
            Enumerable.Range(min.q, max.q - min.q + 1).SelectMany(
                x => Enumerable.Range(min.r, max.r - min.r + 1),
                (x, y) => new Hex(x, y)
            ).ToList();

        public static List<Hex> Triangle(int size) => Triangle(Hex.Zero, size);

        public static List<Hex> Triangle(Hex pos, int size) =>
            Enumerable.Range(pos.q, size + 1).SelectMany(
                x => Enumerable.Range(pos.r, size - x + 1),
                (x, y) => new Hex(x, y)
            ).ToList();

        public static List<Hex> Hexagon(int radius) => Hex.Zero.Range(radius);

        public static List<Hex> Hexagon(Hex pos, int radius) => pos.Range(radius);

        public static List<Hex> PointyRectangle(int left, int right, int top, int bottom) =>
            Enumerable.Range(top, bottom - top + 1).SelectMany(
                y =>
                {
                    var yOffset = y >> 1;
                    var start = left - yOffset;
                    var end = right - yOffset - start + 1;
                    return Enumerable.Range(start, end);
                },
                (y, x) => new Hex(x, y)
            ).ToList();

        public static List<Hex> FlatRectangle(int left, int right, int top, int bottom) =>
            Enumerable.Range(left, right - left + 1).SelectMany(
                x =>
                {
                    var xOffset = x >> 1;
                    var start = top - xOffset;
                    var end = bottom - xOffset - start + 1;
                    return Enumerable.Range(start, end);
                },
                (x, y) => new Hex(x, y)
            ).ToList();
    }
}