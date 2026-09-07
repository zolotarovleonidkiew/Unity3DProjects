using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Constants;

/// <summary>
/// GameController
/// </summary>
public class GameController : MonoBehaviour
{
    //singletone
    public static GameController Instance;

    //armies
    [SerializeField] private List<Alien> aliens = new();
    [SerializeField] private List<Hero> heroes = new();

    //game round data
    public int CurrentRound { get; private set; } = 1;
    public bool IsHeroTurn { get; private set; } = true;
    private int activeHeroIndex = 0;
    private bool gameStarted;


    //tactical data - visuals
    [SerializeField]  private ObstructionManager obstructionManager;

    //tactical data - inner kitchen
    [SerializeField] private int heroMovesPerTurn = Constants.GlobalLivingConstans.MaxActionRounds;
    
    /// <summary>
    /// Викликається із BackgroundGenerationScript для реєстраціі ObstructionManager
    /// </summary>
    public void RegisterObstructionManager(ObstructionManager om)
    {
        if (obstructionManager is null)
        {
            obstructionManager = om;
        }
        else
        {
            throw new System.Exception("[GameController] ObstructionManager вже зареєстрований!");
        }
    }

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
    /// Active Hero
    /// </summary>
    public Hero ActiveHero => (heroes.Count > 0 && activeHeroIndex >= 0 && activeHeroIndex < heroes.Count)
     ? heroes[activeHeroIndex]
     : null;

    private bool AnyHeroWithActiveMovePoints => heroes.Any(h => h.CanMoving);

    //Plans for future
    [SerializeField] private TacticalMapTargetsEnum MapTarget = TacticalMapTargetsEnum.AlienAnnihilation;
    [SerializeField] private float SquadReputation = 50;

    //************************************************************************************************************************



    /// <summary>
    /// Викликається із BackgroundGenerationScript - відкласти початок першого ходу до повного завершення реєстрацій героїв та інший обїектів
    /// </summary>
    public void BeginGame()
    {
        if (gameStarted || heroes.Count == 0)
            return;

        gameStarted = true;
        StartHeroTurn();
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
    }

    private float lastManualSwitchTime;
    private void Update()
    {
        if (heroes.Count == 0) return;

        if (!gameStarted)
        {
            BeginGame();
            return;
        }

        if (IsHeroTurn)
        {
            // Переключення між героями (Tab)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                lastManualSwitchTime = Time.time;
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

            //автоматичний перехід хода до наступного героя, який має move point
            if (Time.time - lastManualSwitchTime > 0.1f)
            {
                if (AnyHeroWithActiveMovePoints && !ActiveHero.CanMoving)
                {
                    int? nextHeroIndex = default;

                    for (int i = 0; i < heroes.Count; i++)
                    {
                        if (heroes[i].CanMoving)
                        {
                            nextHeroIndex = i;
                            break;
                        }
                    }
                    SwitchToNextHero(nextHeroIndex);
                }
            }

        }
        else
        {
            // TODO: логіка ходу прибульців
            EndAlienTurn();
        }

        //to do
        //CheckForWinLoseConditions();
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
            // Сообщаем ObstructionManager, какой герой сейчас активен — чтобы управлять обструкциями
            obstructionManager?.SetActiveHeroObstructionTarget(ActiveHero.transform);
        }

       // Debug.Log($"[GameController] Починається хід героя. Раунд: {CurrentRound}. Активний: {ActiveHero?.name}");
    }

    private void EndHeroTurn()
    {
        // ховаємо підсвітку активного героя (щоб після переходу не залишилась)
        ActiveHero?.HideAvailableMoves();
        //ActiveHero?.SetActiveFlag(false); //to do

        IsHeroTurn = false;
        //Debug.Log("[GameController] Хід героя завершено. Тепер ходять прибульці...");
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

    private void SwitchToNextHero(int? heroIndex = null)
    {
        if (heroes.Count <= 1) return;

        // приховуємо підсвітку попереднього героя
        ActiveHero?.HideAvailableMoves();

        if (heroIndex is null)
        {
            int startIndex = activeHeroIndex;

            do
            {
                activeHeroIndex = (activeHeroIndex + 1) % heroes.Count;

                // якщо знайшли героя з ходами — виходимо
                if (heroes[activeHeroIndex].CanMoving)
                    break;

            } while (activeHeroIndex != startIndex);
        }
        else
        {
            activeHeroIndex = heroIndex.Value;
        }

        // не даємо нових ходів при простому переключенні, просто показуємо підсвітку нового активного героя
        ActiveHero?.ShowAvailableMoves();

        //переводимо камер в режмс слідування при переключенні героя
        FindObjectOfType<CameraMovement>().FocusOnHero();

        // Обновляем ObstructionManager при смене активного героя
        obstructionManager?.SetActiveHeroObstructionTarget(ActiveHero?.transform);

        Debug.Log($"[GameController] Активний герой змінено на: {ActiveHero?.name} (index {activeHeroIndex})");
    }

    //WIN/LOOSE conditions
    //private void CheckForWinLoseConditions()
    //{
    //    if ((SquadReputation < 0) || (heroes.All(h => !h.isAlive)))
    //    {
    //        Debug.LogError($">>> TACTICAL MAP LOSE. Repuation = {SquadReputation}");
    //        //stop control
    //    }
    //    else if ((SquadReputation > 0) && (heroes.Any(h => h.isAlive)) && (aliens.All(a => !a.isAlive)))
    //    {
    //        SquadReputation++;
    //        Debug.LogError($">>> TACTICAL MAP WON. Repuation = {SquadReputation}");
    //        //stop control
    //    }
    //}

}