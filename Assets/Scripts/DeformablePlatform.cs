using System;
using System.Collections.Generic;
using Shapes;
using SweetLibs;
using UnityEngine;

public class DeformablePlatform : MonoBehaviour
{
    [SerializeField] private float deformRadius;

    [Space, SerializeField] public CornerStyle CornerStyle;

    [Space, Tooltip("Make sure the order is clock wise starting from top left corners")]
    public CornerShape Shape;

    [SerializeField] private int straightFidelity = 10;
    [SerializeField] private int curveFidelity = 5;
    [SerializeField] private float cornerLength = 0.1f;

    [Header("Spring"), SerializeField] private float springForceMagnitude = 50f;
    [SerializeField] public AnimationCurve springForceCurve;

    [Space, SerializeField, Range(0.01f, 1f)] private float percDecay = 0.4f;
    [SerializeField, Range(1f, 10f)] private float freq = 5f;
    [SerializeField, Range(0.1f, 5f)] private float timeDecay = 3f;

    private Polygon polygonShape;
    private List<SpringPoint> springPoints;
    private List<int> inRadiusPoints;

    private CornerStyle lastCornerStyle;
    private int lastStraightFidelity;
    private int lastCurveFidelity;
    private float lastCornerLength;
    private CornerShape lastShape;
    private bool hasPolygonShape;

    private PolygonCollider2D collider;
    private Vector2[] colliderPoints;

    [Header("Debug")] public bool BreakOnDeform;

    private void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
        polygonShape = GetComponent<Polygon>();
        hasPolygonShape = polygonShape != null;

        springPoints = new List<SpringPoint>();
        inRadiusPoints = new List<int>();

