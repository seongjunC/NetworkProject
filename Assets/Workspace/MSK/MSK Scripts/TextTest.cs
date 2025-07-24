using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTest : MonoBehaviour
{
    [SerializeField] Transform pos;

    private void LateUpdate()
    {
        transform.position = pos.position + new Vector3(0, 1f, 0);

        float parentScaleX = 1f;
        if (transform.parent != null)
            parentScaleX = transform.parent.localScale.x;

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Sign(parentScaleX) * Mathf.Abs(localScale.x);
        transform.localScale = localScale;
    }
}