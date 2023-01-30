using System;
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

        public Hex WorldPosToHex(Vector2 pos)
        {
            var matrix = HexOrientation.InverseMatrix;
            var point = (pos - Origin) / (Size / Orientation.SQRT_3);
            return Hex.Round(
                matrix[0] * point.x + matrix[1] * point.y,
                matrix[2] * point.x + matrix[3] * point.y
            );
        }
    }
}