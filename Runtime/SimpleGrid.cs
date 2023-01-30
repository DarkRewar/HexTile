using System.Collections.Generic;
using UnityEngine;

namespace Lignus.HexTile
{
    public class SimpleGrid : MonoBehaviour
    {
        [SerializeField] private Layout _hexLayout;
        [SerializeField] private int _range = 5;

        [SerializeField] private GameObject _tilePrefab;

        private readonly Dictionary<Hex, GameObject> _tiles = new();

        void Start()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _hexLayout.HexOrientation = Orientation.Pointy();

            foreach (var hex in Generation.Hexagon(_range))
            {
                var tile = Instantiate(_tilePrefab, transform);
                var pos = _hexLayout.HexToWorldPoint(hex);
                tile.transform.position = new Vector3(pos.x, 0, pos.y);
                tile.name = $"Tile[{hex.q}, {hex.r}]";
                _tiles.Add(hex, tile);
            }

            foreach (var mirror in Hex.WraparoundMirrors(_range))
            {
                var tile = Instantiate(_tilePrefab, transform);
                var pos = _hexLayout.HexToWorldPoint(mirror);
                tile.transform.position = new Vector3(pos.x, 0, pos.y);
                tile.name = $"Tile[{mirror.q}, {mirror.r}]";
            }
        }
    }
}
