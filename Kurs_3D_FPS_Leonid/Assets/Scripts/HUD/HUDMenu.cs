﻿using UnityEngine;
using UnityEngine.UI;

public class HUDMenu : MonoBehaviour
{
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _hpAmmo;
    [SerializeField] private Text _temporaryMessage;

    void Start()
    {
        _temporaryMessage.text = "";
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
    }

    /// <summary>
    /// HUD : Ammo
    /// </summary>
    public void UpdateAmmo(int ammo)
    {
        _hpAmmo.text = ammo.ToString();
    }

    public void UpdateTemporaryMessage(string msg)
    {
        _temporaryMessage.text = msg;
    }
}
