using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    [SerializeField] private List<GameMapSO> _maps;
    public GameMapSO currentMap;
    [SerializeField] private BoardPoint _pointPrefab;
    [SerializeField] private Transform _pointContainer;
    [SerializeField] private Transform _arrowContainer;
    [SerializeField] private float _spacing = 0.4f;
    [SerializeField] private GameObject _arrowBodyPrefab;
    [SerializeField] private GameObject _arrowHeadPrefab;

    public Dictionary<string, BoardPoint> pointsDic = new Dictionary<string, BoardPoint>();
    private int _mapWidth;
    private int _mapHeight;

    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnLoadMap>(OnLoadMapSuccess);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnLoadMap>(OnLoadMapSuccess);
    }

    private void OnLoadMapSuccess(Events.OnLoadMap message)
    {
        ClearMap();
        GeneratePoints(message.mapID);
        GenerateArrow(message.mapID);
    }

    private void Start()
    {
        ClearMap();
        EventCenter.Broadcast<Events.OnLoadMap>(new Events.OnLoadMap { mapID = 0 });
    }
    public void ClearMap()
    {
        foreach (Transform child in _pointContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _arrowContainer)
        {
            Destroy(child.gameObject);
        }
        pointsDic.Clear();
    }
    public void GeneratePoints(int mapIndex)
    {
        
        if (mapIndex < 0 || mapIndex >= _maps.Count)
        {
            Debug.LogError($"Invalid map index: {mapIndex}");
            return;
        }

        currentMap = _maps[mapIndex];
        _mapWidth = currentMap.mapWidth;
        _mapHeight = currentMap.mapHeight;
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
                pointsDic.Add(point.id, point);
            }
        }
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
                    //初始化箭头对应的逻辑点，并生成箭头的Sprite
                    BoardPoint haedBoardPoint = pointsDic[line.points[j].id];
                    haedBoardPoint.lineID = i;
                    haedBoardPoint.direction = line.points[j].direction;
                    haedBoardPoint.isOccupied = true;
                    ArrowHead arrowHead = Instantiate(_arrowHeadPrefab, _arrowContainer).GetComponent<ArrowHead>();

                    //让他们互相引用
                    haedBoardPoint.SetSprite(arrowHead.GetComponent<SpriteRenderer>());
                    arrowHead.SetBoardPoint(haedBoardPoint);

                    //修正位置
                    Transform headChildPivot = arrowHead.pivotTransform;
                    arrowHead.transform.rotation = Quaternion.Euler(0, 0, ArrowDirection.GetRotationByDirection(line.points[j].direction));
                    Vector3 headPivotOffset = headChildPivot.position - arrowHead.transform.position;
                    arrowHead.transform.position = haedBoardPoint.transform.position - headPivotOffset;

                    
                    continue;
                }
                //初始化箭体对应的逻辑点，并生成箭体的Sprite
                BoardPoint boardPoint = pointsDic[line.points[j].id];
                boardPoint.lineID = i;
                boardPoint.direction = line.points[j].direction;
                boardPoint.isOccupied = true;
                ArrowBody arrowBody = Instantiate(_arrowBodyPrefab, _arrowContainer).GetComponent<ArrowBody>();
                //让他们互相引用
                boardPoint.SetSprite(arrowBody.GetComponent<SpriteRenderer>());
                arrowBody.SetBoardPoint(boardPoint);

                //修正位置
                // 用旋转后的世界偏移反推父物体位置，让 pivot 精准落在 BoardPoint 上。
                Transform childPivot = arrowBody.pivotTransform;
                arrowBody.transform.rotation = Quaternion.Euler(0, 0, ArrowDirection.GetRotationByDirection(line.points[j].direction));
                Vector3 pivotOffset = childPivot.position - arrowBody.transform.position;
                arrowBody.transform.position = boardPoint.transform.position - pivotOffset;
            }
        }
    }
}
