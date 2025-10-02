using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OperationNamesService : MonoBehaviour
{
    private readonly string[] FirstPart = new string[] { 
        "Білий", 
        "П`яний", 
        "Голубий",
        "М`ягкий", 
        "Прозорий", 
        "Чорний", 
        "Червоний",
        "Паралізуючий",
        "Летючий",
        "Пролонгований",
        "Плаваючий",
        "Підозрюючий",
        "Аналізуючий",
        "Відвертий",
        "Несмішний",
        "Кататонічний",
        "Схвильований",
        "Чайний",
        "Морозний",
        "Пливучий",
        "Синій",
        "Сором`язливий"};

    private readonly string[] LastPart = new string[] {
        "клоун",
        "ленін",
        "вулкан",
        "гелікоптер",
        "птеродактіль",
        "пікапер",
        "москалюк",
        "стіл",
        "стакан",
        "вело-тренажер",
        "капець",
        "що-ж-це-коється",
        "кіт",
        "карандаш",
        "Підпалюк",
        "стяг",
        "прапор",
        "мудрець",
        "огород",
        "сад",
        "клац-клац",
        "кроль"};

    /// <summary>
    /// Згенерувати назву тактичної операцїї для відображення в UI
    /// </summary>
    /// <returns></returns>
    public string GenereateOperationName() => $"{FirstPart[Random.Range(0, FirstPart.Length-1)]} {LastPart[Random.Range(0, LastPart.Length-1)]}";
}