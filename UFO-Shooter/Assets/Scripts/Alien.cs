using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public bool isAlive => Health > 0;
    public bool canMoving => ActiveMovementRoundsAvailable > 0;

    [SerializeField] private int Health;
    [SerializeField] private int ActiveMovementRoundsAvailable;
    [SerializeField] private int MovementPointsPerRound = 2;

    [SerializeField] private BackgroundGenerationScript groundObject;

    private int alienI;
    private int alienJ;

    public void SetGroundObject(BackgroundGenerationScript script)
    {
        groundObject = script;
    }

    public void SetMovementPoints(int points)
    {
        ActiveMovementRoundsAvailable = points;
    }

    private void Start()
    {
        Health = Constants.GlobalLivingConstans.MaxHealth;
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
}