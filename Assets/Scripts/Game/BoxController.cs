using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxController : SoftSingleton<BoxController>
{
    public List<Box> Boxes { get; private set; } = new();
    public System.Action<Box> OnBoxFilled, OnBoxEmpty, OnBoxRemove;
    [SerializeField] private Dock dock;
    private Coroutine gameConditionCheckRoutine;
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        GameManager.Instance.OnGameStateChanged += (state) =>
        {
            if (state == GameManager.GameState.Play)
            {
                SpawnBoxes();

                OnBoxFilled += RemoveBox;
                OnBoxEmpty += RemoveBox;
            }
        };
    }
    private void SpawnBoxes()
    {
        var levelManager = LevelManager.Instance;
        var boxDatas = levelManager.CurrentLevelInfo.boxDatas.OrderByDescending(b => b.position.y);
        foreach (var boxData in boxDatas)
        {
            var box = ObjectPooler.Instance.Get("Box").GetComponent<Box>();
            box.transform.SetPositionAndRotation(boxData.position, Quaternion.Euler(0f, boxData.rotation, 0f));
            box.transform.SetParent(transform);

            Physics.SyncTransforms();

            Boxes.Add(box);
            box.Init(boxData);
        }
    }
    private void RemoveBox(Box box)
    {
        ObjectPooler.Instance.ReturnToPool(box.gameObject);

        Boxes.Remove(box);
        OnBoxRemove?.Invoke(box);

        if (gameConditionCheckRoutine != null)
            StopCoroutine(gameConditionCheckRoutine);
        gameConditionCheckRoutine = StartCoroutine(CheckGameCondition());
    }
    private IEnumerator CheckGameCondition()
    {
        CheckSuccess();
        yield return new WaitForSeconds(2f);
        CheckFail();
    }
    private void CheckSuccess()
    {
        if (!Boxes.Any())
        {
            GameManager.Instance.EndGame(true);
        }
    }
    private void CheckFail()
    {
        var products = new List<Product>();
        var activeBoxes = Boxes.Where(box => box.IsOpen && box.Useable);

        var productsInBoxes = activeBoxes
            .SelectMany(box => box.Slots)
            .Where(slot => !slot.IsEmpty)
            .Select(slot => slot.CurrentProduct)
            .ToList();

        var productsInDock = dock.Slots
            .Where(slot => !slot.IsEmpty)
            .Select(slot => slot.CurrentProduct)
            .ToList();

        products.AddRange(productsInBoxes);
        products.AddRange(productsInDock);

        var hasEnoughOfAnyProduct = products
            .GroupBy(p => p.ProductIndex)
            .Any(g => g.Count() >= 6);

        var boxWithMinProduct = activeBoxes
            .OrderBy(box => box.Slots.Count(slot => slot.IsEmpty))
            .FirstOrDefault();

        if (boxWithMinProduct == null)
            return;

        var emptySlotCount = activeBoxes.Where(box => box != boxWithMinProduct).Sum(box => box.Slots.Count(slot => slot.IsEmpty)) + dock.Slots.Count(slot => slot.IsEmpty);
        var isEnoughSpaceForRemoveBox = emptySlotCount >= boxWithMinProduct.Slots.Count(slot => !slot.IsEmpty);

        var isAllSlotsFull = activeBoxes.All(box => box.Slots.All(slot => !slot.IsEmpty)) && dock.Slots.All(slot => !slot.IsEmpty);

        if (isAllSlotsFull || (!hasEnoughOfAnyProduct && !isEnoughSpaceForRemoveBox))
        {
            GameManager.Instance.EndGame(false);
        }
    }

}
