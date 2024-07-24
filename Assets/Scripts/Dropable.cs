using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropable : MonoBehaviour
{
    [Serializable] private class Item{
        public float timer = 10000000;
        public GameObject drop;
    }
    [SerializeField] private List<Item> items;

    private void OnDestroy()
    {
        if(!gameObject.scene.isLoaded){
            return;
        }
        foreach (var item in items)
        {
            Destroy(Instantiate(item.drop, transform.position, transform.rotation), item.timer);
        }   
    }
}