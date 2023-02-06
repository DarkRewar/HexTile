using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lignus.HexTile
{
    [Serializable]
    public class Layout
    {
        public Orientation HexOrientation;
        public Vector2 Origin = Vector2.zero;
        public Vector2 Size = Vector2.one;

        public Vector2 HexToWorldPoint(Hex hex)
        {
            var matrix = HexOrientation.ForwardMatrix;
            return Origin + (Size / Orientation.SQRT_3) * new Vector2(
                matrix[0] * hex.q + matrix[1] * hex.r,
                matrix[2] * hex.q + matrix[3] * hex.r
            );
        }
        
        public Vector3 HexToWorldPoint3(Hex hex)
        {
            var point = HexToWorldPoint(hex);
            return new Vector3(point.x, 0, point.y);
        }

        public Hex WorldPosToHex(Vector3 pos) => WorldPosToHex(new Vector2(pos.x, pos.z));
        public Hex WorldPosToHex(Vector2 pos)
        {
            var matrix = HexOrientation.InverseMatrix;
            var point = (pos - Origin) / (Size / Orientation.SQRT_3);
            return Hex.Round(
                matrix[0] * point.x + matrix[1] * point.y,
                matrix[2] * point.x + matrix[3] * point.y
            );
        }

        public Vector2[] GetHexCorners(Hex hex)
        {
            var center = HexToWorldPoint(hex);
            var corners = new Vector2[6];
            foreach (var corner in Enumerable.Range(0, 6))
            {
                var angle = Math.PI * 2.0f * (HexOrientation.StartRotation + corner) / 6f;
                corners[corner] = center + new Vector2(
                    (Size.x / Orientation.SQRT_3) * (float)Math.Cos(angle),
                    (Size.y / Orientation.SQRT_3) * (float)Math.Sin(angle));
            }
            return corners;
        }

        public Vector3[] GetHexCorners3(Hex hex)
        {
            var center = HexToWorldPoint3(hex);
            var corners = new Vector3[6];
            foreach (var corner in Enumerable.Range(0, 6))
            {
                var angle = Math.PI * 2.0f * (HexOrientation.StartRotation + corner) / 6f;
                corners[5 - corner] = center + new Vector3(
                    (Size.x / Orientation.SQRT_3) * (float)Math.Cos(angle),
                    0,
                    (Size.y / Orientation.SQRT_3) * (float)Math.Sin(angle));
            }
            return corners;
        }

        public Vector3[] GetHexCorners3(List<Hex> hexes)
        {
            if (hexes.Count == 1) return GetHexCorners3(hexes[0]);
            
            var startHex = hexes.First(hex => hex.Neighbors.Any(neighbour => !hexes.Contains(neighbour)));
            List<Vector3> corners = new();

            int startDir;
            for (startDir = 0; startDir < 6; ++startDir)
            {
                if (hexes.Contains(startHex.Neighbor(startDir))) continue;
                break;
            }

            var hexCorners = GetHexCorners3(startHex);
            var firstCorner = hexCorners[startDir];
            var currentCorner = firstCorner;
            corners.Add(firstCorner);
            
            do
            {
                startDir = (startDir + 1) % 6;
                currentCorner = hexCorners[startDir];
                if (hexes.Contains(startHex.Neighbor(startDir)))
                {
                    startHex = startHex.Neighbor(startDir);
                    startDir = (startDir + 3) % 6;
                    hexCorners = GetHexCorners3(startHex);
                    currentCorner = hexCorners[(startDir + 1)%6];
                }
                else
                {
                    corners.Add(currentCorner);
                }
            } while (firstCorner != currentCorner);
            
            return corners.ToArray();
        }
    }
}