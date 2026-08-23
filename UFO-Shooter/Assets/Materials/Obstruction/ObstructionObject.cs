using UnityEngine;

public class ObstructionObject : MonoBehaviour
{
    private static readonly int ObjectMinY = Shader.PropertyToID("_ObjectMinY");
    private static readonly int ObjectMaxY = Shader.PropertyToID("_ObjectMaxY");
    private static readonly int Fade = Shader.PropertyToID("_Fade");

    //fade animations
    private float targetFade = 1f;
    private float currentFade = 1f;
    
    [SerializeField]
    private float fadeSpeed = 5f;


    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        UpdateObjectBounds();
        ApplyFade();
    }

    private void Update()
    {
        if (Mathf.Approximately(currentFade, targetFade))
            return;

        currentFade = Mathf.MoveTowards(
            currentFade,
            targetFade,
            fadeSpeed * Time.deltaTime);

        ApplyFade();
    }

    private void UpdateObjectBounds()
    {
        if (renderers.Length == 0)
            return;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Renderer renderer in renderers)
        {
            Bounds bounds = renderer.bounds;

            // Берём 8 углов мирового bounding box
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            Vector3[] corners =
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, max.y, max.z)
            };

            foreach (Vector3 corner in corners)
            {
                Vector3 localPoint = transform.InverseTransformPoint(corner);

                minY = Mathf.Min(minY, localPoint.y);
                maxY = Mathf.Max(maxY, localPoint.y);
            }
        }

        foreach (Renderer renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetFloat(ObjectMinY, minY);
            propertyBlock.SetFloat(ObjectMaxY, maxY);

            renderer.SetPropertyBlock(propertyBlock);
        }

        Debug.Log(
            $"[{name}.UpdateObjectBounds()] ObjectMinY = {minY}, ObjectMaxY = {maxY}"
        );
    }

    public void SetFade(float value)
    {
        foreach (Renderer renderer in renderers)
        {
            targetFade = Mathf.Clamp01(value);
        }
    }

    private void ApplyFade()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetFloat(Fade, currentFade);

            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}