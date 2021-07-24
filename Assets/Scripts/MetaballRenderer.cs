using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaballRenderer : MonoBehaviour
{
    [SerializeField] private int maxBlobs = 500;

    [SerializeField] private Transform blob1;
    [SerializeField] private Transform blob2;

    private Material metaballMaterial;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Vector4[] blobs;
    private Transform[] blobTransforms;
    private int numBlobs;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        metaballMaterial = meshRenderer.sharedMaterial;
        meshRenderer.sortingOrder = 0;

        blobs = new Vector4[maxBlobs];
        blobTransforms = new Transform[maxBlobs];
    }

    private void Update()
    {
        Vector3 offset = new Vector3(0.5f, 0.5f);
        metaballMaterial.SetVector("_C0", transform.InverseTransformPoint(blob1.position) + offset);
        metaballMaterial.SetVector("_C1", transform.InverseTransformPoint(blob2.position) + offset);

        for (int i = 0; i < numBlobs; i++)
        {
            Vector3 invPosition = transform.InverseTransformPoint(blobTransforms[i].position);
            
            blobs[i].x = invPosition.x;
            blobs[i].y = invPosition.y;
            blobs[i].z = blobTransforms[i].localScale.x;
        }

        metaballMaterial.SetInt("_Blobs_Length", numBlobs);
        metaballMaterial.SetVectorArray("_Blobs", blobs);
    }

    public void AddBlob(Transform blobTransform)
    {
        blobTransforms[numBlobs] = blobTransform;
        numBlobs++;
    }

    public void ClearBlobs()
    {
        for (int i = 0; i < numBlobs; i++)
        {
            blobs[i].z = 0f;
        }

        numBlobs = 0;
    }
}
