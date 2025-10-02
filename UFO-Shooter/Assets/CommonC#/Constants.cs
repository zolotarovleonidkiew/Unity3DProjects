
using System;

public static class Constants
{
    /// <summary>
    /// TEST-ONLY !!! 
    /// </summary>
    public static bool IsDebug = true;

    /// <summary>
    /// Константи для будування тактичних карт
    /// </summary>
    public static class TacticMapConstructingConstants
    {
        /// <summary>
        /// Висота 2ого поверху (відставь від 1ого до другого поверху)
        /// </summary>
        public static float Floor2Height = 4.2f; //(0,2f - висота ліфта)
    }

    /// <summary>
    /// Теги обєктів
    /// </summary>
    public static class TagConstans
    {
        public static string FloorGridTag = "FloorGridTag";
        public static string AlienTag = "Alien";
        public static string HeroTag = "Player";
    }

    /// <summary>
    /// Гемплей тактичної карти
    /// </summary>
    public static class GlobalLivingConstans
    {
        public static int MaxHealth = 100;
        public static int MaxActionRounds = 2;//макс. кількість пересувань 1 героя за раунд
    }

    /// <summary>
    /// GUI константи
    /// </summary>
    public static class GUIConstants
    {
        public static string OurTurn = "Наш хід...";
        public static string AlienTurn = "Хід прибульців...";
        public static int TurnMessageDisplayTimeInSecond = 3;
    }

    /// <summary>
    /// INT-значення для кастоминихх layer's
    /// </summary>
    public static class Layers
    {
        public static int LiftTriggerLayer = 10;
    }

    [Serializable]
    /// <summary>
    /// Цілі на конкретній тактичній карті
    /// </summary>
    public enum TacticalMapTargetsEnum
    {
        None = 0,
        /// <summary>
        /// Знищити всіх прибульців на території
        /// (прибульці нас не очікують)
        /// (багаті трофеї)
        /// </summary>
        AlienAnnihilation,
        /// <summary>
        /// Захист мирного населення, по факту те саме що і "AlienAnnihilation", але на карті є мирні жителі
        /// (прибульці нас не очікують)
        /// (трофеї мінімальні)
        /// </summary>
        ProtectionOfCivilians,
        /// <summary>
        /// Керування військовою базою під час облави прибульців.
        /// Прибульці йдуть волнами, і краще якнайшвидше їх прибити
        /// (прибульці готові до зустрічі з нами)
        /// (середній равень трофеїв)
        /// </summary>
        MilitaryBaseDefense,
        /// <summary>
        /// Штурм позицій прибульців
        /// (прибульці нас не очікують)
        /// (середній равень трофеїв)
        /// </summary>
        Assault,
        /// <summary>
        /// Зачистка місця приземлення підбитого НЛО від всякої інопланетної живності
        /// (прибульці готові до зустрічі з нами)
        /// (багаті трофеї)
        /// Можна не зачищати а віддати наоткуп місцевій армії, і в залежності від "результату" буде Репутація+/-
        /// </summary>
        DownedUFO,
        /// <summary>
        /// Герої виїзжають на точку для аналіза ситуації простонеба і намагаються вижити під осадою прибульців
        /// (прибульці готові до зустрічі з нами)
        /// (трофеї мінімальні)
        /// </summary>
        AlienSiege
    }


}