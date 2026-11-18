using System.Collections;
using UnityEngine;

public class AlienDroneBehaviour : MonoBehaviour
{
    [SerializeField]
    private float flightHeight = 4.0f;

    [SerializeField]
    private bool canLand;

    [SerializeField]
    private float verticalSpeed = 1.0f;

    [SerializeField]
    private GameObject prefabBullet;

    [SerializeField]
    private float bulletSpeed = 10.0f;

    private Transform droneRifle;
    private Transform targetShootingPoint;
    private float surfaceHeight;
    private float targetHeight;

    public bool CanLand => canLand;

    public float FlightHeight => flightHeight;

    public bool IsLanded { get; private set; }

    private void Start()
    {
        targetHeight = surfaceHeight + flightHeight;
        IsLanded = false;

        droneRifle = transform.Find("Drone_Gun/Drone_Rifle");
        targetShootingPoint = droneRifle != null
            ? droneRifle.Find("TargetShootingPoint")
            : null;
    }

    private void Update()
    {
        Vector3 position = transform.position;
        position.y = Mathf.MoveTowards(position.y, targetHeight, verticalSpeed * Time.deltaTime);
        transform.position = position;

        IsLanded = canLand && Mathf.Approximately(position.y, surfaceHeight);
    }

    public void SetSurfaceHeight(float height)
    {
        surfaceHeight = height;
        targetHeight = surfaceHeight + flightHeight;

        Rigidbody body = GetComponent<Rigidbody>();
        if (body == null)
        {
            body = GetComponentInParent<Rigidbody>();
        }

        if (body != null)
        {
            body.useGravity = false;
            body.isKinematic = true;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        Vector3 position = transform.position;
        position.y = targetHeight;
        transform.position = position;
    }

    public void Land()
    {
        if (!canLand)
        {
            return;
        }

        targetHeight = surfaceHeight;
    }

    public void TakeOff()
    {
        targetHeight = surfaceHeight + flightHeight;
    }

    public void Shoot(GameObject target)
    {
        if (target == null || prefabBullet == null || targetShootingPoint == null || bulletSpeed <= 0.0f)
        {
            return;
        }

        Vector3 direction = target.transform.position - targetShootingPoint.position;
        if (direction.sqrMagnitude > 0.0f)
        {
            droneRifle.rotation = Quaternion.LookRotation(direction);
        }

        GameObject bullet = Instantiate(
            prefabBullet,
            targetShootingPoint.position,
            direction.sqrMagnitude > 0.0f ? Quaternion.LookRotation(direction) : targetShootingPoint.rotation);

        StartCoroutine(FlyBullet(bullet.transform, target.transform));
    }

    private IEnumerator FlyBullet(Transform bullet, Transform target)
    {
        while (bullet != null && target != null)
        {
            Vector3 direction = target.position - bullet.position;
            float distance = direction.magnitude;
            float step = bulletSpeed * Time.deltaTime;

            if (distance <= step)
            {
                bullet.position = target.position;
                Destroy(bullet.gameObject);
                yield break;
            }

            bullet.position = Vector3.MoveTowards(bullet.position, target.position, step);
            bullet.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }

        if (bullet != null)
        {
            Destroy(bullet.gameObject);
        }
    }
}