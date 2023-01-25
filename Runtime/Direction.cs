namespace Lignus.HexTile
{
    public enum Direction
    {
        /// (1, 0)
        BottomRight = 0,

        /// (1, -1)
        TopRight = 1,

        /// (0, -1)
        Top = 2,

        /// (-1, 0)
        TopLeft = 3,

        /// (-1, 1)
        BottomLeft = 4,

        /// (0, 1)
        Bottom = 5,
    }
}