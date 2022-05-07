using System.Linq;
using UnityEngine;

/// <summary>
/// Камера и верхняя панель двигается вместе с выбранным персонажем
/// </summary>
public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] public Transform[] VikingTransforms;

    public Character[] PlayerCharacters;
    private Character CurrentPlayer;
    int selectedUserId = 0;

    public CameraFollowPlayer()
    {
        PlayerCharacters = new Character[] {
            new Viking2_Olaf(),
            new Viking1_Ulrick(),
            new Viking3_Baelog()
        };

        CurrentPlayer = PlayerCharacters[selectedUserId];
    }
    void Start()
    {
        Debug.LogWarning($"CameraFollowPlayer=>Start=>CurrentPlayer : {CurrentPlayer.Name}");
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //var oldName = CurrentPlayer.Name;

            selectedUserId =
                selectedUserId + 1 >= PlayerCharacters.Length - 1 ?
                selectedUserId = 0 :
                selectedUserId + 1;

            CurrentPlayer = PlayerCharacters[selectedUserId];
        }

        //move camera
        UpdateCameraPosition(VikingTransforms[selectedUserId]);

        //move upper panel
        UpdateUpperPanelPosition(VikingTransforms[selectedUserId]);
    }

    private void UpdateCameraPosition(Transform currentVikingTransform)
    {
        Vector3 cameraPosition = transform.position;

        cameraPosition.x = currentVikingTransform.position.x;
        cameraPosition.y = currentVikingTransform.position.y + 9;  //+ camera.pixelHeight / 2;;

        transform.position = cameraPosition;
    }

    private void UpdateUpperPanelPosition(Transform currentVikingTransform)
    {
        var UpperPanelObjects = GameObject.FindGameObjectsWithTag("UpperPanelTag");

        foreach (var po in UpperPanelObjects)
        {
            Vector3 poPosition = po.transform.position;

            poPosition.x = currentVikingTransform.position.x;
            poPosition.y = currentVikingTransform.position.y + 10.5f + 9.1f; // ломает !!!! ???

            po.transform.position = poPosition;
        }

        //update Baelog ico position
        var BaelogIco = UpperPanelObjects.Where(u => u.name == "BaleogStatusBar").First();

        Vector3 BaelogIcoTransform = BaelogIco.transform.position;

        BaelogIcoTransform.x -= 16.34f;

        BaelogIco.transform.position = BaelogIcoTransform;

        //update Ulrich ico position
        var UlrichIco = UpperPanelObjects.Where(u=>u.name == "UlrichStatusBar").First();

        Vector3 UlrichIcoTransform = UlrichIco.transform.position;

        UlrichIcoTransform.x += 16;

        UlrichIco.transform.position = UlrichIcoTransform;


        // TODO:
        // update inventary positions

    }
}
