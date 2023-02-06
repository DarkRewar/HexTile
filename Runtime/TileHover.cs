using System;
using System.Linq;
using UnityEngine;

namespace Lignus.HexTile.Samples
{
    [RequireComponent(typeof(LineRenderer))]
    public class TileHover : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Camera _camera;
        [SerializeField] private SimpleGrid _grid;

        private void Update()
        {
            var mouse = Input.mousePosition;
            mouse.z = 1;
            var viewPoint = _camera.ScreenToWorldPoint(mouse);
            Ray ray = new(_camera.transform.position, viewPoint - _camera.transform.position);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000))
            {
                var hex = _grid.HexLayout.WorldPosToHex(hitInfo.point);
                var corners = _grid.HexLayout.GetHexCorners(hex);
                _lineRenderer.SetPositions(corners.Select(corner => new Vector3(corner.x, 0.3f, corner.y)).ToArray());
            }
        }
    }
}