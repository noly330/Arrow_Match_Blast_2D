using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System;

public class BoardPoint : MonoBehaviour
{
    public Point pointInfo;
    public int lineID;
    public bool isOccupied;
    private SpriteRenderer _spriteRenderer;


    private void OnEnable()
    {
        isOccupied = false;

        EventCenter.AddListener<Events.OnArrowAllPointImageClear>(OnArrowAllPointImageClear);
    }


    private void OnDisable()
    {
        isOccupied = false;
        EventCenter.RemoveListener<Events.OnArrowAllPointImageClear>(OnArrowAllPointImageClear);
    }


    public void SetSprite(SpriteRenderer spriteRenderer)
    {
        this._spriteRenderer = spriteRenderer;
    }


    public async UniTask ReduceSpriteAmount(float time)
    {
        if (_spriteRenderer == null)
        {
            return;
        }
        float startWidth = _spriteRenderer.size.x;
        float startHeight = _spriteRenderer.size.y;
        float elapsed = 0f;
        while (elapsed < time)
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            elapsed += Time.deltaTime;

            float nextHeight = Mathf.Lerp(startHeight, 0f, elapsed / time);
            _spriteRenderer.size = new Vector2(startWidth, nextHeight);
            await UniTask.Yield();
        }

        if (_spriteRenderer == null)
        {
            return;
        }

        _spriteRenderer.transform.gameObject.SetActive(false);
    }

    public async UniTask MoveArrowHead()
    {
        if (_spriteRenderer == null)
        {
            return;
        }
        Debug.Log($"开始移动箭头 {pointInfo.id}");

        float elapsed = 0f;
        while (elapsed < 20f)
        {
            if (_spriteRenderer == null)
            {
                return;
            }
            elapsed += Time.deltaTime;
            Vector3 nextPosition = _spriteRenderer.transform.position + ArrowDirection.GetDirectionVector(pointInfo.direction) * GameManager.Instance.moveSpeed * Time.deltaTime;
            _spriteRenderer.transform.position = nextPosition;
            await UniTask.Yield();
        }
    }

    public bool isIncrease;
    public async UniTask IncreaseArrowHeadBody()
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        ArrowHead arrowHead = _spriteRenderer.GetComponent<ArrowHead>();
        if (arrowHead == null)
        {
            Debug.LogError("这个点上的图片不是箭头");
            return;
        }
        isIncrease = true;
        while (isIncrease)
        {
            if (_spriteRenderer == null || arrowHead == null || arrowHead.arrowHeadBody == null)
            {
                return;
            }

            float nextHeight = arrowHead.arrowHeadBody.size.y + GameManager.Instance.moveSpeed * 10f * Time.deltaTime;
            arrowHead.arrowHeadBody.size = new Vector2(arrowHead.arrowHeadBody.size.x, nextHeight);
            await UniTask.Yield();
        }

    }
    private void OnArrowAllPointImageClear(Events.OnArrowAllPointImageClear message)
    {
        if (message.arrowID == lineID)
        {
            isIncrease = false;
        }
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}
