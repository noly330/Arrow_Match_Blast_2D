using UnityEngine;

public class CameraMapController : MonoBehaviour
{
    [SerializeField] private float mapSizeScale = 0.2f;
    [SerializeField] private float mapSizeOffset = 2f;
    [SerializeField] private float mapSpacing = 0.4f;
    [SerializeField] private float edgePadding = 1.5f;
    [SerializeField] private float mouseZoomSpeed = 0.5f;
    [SerializeField] private float touchZoomSpeed = 0.01f;

    private Camera _camera;
    private float _baseSize;
    private Vector3 _lastDragWorldPosition;
    private bool _isDragging;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        EventCenter.AddListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Events.OnLoadMapSucceed>(OnLoadMapSucceed);
    }

    private void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
    }

    private void OnLoadMapSucceed(Events.OnLoadMapSucceed message)
    {
        GameMapSO map = GetMap(message.mapID);
        if (map == null)
        {
            return;
        }

        _baseSize = Mathf.Max(map.mapWidth, map.mapHeight) * mapSizeScale + mapSizeOffset;
        _camera.orthographicSize = _baseSize;

        float edgeX = (map.mapWidth - 1) * 0.5f * mapSpacing;
        float edgeY = (map.mapHeight - 1) * 0.5f * mapSpacing;
        _minX = -edgeX - edgePadding;
        _maxX = edgeX + edgePadding;
        _minY = -edgeY - edgePadding;
        _maxY = edgeY + edgePadding;

        transform.position = ClampPosition(new Vector3(0f, 0f, transform.position.z));
        _isDragging = false;
    }

    private void HandleMouseInput()
    {
        if (Input.touchCount > 0)
        {
            return;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (!Mathf.Approximately(scroll, 0f))
        {
            SetSize(_camera.orthographicSize - scroll * mouseZoomSpeed);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _lastDragWorldPosition = GetWorldPosition(Input.mousePosition);
            _isDragging = true;
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            Drag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _lastDragWorldPosition = GetWorldPosition(touch.position);
                _isDragging = true;
            }
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && _isDragging)
            {
                Drag(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isDragging = false;
            }
        }
        else if (Input.touchCount >= 2)
        {
            _isDragging = false;
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);
            Vector2 firstPrevious = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondPrevious = secondTouch.position - secondTouch.deltaPosition;
            float previousDistance = Vector2.Distance(firstPrevious, secondPrevious);
            float currentDistance = Vector2.Distance(firstTouch.position, secondTouch.position);

            SetSize(_camera.orthographicSize - (currentDistance - previousDistance) * touchZoomSpeed);
        }
    }

    private void Drag(Vector2 screenPosition)
    {
        Vector3 currentWorldPosition = GetWorldPosition(screenPosition);
        Vector3 delta = _lastDragWorldPosition - currentWorldPosition;
        transform.position = ClampPosition(transform.position + delta);
        _lastDragWorldPosition = GetWorldPosition(screenPosition);
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = transform.position.z;
        return worldPosition;
    }

    private void SetSize(float size)
    {
        _camera.orthographicSize = Mathf.Clamp(size, _baseSize - 2f, _baseSize + 2f);
        transform.position = ClampPosition(transform.position);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, _minX, _maxX);
        position.y = Mathf.Clamp(position.y, _minY, _maxY);
        return position;
    }

    private static GameMapSO GetMap(int mapID)
    {
        foreach (GameMapSO map in MapManager.Instance.GetMaps())
        {
            if (map != null && map.mapID == mapID)
            {
                return map;
            }
        }

        return null;
    }
}
