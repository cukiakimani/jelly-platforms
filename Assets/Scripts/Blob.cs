using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{
    [SerializeField] MetaballRenderer metaballRenderer;

    void Start()
    {
        metaballRenderer.AddBlob(transform);
    }
}
