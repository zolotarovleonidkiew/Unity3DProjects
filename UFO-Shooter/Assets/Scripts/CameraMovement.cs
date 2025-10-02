using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [Header("Game Controller ref")]
    [SerializeField] private GameController gameController;

    [Header("Camera follow")]
    [SerializeField] private Vector3 offset = new Vector3(0, 15, -10);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationX = 45f;

    [Header("Manual movement")]
    [SerializeField] private float moveSpeed = 10f;      // швидкість руху WASD
    [SerializeField] private float zoomSpeed = 5f;       // швидкість зума (units per scroll step)
    [SerializeField] private float minZoom = 3f;         // мінімальна відстань до цілі
    [SerializeField] private float maxZoom = 50f;        // максимальна відстань
    [SerializeField] private float rotationSpeed = 90f;  // градусів/сек для ←/→

    private Transform target;
    private float currentZoom;
    private float currentYaw = 0f;   // акумуляція кута (градуси)
    private bool followHero = true;

    private void Awake()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        currentZoom = offset.magnitude;
    }

    private void Start()
    {
        FocusOnHero();
    }

    private void LateUpdate()
    {
        HandleInput();

        if (followHero && target != null)
        {
            ApplyOrbitAroundTarget();
        }
    }

    private void HandleInput()
    {
        //old
        //// --- WASD (ручний пан) ---
        //float moveX = 0f;
        //float moveZ = 0f;

        //if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        //if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        //if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        //if (Input.GetKey(KeyCode.D)) moveX += 1f;

        //if (Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveZ) > 0.01f)
        //{
        //    followHero = false;
        //    Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        //    transform.Translate(move, Space.World);

        //    // якщо є target — синхронізуємо поточну відстань після пана
        //    if (target != null)
        //        currentZoom = Vector3.Distance(transform.position, target.position);
        //}

        //// --- Стрілки ← / → для повороту ---
        //float yawDelta = 0f;
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    yawDelta = -rotationSpeed * Time.deltaTime;
        //}
        //else if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    yawDelta = rotationSpeed * Time.deltaTime;
        //}

        //if (Mathf.Abs(yawDelta) > 0.0001f)
        //{
        //    followHero = false;

        //    if (target != null)
        //    {
        //        // обертаємо навколо цілі
        //        transform.RotateAround(target.position, Vector3.up, yawDelta);
        //        // синхронізуємо yaw та distance
        //        currentYaw = transform.eulerAngles.y;
        //        currentZoom = Vector3.Distance(transform.position, target.position);

        //        // зберігаємо нахил (rotationX)
        //        Vector3 e = transform.eulerAngles;
        //        transform.rotation = Quaternion.Euler(rotationX, e.y, 0f);
        //    }
        //    else
        //    {
        //        // немає target — обертаємо камеру у вільному просторі
        //        transform.Rotate(0f, yawDelta, 0f, Space.World);
        //        currentYaw = transform.eulerAngles.y;
        //    }
        //}

        //// --- Зум колесиком (працює завжди) ---
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        //if (Mathf.Abs(scroll) > 0.0001f)
        //{
        //    // позитивний scroll -> зазвичай наближення (залежить від миші)
        //    if (target != null)
        //    {
        //        // напрям від цілі до камери
        //        Vector3 dirToCam = (transform.position - target.position).normalized;
        //        float dist = Vector3.Distance(transform.position, target.position);
        //        dist -= scroll * zoomSpeed;
        //        dist = Mathf.Clamp(dist, minZoom, maxZoom);

        //        transform.position = target.position + dirToCam * dist;
        //        currentZoom = dist;

        //        // синхронізуємо yaw (щоб ApplyOrbit продовжив працювати коректно)
        //        currentYaw = transform.eulerAngles.y;

        //        // якщо в режимі слідкування - застосувати одразу
        //        if (followHero)
        //            ApplyOrbitAroundTargetImmediate();
        //    }
        //    else
        //    {
        //        // без target — зумемо по forward камери
        //        transform.position += transform.forward * (scroll * zoomSpeed);
        //    }
        //}

        //// --- Повернути фокус на героя (C або TAB) ---
        //if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Tab))
        //{
        //    FocusOnHero();
        //}

        //new
        // --- WASD (ручний пан) ---
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        if (Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveZ) > 0.01f)
        {
            followHero = false;

            // Беремо forward/right камери, але ігноруємо нахил по Y
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 move = (forward * moveZ + right * moveX).normalized * moveSpeed * Time.deltaTime;

            transform.position += move;

            // якщо є target — синхронізуємо поточну відстань після пана
            if (target != null)
                currentZoom = Vector3.Distance(transform.position, target.position);
        }

        // --- Стрілки ← / → для повороту ---
        float yawDelta = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            yawDelta = -rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            yawDelta = rotationSpeed * Time.deltaTime;
        }

        if (Mathf.Abs(yawDelta) > 0.0001f)
        {
            followHero = false;

            if (target != null)
            {
                transform.RotateAround(target.position, Vector3.up, yawDelta);
                currentYaw = transform.eulerAngles.y;
                currentZoom = Vector3.Distance(transform.position, target.position);

                Vector3 e = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(rotationX, e.y, 0f);
            }
            else
            {
                transform.Rotate(0f, yawDelta, 0f, Space.World);
                currentYaw = transform.eulerAngles.y;
            }
        }

        // --- Зум колесиком (працює завжди) ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            if (target != null)
            {
                Vector3 dirToCam = (transform.position - target.position).normalized;
                float dist = Vector3.Distance(transform.position, target.position);
                dist -= scroll * zoomSpeed;
                dist = Mathf.Clamp(dist, minZoom, maxZoom);

                transform.position = target.position + dirToCam * dist;
                currentZoom = dist;
                currentYaw = transform.eulerAngles.y;

                if (followHero)
                    ApplyOrbitAroundTargetImmediate();
            }
            else
            {
                transform.position += transform.forward * (scroll * zoomSpeed);
            }
        }

        // --- Повернути фокус на героя (C або TAB) ---
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Tab))
        {
            FocusOnHero();
        }
    }

    // Плавно застосувати orbit (коли followHero==true)
    private void ApplyOrbitAroundTarget()
    {
        if (target == null) return;

        Vector3 dir = Quaternion.Euler(0f, currentYaw, 0f) * offset.normalized;
        Vector3 desiredPos = target.position + dir * currentZoom;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(rotationX, currentYaw, 0f);
    }

    // Негайно застосувати orbit (одразу при зміні zoom чи фокусі)
    private void ApplyOrbitAroundTargetImmediate()
    {
        if (target == null) return;

        Vector3 dir = Quaternion.Euler(0f, currentYaw, 0f) * offset.normalized;
        transform.position = target.position + dir * currentZoom;
        transform.rotation = Quaternion.Euler(rotationX, currentYaw, 0f);
    }

    /// <summary>
    /// Фокусує камеру на активному герої (скидає всі повороти)
    /// </summary>
    public void FocusOnHero()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        if (gameController == null || gameController.ActiveHero == null)
            return;

        target = gameController.ActiveHero.transform;

        // увімкнути слідкування і скинути обертання
        followHero = true;
        currentYaw = 0f;

        // синхронізуємо відстань згідно поточного положення (або встановимо за offset)
        currentZoom = offset.magnitude;

        // оновити позицію одразу
        ApplyOrbitAroundTargetImmediate();
    }
}