using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DIalogWIndow : MonoBehaviour
{
    [SerializeField] Button _credits;
    [SerializeField] Button _cinematicks;

    [SerializeField] Toggle _enableXXX;
    [SerializeField] Toggle _allowXXX;
    [SerializeField] Toggle _fullscreen;
    [SerializeField] Toggle _soundInBackground;

    [SerializeField] Dropdown _resolution;
    [SerializeField] Dropdown _quality;

    [SerializeField] Slider _masterVolume;
    [SerializeField] Slider _musicVolume;

    void Start()
    {
        _credits.onClick.AddListener(OnCreditsButtonClickHandler);
        _cinematicks.onClick.AddListener(OnCinematicksButtonClickHandler);
        _enableXXX.onValueChanged.AddListener(OnEnableToggleChangedHandler);
        _allowXXX.onValueChanged.AddListener(OnAllowToggleChangedHandler);
        _fullscreen.onValueChanged.AddListener(OnFullscreebToggleChangedHandler);
        _soundInBackground.onValueChanged.AddListener(OnSoundInBackToggleChangedHandler);

        _resolution.onValueChanged.AddListener(delegate {
            ResolutionDropdownValueChanged(_resolution);
        });
        _quality.onValueChanged.AddListener(delegate {
            QualityDropdownValueChanged(_quality);
        });

        _masterVolume.onValueChanged.AddListener(delegate {
            MasterVolumeValueChanged(_masterVolume);
        });
        _musicVolume.onValueChanged.AddListener(delegate {
            MusicVolumeValueChanged(_musicVolume);
        });
    }

 
    private void OnCreditsButtonClickHandler()
    {
        Debug.Log("[OnCreditsButtonClickHandler] OK");
    }

    private void OnCinematicksButtonClickHandler()
    {
        Debug.Log("[OnCinematicksButtonClickHandler] OK");
    }

    private void OnEnableToggleChangedHandler(bool isOn)
    {
        Debug.Log($"[OnEnableToggleChangedHandler] => {isOn}");

        var label = _enableXXX.transform.GetChild(1).GetComponent<Text>();
        label.text = isOn ? "ON" : "OFF";
    }

    private void OnAllowToggleChangedHandler(bool isOn)
    {
        Debug.Log($"[OnAllowToggleChangedHandler] => {isOn}");

        var label = _allowXXX.transform.GetChild(1).GetComponent<Text>();
        label.text = isOn ? "ON" : "OFF";
    }

    private void OnFullscreebToggleChangedHandler(bool isOn)
    {
        Debug.Log($"[OnFullscreebToggleChangedHandler] => {isOn}");

        var label = _fullscreen.transform.GetChild(1).GetComponent<Text>();
        label.text = isOn ? "ON" : "OFF";
    }

    private void OnSoundInBackToggleChangedHandler(bool isOn)
    {
        Debug.Log($"[OnSoundInBackToggleChangedHandler] => {isOn}");

        var label = _soundInBackground.transform.GetChild(1).GetComponent<Text>();
        label.text = isOn ? "ON" : "OFF";
    }

    void ResolutionDropdownValueChanged(Dropdown change)
    {
        Debug.Log($"[ResolutionDropdownValueChanged] => {change.value}:{change.captionText.text}");
    }

    void QualityDropdownValueChanged(Dropdown change)
    {
        Debug.Log($"[QualityDropdownValueChanged] => {change.value}:{change.captionText.text}");
    }

    void MasterVolumeValueChanged(Slider _masterVolume)
    {
        Debug.Log($"[MasterVolumeValueChanged] => {_masterVolume.value}");
    }

    void MusicVolumeValueChanged(Slider _masterVolume)
    {
        Debug.Log($"[MasterVolumeValueChanged] => {_masterVolume.value}");
    }
}
