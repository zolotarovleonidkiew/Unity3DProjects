using UnityEngine;

public class LaunchTheBall : MonoBehaviour
{
    [SerializeField] private Rigidbody linkedEntity;

    private Ray _ray;
    private RaycastHit _raycastHit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartBallRolling();
        }
    }

    private void StartBallRolling()
    {
        var mousePosition = Input.mousePosition;

        _ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(_ray, out _raycastHit))
        {
            linkedEntity.isKinematic = false;
        }
    }

}
