using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Hero))]
public class HeroMovement : MonoBehaviour
{
    private Hero hero;
    private BackgroundGenerationScript ground;
    private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // швидкість героя
    [SerializeField] private float rotationSpeed = 360f; // degrees per second, швидкість повороту героя

    /// <summary>
    /// Used in BackgroundGenerationScript
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        if (speed >= 0)
        {
            moveSpeed = speed;
        }        
    }

    private bool isMoving = false;
    private int targetI, targetJ;

    //Необхідна при заході на Преешкоду (схил та інші), бо
    // заходимо через Ramp
    private Queue<Vector3> currentPath = new Queue<Vector3>();
    private readonly CreatureMovingService creatureMovingService = new CreatureMovingService();

    private void Start()
    {
        hero = GetComponent<Hero>();
        ground = FindAnyObjectByType<BackgroundGenerationScript>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // якщо контролер не встановлений — нічого не робимо
        if (GameController.Instance == null) return;

        // Тільки активний герой має право реагувати на введення
        if (GameController.Instance.ActiveHero != hero)
            return;

        // Якщо герой уже рухається
        if (isMoving)
        {
            MoveTowardsTarget();
            return;
        }

        // Обробка кліку тільки коли герой може рухатись
        if (hero.CanMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TrySetNewTarget();
            }
        }
    }

    private void TrySetNewTarget()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int mask = ~LayerMask.GetMask("LiftTrigger");

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
            return;

        GameObject clicked = hit.collider.gameObject;

        // переконаємось що це клітинка
        if (!clicked.CompareTag(Constants.TagConstans.FloorGridTag))
            return;

        var targetWayPoint = clicked.GetComponent<WayPoint>();
        if (targetWayPoint == null)
            return;

        int i = Mathf.RoundToInt(targetWayPoint.Corrds.x);
        int j = Mathf.RoundToInt(targetWayPoint.Corrds.y);
        targetI = i;
        targetJ = j;

        var heroCoords = hero.GetHeroCoords();
        if (StaticTacticalData.GroundHierarchy == null)
            return;

        if (!creatureMovingService.TryCreatePath(
                StaticTacticalData.GroundHierarchy,
                (GroundHierarchyLevel)hero.CurrentFloor,
                heroCoords,
                targetWayPoint,
                hero.MovementPointsPerRound,
                out var newPath))
            return;

        currentPath = newPath;

        // під час руху прибираємо підсвітку
        hero.ClearHighlights();

        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        if (!isMoving || currentPath.Count == 0) return;

        Vector3 targetPos = currentPath.Peek();

        // фіксуємо Y, щоб герой не коливався по вертикалі
        targetPos.y = transform.position.y;

        // Rotate towards movement direction (only on Y axis)
        Vector3 lookDir = targetPos - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            // RotateTowards uses a max degrees delta, avoiding fractional-slerp semantics that can "halve" the remaining angle
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float step = moveSpeed * Time.deltaTime;

        // рухаємо героя гарантовано до точки
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // якщо вже дійшли
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            currentPath.Dequeue();

            if (currentPath.Count == 0)
            {
                isMoving = false;

                // тут можна викликати "кінець ходу героя"
                // hero.mov OnMoveFinished();

                // оновлюємо координати героя
                hero.UpdateHeroCoords(targetI, targetJ);

                // витрачаємо 1 хід
                hero.UseActionPoint();

                // після завершення руху — або показуємо нову підсвітку, або ховаємо
                if (hero.CanMoving)
                    hero.ShowAvailableMoves();
                else
                    hero.HideAvailableMoves();
            }
        }
    }
}