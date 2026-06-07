using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _failPanel;

    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
        EventCenter.AddListener<Events.OnGameWin>(OnGameWin);
        EventCenter.AddListener<Events.OnGameFail>(OnGameFail);
    }
    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
        EventCenter.RemoveListener<Events.OnGameWin>(OnGameWin);
        EventCenter.RemoveListener<Events.OnGameFail>(OnGameFail);
    }
    private void OnLoadMapSucceed(Events.OnLoadMapSucceed succeed)
    {
        _winPanel.SetActive(false);
        _failPanel.SetActive(false);
    }
    private void OnGameWin(Events.OnGameWin e)
    {
        _winPanel.SetActive(true);
        _failPanel.SetActive(false);
    }
    private void OnGameFail(Events.OnGameFail e)
    {
        _winPanel.SetActive(false);
        _failPanel.SetActive(true);
    }
}
