using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoxLayerController : MonoBehaviour
{
    public bool IsFree { get; private set; }
    public System.Action OnFree;
    public List<Box> UpperBoxes { get; private set; }
    [SerializeField] private Renderer boxRenderer;
    [SerializeField] private Material defaultMaterial, blockedMaterial;
    [SerializeField] private float checkBlockHeight = 1f;
    [SerializeField] private Vector3 checkBlockSize = new Vector3(1f, .25f, 0.5f);
    [SerializeField] private LayerMask blockLayerMask;
    public void Clear()
    {
        IsFree = false;
        OnFree = null;
        UpperBoxes.Clear();
        boxRenderer.material = defaultMaterial;
    }
    public void CheckUpperBoxes()
    {
        UpperBoxes = GetBoxesFromAbove();

        if (!UpperBoxes.Any())
        {
            SetFree();
        }
        else
        {
            SetBlocked();
            BoxController.Instance.OnBoxRemove += CheckUpperFree;
        }
    }
    private void CheckUpperFree(Box targetBox)
    {
        if (UpperBoxes.Contains(targetBox))
        {
            UpperBoxes.Remove(targetBox);
            if (!UpperBoxes.Any())
            {
                SetFree();
            }
        }
    }
    private void SetFree()
    {
        IsFree = true;
        OnFree?.Invoke();
        boxRenderer.material = defaultMaterial;
    }
    private void SetBlocked()
    {
        IsFree = false;
        boxRenderer.material = blockedMaterial;
    }
    private List<Box> GetBoxesFromAbove()
    {
        Vector3 boxCenter = transform.position + transform.up * checkBlockHeight;
        Vector3 checkSize = checkBlockSize / 2f;

        Collider[] hits = Physics.OverlapBox(boxCenter, checkSize, transform.rotation, blockLayerMask);

        var boxes = hits
            .Select(hit => hit.GetComponentInParent<Box>())
            .Where(box => box != null && box != this)
            .ToList();
        return boxes;
    }
}
