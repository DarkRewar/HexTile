using System;
using System.Collections.Generic;
using System.Linq;

namespace Lignus.HexTile
{
    public static class Generation
    {
        public static List<Hex> Parallelogram(Hex min, Hex max) =>
            Enumerable.Range(min.q, max.q).SelectMany(
                x => Enumerable.Range(min.r, max.r),
                (x, y) => new Hex(x, y)
            ).ToList();
        
        public static List<Hex> Triangle(Hex pos, int size) =>
            Enumerable.Range(pos.q, pos.q + size).SelectMany(
                x => Enumerable.Range(pos.r - x, pos.r + size),
                (x, y) => new Hex(x, y)
            ).ToList();

        public static List<Hex> Hexagon(Hex pos, int radius)
        {
            List<Hex> hexes = new();
            
            for (int q = -radius; q <= radius; ++q)
            {
                int rStart = Math.Max(-radius, -q - radius),
                    rEnd = Math.Min(radius, -q + radius);
                for (int r = rStart; r <= rEnd; ++r)
                {
                    hexes.Add((q, r));
                }
            }
        
            return hexes;
        }

        // public static List<Hex> Hexagon(Hex pos, int radius) =>
        //     Enumerable.Range(pos.q - radius, 2 * radius).SelectMany(
        //         x => Enumerable.Range(
        //             Math.Max(pos.r - radius, pos.r - x - radius), 
        //             Math.Min(pos.r + radius, pos.r - x + radius) - Math.Max(pos.r - radius, pos.r - x - radius - 1)),
        //         (x, y) => new Hex(x, y)
        //     ).ToList();

        public static List<Hex> Hexagon(int radius) => Hexagon(Hex.Zero, radius);
            
        //     ((pos.x() - radius)..=(pos.x() + radius)).flat_map(move |x| {
        //     (((pos.y() - radius).max(pos.y() - x - radius))
        //             ..=((pos.y() + radius).min(pos.y() - x + radius)))
        //         .map(move |y| Hex::new(x, y))
        // })
        
        // /// Generates a rectangle with the given bounds for "pointy topped" hexagons
        // pub fn pointy_rectangle([left, right, top, bottom]: [i32; 4]) -> impl Iterator<Item = Hex> {
        //     (top..=bottom).flat_map(move |y| {
        //         let y_offset = y >> 1;
        //         ((left - y_offset)..=(right - y_offset)).map(move |x| Hex::new(x, y))
        //     })
        // }
        //
        // /// Generates a rectangle with the given bounds for "flat topped" hexagons
        // pub fn flat_rectangle([left, right, top, bottom]: [i32; 4]) -> impl Iterator<Item = Hex> {
        //     (left..=right).flat_map(move |x| {
        //         let x_offset = x >> 1;
        //         ((top - x_offset)..=(bottom - x_offset)).map(move |y| Hex::new(x, y))
        //     })
        // }
    }
}