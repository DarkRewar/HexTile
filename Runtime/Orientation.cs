namespace Lignus.HexTile
{
    public struct Orientation
    {
        public const float SQRT_3 = 1.732_050_8f;

        public float[] ForwardMatrix;
        public float[] InverseMatrix;
        public float StartRotation;

        public static Orientation Pointy() => new()
        {
            ForwardMatrix = new[] {SQRT_3, SQRT_3 / 2, 0, 3f / 2},
            InverseMatrix = new[] {SQRT_3 / 3, -1f / 3, 0, 2f / 3},
            StartRotation = 0.5f,
        };

        public static Orientation Flat() => new()
        {
            ForwardMatrix = new[] {3f / 2, 0, SQRT_3 / 2, SQRT_3},
            InverseMatrix = new[] {2f / 3, 0, -1f / 3, SQRT_3 / 3},
            StartRotation = 0,
        };
    }
}