using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squish : MonoBehaviour
{
    [SerializeField] private float shootSpeed;

    private bool deform;
    private bool shooting;
    private Vector3 shootPosition;
    private Vector3 shootDirection;

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = deform ? transform.position : mousePos;

        if (Input.GetMouseButtonDown(0))
        {
            shootPosition = mousePos;
            deform = true;
        }

        if (Input.GetMouseButton(0))
        {
            shootDirection = transform.position - (Vector3)mousePos;
            shootDirection.Normalize();
            Debug.DrawRay(transform.position, shootDirection, Color.red);
        }

        if (Input.GetMouseButtonUp(0))
        {
            shooting = true;
        }

        if (shooting)
        {
            Vector3 nextPosition = transform.position + shootDirection * shootSpeed * Time.deltaTime;
            var hit = Physics2D.Raycast(transform.position, shootDirection, shootSpeed * Time.deltaTime, 1 << LayerMask.NameToLayer("Platform"));

            if (hit)
            {
                var deformablePlatform = hit.collider.GetComponent<DeformablePlatform>();
                deformablePlatform.Deform(transform.position);
                deform = shooting = false;
            }

            transform.position = nextPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // var deformablePlatform = collider.GetComponent<DeformablePlatform>();

        // if (deformablePlatform == null || !deform)
        //     return;

        // var deformPosition = transform.position - shootDirection * 0.2f;
        // deformablePlatform.Deform(transform.position);
        // deform = shooting = false;
    }
}
