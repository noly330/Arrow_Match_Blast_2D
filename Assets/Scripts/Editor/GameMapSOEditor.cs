using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameMapSO))]
public class GameMapSOEditor : Editor
{
    private static readonly Color[] LineColors =
    {
        new Color(0.95f, 0.45f, 0.45f),
        new Color(0.45f, 0.75f, 1f),
        new Color(0.55f, 0.9f, 0.5f),
        new Color(1f, 0.85f, 0.35f),
        new Color(0.8f, 0.6f, 1f),
        new Color(1f, 0.6f, 0.85f),
    };

    private readonly List<Point> _draftPoints = new List<Point>();
    private char _selectedDirection = 'w';
    private bool _isDeletingLine;
    private bool _showRawLines;

    public override void OnInspectorGUI()
    {
        GameMapSO map = (GameMapSO)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mapID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mapWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mapHeight"));
        _showRawLines = EditorGUILayout.Foldout(_showRawLines, "Raw Lines");
        if (_showRawLines)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lines"), true);
        }
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(12f);
        DrawMapEditor(map);
    }

    private void DrawMapEditor(GameMapSO map)
    {
        EnsureLines(map);

        EditorGUILayout.LabelField("Visual Map Editor", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Choose a direction, click grid points in order, then click Confirm Line. In delete mode, left-click a saved line point to remove that line.", MessageType.Info);

        using (new EditorGUILayout.HorizontalScope())
        {
            DrawDirectionButton("Up (w)", 'w');
            DrawDirectionButton("Down (s)", 's');
            DrawDirectionButton("Left (a)", 'a');
            DrawDirectionButton("Right (d)", 'd');
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField($"Draft Points: {_draftPoints.Count}");

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = _draftPoints.Count > 0;
            if (GUILayout.Button("Undo Draft Point"))
            {
                _draftPoints.RemoveAt(_draftPoints.Count - 1);
            }

            if (GUILayout.Button("Clear Draft"))
            {
                _draftPoints.Clear();
            }

            if (GUILayout.Button("Confirm Line"))
            {
                Undo.RecordObject(map, "Add Map Line");
                Line line = new Line { points = new List<Point>() };
                foreach (Point point in _draftPoints)
                {
                    line.points.Add(new Point { id = point.id, direction = point.direction });
                }
                map.lines.Add(line);
                _draftPoints.Clear();
                EditorUtility.SetDirty(map);
            }
            GUI.enabled = true;
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (map.lines.Count == 0)
            {
                _isDeletingLine = false;
            }

            GUI.enabled = map.lines.Count > 0;
            bool deleteMode = GUILayout.Toggle(_isDeletingLine, "Delete Line Mode", "Button");
            if (deleteMode != _isDeletingLine)
            {
                _isDeletingLine = deleteMode;
            }

            if (GUILayout.Button("Clear All Lines"))
            {
                if (EditorUtility.DisplayDialog("Clear All Lines", "Remove all saved lines from this map?", "Clear", "Cancel"))
                {
                    Undo.RecordObject(map, "Clear Map Lines");
                    map.lines.Clear();
                    _isDeletingLine = false;
                    EditorUtility.SetDirty(map);
                }
            }
            GUI.enabled = true;
        }

        EditorGUILayout.Space(6f);
        DrawGrid(map);
    }

    private void DrawDirectionButton(string label, char direction)
    {
        bool wasSelected = _selectedDirection == direction;
        if (GUILayout.Toggle(wasSelected, label, "Button") && !wasSelected)
        {
            _selectedDirection = direction;
        }
    }

    private void DrawGrid(GameMapSO map)
    {
        if (map.mapWidth <= 0 || map.mapHeight <= 0)
        {
            EditorGUILayout.HelpBox("Set Map Width and Map Height above 0 to edit the grid.", MessageType.Warning);
            return;
        }

        Dictionary<string, SavedPointInfo> savedPoints = GetSavedPoints(map);
        Dictionary<string, int> draftPointIndexes = GetDraftPointIndexes();

        Color defaultColor = GUI.backgroundColor;
        Color defaultContentColor = GUI.contentColor;
        for (int y = map.mapHeight - 1; y >= 0; y--)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int x = 0; x < map.mapWidth; x++)
                {
                    string id = $"{x},{y}";
                    bool isDraft = draftPointIndexes.ContainsKey(id);
                    bool isSaved = savedPoints.ContainsKey(id);
                    int draftIndex = isDraft ? draftPointIndexes[id] : -1;
                    SavedPointInfo savedPoint = isSaved ? savedPoints[id] : default;
                    char direction = isDraft ? _draftPoints[draftIndex].direction : isSaved ? savedPoint.Direction : ' ';
                    int lineIndex = isSaved ? savedPoint.LineIndex : -1;
                    bool isHead = isSaved && savedPoint.IsHead;

                    GUI.backgroundColor = isDraft ? new Color(0.4f, 0.8f, 1f) : isHead ? new Color(0.12f, 0.12f, 0.12f) : isSaved ? savedPoint.Color : defaultColor;
                    GUI.contentColor = isHead && !isDraft ? Color.white : defaultContentColor;
                    DrawPointButton(map, id, direction, draftIndex, lineIndex, isHead);
                }
            }
        }
        GUI.backgroundColor = defaultColor;
        GUI.contentColor = defaultContentColor;
    }

    private void DrawPointButton(GameMapSO map, string id, char direction, int draftIndex, int lineIndex, bool isHead)
    {
        Rect rect = GUILayoutUtility.GetRect(52f, 42f, GUILayout.Width(52f), GUILayout.Height(42f));
        GUI.Box(rect, GetPointLabel(id, direction, draftIndex, lineIndex, isHead), GUI.skin.button);

        Event current = Event.current;
        if (current.type != EventType.MouseDown || !rect.Contains(current.mousePosition))
        {
            return;
        }

        if (current.button == 0)
        {
            if (_isDeletingLine)
            {
                if (lineIndex >= 0)
                {
                    Undo.RecordObject(map, "Remove Map Line");
                    map.lines.RemoveAt(lineIndex);
                    _isDeletingLine = map.lines.Count > 0;
                    EditorUtility.SetDirty(map);
                }
                current.Use();
                return;
            }

            if (draftIndex >= 0)
            {
                _draftPoints[draftIndex].direction = _selectedDirection;
            }
            else
            {
                _draftPoints.Add(new Point { id = id, direction = _selectedDirection });
            }
            current.Use();
        }
        else if (current.button == 1 && draftIndex >= 0)
        {
            _draftPoints.RemoveAt(draftIndex);
            current.Use();
        }
    }

    private static string GetPointLabel(string id, char direction, int draftIndex, int lineIndex, bool isHead)
    {
        if (draftIndex >= 0)
        {
            return $"{id}\n#{draftIndex} {direction}";
        }

        if (lineIndex >= 0)
        {
            return $"{id}\nL{lineIndex} {direction}";
        }

        return direction == ' ' ? id : $"{id}\n{direction}";
    }

    private static Dictionary<string, SavedPointInfo> GetSavedPoints(GameMapSO map)
    {
        Dictionary<string, SavedPointInfo> points = new Dictionary<string, SavedPointInfo>();
        for (int i = 0; i < map.lines.Count; i++)
        {
            Line line = map.lines[i];
            if (line == null || line.points == null)
            {
                continue;
            }

            for (int j = 0; j < line.points.Count; j++)
            {
                Point point = line.points[j];
                if (point != null && !string.IsNullOrEmpty(point.id))
                {
                    points[point.id] = new SavedPointInfo(point.direction, i, LineColors[i % LineColors.Length], j == line.points.Count - 1);
                }
            }
        }
        return points;
    }

    private Dictionary<string, int> GetDraftPointIndexes()
    {
        Dictionary<string, int> points = new Dictionary<string, int>();
        for (int i = 0; i < _draftPoints.Count; i++)
        {
            points[_draftPoints[i].id] = i;
        }
        return points;
    }

    private static void EnsureLines(GameMapSO map)
    {
        if (map.lines == null)
        {
            Undo.RecordObject(map, "Initialize Map Lines");
            map.lines = new List<Line>();
            EditorUtility.SetDirty(map);
        }
    }

    private struct SavedPointInfo
    {
        public readonly char Direction;
        public readonly int LineIndex;
        public readonly Color Color;
        public readonly bool IsHead;

        public SavedPointInfo(char direction, int lineIndex, Color color, bool isHead)
        {
            Direction = direction;
            LineIndex = lineIndex;
            Color = color;
            IsHead = isHead;
        }
    }
}
