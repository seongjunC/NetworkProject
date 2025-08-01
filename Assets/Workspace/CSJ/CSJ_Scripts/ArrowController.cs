using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
