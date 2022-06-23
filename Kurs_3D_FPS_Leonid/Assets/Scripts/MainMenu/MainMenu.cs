using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] HUDMenu _hud;

    [SerializeField] Button _startAsVegan;
    [SerializeField] Button _startAsME;

    public Teams UserTeam => _userTeam;

    private Teams _userTeam;

    void Start()
    {
        _startAsVegan.onClick.AddListener(StartAsVegan);
        _startAsME.onClick.AddListener(StartAsME);
    }


    void Update()
    {
        
    }

    private void StartAsVegan()
    {
        CreateTeams(Teams.Vegan);
    }

    private void StartAsME()
    {
        CreateTeams(Teams.MeatEater);
    }

    private void CreateTeams(Teams userTeam)
    {
        _userTeam = userTeam;

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);

        _hud.Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _hud.Hide();
    }
}
