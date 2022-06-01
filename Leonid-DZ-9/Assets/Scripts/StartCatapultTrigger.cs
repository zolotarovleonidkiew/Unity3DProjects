using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCatapultTrigger : MonoBehaviour
{
    //https://www.youtube.com/watch?v=Fzs0b3UveCw

    [SerializeField] private float velocity;
    [SerializeField] private Rigidbody _catapulta;

    private bool _shouldLaunch;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Catapult started with angularVelocity: {velocity}");

        //  _catapulta.angularVelocity = new Vector3(velocity, 0, 0);

        _shouldLaunch = true;
    }

    private void FixedUpdate()
    {
        if (_shouldLaunch)
        {
            if (_catapulta.rotation.x >= -90)
            {
                float x = velocity * 5;
                float y = 0;
                float z = 0;

                _catapulta.angularVelocity = new Vector3(x, y, z);
            }
            else
            {
                _shouldLaunch = false;
            }
        }
    }
}
