using UnityEngine;
using Cysharp.Threading.Tasks;

public class BoardPoint : MonoBehaviour
{
    public string id;
    public int lineID;
    public bool isOccupied;
    private SpriteRenderer _spriteRenderer;
    
    
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
            elapsed += Time.deltaTime;

            float nextHeight = Mathf.Lerp(startHeight, 0f, elapsed / time);
            _spriteRenderer.size = new Vector2(startWidth, nextHeight);
            await UniTask.Yield();
        }
        _spriteRenderer.transform.gameObject.SetActive(false);
    }
}
