using UnityEngine;

/// <summary>
/// Оружие всегда указывает в центр экрана
/// </summary>
public class WeaponToCenterScreen : MonoBehaviour
{
    private bool _isPlayer;

    /// <summary>
    /// Для манипуляции с руками иногда нужно, чтобы они перестали показывать на центр экрана
    /// </summary>
    public bool StopPointingToCenter = false;

    private void Awake()
    {
        _isPlayer = transform.parent.parent.name == "Player";
    }

    private void FixedUpdate()
    {
        if (!StopPointingToCenter)
        {
            if (_isPlayer)
            {
                PointWeaponToCenterScreen();
            }

        }
    }

    void PointWeaponToCenterScreen()
    {
        RaycastHit screenRayInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0)), out screenRayInfo);

        Debug.DrawLine(transform.position, screenRayInfo.point, Color.black);

        if (screenRayInfo.point != Vector3.zero && screenRayInfo.collider?.gameObject.tag != "Inter-object")
        {
            transform.LookAt(screenRayInfo.point);

            transform.rotation *= Quaternion.Euler(180, 0, 180); /// !!!!!!!!!!!!!!!!
        }
    }
}
