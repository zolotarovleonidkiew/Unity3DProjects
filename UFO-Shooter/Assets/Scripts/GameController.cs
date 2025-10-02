using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Constants;

/// <summary>
/// GameController
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public int CurrentRound { get; private set; } = 1;
    public bool IsHeroTurn { get; private set; } = true;

    [SerializeField] private List<Alien> aliens = new();
    [SerializeField] private List<Hero> heroes = new();
    [SerializeField] private int heroMovesPerTurn = Constants.GlobalLivingConstans.MaxActionRounds;

    //to do
    [SerializeField] private TacticalMapTargetsEnum MapTarget = TacticalMapTargetsEnum.AlienAnnihilation;
    [SerializeField] private float SquadReputation = 50;

    private int activeHeroIndex = 0;

    public Hero ActiveHero => (heroes.Count > 0 && activeHeroIndex >= 0 && activeHeroIndex < heroes.Count)
     ? heroes[activeHeroIndex]
     : null;

    private bool AnyHeroWithActiveMovePoints => heroes.Any(h => h.CanMoving);

    /// <summary>
    /// Викликається із BackgroundGenerationScript для додавання героя
    /// </summary>
    public void RegisterHero(Hero h)
    {
        if (!heroes.Contains(h))
        {
            heroes.Add(h);
            // При реєстрації одразу вимикаємо підсвітку — покажемо її пізніше для активного героя
            h.HideAvailableMoves();
        }
    }

    /// <summary>
    /// Викликається із BackgroundGenerationScript для додавання alien'а
    /// </summary>
    public void RegisterAlien(Alien a)
    {
        if (!aliens.Contains(a))
        {
            aliens.Add(a);
        }
    }

    /// <summary>
    /// Викликається із BackgroundGenerationScript - відкласти початок першого ходу до повного завершення реєстрацій героїв та інший обїектів
    /// </summary>
    public void BeginGame()
    {
        if (heroes.Count > 0)
        {
            StartHeroTurn();
        }
    }

    /// <summary>
    /// Change turn (from GUI)
    /// </summary>
    public void ChangeTurnFromGUI()
    {
        EndHeroTurn();
    }

    /// <summary>
    /// Next character (from GUI)
    /// </summary>
    public void NextHeroFromGUI()
    {
        SwitchToNextHero();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (heroes.Count > 0)
        {
            StartHeroTurn();
        }
    }

    private void Update()
    {
        if (heroes.Count == 0) return;

        if (IsHeroTurn)
        {
            // Переключення між героями (Tab)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchToNextHero();
            }

            // Закінчити хід вручну
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                EndHeroTurn();
            }
            else if (!AnyHeroWithActiveMovePoints)
            {
                EndHeroTurn();
            }
        }
        else
        {
            // TODO: логіка ходу прибульців
            EndAlienTurn();
        }

        //to do
        CheckForWinLoseConditions();
    }

    public void StartHeroTurn()
    {
        IsHeroTurn = true;
        activeHeroIndex = 0;

        // ховаємо підсвітку всіх героїв, щоб не було множинних підсвіток
        foreach (var hh in heroes)
            hh.HideAvailableMoves();

        if (ActiveHero != null)
        {
            ActiveHero.ShowAvailableMoves();
        }

        Debug.Log($"[GameController] Починається хід героя. Раунд: {CurrentRound}. Активний: {ActiveHero?.name}");
    }

    private void EndHeroTurn()
    {
        // ховаємо підсвітку активного героя (щоб після переходу не залишилась)
        ActiveHero?.HideAvailableMoves();
        //ActiveHero?.SetActiveFlag(false); //to do

        IsHeroTurn = false;
        Debug.Log("[GameController] Хід героя завершено. Тепер ходять прибульці...");
    }

    private void EndAlienTurn()
    {
        IsHeroTurn = true;
        CurrentRound++;
        //hero.SetMovementPoints(heroMovesPerTurn);

        //to do?
        //даємо кожному грою на 2 раунди
        foreach (var hh in heroes)
            hh.SetMovementPoints(heroMovesPerTurn);

        // ховаємо підсвітки всіх героїв (на всяк випадок)
        foreach (var hh in heroes)
            hh.HideAvailableMoves();

        activeHeroIndex = 0; // перший герой знову активний на початку нового раунду

        if (ActiveHero != null)
        {
            //ActiveHero.SetMovementPoints(heroMovesPerTurn);
            ActiveHero.ShowAvailableMoves();

            //переводимо камер в режмс слідування при переключенні героя
            FindObjectOfType<CameraMovement>().FocusOnHero();
        }

        Debug.Log($"[GameController] Хід прибульців завершено. Починається новий раунд: {CurrentRound}");
    }

    private void SwitchToNextHero()
    {
        if (heroes.Count <= 1) return;

        // приховуємо підсвітку попереднього героя
        ActiveHero?.HideAvailableMoves();

        activeHeroIndex = (activeHeroIndex + 1) % heroes.Count;

        // не даємо нових ходів при простому переключенні, просто показуємо підсвітку нового активного героя
        ActiveHero?.ShowAvailableMoves();

        //переводимо камер в режмс слідування при переключенні героя
        FindObjectOfType<CameraMovement>().FocusOnHero();

        Debug.Log($"[GameController] Активний герой змінено на: {ActiveHero?.name} (index {activeHeroIndex})");
    }

    //to do
    //check fow win/lose conditions
    private void CheckForWinLoseConditions()
    {
        if ((SquadReputation < 0) || (heroes.All(h => !h.isAlive)))
        {
            Debug.LogError($">>> TACTICAL MAP LOSE. Repuation = {SquadReputation}");
            //stop control
        }
        else if ((SquadReputation > 0) && (heroes.Any(h => h.isAlive)) && (aliens.All(a=>!a.isAlive)) )
        {
            SquadReputation++;
            Debug.LogError($">>> TACTICAL MAP WON. Repuation = {SquadReputation}");
            //stop control
        }
    }

}