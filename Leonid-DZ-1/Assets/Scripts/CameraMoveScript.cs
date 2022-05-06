using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveScript : MonoBehaviour
{
    void Update()
    {
        float _mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float _mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;
        transform.localRotation = Quaternion.Euler(new Vector4(-1f * (_mouseY * 180f), _mouseX * 360f, transform.localRotation.z));
    }
}
