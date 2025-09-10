using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class InputController : MonoBehaviour
{
    private Camera mainCamera;
    public Container CurrentContainer { get; private set; }
    private List<Product> selectedProducts = new();
    [SerializeField] private LayerMask clickableLayerMask;
    [SerializeField] private ClipProperty selectClip;
    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 50f, clickableLayerMask))
        {
            Container clickedContainer = hit.collider.GetComponentInParent<Container>();
            if (clickedContainer == null || !clickedContainer.Useable)
            {
                DeselectAll(true);
                return;
            }

            if (CurrentContainer == null)
            {
                CurrentContainer = clickedContainer;
                SelectProductsFrom(clickedContainer);
            }
            else if (CurrentContainer == clickedContainer)
            {
                SelectProductsFrom(clickedContainer);
            }
            else if (CurrentContainer != clickedContainer && selectedProducts.Count > 0)
            {
                MoveSelectedProductsTo(clickedContainer);

                CurrentContainer.OnSlotsUpdated?.Invoke();
                clickedContainer.OnSlotsUpdated?.Invoke();

                DeselectAll(true);
            }
        }
        else
        {
            DeselectAll(true);
        }
    }

    private void SelectProductsFrom(Container container)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Vector3 localPoint = container.transform.InverseTransformPoint(hit.point);
        var nonEmptySlots = container.Slots.Where(x => !x.IsEmpty).ToList();
        if (nonEmptySlots.Count == 0) return;

        var closestSlot = nonEmptySlots
            .OrderBy(slot => Mathf.Abs(slot.transform.localPosition.x - localPoint.x))
            .First();

        int targetProductIndex = closestSlot.CurrentProduct.ProductIndex;

        var matchingSlots = nonEmptySlots.Where(s => s.CurrentProduct.ProductIndex == targetProductIndex).ToList();

        bool isSameSelection = selectedProducts.Count == matchingSlots.Count && selectedProducts.All(p => p.ProductIndex == targetProductIndex);
        if (isSameSelection)
        {
            DeselectAll(false);
            return;
        }

        if (CurrentContainer == container)
        {
            DeselectAll(false);
        }

        foreach (var slot in matchingSlots)
        {
            Product product = slot.CurrentProduct;
            product.SelectProduct();
            selectedProducts.Add(product);
        }

        CurrentContainer = container;

        AudioManager.Instance.PlaySound(selectClip);
    }

    private void MoveSelectedProductsTo(Container targetContainer)
    {
        if (selectedProducts.Count == 0) return;

        var emptySlots = targetContainer.Slots.Where(s => s.IsEmpty).ToList();
        int moveCount = Mathf.Min(emptySlots.Count, selectedProducts.Count);

        for (int i = 0; i < moveCount; i++)
        {
            var product = selectedProducts[i];
            var targetSlot = emptySlots[i];

            var oldSlot = product.GetComponentInParent<Slot>();
            oldSlot.Clear();

            var delay = i * .05f;
            product.MoveToSlot(targetSlot, delay);
        }
    }

    private void DeselectAll(bool clearCurrentContainer)
    {
        foreach (var product in selectedProducts)
        {
            product.DeselectProduct();
        }

        selectedProducts.Clear();

        if (clearCurrentContainer || selectedProducts.Count == 0)
        {
            CurrentContainer = null;
        }
    }
}
