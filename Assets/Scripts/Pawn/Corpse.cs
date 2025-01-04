using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    [SerializeField] private float _destroyTime;

    private void Start()
    {
        Destroy(gameObject, _destroyTime);
    }
}
