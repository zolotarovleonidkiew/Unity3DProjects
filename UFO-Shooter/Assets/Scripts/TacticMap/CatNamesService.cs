using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatNamesService : MonoBehaviour
{
    private readonly string[] CatNamesCollection = new string[] { 
        "Мо-Мо",
        "Мау-Мау",
        "Чернушеску",
        "Величний",
        "Тінь",
        "Шипучка",
        "Аспірін-Ша",
        "Калачік",
        "Турбуленціо",
        "Тігра",
        "Киц-киц-муркиц",
        "Сімпатюля",
        "Вушастик",
        "Мяуріціо",
        "Хвостюля",
        "Дартань-мяв",
        "Черепашка",
        "Віола",
        "Венеція"
    };

    /// <summary>
    /// Згенерувати назву тактичної операцїї для відображення в UI
    /// </summary>
    /// <returns></returns>
    public string GenereateOperationName() => $"{CatNamesCollection[Random.Range(0, CatNamesCollection.Length - 1)]}";
}
