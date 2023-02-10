using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lignus.HexTile.Editor
{
    [Overlay(typeof(SceneView), "HexGrid", true)]
    public class HexGridEditor : Overlay
    {
        public static readonly Color GridColor = new(0.5f, 0.5f, 0.5f);
        public static readonly Color HexCursorColor = new(231f / 255, 76f / 255, 60f / 255, 0.8f);

        internal IMGUIContainer _imguiContainer { get; private set; }

        private Layout _hexLayout;
        private Plane _plane;
        private bool _isDragging = false;
        private Hex _lastHexClick;
        private bool _autoName = true;

        public override VisualElement CreatePanelContent()
        {
            _plane = new Plane(Vector3.up, Vector3.zero);

            _hexLayout = new Layout() { HexOrientation = Orientation.Pointy() };

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            this.displayedChanged += OnDisplayChanged;

            this._imguiContainer = new IMGUIContainer();

            var toggle = new Toggle() { label = "Display", value = true };
            toggle.RegisterCallback<ChangeEvent<bool>>(OnToggleChanged);
            this._imguiContainer.Add(toggle);

            var autoNames = new Toggle() { label = "Auto Names", value = true };
            autoNames.RegisterCallback<ChangeEvent<bool>>(x => _autoName = x.newValue);
            this._imguiContainer.Add(autoNames);

            return (VisualElement)this._imguiContainer;

        }

        private void OnToggleChanged(ChangeEvent<bool> evt) => OnDisplayChanged(evt.newValue);

        private void OnDisplayChanged(bool display)
        {
            if (SceneView.lastActiveSceneView is null) return;

            SceneView.lastActiveSceneView.showGrid = !display;
            SceneView.duringSceneGui -= OnSceneGUI;
            if (display)
            {
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (SceneView.lastActiveSceneView is null) return;

            var offset = _hexLayout.WorldPosToHex(sceneView.pivot);
            var hexes = Generation.Hexagon(offset, 10);
            Handles.color = GridColor;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

            foreach (Hex hex in hexes)
            {
                DrawHexBorder(hex);
            }

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (_plane.Raycast(ray, out float enter))
            {
                var position = ray.GetPoint(enter);
                var cursorHex = _hexLayout.WorldPosToHex(position);

                if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorClosed")
                {
                    var obj = EditorGUIUtility.GetObjectPickerObject();
                    if (obj is not null)
                    {
                        var gameObject = PrefabUtility.InstantiatePrefab(obj, Selection.activeTransform) as GameObject;
                        gameObject.transform.position = _hexLayout.HexToWorldPoint3(_lastHexClick);
                        if (_autoName)
                            gameObject.name = $"{gameObject.name} {_lastHexClick}";
                    }
                    _isDragging = false;
                }
                else if (e.type == EventType.MouseUp && e.button == 1)
                {
                    if (!_isDragging)
                    {
                        _lastHexClick = cursorHex;
                        EditorGUIUtility.ShowObjectPicker<GameObject>(
                            null,
                            false,
                            "t:prefab",
                            1);
                    }

                    _isDragging = false;
                }
                else if (e.type == EventType.MouseUp)
                {
                    _isDragging = false;
                }
                else if (e.type == EventType.MouseDrag && e.button == 0 && !e.alt && !e.functionKey)
                {
                    _isDragging = true;
                    GameObject[] selectedGameObjects = Selection.gameObjects;
                    if (selectedGameObjects.Length > 0)
                    {
                        var go = selectedGameObjects[0];
                        go.transform.SetPositionAndRotation(
                            _hexLayout.HexToWorldPoint3(cursorHex),
                            Quaternion.identity);
                        if (_autoName)
                            go.name = $"{PrefabUtility.GetCorrespondingObjectFromSource(go).name} {cursorHex}";
                        e.Use();
                    }
                }

                if (!_isDragging)
                {
                    DrawHex(cursorHex, HexCursorColor);
                }
            }

            HandleUtility.Repaint();
        }

        private void DrawHexBorder(Hex hex)
        {
            var corners = _hexLayout.GetHexCorners3(hex);
            DrawCorners(corners);
        }

        private void DrawHex(Hex hex, Color color)
        {
            var oldColor = Handles.color;
            Handles.color = color;

            var corners = _hexLayout.GetHexCorners3(hex);
            Handles.DrawAAConvexPolygon(corners);

            Handles.color = oldColor;
        }

        private void DrawCorners(Vector3[] corners)
        {
            for (int i = 0; i <= corners.Length - 2; ++i)
            {
                Handles.DrawLine(corners[i], corners[i + 1]);
            }
            Handles.DrawLine(corners[^1], corners[0]);
        }
    }
}