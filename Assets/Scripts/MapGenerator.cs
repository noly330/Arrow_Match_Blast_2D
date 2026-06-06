using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameMapSO> _maps;
    [SerializeField] private BoardPoint _pointPrefab;
    [SerializeField] private Transform _pointContainer;
    [SerializeField] private Transform _arrowContainer;
    [SerializeField] private float _spacing = 0.4f;
    [SerializeField] private GameObject _arrowBodyPrefab;
    [SerializeField] private GameObject _arrowHeadPrefab;

    private Dictionary<string, BoardPoint> _pointsDic = new Dictionary<string, BoardPoint>();
    private int _mapWidth;
    private int _mapHeight;

    private void Start()
    {
        GeneratePoints(0);
    }

    public void GeneratePoints(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= _maps.Count)
        {
            Debug.LogError($"Invalid map index: {mapIndex}");
            return;
        }

        GameMapSO map = _maps[mapIndex];
        _mapWidth = map.mapWidth;
        _mapHeight = map.mapHeight;
        _spacing = 0.4f;

        if (_pointPrefab == null)
        {
            Debug.LogError("Point Prefab is missing.");
            return;
        }

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float posX = (x - (_mapWidth - 1) * 0.5f) * _spacing;
                float posY = (y - (_mapHeight - 1) * 0.5f) * _spacing;

                Vector3 position = new Vector3(posX, posY, 0f);
                BoardPoint point = Instantiate(_pointPrefab, position, Quaternion.identity, _pointContainer);
                point.id = $"{x},{y}";
                _pointsDic.Add(point.id, point);
            }
        }
        GenerateArrow(mapIndex);
    }

    private void GenerateArrow(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= _maps.Count)
        {
            Debug.LogError($"Invalid map index: {mapIndex}");
            return;
        }
        GameMapSO map = _maps[mapIndex];
        for (int i = 0; i < map.lines.Count; i++)
        {
            Line line = map.lines[i];
            for (int j = 0; j < line.points.Count; j++)
            {
                if (j == line.points.Count - 1)
                {
                    BoardPoint haedBoardPoint = _pointsDic[line.points[j].id];
                    ArrowHead arrowHead = Instantiate(_arrowHeadPrefab, _arrowContainer).GetComponent<ArrowHead>();

                    Transform headChildPivot = arrowHead.pivotTransform;
                    arrowHead.transform.rotation = Quaternion.Euler(0, 0, ArrowDirection.GetRotationByDirection(line.points[j].direction));
                    Vector3 headPivotOffset = headChildPivot.position - arrowHead.transform.position;
                    arrowHead.transform.position = haedBoardPoint.transform.position - headPivotOffset;
                    continue;
                }
                BoardPoint boardPoint = _pointsDic[line.points[j].id];
                ArrowBody arrowBody = Instantiate(_arrowBodyPrefab, _arrowContainer).GetComponent<ArrowBody>();

                Transform childPivot = arrowBody.pivotTransform;

                arrowBody.transform.rotation = Quaternion.Euler(0, 0, ArrowDirection.GetRotationByDirection(line.points[j].direction));

                // 用旋转后的世界偏移反推父物体位置，让 pivot 精准落在 BoardPoint 上。
                Vector3 pivotOffset = childPivot.position - arrowBody.transform.position;
                arrowBody.transform.position = boardPoint.transform.position - pivotOffset;
            }
        }
    }
}
