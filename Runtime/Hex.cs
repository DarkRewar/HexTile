using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace Lignus.HexTile
{
    public struct Hex
    {
        public static Hex Zero => (0, 0);
        public static Hex One => new(1, 1);
        public static Hex Right => new(1, 0);
        public static Hex Up => new(1, 0);

        public static Hex[] NeighborsCoordinates = new[]
        {
            new Hex(1, 0), new Hex(1, -1), new Hex(0, -1), new Hex(-1, 0), new Hex(-1, 1),
            new Hex(0, 1),
        };
        
        public static Hex[] DiagonalCoordinates = new[]
        {
            new Hex(2, -1), new Hex(1, -2), new Hex(-1, -1), new Hex(-2, 1), new Hex(-1, 2),
            new Hex(1, 1),
        };

        public int q;
        public int r;
        public int s => -q - r;

        public int Length => (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2;

        public List<Hex> Neighbors
        {
            get
            {
                var self = this;
                return NeighborsCoordinates.Select(x => self + x).ToList();
            }
        }

        public Hex(int v) => q = r = v;
        
        public Hex(int x, int y)
        {
            q = x;
            r = y;
        }
        
        public Hex(int x, int y, int z)
        {
            if (x + y + z != 0) 
                throw new ArgumentException($"Wrong hex coordinates. Expected 0, got {x+y+z}");
            q = x;
            r = y;
        }

        public override string ToString() => $"Hex({q}, {r}, {s})";

        public Hex RotateLeft() => new(-s, -q);
        public Hex RotateLeftAround(Hex anchor) => (this - anchor).RotateLeft() + anchor;
        public Hex RotateRight() => new(-r, -s);
        public Hex RotateRightAround(Hex anchor) => (this - anchor).RotateRight() + anchor;
        public int DistanceTo(Hex hex) => Distance(this, hex);

        public Hex NeighborCoord(int direction) => NeighborsCoordinates[direction];

        public Hex Neighbor(int direction) => this + NeighborCoord(direction);

        public List<Hex> Ring(int range)
        {
            if (range <= 0)
                throw new ArgumentException("Range must be superior to 0");

            var hex = this + (NeighborsCoordinates[(int)Direction.BottomLeft] * range);
            var res = new List<Hex>();

            foreach (var dir in NeighborsCoordinates)
            {
                for (int j = 0; j < range; ++j)
                {
                    res.Add(hex);
                    hex += dir;
                }
            }

            return res;
        }

        public List<Hex> LineTo(Hex hex)
        {
            var distance = DistanceTo(hex);
            var (a, b) = (
                new Vector2(q, r),
                new Vector2(hex.q, hex.r)
            );

            List<Hex> _hexes = new();
            for (int step = 0; step < distance; step++)
            {
                var tempHex = Vector2.Lerp(a, b, (float) step / distance);
                _hexes.Add(Round(tempHex.x, tempHex.y));
            }

            return _hexes;
        }

        public List<Hex> Range(int range)
        {
            List<Hex> hexes = new();
            foreach (var x in Enumerable.Range(-range, range))
            {
                foreach (var y in Enumerable.Range(Math.Max(-range, -x - range), Math.Min(range, range - x)))
                {
                    hexes.Add((x, y));
                }
            }

            return hexes;
        }
        
        public List<Hex> Range(int range, Hex offset) => 
            Range(range).Select(hex => hex + offset).ToList();

        public static Hex Round(float x, float y)
        {
            var (q, r) = (Mathf.RoundToInt(x), Mathf.RoundToInt(y));
            x -= Mathf.RoundToInt(x); // remainder
            y -= Mathf.RoundToInt(y); // remainder
            if (x * x >= y * y)
                q += Mathf.RoundToInt(0.5f * y + x);

            if (x * x < y * y)
                r += Mathf.RoundToInt(0.5f * x + y);

            return new Hex(q, r);
        }
        
        public static int Distance(Hex a, Hex b) => (a - b).Length;

        public static implicit operator Hex((int q, int r) value) => new Hex(value.q, value.r);

        public static Hex operator +(Hex left, Hex right) =>
            new(left.q + right.q, left.r + right.r);
        public static Hex operator +(Hex left, int i) =>
            new(left.q + i, left.r + i);

        public static Hex operator -(Hex left, Hex right) =>
            new(left.q - right.q, left.r - right.r);
        public static Hex operator -(Hex left, int i) =>
            new(left.q - i, left.r - i);

        public static Hex operator *(Hex left, Hex right) =>
            new(left.q * right.q, left.r * right.r);
        public static Hex operator *(Hex left, int right) =>
            new(left.q * right, left.r * right);

        public static Hex operator /(Hex left, Hex right) =>
            new(left.q / right.q, left.r / right.r);
        public static Hex operator /(Hex left, int right) =>
            new(left.q / right, left.r / right);
    }
}