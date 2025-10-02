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

    public Alien CreateAlien(GameObject targetCell, int index)
    {
        if (targetCell == null) return null;

        Vector3 pos = targetCell.transform.position;
        float alienY = _ground.bigHeight + _alienSize / 2f;

        GameObject alienGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        alienGO.name = $"Alien_{index}";
        alienGO.transform.localScale = new Vector3(_alienSize, _alienSize, _alienSize);
        alienGO.transform.position = new Vector3(pos.x, alienY, pos.z);
        //alienGO.GetComponent<Renderer>().material = _alienMaterial;
        var rend = alienGO.GetComponent<Renderer>();
        if (_alienMaterial != null)
            rend.material = new Material(_alienMaterial);
        else
            rend.material = new Material(Shader.Find("Standard"));

        alienGO.tag = Constants.TagConstans.AlienTag;

        var rb = alienGO.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        var alien = alienGO.AddComponent<Alien>();
        alien.SetGroundObject(_ground);

        alienGO.transform.SetParent(AlienCollectionGUI.transform);

        return alien;
    }
}