using System.Linq;
using UnityEngine;

/// <summary>
/// ������ � ������� ������ ��������� ������ � ��������� ����������
/// </summary>
public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] public Transform[] VikingTransforms;

    public Character[] PlayerCharacters;
    private Character CurrentPlayer;
    int selectedUserId = 0;


    /// <summary>
    /// ��� ������ ������� ������ ��� �� ������, ���� ������ �� ���� �� ������������ ������
    /// </summary>
    /// <param name="viking"></param>
    public void DeleteDeadVikingFromQueue(Character viking)
    {

    }

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
        if (VikingTransforms.Count(v => v == null) == 3)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectedUserId = GetNextActiveVikingIndex(selectedUserId);

            CurrentPlayer = PlayerCharacters[selectedUserId];
        }

        //check viking was dead
        if (VikingTransforms[selectedUserId] == null)
        {
            selectedUserId = GetNextActiveVikingIndex(selectedUserId);
        }

        //move camera
        UpdateCameraPosition(VikingTransforms[selectedUserId]);

        //move upper panel
        UpdateUpperPanelPosition(VikingTransforms[selectedUserId]);
    }

    private int GetNextActiveVikingIndex(int currentIndex)
    {
        var inextI = currentIndex + 1;

        if (inextI > VikingTransforms.Length - 1)
        {
            inextI = 0;
        }

        if (VikingTransforms[inextI] == null)
        {
            return GetNextActiveVikingIndex(inextI);
        }
        else
        {
            selectedUserId = inextI;
            return inextI;
        }
    }

    private void UpdateCameraPosition(Transform currentVikingTransform)
    {
        Vector3 cameraPosition = transform.position;

        cameraPosition.x = currentVikingTransform.position.x;
        cameraPosition.y = currentVikingTransform.position.y + 9;

        transform.position = cameraPosition;
    }

    private void UpdateUpperPanelPosition(Transform currentVikingTransform)
    {
        var UpperPanelObjects = GameObject.FindGameObjectsWithTag("UpperPanelTag");

        foreach (var po in UpperPanelObjects)
        {
            Vector3 poPosition = po.transform.position;

            poPosition.x = currentVikingTransform.position.x;
            poPosition.y = currentVikingTransform.position.y + 10.5f + 9f;

            po.transform.position = poPosition;
        }

        //update Baelog ico position
        var BaelogIco = UpperPanelObjects.Where(u => u.name == "BaleogStatusBar").First();

        Vector3 BaelogIcoTransform = BaelogIco.transform.position;

        BaelogIcoTransform.x -= 20;//16.34f;

        BaelogIco.transform.position = BaelogIcoTransform;

        //update Olaf ico position
        var OlafIco = UpperPanelObjects.Where(u => u.name == "OlafStatusBar").First();

        Vector3 OlafIcoTransform = OlafIco.transform.position;

        OlafIcoTransform.x -= 3 ;//16.34f;

        OlafIco.transform.position = OlafIcoTransform;

        //update Ulrich ico position
        var UlrichIco = UpperPanelObjects.Where(u=>u.name == "UlrichStatusBar").First();

        Vector3 UlrichIcoTransform = UlrichIco.transform.position;

        UlrichIcoTransform.x += 16-3;

        UlrichIco.transform.position = UlrichIcoTransform;
    }
}
