using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Product : MonoBehaviour, IPoolable
{
    [SerializeField] private List<GameObject> productModels;
    public int ProductIndex { get; private set; }
    private ProductFocus productFocus;
    [SerializeField] private ClipProperty placeClip;
    void Awake()
    {
        productFocus = GetComponent<ProductFocus>();
    }
    public void SetProductIndex(int index)
    {
        ProductIndex = index;
        if (ProductIndex < 0 || ProductIndex >= productModels.Count)
        {
            Debug.LogError($"Invalid product model index: {index}");
            return;
        }

        foreach (var productModel in productModels)
        {
            productModel.SetActive(false);
        }

        productModels[ProductIndex].SetActive(true);
    }
    public void MoveToSlot(Slot slot, float delay = 0f)
    {
        slot.SetProduct(this, false);

        transform.DOKill();

        var seq = DOTween.Sequence();

        var jumpDuration = 0.5f;
        var squashTime = jumpDuration / 3f;

        Vector3 originalScale = transform.localScale;
        Vector3 stretchScale = new Vector3(0.9f, 1.3f, 0.9f);
        Vector3 squashScale = new Vector3(1.2f, 0.4f, 1.2f);

        seq.Append(transform.DOLocalJump(Vector3.zero, 5f, 1, jumpDuration).SetEase(Ease.OutQuad))
            .Join(transform.DOLocalRotateQuaternion(Quaternion.identity, jumpDuration).SetEase(Ease.OutQuad))
            .Join(transform.DOScale(stretchScale, squashTime).SetEase(Ease.OutSine))
            .Append(transform.DOScale(squashScale, squashTime).SetEase(Ease.InSine))
            .Append(transform.DOScale(originalScale, squashTime).SetEase(Ease.OutBack))
            .SetDelay(delay);

        seq.OnComplete(() =>
        {
            AudioManager.Instance.PlaySound(placeClip);
        });
    }

    public void SelectProduct()
    {
        productFocus.Focus();
    }
    public void DeselectProduct()
    {
        productFocus.Unfocus();
    }

    public void OnGet()
    {
    }

    public void OnReturn()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }
}
