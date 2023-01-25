using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lignus.HexTile
{
    public class SimpleGrid : MonoBehaviour
    {
        [SerializeField]private Layout _hexLayout;
        [SerializeField] private int _range = 5;

        [SerializeField] private GameObject _tilePrefab;

        private Dictionary<Hex, GameObject> _tiles = new();

        // Start is called before the first frame update
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
                _tiles.Add(hex, tile);
            }
        }
    }
}
