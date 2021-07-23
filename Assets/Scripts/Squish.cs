using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SweetLibs;
using DG.Tweening;

public class Squish : MonoBehaviour
{
    [SerializeField] private float shootSpeed;

    [SerializeField] private float pulseMaxRadius;

    [SerializeField] private float pulseTime = 1f;
    [SerializeField] private Ease pulseEase;

    private bool deform;
    private bool shooting;
    private bool pulsing;
    private Vector3 shootPosition;
    private Vector3 shootDirection;
    private float pulseRadius;

    private void Update()
    {
        if (pulsing)
            return;

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

        if (Input.GetMouseButtonDown(1))
        {
            pulsing = true;

            var pulse = DOTween.To(() => pulseRadius, x => pulseRadius = x, pulseMaxRadius, pulseTime);
            pulse.SetEase(pulseEase);
            
            pulse.OnUpdate(() =>
            {
                DebugHelpers.DrawCircle(transform.position, Color.red, pulseRadius);
                var collider = Physics2D.OverlapCircle(transform.position, pulseRadius);
                if (collider)
                {
                    var deformablePlatform = collider.GetComponent<DeformablePlatform>();
                    if (deformablePlatform)
                    {
                        deformablePlatform.Reform();
                    }
                }
            });

            pulse.OnComplete(() =>
            {
                pulsing = false;
                pulseRadius = 0f;
            });
        }
    }
}
