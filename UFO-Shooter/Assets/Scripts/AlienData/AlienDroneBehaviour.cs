using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AlienDroneBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool isHeavyDrone;

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

    //Heavy drone - rockets +
    private bool availableRocket1 = true;
    private bool availableRocket2 = true;
    private bool availableRocket3 = true;
    private bool availableRocket4 = true;
    private GameObject rocket1;
    private GameObject rocket2;
    private GameObject rocket3;
    private GameObject rocket4;
    //Heavy drone - rockets -

    private void Start()
    {
        InitializeRockets();

        targetHeight = surfaceHeight + flightHeight;
        IsLanded = false;

        droneRifle = transform.Find("Drone_Gun/Drone_Rifle");
        targetShootingPoint = droneRifle != null
            ? droneRifle.Find("TargetShootingPoint")
            : null;
    }

    private void InitializeRockets()
    {
        if (!isHeavyDrone)
        {
            rocket1 = null;
            rocket2 = null;
            rocket3 = null;
            rocket4 = null;
            availableRocket1 = false;
            availableRocket2 = false;
            availableRocket3 = false;
            availableRocket4 = false;
            return;
        }

        rocket1 = FindRocket("Rocket_1");
        rocket2 = FindRocket("Rocket_2");
        rocket3 = FindRocket("Rocket_3");
        rocket4 = FindRocket("Rocket_4");

        availableRocket1 = rocket1 != null;
        availableRocket2 = rocket2 != null;
        availableRocket3 = rocket3 != null;
        availableRocket4 = rocket4 != null;
    }

    private GameObject FindRocket(string rocketName)
    {
        Transform rocketTransform = transform.Find(rocketName);
        return rocketTransform != null ? rocketTransform.gameObject : null;
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
        if (isHeavyDrone)
        {
            HeavyDroneShoots(target);
        }
        else
        {
            LightDroneShoots(target);
        }
        //if (target == null || prefabBullet == null || targetShootingPoint == null || bulletSpeed <= 0.0f)
        //{
        //    return;
        //}

        //Vector3 direction = target.transform.position - targetShootingPoint.position;
        //if (direction.sqrMagnitude > 0.0f)
        //{
        //    droneRifle.rotation = Quaternion.LookRotation(direction);
        //}

        //GameObject bullet = Instantiate(
        //    prefabBullet,
        //    targetShootingPoint.position,
        //    direction.sqrMagnitude > 0.0f ? Quaternion.LookRotation(direction) : targetShootingPoint.rotation);

        //StartCoroutine(FlyBullet(bullet.transform, target.transform));
    }

    private IEnumerator FlyProjectile(GameObject projectile, Transform target, bool isRocket)
    {
        Transform projectileTransform = projectile.transform;
        while (projectileTransform != null && target != null)
        {
            Vector3 direction = target.position - projectileTransform.position;
            float distance = direction.magnitude;
            float step = bulletSpeed * Time.deltaTime;

            if (distance <= step)
            {
                projectileTransform.position = target.position;
                if (isRocket)
                {
                    OnRocketImpact(projectileTransform.position);
                }

                Destroy(projectile);
                yield break;
            }

            projectileTransform.position = Vector3.MoveTowards(projectileTransform.position, target.position, step);
            projectileTransform.rotation = Quaternion.LookRotation(direction);
            yield return null;
        }

        if (projectileTransform != null)
        {
            Destroy(projectile);
        }
    }

    protected virtual void OnRocketImpact(Vector3 impactPosition)
    {
    }

    private void LightDroneShoots(GameObject target)
    {
        ShootFromGun(droneRifle, targetShootingPoint, target);
    }

    private void HeavyDroneShoots(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        // Вероятности выбор оружия:
        int rocketProbability = 30;
        int smallGunProbability = 15;
        int regularGunProbability = 100 - rocketProbability - smallGunProbability;

        int weaponRoll = Random.Range(0, 100);

        if (weaponRoll < rocketProbability)
        {
            if (!TryShootNextRocket(target))
            {
                ShootHeavyRegularGuns(target);
            }
            return;
        }

        if (weaponRoll < rocketProbability + smallGunProbability)
        {
            ShootHeavySmallGuns(target);
            return;
        }

        if (weaponRoll < rocketProbability + smallGunProbability + regularGunProbability)
        {
            ShootHeavyRegularGuns(target);
        }
    }

    private bool TryShootNextRocket(GameObject target)
    {
        if (TryLaunchRocket(ref rocket1, ref availableRocket1, target)) return true;
        if (TryLaunchRocket(ref rocket2, ref availableRocket2, target)) return true;
        if (TryLaunchRocket(ref rocket3, ref availableRocket3, target)) return true;
        if (TryLaunchRocket(ref rocket4, ref availableRocket4, target)) return true;
        return false;
    }

    private bool TryLaunchRocket(ref GameObject rocket, ref bool available, GameObject target)
    {
        if (!available || rocket == null)
        {
            rocket = null;
            available = false;
            return false;
        }

        if (target == null || bulletSpeed <= 0.0f)
        {
            return false;
        }

        GameObject launchedRocket = rocket;
        Transform rocketTransform = launchedRocket.transform;
        rocketTransform.SetParent(null, true);

        Vector3 direction = target.transform.position - rocketTransform.position;
        if (direction.sqrMagnitude > 0.0f)
        {
            rocketTransform.rotation = Quaternion.LookRotation(direction);
        }

        rocket = null;
        available = false;
        StartCoroutine(FlyProjectile(launchedRocket, target.transform, true));
        return true;
    }

    private void ShootHeavySmallGuns(GameObject target)
    {
        Transform smallGun1 = transform.Find("Small_gun_1");
        Transform smallGun2 = transform.Find("Small_gun_2");

        ShootFromGun(smallGun1, smallGun1 != null ? smallGun1.Find("TargetShootingPoint") : null, target);
        ShootFromGun(smallGun2, smallGun2 != null ? smallGun2.Find("TargetShootingPoint") : null, target);
    }

    private void ShootHeavyRegularGuns(GameObject target)
    {
        Transform droneGun1 = transform.Find("Drone_Gun_1");
        Transform droneGun2 = transform.Find("Drone_Gun_2");

        Transform muzzle1 = droneGun1 != null
            ? droneGun1.Find("Drone_Rifle/TargetShootingPoint")
            : null;
        Transform muzzle2 = droneGun2 != null
            ? droneGun2.Find("Drone_Rifle/TargetShootingPoint")
            : null;

        ShootFromGun(droneGun1, muzzle1, target);
        ShootFromGun(droneGun2, muzzle2, target);
    }

    private void ShootFromGun(Transform gun, Transform shootingPoint, GameObject target)
    {
        if (gun == null || shootingPoint == null || target == null || prefabBullet == null || bulletSpeed <= 0.0f)
        {
            return;
        }

        Vector3 aimDirection = target.transform.position - gun.position;
        if (aimDirection.sqrMagnitude > 0.0f)
        {
            gun.rotation = Quaternion.LookRotation(aimDirection);
        }

        Vector3 bulletDirection = target.transform.position - shootingPoint.position;
        Quaternion bulletRotation = bulletDirection.sqrMagnitude > 0.0f
            ? Quaternion.LookRotation(bulletDirection)
            : shootingPoint.rotation;

        GameObject bullet = Instantiate(prefabBullet, shootingPoint.position, bulletRotation);
        StartCoroutine(FlyProjectile(bullet, target.transform, false));
    }
}