using System;
using UnityEngine;

public class AllItemsController : MonoBehaviour
{
    public int MedKitRespawnSecons;
    public int WeaponRespawnSecons;

    #region Red Team Items
    [SerializeField] private Healthitem _redMedKit1;
    [SerializeField] private Healthitem _redMedKit2;
    [SerializeField] private Healthitem _redMedKit3;

    [SerializeField] private WeaponItem _redGrenade;
    [SerializeField] private WeaponItem _redRevolver;
    [SerializeField] private WeaponItem _redTommyGun;
    #endregion

    #region Green Team Items
    [SerializeField] private Healthitem _greenMedKit1;
    [SerializeField] private Healthitem _greenMedKit2;
    [SerializeField] private Healthitem _greenMedKit3;

    [SerializeField] private WeaponItem _greenGrenade;
    [SerializeField] private WeaponItem _greenRevolver;
    [SerializeField] private WeaponItem _greenTommyGun;
    #endregion

    public void DisableItem(GameObject go)
    {
        go.SetActive(false);

        PrintDebug($"[AllItemsController] {go.name} was disabled.");
    }

    public void EnbleItem(GameObject go)
    {
        go.SetActive(true);

        PrintDebug($"[AllItemsController] {go.name} was enabled.");
    }

    private void PrintDebug(string msg)
    {
       // Debug.Log(msg);
    }
}