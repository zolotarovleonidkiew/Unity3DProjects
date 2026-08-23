using UnityEngine;

/// <summary>
/// A test script to control the fade of an obstruction object in the scene.
/// </summary>
public class ObstructionFadeTest : MonoBehaviour
{
    [SerializeField]
    private ObstructionObject obstruction;

    [SerializeField]
    [Range(0f, 1f)]
    private float fade = 1f;

    private void Update()
    {
        if (obstruction != null)
        {
            obstruction.SetFade(fade);
        }
    }
}