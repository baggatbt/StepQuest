using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class Inventory : MonoBehaviour
 {
    public List<Equipment> items = new List<Equipment>();
    public int maxItems = 20; // Can be adjusted during balancing

    public void Add(Equipment item)
    {
        if (items.Count < maxItems)
        {
            items.Add(item);
        }
    }

    public void Remove(Equipment item)
    {
        items.Remove(item);
    }
 }



