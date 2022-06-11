using UnityEngine;

[CreateAssetMenu(fileName = "CornerShape", menuName = "Custom/Create CornerShape", order = 0)]
public class CornerShape : ScriptableObject
{
    [Tooltip("Make sure the order is clock wise starting from top left corners")]
    public Vector2[] Corners;
}