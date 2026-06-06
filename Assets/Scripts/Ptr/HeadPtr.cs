using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HeadPtr : MonoBehaviour
{
    private GameMapSO _gameMapSO;
    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnArrowClicked>(OnArrowClicked);
    }

    private void OnDisable() {
        EventCenter.RemoveListener<Events.OnArrowClicked>(OnArrowClicked);


    }

    private void OnArrowClicked(Events.OnArrowClicked clicked)
    {
        _gameMapSO = MapManager.Instance.currentMap;
        MoveArrowHead(clicked.arrowID).Forget();
    }

    private async UniTask MoveArrowHead(int arrowID)
    {
        if(_gameMapSO == null)
        {
            return;
        }

        int headIndex = _gameMapSO.lines[arrowID].points.Count - 1;
        string headID = _gameMapSO.lines[arrowID].points[headIndex].id;
        BoardPoint headPoint = MapManager.Instance.pointsDic[headID];
        if(headPoint == null)
        {
            return;
        }
        if (!headPoint.isOccupied)
        {
            return;
        }
        headPoint.MoveArrowHead().Forget();
        headPoint.IncreaseArrowHeadBody().Forget();
    }
}
