using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] Button _closeButton;
    [SerializeField] Toggle _musicOn;
    [SerializeField] Toggle _soundEffectsOn;
    [SerializeField] Dropdown _Language;
    [SerializeField] Button _facebookConnect;
    [SerializeField] Button _devicesConnect;

    void Start()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClickHandler);
        _musicOn.onValueChanged.AddListener(OnMusicToggleChangedHandler);
        _soundEffectsOn.onValueChanged.AddListener(OnSoundEffectsToggleChangedHandler);
        _Language.onValueChanged.AddListener(delegate {
            DropdownValueChanged(_Language);
        });
        _facebookConnect.onClick.AddListener(OnFacebookConnectClickHandler);
        _devicesConnect.onClick.AddListener(OnDevicesConnectClickHandler);
        
    }


    void Update()
    {
        
    }

    private void OnCloseButtonClickHandler()
    {
        Debug.Log("[OnCloseButtonClickHandler] OK");
    }

    private void OnMusicToggleChangedHandler(bool isOn)
    {
        Debug.Log($"[OnMusicToggleChangeHandler] => {isOn}");

        var label = _musicOn.transform.GetChild(1).GetComponent<Text>();
        label.text = isOn ? "ON" : "OFF";
    }

    private void OnSoundEffectsToggleChangedHandler(bool isOn)
    {
        Debug.Log($"[OnSoundEffectsToggleChangeHandler] => {isOn}");

        var label = _soundEffectsOn.transform.GetChild(1).GetComponent<Text>();
        label.text = isOn ? "ON" : "OFF";
    }

    private void OnLenguageValueChangedHandler(string val)
    {
        Debug.Log($"[OnLenguageValueChangedHandler] => {val}");
    }

    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log($"[OnLenguageValueChangedHandler] => {change.value}:{change.captionText.text}");
    }

    private void OnFacebookConnectClickHandler()
    {
        Debug.Log("[OnFacebookConnectClickHandler] OK");
    }

    private void OnDevicesConnectClickHandler()
    {
        Debug.Log("[OnDevicesConnectClickHandler] OK");
    }
}
