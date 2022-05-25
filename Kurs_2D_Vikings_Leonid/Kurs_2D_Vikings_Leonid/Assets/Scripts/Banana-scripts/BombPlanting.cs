using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Banana_scripts
{
    public class BombPlanting
    {
        public void Plant(Vector3 targetPosition, Sprite explotionSprite, Sprite bombHasBeenPlanted)
        {
            GameObject plantedBomb = new GameObject($"{Guid.NewGuid()}_generatedBomb");
            plantedBomb.AddComponent<SpriteRenderer>();
            plantedBomb.AddComponent<BoxCollider2D>();
            plantedBomb.AddComponent<Transform>();
            plantedBomb.AddComponent<Rigidbody2D>();

            var script = plantedBomb.AddComponent<PlantedBombTimer>();
            script.Explotion = explotionSprite;

            var tc = plantedBomb.GetComponent<Transform>();
            tc.position = targetPosition;

            var sr = plantedBomb.GetComponent<SpriteRenderer>();
            sr.sprite = bombHasBeenPlanted;
            sr.size = new Vector2(300, 300);
            sr.sortingOrder = 5;

            var bc2 = plantedBomb.AddComponent<BoxCollider2D>();
            bc2.isTrigger = false;
            bc2.size = new Vector2(2, 2);

            var bc = plantedBomb.GetComponent<BoxCollider2D>();
            bc.isTrigger = true;
            bc.size = new Vector2(10, 10);

        }
    }
}
