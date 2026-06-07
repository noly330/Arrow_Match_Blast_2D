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
        EventCenter.AddListener<Events.OnArrowClickSucceed>(OnArrowClickSucceed);
    }

    private void OnDisable() {
        EventCenter.RemoveListener<Events.OnArrowClickSucceed>(OnArrowClickSucceed);


    }

    private void OnArrowClickSucceed(Events.OnArrowClickSucceed clicked)
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
            Debug.LogError($"头点 {headID} 不存在");
            return;
        }
        headPoint.MoveArrowHead().Forget();

        if(_gameMapSO.lines[arrowID].points.Count == 1)
        {
            return;
        }
        headPoint.IncreaseArrowHeadBody().Forget();
    }
}
