using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private float strength;
    [SerializeField] private int vibrato;
    [SerializeField] private float randomness;

    [Space, SerializeField] private float punchTime;
    [SerializeField] private int punchVibrato;
    [SerializeField] private float punchElasticity;

    [SerializeField] private Transform punchTransform;

    // Start is called before the first frame update
    void Start()
    {
        punchTransform.position = new Vector3(0f, 0f, Camera.main.orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpringSize();
        }
    }

    public void Shake()
    {
        transform.DOShakePosition(time, strength, vibrato, randomness);
    }

    public void SpringSize()
    {
        var punchSize = punchTransform.DOPunchPosition(Vector3.back, punchTime, punchVibrato, punchElasticity); 
        punchSize.OnUpdate(() => {
            Camera.main.orthographicSize = punchTransform.position.z;
        });

    }
}
