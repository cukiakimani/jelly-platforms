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

    [Space, SerializeField] private Transform blob1;
    [SerializeField] private Transform blob2;

    [Space, SerializeField] private float idleSize1 = 0.08f;
    [SerializeField] private float growSize1 = 0.2f;
    [SerializeField] private float growSize2 = 0.1f;
    [SerializeField] private float growTime = 0.3f;
    [SerializeField] private Ease growEase;
    [SerializeField] private float growEaseAmplitude;
    [SerializeField] private float growEasePeriod;
    [SerializeField] private Ease shrinkEase;
    [SerializeField] private float shrinkTime;
    [SerializeField] private float pullDistance = 0.22f;

    private bool deform;
    private bool shooting;
    private bool pulsing;
    private Vector3 shootPosition;
    private Vector3 shootDirection;
    private float pulseRadius;

    private void Start()
    {
        blob1.transform.localPosition = new Vector3(0f, 0f, idleSize1);
        blob2.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

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

            blob1.DOLocalMoveZ(growSize1, growTime).SetEase(growEase, growEaseAmplitude, growEasePeriod);
            blob2.DOLocalMoveZ(growSize2, 0.1f);
        }

        if (Input.GetMouseButton(0))
        {
            shootDirection = transform.position - (Vector3)mousePos;
            shootDirection.Normalize();

            Vector3 pullPosition = transform.InverseTransformPoint((Vector3)mousePos);
            pullPosition = Vector3.ClampMagnitude(pullPosition, pullDistance);
            pullPosition.z = growSize2;
            blob2.localPosition = pullPosition;
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

                blob1.localPosition = Vector3.zero;
                blob2.localPosition = Vector3.zero;

                blob1.DOLocalMoveZ(idleSize1, shrinkTime).SetEase(shrinkEase);
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
