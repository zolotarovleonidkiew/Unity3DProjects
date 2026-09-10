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

    [SerializeField] private Image imgWeapon1;
    [SerializeField] private Image imgWeapon2;
    [SerializeField] private Image imgGrenade1;
    [SerializeField] private Image imgGrenade2;
    [SerializeField] private Image imgGrenade3;
    [SerializeField] private GameObject w1GreyPanel;
    [SerializeField] private GameObject w2GreyPanel;
    [SerializeField] private TextMeshProUGUI txtHeroName;
    [SerializeField] private TextMeshProUGUI txtHeroHealth;
    [SerializeField] private TextMeshProUGUI txtWeapon1MagazinesCount;
    [SerializeField] private TextMeshProUGUI txtWeapon2MagazinesCount;
    [SerializeField] private TextMeshProUGUI txtWeapon1AmmoCount;
    [SerializeField] private TextMeshProUGUI txtWeapon2AmmoCount;

    //******************************************************
    //Collection of GUI elements for the weapon display
    [SerializeField] public Sprite spriteNA;// DEFAULT - no weapon

    //Blaster weapons
    [SerializeField] public Sprite spriteAlienSmallHandBlaster;
    [SerializeField] public Sprite spriteBlasterRifle;
    [SerializeField] public Sprite spriteBlasterGrenade;
    [SerializeField] public Sprite spriteBlasterShotgun;
    
    //Lazer weapons
    [SerializeField] public Sprite spriteLazerRifle;
    [SerializeField] public Sprite spriteLazerShotGun;
    
    //Bullet-weapons    
    [SerializeField] public Sprite spritePistol;
    [SerializeField] public Sprite spriteRifle;
    [SerializeField] public Sprite spriteShotgun;
    [SerializeField] public Sprite spriteGrenade;
    [SerializeField] public Sprite spritePoisonGrenade;

    //Rocket weapons
    [SerializeField] private Sprite spriteRocketLauncher;   
    //******************************************************

    private bool? heroTurnState = null;

    void Start()
    {
        ResolveDisplayReferences();
        RegisterWeaponSelection(imgWeapon1, 0);
        RegisterWeaponSelection(imgWeapon2, 1);

        btnChangeTurn.onClick.AddListener(() => ChangeTurn());
        btnNextCharacter.onClick.AddListener(() => NextCharacter());

        txtTurnLabel.enabled = false;

        //monitoring player state
        if (heroTurnState == null)
        {
            heroTurnState = gameController.IsHeroTurn;
            ShowGUITurnText();
        }

        UpdateActiveHeroDisplay();
    }

    private void RegisterWeaponSelection(Image weaponImage, int weaponIndex)
    {
        if (weaponImage == null)
            return;

        Button button = weaponImage.GetComponentInParent<Button>();
        if (button == null)
            button = weaponImage.gameObject.AddComponent<Button>();

        button.onClick.AddListener(() => gameController?.ActiveHero?.SelectWeapon(weaponIndex));
    }

    #region Events
    void Update()
    {
        UpdateActiveHeroDisplay();

        //monitoring player state
        if (heroTurnState != gameController.IsHeroTurn)
        {
            heroTurnState = gameController.IsHeroTurn;
            ShowGUITurnText();
        }
    }

    private void ResolveDisplayReferences()
    {
        txtHeroName ??= FindText("txtHeroName");
        txtHeroHealth ??= FindText("txtHeroHealth");
        txtWeapon1MagazinesCount ??= FindText("txtWeapon1_magazinesCount");
        txtWeapon2MagazinesCount ??= FindText("txtWeapon2_magazinesCount");
        txtWeapon1AmmoCount ??= FindText("txtWeapon1_AmmoCount");
        txtWeapon2AmmoCount ??= FindText("txtWeapon2_AmmoCount");
        imgWeapon1 ??= FindImage("img_Weapon1");
        imgWeapon2 ??= FindImage("img_Weapon2");
        imgGrenade1 ??= FindImage("img_Grenade_1");
        imgGrenade2 ??= FindImage("img_Grenade_2");
        imgGrenade3 ??= FindImage("img_Grenade_3");
        w1GreyPanel ??= transform.Find("W1_GreyPanel")?.gameObject ?? GameObject.Find("W1_GreyPanel");
        w2GreyPanel ??= transform.Find("W2_GreyPanel")?.gameObject ?? GameObject.Find("W2_GreyPanel");
    }

    private static TextMeshProUGUI FindText(string objectName)
    {
        return GameObject.Find(objectName)?.GetComponent<TextMeshProUGUI>();
    }

    private static Image FindImage(string objectName)
    {
        return GameObject.Find(objectName)?.GetComponent<Image>();
    }

    private void UpdateActiveHeroDisplay()
    {
        if (gameController == null)
            return;

        ResolveDisplayReferences();

        Hero activeHero = gameController.ActiveHero;
        if (activeHero == null)
        {
            UpdateGrenadeDisplay(0);
            UpdateWeaponSelectionDisplay(-1);
            return;
        }

        if (txtHeroName != null)
            txtHeroName.text = activeHero.name;
        if (txtHeroHealth != null)
            txtHeroHealth.text = activeHero.CurentHealth.ToString();

        UpdateWeaponDisplay(0, imgWeapon1, txtWeapon1MagazinesCount, txtWeapon1AmmoCount, activeHero);
        UpdateWeaponDisplay(1, imgWeapon2, txtWeapon2MagazinesCount, txtWeapon2AmmoCount, activeHero);
        UpdateGrenadeDisplay(activeHero.GrenadeCount);
        UpdateWeaponSelectionDisplay(activeHero.GetActiveWeaponIndex());
    }

    private void UpdateWeaponSelectionDisplay(int activeWeaponIndex)
    {
        //if (w1GreyPanel != null)
        //{
        //    w1GreyPanel.SetActive(activeWeaponIndex == 0);
        //    w2GreyPanel.SetActive(activeWeaponIndex != 0);
        //}

        //if (w2GreyPanel != null)
        //{
        //    w2GreyPanel.SetActive(activeWeaponIndex == 1);
        //    w1GreyPanel.SetActive(activeWeaponIndex != 1);
        //}

        if (activeWeaponIndex == 0)
        {
            w1GreyPanel.SetActive(true);
            w2GreyPanel.SetActive(false);
        }
        else
        {
            w1GreyPanel.SetActive(false);
            w2GreyPanel.SetActive(true);
        }

    }

    private void UpdateGrenadeDisplay(int grenadeCount)
    {
        SetGrenadeVisibility(imgGrenade1, grenadeCount >= 1);
        SetGrenadeVisibility(imgGrenade2, grenadeCount >= 2);
        SetGrenadeVisibility(imgGrenade3, grenadeCount >= 3);
    }

    private static void SetGrenadeVisibility(Image grenadeImage, bool visible)
    {
        if (grenadeImage != null)
            grenadeImage.gameObject.SetActive(visible);
    }

    private void UpdateWeaponDisplay(
        int weaponIndex,
        Image image,
        TextMeshProUGUI magazinesText,
        TextMeshProUGUI ammoText,
        Hero activeHero)
    {
        HeroWeapon heroWeapon = activeHero.Weapons != null && activeHero.Weapons.Count > weaponIndex
            ? activeHero.Weapons[weaponIndex]
            : null;
        WeaponData weaponData = heroWeapon?.data;

        if (image != null)
            image.sprite = weaponData?.SpriteWeapon ?? spriteNA;
        if (magazinesText != null)
            magazinesText.text = weaponData?.currentMagazines.ToString() ?? "0";
        if (ammoText != null)
            ammoText.text = weaponData?.currentAmmoCount.ToString() ?? "0";
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