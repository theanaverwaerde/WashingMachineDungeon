using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    [SerializeField] private float time = 5;

    private void Start()
    {
        Destroy(this, time);
    }
}
