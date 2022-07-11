using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRevolverShootAnim : MonoBehaviour
{
    Animator a;
    Animator b;

    void Start()
    {
        a = GetComponentInChildren<Animator>(); // null
        b = GetComponent<Animator>(); // null1
    }

    void FixedUpdate()
    {
        //a.Play("Base Layer.SHOOT");
        //b.Play("Base Layer.SHOOT");
    }
}
