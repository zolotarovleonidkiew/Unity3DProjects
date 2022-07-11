using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDMenu : MonoBehaviour
{
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _hpAmmo;
    [SerializeField] private Text _temporaryMessage;
    [SerializeField] private Text _headerMessage;

    [SerializeField] private Button _slot1;
    [SerializeField] private Button _slot2;
    [SerializeField] private Button _slot3;
    [SerializeField] private Button _fire;

    [SerializeField] private InventoryController _inventory;
    [SerializeField] private WeaponShoot _weapon;

    void Start()
    {
        _slot1.onClick.AddListener(SelecttWeapon1);
        _slot2.onClick.AddListener(SelecttWeapon2);
        _slot3.onClick.AddListener(SelecttWeapon3);
        _fire.onClick.AddListener(Fire);

        _temporaryMessage.text = "";
        _headerMessage.text = "";
    }


    void Update()
    {

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// HUD : HP
    /// </summary>
    public void UpdateHP(int hp)
    {
        _hpText.text = hp.ToString();

        UpdateHeader($"HP updated [{hp}]");
    }

    /// <summary>
    /// HUD : Ammo
    /// </summary>
    public void UpdateAmmo(int ammo)
    {
        _hpAmmo.text = ammo.ToString();

        UpdateHeader($"Ammo updated [{ammo}]");
    }

    public void UpdateTemporaryMessage(string msg)
    {
        _temporaryMessage.text = msg;
    }

    public void UpdateHeader(string msg)
    {
        _headerMessage.text = msg;

        float _timeCounter = 0;
        float _duration = 5;

        DOTween.To(() => _timeCounter, x => _timeCounter = x, _duration, _duration)
         .OnComplete( ()=> UpdateHeader(""))
         .SetEase(Ease.Linear);
    }

    private void SelecttWeapon1()
    {
        _inventory.ChangeSelectedWeapon(0);
    }

    private void SelecttWeapon2()
    {
        _inventory.ChangeSelectedWeapon(1);
    }

    private void SelecttWeapon3()
    {
        _inventory.ChangeSelectedWeapon(2);
    }

    private void Fire()
    {
     //   var animator = _inventory.PlayerWeaponPlaceHolder.transform.GetChild(0).GetComponent<Animator>();
 
        _weapon.TryShoot();
    }
}
