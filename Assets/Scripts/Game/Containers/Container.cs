using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] protected List<Slot> slots;
    public List<Slot> Slots { get => slots; }
    public bool Useable;
    public System.Action OnSlotsUpdated;
}
