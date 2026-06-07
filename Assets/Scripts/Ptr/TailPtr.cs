using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TailPtr : MonoBehaviour
{
    private GameMapSO _gameMapSO;
    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnArrowClickSucceed>(OnArrowClickSucceed);
        EventCenter.AddListener<Events.OnArrowClickFail>(OnArrowClickFail);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnArrowClickSucceed>(OnArrowClickSucceed);
        EventCenter.RemoveListener<Events.OnArrowClickFail>(OnArrowClickFail);
    }


    private void OnArrowClickSucceed(Events.OnArrowClickSucceed clicked)
    {
        _gameMapSO = MapManager.Instance.currentMap;
        ReduceSpriteAmount(clicked.arrowID).Forget();
    }


    private async UniTask ReduceSpriteAmount(int index)  //传的是线的索引
    {
        
        //先把所有BoardPoint的isOccupied设为false
        for (int i = 0; i < _gameMapSO.lines[index].points.Count; i++)
        {
            BoardPoint point = MapManager.Instance.pointsDic[_gameMapSO.lines[index].points[i].id];
            point.SetColor(Color.black);
            //当发现有BoardPoint没有被占用，说明这条线已经被清除了，直接返回
            if (!point.isOccupied)
            {
                return;
            }
            point.isOccupied = false;
        }

        //再执行ReduceSpriteAmount方法
        for (int i = 0; i < _gameMapSO.lines[index].points.Count - 1; i++)
        {
            BoardPoint point = MapManager.Instance.pointsDic[_gameMapSO.lines[index].points[i].id];
            await point.ReduceSpriteAmount(GameManager.Instance.reduceTime);
        }

        EventCenter.Broadcast<Events.OnArrowAllPointImageClear>(new Events.OnArrowAllPointImageClear
        {
            arrowID = index,
        });
    }
    private void OnArrowClickFail(Events.OnArrowClickFail message)
    {
        _gameMapSO = MapManager.Instance.currentMap;
        for (int i = 0; i < _gameMapSO.lines[message.arrowID].points.Count; i++)
        {
            BoardPoint point = MapManager.Instance.pointsDic[_gameMapSO.lines[message.arrowID].points[i].id];
            point.SetColor(Color.red);
        }
    }

    private async UniTask ClickFailNotice()
    {

    }
}
