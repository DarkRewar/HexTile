using System;
using System.Linq;
using UnityEngine;

namespace Lignus.HexTile
{
    [Serializable]
    public class Layout
    {
        public Orientation HexOrientation;
        public Vector2 Origin;
        public Vector2 Size;

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

        public Vector2[] HexCorners(Hex hex)
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
    }
}