using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ProductFocus : MonoBehaviour
{
    [SerializeField] private Transform productVisual;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatAmount = 0.5f;
    [SerializeField] private Vector3 floatDirection = Vector3.up;

    private Vector3 initialPosition;
    private bool isFloating = false;
    private float timeOffset;

    private void Update()
    {
        if (isFloating && productVisual != null)
        {
            float offset = Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatAmount;
            var targetPos = initialPosition + floatDirection.normalized * offset + Vector3.up * 1f;
            productVisual.localPosition = Vector3.Lerp(productVisual.transform.localPosition, targetPos, Time.deltaTime * 10f);
        }
    }

    public void Focus()
    {
        productVisual.DOKill();
        StartFloating();
    }

    public void Unfocus()
    {
        StopFloating();
        productVisual.DOKill();
        productVisual.DOLocalMoveY(0f, 0.15f);
    }

    private void StartFloating()
    {
        initialPosition = productVisual.localPosition;
        timeOffset = Random.Range(0f, 100f);
        isFloating = true;
    }

    private void StopFloating()
    {
        isFloating = false;
    }
}
