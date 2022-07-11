using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasting : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Raycast();
        }
    }

    void Raycast()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z;

        _ray = Camera.main.ScreenPointToRay(mousePosition);

        Debug.DrawLine(_ray.origin, _ray.direction, Color.red); //DEBUG

        if (Physics.Raycast(_ray, out _raycastHit))
        {
            ////CreateShootImpact(_raycastHit.point);

            ////return true;
            ///
            Debug.Log($"{_raycastHit.collider.name}");
        }
    }
}
