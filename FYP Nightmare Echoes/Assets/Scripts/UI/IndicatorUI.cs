using NightmareEchoes.TurnOrder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NightmareEchoes.UI
{
    public class IndicatorUI : MonoBehaviour
    {
        [SerializeField] private float frequency = 1.0f;
        [SerializeField] private float magnitude = 1.0f;
        [SerializeField] private float offset = 0.5f;

        void Update()
        {
            if(TurnOrderController.Instance.CurrentUnit != null)
            {
                if(!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                transform.position = new Vector3(
                    TurnOrderController.Instance.CurrentUnit.transform.position.x,
                    TurnOrderController.Instance.CurrentUnit.transform.position.y + offset,
                    TurnOrderController.Instance.CurrentUnit.transform.position.y) + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
