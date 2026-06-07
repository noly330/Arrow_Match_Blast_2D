using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectPanel : MonoBehaviour
{
    [SerializeField] Transform _selectButtonCounter;
    [SerializeField] private SelectButton _selectButtonPrefab;

    private void Awake()
    {
        EventCenter.AddListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
    }

    private void OnLoadMapSucceed(Events.OnLoadMapSucceed succeed)
    {
        transform.gameObject.SetActive(false);
    }

    private void Start() {
        List<GameMapSO> maps = MapManager.Instance.GetMaps();
        foreach (var map in maps)
        {
            SelectButton button = Instantiate(_selectButtonPrefab, _selectButtonCounter.transform);
            button.mapID = map.mapID;
            button._chapterTextUGUI.text = map.mapID.ToString();
        }
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
    }
}
