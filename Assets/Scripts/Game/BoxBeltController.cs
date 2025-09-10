using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoxBeltController : MonoBehaviour
{
    [SerializeField] private Transform beltTransform;
    public List<BoxBelt> Belts { get; private set; } = new();
    public System.Action OnFree;
    public bool IsFree => Belts.Count <= 0;
    private Box box;
    private void Awake()
    {
        box = GetComponent<Box>();
    }
    public void Clear()
    {
        foreach (var belt in Belts)
        {
            ObjectPooler.Instance.ReturnToPool(belt.gameObject);
        }
        Belts.Clear();
        OnFree = null;
    }
    public void SpawnBelts(int count)
    {
        if (count <= 0) return;

        float spacing = 0.5f;
        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            var belt = ObjectPooler.Instance.Get("Belt").GetComponent<BoxBelt>();

            belt.transform.SetParent(beltTransform);

            var pos = new Vector3(startX + i * spacing, 0, 0);
            var rot = Quaternion.identity;
            belt.transform.SetLocalPositionAndRotation(pos, rot);

            Belts.Add(belt);
        }

        BoxController.Instance.OnBoxFilled += (goneBox) =>
        {
            if (IsFree) return;
            if (!box.BoxLayerController.IsFree) return;
            if (box.BoxLayerController.UpperBoxes.Contains(goneBox)) return;

            CutLast();
        };
    }
    public void CutLast()
    {
        Belts[0].CutBelt();
        Belts.Remove(Belts[0]);
        DOVirtual.DelayedCall(1f, () =>
        {
            OnFree?.Invoke();
        }, false);
    }
}
