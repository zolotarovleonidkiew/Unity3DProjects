using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinDisctanceScript : MonoBehaviour
{

    [SerializeField] float x = 2;
    [SerializeField] float z = 2;

    void Start()
    {

    }


    void Update()
    {
        var t = transform.position;

        Vector3 xmin1 = new Vector3(t.x - x, t.y, t.z);
        Vector3 xplus1 = new Vector3(t.x + x, t.y, t.z);
        Vector3 zmin1 = new Vector3(t.x, t.y, t.z - z);
        Vector3 zplus1 = new Vector3(t.x, t.y, t.z + z);

        Debug.DrawLine(t, xmin1, Color.green);
        Debug.DrawLine(t, xplus1, Color.green);
        Debug.DrawLine(t, zmin1, Color.green);
        Debug.DrawLine(t, zplus1, Color.green);
    }
}
