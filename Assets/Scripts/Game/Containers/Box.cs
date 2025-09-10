using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Box : Container, IPoolable
{
    public bool IsOpen;
    [SerializeField] private Product productPrefab;
    [SerializeField] private Animator animator;
    public BoxLayerController BoxLayerController { get; private set; }
    public BoxBeltController BoxBeltController { get; private set; }
    private BoxData boxData;
    void Awake()
    {
        BoxLayerController = GetComponent<BoxLayerController>();
        BoxBeltController = GetComponent<BoxBeltController>();
    }
    public void Init(BoxData boxData)
    {
        this.boxData = boxData;

        BoxBeltController.SpawnBelts(boxData.beltCount);
        BoxLayerController.CheckUpperBoxes();

        if (CanOpen())
        {
            OpenBox();
        }
        else
        {
            BoxLayerController.OnFree += CheckOpen;
            BoxBeltController.OnFree += CheckOpen;
        }

        OnSlotsUpdated += () =>
        {
            RemoveIfEmpty();
            SendIfFilled();
        };
    }
    private void CheckOpen()
    {
        if (CanOpen())
        {
            OpenBox();
        }
    }
    private bool CanOpen()
    {
        var isAboveFree = BoxLayerController.IsFree;
        var noBelt = BoxBeltController.IsFree;
        return isAboveFree && noBelt;
    }
    public void OpenBox()
    {
        for (int i = 0; i < 6; i++)
        {
            if (boxData.slots[i] >= 0)
            {
                var product = ObjectPooler.Instance.Get("Product").GetComponent<Product>();
                product.SetProductIndex(boxData.slots[i]);
                slots[i].SetProduct(product);
            }
        }

        IsOpen = true;
        Useable = true;
        animator.SetTrigger("Open");
    }
    private void CloseBox()
    {
        IsOpen = false;
        Useable = false;
        DOVirtual.DelayedCall(1f, () =>
        {
            animator.SetTrigger("Close");
        }, false);

    }
    public void RemoveIfEmpty()
    {
        if (slots.All(slot => slot.IsEmpty))
        {
            Useable = false;
            transform.DOScaleY(0f, .5f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    BoxController.Instance.OnBoxEmpty?.Invoke(this);
                }).SetDelay(.25f);
        }
    }
    public void SendIfFilled()
    {
        if (slots.All(slot => !slot.IsEmpty))
        {
            var firstProductId = slots[0].CurrentProduct.ProductIndex;
            if (slots.All(slot => slot.CurrentProduct.ProductIndex == firstProductId))
            {
                CloseBox();
                MoveEffect(() => BoxController.Instance.OnBoxFilled?.Invoke(this));
            }
        }
    }
    private void MoveEffect(Action OnMoveDone = null)
    {
        var isRight = transform.position.x > 0f;

        float prepOffsetX = isRight ? -1f : 1f;
        float targetX = isRight ? 10f : -10f;

        float rotationZ = isRight ? 20f : -20f;

        Quaternion initialRotation = transform.rotation;

        Quaternion prepTiltRotation = Quaternion.AngleAxis(-rotationZ * 0.5f, Vector3.forward) * initialRotation;
        Quaternion launchTiltRotation = Quaternion.AngleAxis(rotationZ, Vector3.forward) * initialRotation;

        var seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(1.5f, .15f).SetRelative())
            .Append(transform.DOMoveX(prepOffsetX, 0.4f).SetEase(Ease.InOutSine))
            .Join(transform.DORotateQuaternion(prepTiltRotation, 0.2f).SetEase(Ease.InOutSine))

            .Append(transform.DOMoveX(targetX, 0.4f).SetEase(Ease.InOutSine))
            .Join(transform.DORotateQuaternion(launchTiltRotation, 0.2f).SetEase(Ease.InOutSine))

            .SetDelay(1.5f)
            .OnComplete(() =>
            {
                OnMoveDone?.Invoke();
            });
    }
    public void OnGet()
    {
    }

    public void OnReturn()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;

        animator.Rebind();
        animator.Play("BoxIdle");

        IsOpen = false;
        Useable = false;
        OnSlotsUpdated = null;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
            {
                ObjectPooler.Instance.ReturnToPool(slot.CurrentProduct.gameObject);
                slot.Clear();
            }
        }
        boxData = null;

        BoxLayerController.Clear();
        BoxBeltController.Clear();
    }
}
