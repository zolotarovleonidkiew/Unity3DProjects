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

        //old
        //if (!Physics.Raycast(ray, out RaycastHit hit))        
        //new
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
            return;

        GameObject clicked = hit.collider.gameObject;

        // переконаємось що це клітинка
        if (!clicked.CompareTag(Constants.TagConstans.FloorGridTag))
            return;

        // витягуємо індекси клітинки
        if (!ground.GetGridCoordsFromWorld(clicked.transform.position, out int i, out int j))
            return;

        // перевірка доступності саме для цього героя
        if (!hero.IsCellAvailable(i, j))
            return;

        // встановлюємо ціль і починаємо рух
        targetPos = new Vector3(clicked.transform.position.x, transform.position.y, clicked.transform.position.z);
        targetI = i;
        targetJ = j;

        // під час руху прибираємо підсвітку
        hero.ClearHighlights();

        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            isMoving = false;

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