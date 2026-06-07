using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterTextUI : MonoBehaviour
{
    private TextMeshProUGUI _chapterTextUGUI;

    private void Awake()
    {
        _chapterTextUGUI = transform.GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
    }
    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
    }
    private void OnLoadMapSucceed(Events.OnLoadMapSucceed succeed)
    {
        _chapterTextUGUI.text = $"第 {succeed.mapID} 关";
    }
}
