using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _btnlaunch;
    [SerializeField] private InputField _velocityX;

    [SerializeField] private Rigidbody _catapult1;

    private bool _shouldLaunch;

    private void Start()
    {
        _btnlaunch.onClick.AddListener(HardwareLauch);
    }

    private void HardwareLauch()
    {
        _shouldLaunch = true;
    }

    private void FixedUpdate()
    {
        if (_shouldLaunch)
        {
            if (_catapult1.rotation.x >= -90)
            {
                float x = float.Parse(_velocityX.text) * 5;
                float y = 0;
                float z = 0;

                _catapult1.angularVelocity = new Vector3(x, y, z);
            }
            else
            {
                _shouldLaunch = false;
            }
        }
    }
}
