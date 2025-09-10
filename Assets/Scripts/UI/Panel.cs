using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public virtual void SetActive(bool isActive)
    {
        if (gameObject.activeSelf == isActive)
            return;
        gameObject.SetActive(isActive);
    }
}
