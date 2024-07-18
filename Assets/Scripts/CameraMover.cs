using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public GameObject target;

    private Vector3 distance;
    // Update is called once per frame
    private void Start()
    {
        distance = transform.position - target.transform.position;
    }

    void Update()
    {
        transform.position = target.transform.position + distance;
    }
}
