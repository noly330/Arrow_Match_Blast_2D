using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectButton : MonoBehaviour
{
    public int mapID;
    public TextMeshProUGUI _chapterTextUGUI;

    private void OnEnable()
    {
        //_chapterTextUGUI.text = mapID.ToString();
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            EventCenter.Broadcast<Events.OnLoadMapSucceed>(new Events.OnLoadMapSucceed { mapID = mapID });
        });
    }

    private void OnDisable()
    {
        //_chapterTextUGUI.text = "";
        transform.GetComponent<Button>().onClick.RemoveListener(() =>
        {
            EventCenter.Broadcast<Events.OnLoadMapSucceed>(new Events.OnLoadMapSucceed { mapID = mapID });
        });
    }
}
