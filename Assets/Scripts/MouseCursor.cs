using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using SweetLibs;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public float Radius = 1f;

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;
    }

    private void OnDrawGizmos()
    {
        DebugHelpers.DrawCircle(transform.position, Color.magenta, Radius);
    }
}