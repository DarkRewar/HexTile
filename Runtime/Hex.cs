using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lignus.HexTile
{
    public struct Hex : IEquatable<Hex>
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
                throw new ArgumentException($"Wrong hex coordinates. Expected 0, got {x + y + z}");
            q = x;
            r = y;
        }

        public override string ToString() => $"Hex({q}, {r}, {s})";

        public bool Equals(Hex other) => q == other.q && r == other.r;

        public override bool Equals(object obj) => obj is Hex other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(q, r);

        public Hex RotateLeft() => new(-s, -q);
        public Hex RotateLeftAround(Hex anchor) => (this - anchor).RotateLeft() + anchor;
        public Hex RotateRight() => new(-r, -s);
        public Hex RotateRightAround(Hex anchor) => (this - anchor).RotateRight() + anchor;
        public int DistanceTo(Hex hex) => Distance(this, hex);

        public Hex NeighborCoord(int direction) => NeighborsCoordinates[direction];

        public Hex Neighbor(int direction) => this + NeighborCoord(direction);
        public Hex Neighbor(Direction direction) => this + NeighborCoord((int)direction);
        
        public List<Hex> Ring(uint range, Direction startDirection, bool clockwise)
        {
            if (range == 0) return new List<Hex>{this};

            List<Hex> res = new();
            Hex hex = this + NeighborsCoordinates[(int)startDirection] * (int)range;
            int start = clockwise ? NeighborsCoordinates.Length - 1 : 0,
                end = clockwise ?  -1 : NeighborsCoordinates.Length;
            for (int i = start; i != end; i += clockwise ? -1 : 1)
            {
                var way = clockwise 
                    ? i + (int)startDirection - 1
                    : i + (int) startDirection + 2;
                way %= NeighborsCoordinates.Length;
                var dir = way < 0 
                    ? NeighborsCoordinates[^Math.Abs(way)]
                    : NeighborsCoordinates[way];
                for (int j = 0; j < range; ++j)
                {
                    res.Add(hex);
                    hex += dir;
                }
            }

            return res;
        }

        public List<Hex> Ring(uint range) => Ring(range, Direction.BottomRight, false);

        public List<Hex> SpiralRange(uint range, Direction startDirection, bool clockwise)
        {
            var res = new List<Hex>();
            for (uint i = 0; i <= range; ++i)
            {
                res.AddRange(Ring(i, startDirection, clockwise));
            }

            return res;
        }

        public List<Hex> SpiralRange(uint range) => SpiralRange(range, Direction.BottomRight, false);

        public List<Hex> LineTo(Hex hex)
        {
            var distance = DistanceTo(hex);
            var (a, b) = (
                new Vector2(q, r),
                new Vector2(hex.q, hex.r)
            );

            List<Hex> hexes = new();
            for (int step = 0; step < distance; step++)
            {
                var tempHex = Vector2.Lerp(a, b, (float)step / distance);
                hexes.Add(Round(tempHex.x, tempHex.y));
            }

            return hexes;
        }

        public List<Hex> Range(int range)
        {
            var self = this;
            return  Enumerable.Range(-range, 2 * range + 1).SelectMany(
                x => Enumerable.Range(
                    Math.Max(-range, -x - range),
                    Math.Min(range, -x + range) - Math.Max(-range, -x - range) + 1),
                (x, y) => new Hex(x + self.q, y + self.r)
            ).ToList();
        }

        public Hex WrapInRange(int radius) => WrapWith(radius, WraparoundMirrors(radius));

        public Hex WrapWith(int radius, Hex[] mirrors)
        {
            if (Length <= radius) return this;

            foreach (var m in mirrors)
            {
                var p = m + this;
                if (p.Length <= radius) return p;
            }

            return this;
        }

        #region STATIC METHODS

        public static int RingCount(int range) => 6 * range;

        /// <summary>
        /// Computes the 6 mirror centers of the origin for hexagonal *wraparound* maps
        /// of given `radius`.
        ///
        /// # Notes
        /// * See [`Self::wrap_in_range`] for a usage
        /// * Use [`HexMap`] for improved wrapping
        /// * See this [article] for more information.
        ///
        /// [article]: https://www.redblobgames.com/grids/hexagons/#wraparound
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Hex[] WraparoundMirrors(int radius)
        {
            var mirror = new Hex(2 * radius + 1, -radius);
            return new Hex[] {
                mirror.RotateLeft(),
                mirror,
                mirror.RotateRight(),
                -mirror.RotateLeft(),
                -mirror,
                -mirror.RotateRight()
            };
        }

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

        #endregion

        #region OPERATORS

        public static implicit operator Hex((int q, int r) value) => new Hex(value.q, value.r);

        public static bool operator ==(Hex left, Hex right) => left.Equals(right);
        public static bool operator !=(Hex left, Hex right) => !left.Equals(right);

        public static Hex operator +(Hex left, Hex right) =>
            new(left.q + right.q, left.r + right.r);
        public static Hex operator +(Hex left, int right) =>
            new(left.q + right, left.r + right);

        public static Hex operator -(Hex self) => new(-self.q, -self.r);
        public static Hex operator -(Hex left, Hex right) =>
            new(left.q - right.q, left.r - right.r);
        public static Hex operator -(Hex left, int i) =>
            new(left.q - i, left.r - i);

        public static Hex operator *(Hex left, Hex right) =>
            new(left.q * right.q, left.r * right.r);
        public static Hex operator *(Hex left, int right) =>
            new(left.q * right, left.r * right);
        public static Hex operator *(int left, Hex right) =>
            new(left * right.q, left * right.r);

        public static Hex operator /(Hex left, Hex right) =>
            new(left.q / right.q, left.r / right.r);
        public static Hex operator /(Hex left, int right) =>
            new(left.q / right, left.r / right);

        #endregion
    }
}