using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ArrowMatchBlast/GameMapSO")]

public class GameMapSO : ScriptableObject
{
    public int mapID;
    public int mapWidth;
    public int mapHeight;
    public List<Line> lines;
}

[System.Serializable]
public class Line
{
    public List<Point> points;
}

[System.Serializable]
public class Point
{
    public string id;
    public char direction;
}