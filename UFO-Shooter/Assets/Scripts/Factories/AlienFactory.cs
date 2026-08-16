using UnityEngine;

public class AlienFactory
{
    private float _alienSize;
    private Material _alienMaterial;
    private BackgroundGenerationScript _ground;
    private GameObject AlienCollectionGUI;

    public AlienFactory(float alienSize, Material alienMaterial, BackgroundGenerationScript ground, GameObject alienCollectionGUI)
    {
        _alienSize = alienSize;
        _alienMaterial = alienMaterial;
        _ground = ground;
        AlienCollectionGUI = alienCollectionGUI;
    }

    public Alien CreateAlien(GameObject targetCell, int index, GameObject creaturePrefab = null)
    {
        if (targetCell == null) return null;

        Vector3 pos = targetCell.transform.position;
        float alienY = _ground.bigHeight + _alienSize / 2f;

        GameObject alienGO;

        if (creaturePrefab != null)
        {
            alienGO = Object.Instantiate(creaturePrefab);            
            // place prefab at desired position (keep prefab rotation)           
        }
        else
        {
            alienGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            alienGO.transform.localScale = new Vector3(_alienSize, _alienSize, _alienSize);            
        }

        alienGO.transform.position = new Vector3(pos.x, alienY, pos.z);

        alienGO.name = $"Alien_{index}";
        alienGO.tag = Constants.TagConstans.AlienTag;

        //+
        var col = alienGO.GetComponent<Collider>();
        if (col == null)
        {
            // add a capsule collider as a sensible default
            var capsule = alienGO.AddComponent<CapsuleCollider>();
            capsule.height = _alienSize;
            capsule.radius = _alienSize / 2f;
            capsule.center = new Vector3(0f, _alienSize / 2f, 0f);
            col = capsule;
        }

        // Make sure transforms are synced so bounds are correct, then align bottom of collider to desired ground Y
        Physics.SyncTransforms();
        float bottomY = col.bounds.min.y;
        float deltaY = alienY - bottomY;
        if (Mathf.Abs(deltaY) > 0.0001f)
        {
            alienGO.transform.position += new Vector3(0f, deltaY + 0.001f, 0f);
        }

        // Ensure Rigidbody exists
        var rb = alienGO.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = alienGO.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        //-

        var alien = alienGO.AddComponent<Alien>();
        alien.SetGroundObject(_ground);

        alienGO.transform.SetParent(AlienCollectionGUI.transform);

        return alien;
    }
}