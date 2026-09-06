using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // швидк≥сть кул≥
    public bool IsHeroShooter { get; private set; }
    public bool IsAlienShooter { get; private set; }
    public string ShooterName { get; private set; }
    public string WeaponName { get; private set; }
    public WeaponData WeaponData { get; private set; }

    private Transform target;
    private bool hasHit;

    public void SetShotData(GameObject shooter, WeaponData weaponData)
    {
        WeaponData = weaponData;
        IsHeroShooter = shooter != null && shooter.GetComponent<Hero>() != null;
        IsAlienShooter = shooter != null && shooter.GetComponent<Alien>() != null;
        ShooterName = shooter != null ? shooter.name : "Unknown";
        WeaponName = weaponData != null && !string.IsNullOrWhiteSpace(weaponData.weaponName)
            ? weaponData.weaponName
            : weaponData != null ? weaponData.weaponType.ToString() : "Unknown";
    }

    public bool TryRegisterHit()
    {
        if (hasHit) return false;

        hasHit = true;
        return true;
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        // –озвертаЇмо пулю у напр€мку ц≥л≥
        transform.LookAt(target.position);
    }
    
    void Start()
    {        
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = transform.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }


    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // –ухаЇмо пулю вперед
        transform.position += transform.forward * speed * Time.deltaTime;

        // якщо долет≥ла достатньо близько Ч вважаЇмо влучанн€
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            Debug.Log("Bullet hit " + target.name);
            Destroy(gameObject);
        }
    }
}
