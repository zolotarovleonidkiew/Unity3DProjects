using System;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public bool isAlive => CurentHealth > 0;
    public bool canMoving => ActiveMovementRoundsAvailable > 0;

    #region Alien data
    public int MaxHealth;
    public int CurentHealth;

    public int Damage;
    public float Speed;
    public float DetectionRange;
    public float AttackCooldown = 1f;

    public bool isFlying = false;
    public bool CanFly = false;
    public float FlyHeight = 0f;

    #endregion

    [SerializeField] private int ActiveMovementRoundsAvailable;
    [SerializeField] private int MovementPointsPerRound = 2;

    private int alienI;
    private int alienJ;

    public void SetMovementPoints(int points)
    {
        ActiveMovementRoundsAvailable = points;
    }

    private void Start()
    {
        //...
    }

    public void UpdateAlienCoords(int i, int j)
    {
        alienI = i;
        alienJ = j;
    }

    public void DoAlienTurn()
    {
        if (!isAlive) return;

        Debug.Log("[Alien] роблю хід...");

        // TODO: тут буде логіка руху (поки просто забираємо всі ходи)
        ActiveMovementRoundsAvailable = 0;
    }

    /// <summary>
    /// Apply damage to the alien, reducing current health. If health drops to 0 or below, the alien is considered dead.
    /// </summary>
    internal void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            CurentHealth -= damage;
            if (CurentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log($"[Hero] {name} has died.");
    }
}