        CreatePoints();
        UpdateSettings();
    }

    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     mousePos.z = 0f;

        //     Deform(mousePos);
        //     DebugHelpers.BreakIf(BreakOnDeform);
        // }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     Reform();
        // }

        bool changedShapeStyle = lastCornerStyle != CornerStyle;
        bool changedFidelity = lastCurveFidelity != curveFidelity || straightFidelity != lastStraightFidelity;
        bool changedCornerLength = Mathf.Abs(lastCornerLength - cornerLength) > 0.01f;
        bool changedShape = lastShape != Shape;

        if (changedFidelity || changedShapeStyle || changedCornerLength || changedShape)
        {
            CreatePoints();
        }

        UpdateSettings();

        Cursor.visible = false;

        UpdateColliderPoints();
    }

    private void FixedUpdate()
    {
        UpdatePoints();
    }

    private void UpdateSettings()
    {
        lastCornerStyle = CornerStyle;
        lastCurveFidelity = curveFidelity;
        lastCornerLength = cornerLength;
        lastStraightFidelity = straightFidelity;
        lastShape = Shape;
    }

    private void CreatePoints()
    {
        springPoints.Clear();

        if (hasPolygonShape)
            polygonShape.points.Clear();

        CycleThroughPoints((index, point) =>
        {
            springPoints.Add(new SpringPoint { Anchor = point, Origin = point, Position = point });

            if (hasPolygonShape)
                polygonShape.AddPoint(point);
        });

        CreatePolygonCollider();
    }

    private void CreatePolygonCollider()
    {
        colliderPoints = new Vector2[springPoints.Count];
        UpdateColliderPoints();
    }

    private void UpdateColliderPoints()
    {
        for (int i = 0; i < colliderPoints.Length; i++)
        {
            colliderPoints[i] = springPoints[i].Position;
        }

        collider.points = colliderPoints;
    }

    private void UpdatePoints()
    {
        float omega = 2 * Mathf.PI * freq;
        float zeta = Mathf.Log(percDecay) / (-omega * timeDecay);

        CycleThroughPoints((index, point) =>
        {
            var springPoint = springPoints[index]; springPoint.Position = Spring.Vector3Spring(springPoint.Position, ref springPoint.Velocity,
                     springPoint.Anchor, zeta, omega, Time.deltaTime);

            if (hasPolygonShape)
                polygonShape.SetPointPosition(index, springPoint.Position);
        });
    }

    private void CycleThroughPoints(Action<int, Vector2> doThis)
    {
        switch (CornerStyle)
        {
            case CornerStyle.HardCorners:
                HardCornerShape(doThis);

                break;

            case CornerStyle.SmoothCorners:
                SmoothCornerShape(doThis);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Reform()
    {
        CycleThroughPoints((index, point) =>
            {
                var springPoint = springPoints[index];
                springPoint.Anchor = springPoint.Origin;
            });
    }

    public void Deform(Vector3 deformPosition)
    {
        DebugHelpers.BreakIf(BreakOnDeform);
        DebugHelpers.DebugPoint(deformPosition, Color.magenta, 0.5f);

        // find all points in radius
        inRadiusPoints.Clear();

        for (int i = 0; i < springPoints.Count; i++)
        {
            var springPoint = springPoints[i];

            if (Vector3.Distance(springPoint.Position, deformPosition) <= deformRadius)
            {
                inRadiusPoints.Add(i);
            }
        }

        if (inRadiusPoints.Count == 0)
            return;

        // find the center of those points (just add an extra one if even)
        if (inRadiusPoints.Count % 2 == 0)
        {
            int addedIndex = MathHelpers.Wrap(inRadiusPoints[inRadiusPoints.Count - 1] + 1, 0,
                inRadiusPoints.Count - 1);

            inRadiusPoints.Add(addedIndex);
        }

        int sideCount = inRadiusPoints.Count / 2;
        var midPoint = springPoints[inRadiusPoints[sideCount]];

        var left = springPoints[inRadiusPoints[sideCount - 1]].Position;
        var right = springPoints[inRadiusPoints[sideCount + 1]].Position;
        var leftRight = (left - right).normalized;
        var normal = Vector3.Cross(leftRight, Vector3.forward);
        var direction = (midPoint.Position - deformPosition).normalized;

        direction = new Vector3(Mathf.Abs(normal.x) * Mathf.Sign(direction.x),
            Mathf.Abs(normal.y) * Mathf.Sign(direction.y));

        midPoint.Anchor += direction * springForceMagnitude;
        DebugHelpers.DrawCircle(midPoint.Position, Color.yellow, 0.1f);

        // spring away from the center of the mouse position 
        for (int i = 0; i < sideCount; i++)
        {
            float t = i / (float)sideCount;
            float s = springForceCurve.Evaluate(t);
            float forceMagnitude = Mathf.Lerp(0f, springForceMagnitude, s);

            int j = inRadiusPoints[i];
            springPoints[j].Anchor += direction * forceMagnitude;
            DebugHelpers.DrawCircle(springPoints[j].Position, Color.yellow, 0.1f);

            j = inRadiusPoints[inRadiusPoints.Count - 1 - i];
            springPoints[j].Anchor += direction * forceMagnitude;
            DebugHelpers.DrawCircle(springPoints[j].Position, Color.yellow, 0.1f);
        }
    }

    private void HardCornerShape(Action<int, Vector2> doThis)
    {
        int index = 0;

        for (int i = 0; i < Shape.Corners.Length; i++)
        {
            Vector2 a = Shape.Corners[i];
            Vector2 b = Shape.Corners[i == Shape.Corners.Length - 1 ? 0 : i + 1];
            Vector2 ab = (b - a).normalized;
            float length = (b - a).magnitude;
            float dist = length / (straightFidelity + 1);

            for (int j = 0; j <= straightFidelity; j++)
            {
                Vector2 point = a + ab * (dist * j);
                doThis(index, point);
                index++;
            }
        }
    }

    private void SmoothCornerShape(Action<int, Vector2> doThis)
    {
        int index = 0;

        for (int i = 0; i < Shape.Corners.Length; i++)
        {
            // straight 
            Vector2 a = Shape.Corners[i];
            Vector2 b = Shape.Corners[i == Shape.Corners.Length - 1 ? 0 : i + 1];

            DebugHelpers.DrawCircle(a, Color.cyan, 0.1f);
            DebugHelpers.DrawCircle(b, Color.cyan, 0.1f);

            Vector2 ab = (b - a).normalized;

            Vector2 a1 = a + ab * cornerLength;
            Vector2 b1 = b - ab * cornerLength;

            DebugHelpers.DrawCircle(a1, Color.magenta, 0.05f);
            DebugHelpers.DrawCircle(b1, Color.magenta, 0.05f);

            float length = Vector2.Distance(a1, b1);
            float dist = length / (straightFidelity + 1);

            for (int j = 0; j <= straightFidelity; j++)
            {
                Vector2 point = a1 + ab * (dist * j);
                doThis(index, point);
                index++;
            }

            // curve
            Vector2 c = Shape.Corners[(i + 2) % Shape.Corners.Length];
            Vector2 bc = (c - b).normalized;
            Vector2 b2 = b + bc * cornerLength;

            DebugHelpers.DrawCircle(b2, Color.magenta, 0.05f);

            for (int j = 0; j <= curveFidelity; j++)
            {
                float t = j / (float)(curveFidelity + 1);
                Vector2 point = CalculateBezierPoint(t, b1, b, b, b2);
                doThis(index, point);
                index++;
            }
        }
    }

    private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            CycleThroughPoints((index, point) =>
            {
                DebugHelpers.DebugPoint(springPoints[index].Position, Color.white, 0.1f);
            });

            return;
        }

        CycleThroughPoints((index, point) => { DebugHelpers.DebugPoint(point, Color.red, 0.1f); });
    }
}