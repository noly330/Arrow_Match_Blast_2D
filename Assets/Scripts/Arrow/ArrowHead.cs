using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHead : MonoBehaviour
{
    public Transform pivotTransform;
    private BoardPoint _boardPoint;
    private Collider2D _collider2D;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    public void SetBoardPoint(BoardPoint boardPoint)
    {
        _boardPoint = boardPoint;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryClick(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryClick(Input.GetTouch(0).position);
        }
    }

    private void TryClick(Vector2 screenPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (_collider2D != null && _collider2D.OverlapPoint(worldPosition))
        {
            OnClicked();
        }
    }

    private void OnClicked()
    {
        int lineID = _boardPoint.lineID;
        EventCenter.Broadcast<Events.OnArrowClicked>(new Events.OnArrowClicked{
            arrowID = lineID,
        });
    }
}
