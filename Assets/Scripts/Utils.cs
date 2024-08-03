using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void SetLayer(GameObject gameObject, string layerName){
        var layer = LayerMask.NameToLayer(layerName); 
        foreach (var item in gameObject.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = layer;
        }
    }
}
