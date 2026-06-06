using UnityEngine;

public class ArrowBody : MonoBehaviour
{
    public Transform pivotTransform;
    private BoardPoint _boardPoint;
    private Collider2D _collider2D;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
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
        //将屏幕坐标转换为世界坐标
        Vector2 worldPosition = _camera.ScreenToWorldPoint(screenPosition);

        //检测点击的地方在不在碰撞体内部
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
