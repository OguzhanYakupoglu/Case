using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Slot : MonoBehaviour
{
    public Product CurrentProduct { get; private set; }
    public bool IsEmpty => CurrentProduct == null;

    public void SetProduct(Product product, bool setPosition = true)
    {
        CurrentProduct = product;
        product.transform.SetParent(transform);
        product.transform.localScale = Vector3.one;
        if (setPosition)
            product.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Clear()
    {
        if (CurrentProduct != null)
        {
            CurrentProduct = null;
        }
    }
}
