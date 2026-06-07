using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int _maxHealth = 3;
    private int _currentHealth = 3;
    public int GetCurrentHealth() => _currentHealth;

    [Header("表示箭身消失的时间，越小速度越快")]
    public float reduceTime = 0.1f;
    public float moveSpeed => (0.4f / reduceTime);

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
        EventCenter.AddListener<Events.OnArrowClickStart>(OnArrowClickStart);
        EventCenter.AddListener<Events.OnArrowClickFail>(OnArrowClickFail);
    }



    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
        EventCenter.RemoveListener<Events.OnArrowClickStart>(OnArrowClickStart);
        EventCenter.RemoveListener<Events.OnArrowClickFail>(OnArrowClickFail);
    }



    private void OnLoadMapSucceed(Events.OnLoadMapSucceed succeed)
    {
        _currentHealth = _maxHealth;
    }
    private void OnArrowClickFail(Events.OnArrowClickFail fail)
    {
        _currentHealth--;
        EventCenter.Broadcast<Events.OnHeartUpdate>(new Events.OnHeartUpdate());
        if(_currentHealth <= 0)
        {
            EventCenter.Broadcast<Events.OnGameFail>(new Events.OnGameFail());
        }
    }

    private void OnArrowClickStart(Events.OnArrowClickStart message)
    {
        Debug.Log(message.arrowID);
        if (CheckArrowCanMove(message.arrowID))
        {
            EventCenter.Broadcast<Events.OnArrowClickSucceed>(new Events.OnArrowClickSucceed{
                arrowID = message.arrowID,
            });
        }
        else
        {
            EventCenter.Broadcast<Events.OnArrowClickFail>(new Events.OnArrowClickFail{
                arrowID = message.arrowID,
            });
        }
    }


    private bool CheckArrowCanMove(int arrowID)
    {
        GameMapSO gameMapSO = MapManager.Instance.currentMap;
        Line line = gameMapSO.lines[arrowID];
        string headPointID = line.points[line.points.Count - 1].id;
        BoardPoint boardPoint = MapManager.Instance.pointsDic[headPointID];
        char direction = boardPoint.pointInfo.direction;

        string[] headPointPos = headPointID.Split(',');
        int x = int.Parse(headPointPos[0]);
        int y = int.Parse(headPointPos[1]);

        int offsetX = 0;
        int offsetY = 0;
        if (direction == 'w')
        {
            offsetY = 1;
        }
        else if (direction == 's')
        {
            offsetY = -1;
        }
        else if (direction == 'a')
        {
            offsetX = -1;
        }
        else if (direction == 'd')
        {
            offsetX = 1;
        }
        else
        {
            Debug.LogError($"Invalid direction: {direction}");
            return false;
        }

        int mapWidth = MapManager.Instance.GetMapWidth();
        int mapHeight = MapManager.Instance.GetMapHeight();
        x += offsetX;
        y += offsetY;

        while (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            string pointID = $"{x},{y}";
            if (MapManager.Instance.pointsDic[pointID].isOccupied)
            {
                return false;
            }

            x += offsetX;
            y += offsetY;
        }

        return true;
    }
}
