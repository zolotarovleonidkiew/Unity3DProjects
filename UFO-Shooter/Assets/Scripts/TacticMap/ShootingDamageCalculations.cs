using UnityEngine;

/// <summary>
/// Компонент регистрирует попадания в ТЕКУЩИЙ объект, рассчитывает урон, запускает анимации
/// </summary>
public class ShootingDamageCalculations : MonoBehaviour
{
    public bool isAlien;
    public bool isHero;

    private void OnTriggerEnter(Collider other)
    {
        RegisterBulletHit(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        RegisterBulletHit(collision.collider);
    }

    private void RegisterBulletHit(Collider other)
    {
        if (other == null) return;

        var bullet = other.GetComponentInParent<Bullet>();
        if (bullet == null) return;

        // Only register and log a hit if shooter is on the opposing side and bullet hasn't already been registered
        if (!IsOpposingShooter(bullet) || !bullet.TryRegisterHit()) return;

        Debug.Log($"Bullet hit: shooter '{bullet.ShooterName}', weapon '{bullet.WeaponName}', target '{name}'");
        Destroy(bullet.gameObject);

        //damage processing
        CalculateDamage(bullet.WeaponData);
    }

    private bool IsOpposingShooter(Bullet bullet)
    {
        return (isAlien && bullet.IsHeroShooter) || (isHero && bullet.IsAlienShooter);
    }

    private void CalculateDamage(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogWarning($"Cannot calculate damage: bullet from '{name}' has no WeaponData");
            return;
        }

        int damage = DamageCalculation.CalculateDamage(weaponData);

        if (damage > 0)
        {
            if (isAlien)
            {
                transform.GetComponent<Hero>()?.TakeDamage(damage);
            }
            else
            {
                transform.GetComponent<Alien>()?.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log($"damage = 0");
        }
    }

}