using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lignus.HexTile
{
    public class SimpleGrid : MonoBehaviour
    {
        [SerializeField] private Layout _hexLayout;
        public Layout HexLayout => _hexLayout;
        [SerializeField] private int _range = 5;

        [SerializeField] private GameObject _tilePrefab;

        [Header("Debug")] [SerializeField] private bool _displayCoordinates;

        private readonly Dictionary<Hex, GameObject> _tiles = new();

        void Start()
        {
            _hexLayout.HexOrientation = Orientation.Pointy();
            
            InitializeGrid();
        }

        private void InitializeGrid()
        {
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
        
        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_displayCoordinates && _tiles.Count > 0)
            {
                UnityEditor.Handles.color = Color.white;
                foreach (var tile in _tiles)
                {
                    UnityEditor.Handles.Label(tile.Value.transform.position, tile.Key.ToString());
                }
            }
        }

        #endif
    }
}
