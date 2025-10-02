using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tactical GUI 
/// </summary>
public class GUI_script : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private Button btnChangeTurn;
    [SerializeField] private Button btnNextCharacter;
    [SerializeField] private TextMeshProUGUI txtTurnLabel;

    private bool? heroTurnState = null;

    void Start()
    {
        btnChangeTurn.onClick.AddListener(() => ChangeTurn());
        btnNextCharacter.onClick.AddListener(() => NextCharacter());

        txtTurnLabel.enabled = false;

        //monitoring player state
        if (heroTurnState == null)
        {
            heroTurnState = gameController.IsHeroTurn;
            ShowGUITurnText();
        }
    }

    #region Events
    void Update()
    {
        //monitoring player state
        if (heroTurnState != gameController.IsHeroTurn)
        {
            heroTurnState = gameController.IsHeroTurn;
            ShowGUITurnText();
        }
    }

    void Awake()
    {
        ShowGUITurnText();
    }

    private void ChangeTurn()
    {
        gameController.ChangeTurnFromGUI();
        ShowGUITurnText();
    }

    private void NextCharacter()
    {
        gameController.NextHeroFromGUI();
    }
    #endregion

    private void ShowGUITurnText()
    {
        var msg = gameController.IsHeroTurn ? Constants.GUIConstants.OurTurn : Constants.GUIConstants.AlienTurn;
        txtTurnLabel.enabled = true;
        txtTurnLabel.text = msg;

        StartCoroutine(ChangeTurntextDisbler());
    }

    private IEnumerator ChangeTurntextDisbler()
    {
        yield return new WaitForSeconds(Constants.GUIConstants.TurnMessageDisplayTimeInSecond);

        txtTurnLabel.enabled = false;
    }
}