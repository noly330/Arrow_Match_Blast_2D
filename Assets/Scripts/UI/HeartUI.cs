using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public GameObject[] heartImages;

    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnHeartUpdate>(OnHeartUpdate);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnHeartUpdate>(OnHeartUpdate);
    }

    private void OnHeartUpdate(Events.OnHeartUpdate update)
    {
        for(int i = 0; i < 3; i++)
        {
            heartImages[i].SetActive(i < GameManager.Instance.GetCurrentHealth());
        }
    }
}
