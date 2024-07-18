using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
    public GameObject effect;
    public float timer = 1;

    private void OnDestroy()
    {
        Destroy(Instantiate(effect, transform.position, transform.rotation), timer);
    }
}