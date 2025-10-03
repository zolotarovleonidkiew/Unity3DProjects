using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // швидк≥сть кул≥
    private Transform target;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        // –озвертаЇмо пулю у напр€мку ц≥л≥
        transform.LookAt(target.position);
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
