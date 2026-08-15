using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[RequireComponent(typeof(Hero))]
public class HeroMovement : MonoBehaviour
{
    private Hero hero;
    private BackgroundGenerationScript ground;
    private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // швидкість героя

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
    private Vector3 targetPos;
    private int targetI, targetJ;

    //Необхідна при заході на Преешкоду (схил та інші), бо
    // заходимо через Ramp
    private Queue<Vector3> currentPath = new Queue<Vector3>();

    private void Start()
    {
        hero = GetComponent<Hero>();
        ground = FindObjectOfType<BackgroundGenerationScript>();
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

        // Use the exact contact point (hit.point) instead of transform.position
        Vector3 worldPoint = hit.point;

        // Optional: if your grid is at a fixed Y, force that Y:
        // worldPoint.y = clicked.transform.position.y;

        Debug.Log($"Clicked: {clicked.name}, hit.point={worldPoint}, colliderCenter={hit.collider.bounds.center}");

        if (!GridGenerator_05.GetGridCoordsFromWorld(worldPoint, out int i, out int j))
        {
            Debug.LogWarning($"GetGridCoordsFromWorld returned false for point {worldPoint}");
            return;
        }
        
        // перевірка доступності саме для цього героя
        if (!hero.IsCellAvailable(i, j))
            return;

        // встановлюємо ціль і починаємо рух
        //targetPos = new Vector3(clicked.transform.position.x, transform.position.y, clicked.transform.position.z);
        targetI = i;
        targetJ = j;

        //перевірка що герой та ціль пеерміщення на одній ПРЕГРАДІ
        var heroCoords = hero.GetHeroCoords();
        // guard against missing ground or its grid generator to avoid NullReferenceException
        var heroObstacle = ground?.GetGridGenerator?.GetObstacleAt(heroCoords.x, heroCoords.y);

        var obstacle = ground?.GetGridGenerator?.GetObstacleAt(i, j);

        if ((obstacle != null) && (heroObstacle != obstacle)) // це перешкода
        {
            // шукаємо рампу, яка веде на цю клітинку
            GameObject ramp = FindRampForObstacle(obstacle);//FindRampForObstacle(i, j);
            if (ramp == null) return;

            currentPath.Clear();
            currentPath.Enqueue(ramp.transform.position);       // крок 1 → на рампу
            currentPath.Enqueue(clicked.transform.position);    // крок 2 → у ціль на obstacle
        }
        else
        {
            // звичайний рух
            currentPath.Clear();
            currentPath.Enqueue(clicked.transform.position);
        }
        // під час руху прибираємо підсвітку
        hero.ClearHighlights();

        isMoving = true;
    }

    private GameObject FindRampForObstacle(ObstacleOnTheMap obstacle)
    {
        var rampsCollection = obstacle.RampsCollection;

        if (!rampsCollection.Any())
        {
            Debug.LogError("Нема заїзду на цю перешкоду()");
            return null;
        }

        //проверить длинну и маршрут до рампы
        //брать ближайший

        return rampsCollection.First();

        //delete RampOwner ????
    }

    private void MoveTowardsTarget()
    {
        if (!isMoving || currentPath.Count == 0) return;

        Vector3 targetPos = currentPath.Peek();

        // фіксуємо Y, щоб герой не коливався по вертикалі
        targetPos.y = transform.position.y;

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