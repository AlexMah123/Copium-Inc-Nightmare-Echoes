using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NightmareEchoes.Unit
{
    //made by terrence
    public class AIToBeCopied : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spr;
        [SerializeField] GameObject enemyManual;
        private double AUtil, HUtil, RUtil;
        public int HP;
        public float Dist;
        int gridX, gridY;

        void Start()
        {
            
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (gridX < 8)
                {
                    gridX++;
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (gridX > 0)
                {
                    gridX--;
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (gridY < 8)
                {
                    gridY++;
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (gridY > 0)
                {
                    gridY--;
                }
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (HP < 3)
                {
                    HP++;
                }
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (HP > 0)
                {
                    HP--;
                }
            }
            gameObject.transform.position = new Vector3((gridX * -0.5f) + (gridY * 0.5f), (gridX * 0.25f) + (gridY * 0.25f) - 2, 1);

            Dist = 2 * Vector2.Distance(transform.position, enemyManual.transform.position);

            AUtil = HP;
            HUtil = Dist - HP;
            RUtil = 3 - Dist - HP;

            if (AUtil >= HUtil)
            {
                if (AUtil >= RUtil)
                {
                    spr.color = Color.red;
                }
                else
                {
                    spr.color = Color.yellow;
                }
            }
            else
            {
                if (HUtil >= RUtil)
                {
                    spr.color = Color.green;
                }
                else
                {
                    spr.color = Color.yellow;
                }
            }
        }
    }
}
