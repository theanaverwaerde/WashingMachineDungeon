using System;
using UnityEngine;

public class Rotatee : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private float up;
    private float dist;
    private float init;

    private void Awake()
    {
        init = transform.position.y;
        dist = transform.position.y + up;
    }

    private void Start()
    {
        LeanTween.rotateAround(gameObject, Vector3.up, 360, time).setOnComplete(Start);
        LeanTween.moveY(gameObject, dist, time * .5f).setOnComplete(_ => LeanTween.moveY(gameObject, init, time * .5f));
    }
}
