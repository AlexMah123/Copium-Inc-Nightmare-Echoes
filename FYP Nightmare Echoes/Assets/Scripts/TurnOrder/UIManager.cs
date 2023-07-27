using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using NightmareEchoes.TurnOrder;
using System.ComponentModel;
using Unity.Collections;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Test Buttons")] // basic attack
        [SerializeField] Button testButton;

        [Header("Turn Order Bar")]
        int capIndex = 0;
        [SerializeField] int imagePoolCap = 6;
        [SerializeField] GameObject turnOrderPanel;
        [SerializeField] GameObject ImagePrefab;

        [Space(15)]
        [SerializeField] List<GameObject> imageObjectPool;
        [SerializeField] GameObject turnIndicator;
        [SerializeField] TextMeshProUGUI turnIndicatorText;
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Header("Hotbar Info")]
        [SerializeField] List<Button> currentUnitButtonList;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        BaseUnit CurrentUnit { get => TurnOrderController.Instance.CurrentUnit;}

        [Header("Inspectable Info")]
        [SerializeField] BaseUnit inspectedUnit;
        [SerializeField] List<Button> inspectedUnitButton;
        [SerializeField] Button inspectedUnitProfile;
        [SerializeField] TextMeshProUGUI inspectedUnitNameText;

        [Header("Settings")]
        [SerializeField] Button settingButton;
        [SerializeField] GameObject settingsPanel;
        public bool gameIsPaused = false;

        [Header("Current Unit Indicator")]
        [SerializeField] GameObject indicator;
        [SerializeField] private float frequency = 2.0f;
        [SerializeField] private float magnitude = 0.05f;
        [SerializeField] private float offset = 0.75f;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            InitImagePool();

        }

        private void Start()
        {

        }

        private void Update()
        {

            if(CurrentUnit != null)
            {
                currentUnitNameText.text = $"{CurrentUnit.Name}";
            }

            #region current unit indicator
            if (CurrentUnit != null)
            {
                if (!indicator.activeSelf)
                {
                    indicator.SetActive(true);
                }

                if(CurrentUnit.IsHostile) 
                {
                    indicator.GetComponent<SpriteRenderer>().color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b, enemyTurn.a);
                }
                else
                {
                    indicator.GetComponent<SpriteRenderer>().color = new Color(playerTurn.r, playerTurn.g, playerTurn.b, playerTurn.a);
                }

                indicator.transform.position = new Vector3(CurrentUnit.transform.position.x, CurrentUnit.transform.position.y + offset, CurrentUnit.transform.position.z) 
                    + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
            }
            else
            {
                indicator.SetActive(false);
            }

            #endregion


            #region TurnOrderPanel

            //sets indicator to the first image on the list
            if (imageObjectPool[0].activeSelf)
            {
                if(!turnIndicator.activeSelf)
                {
                    turnIndicator.SetActive(true);
                }

                turnIndicator.transform.position = imageObjectPool[0].transform.position;
            }
            else
            {
                turnIndicator.SetActive(false);
            }
            #endregion


            #region Phase UI

            if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.startPhase)
            {
                turnIndicatorText.text = $"Start Phase";

            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.planPhase)
            {
                turnIndicatorText.text = $"Plan Phase";

            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.playerPhase)
            {
                turnIndicatorText.text = $"Player's Phase";
                turnIndicatorText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.enemyPhase)
            {
                turnIndicatorText.text = $"Enemy's Phase";
                turnIndicatorText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.endPhase)
            {
                turnIndicatorText.text = $"End's Phase";
            }


            #endregion


            #region InspectedUnit
            if (Input.GetMouseButtonDown(1)) // rightclick on an inspectable unit
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject.CompareTag("Inspectable"))
                {
                    inspectedUnit = hit.collider.gameObject.GetComponent<BaseUnit>();
                    EnableInspectedUI(true);
                }
                else
                {
                    EnableInspectedUI(false);
                }


            }
            #endregion


        }


        #region Hotbar Functions
        public void AttackButton()
        {
            CurrentUnit.BasicAttack();
            PassTurn();
        }

        public void Skill1Button()
        {
            CurrentUnit.Skill1();
            PassTurn();
        }

        public void Skill2Button()
        {
            CurrentUnit.Skill2();
            PassTurn();
        }

        public void Skill3Button()
        {
            CurrentUnit.Skill3();
            PassTurn();
        }

        private void PassTurn()
        {
            //if there is at least 2 elements in queue
            if (TurnOrderController.Instance.UnitQueue.Count > 1)
            {
                //if the second element exist, check hostile and change accordingly, else endPhase
                if (TurnOrderController.Instance.UnitQueue.ToArray()[1].IsHostile)
                {
                    TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.enemyPhase);
                }
                else
                {
                    TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.playerPhase);
                }
            }
            else
            {
                TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.endPhase);
            }
        }

        #endregion


        #region UI Function

        void InitImagePool()
        {
            for (int i = 0; i < imagePoolCap; i++)
            {
                GameObject obj = Instantiate(ImagePrefab, turnOrderPanel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";
                imageObjectPool.Add(obj);
            }
        }

        public void UpdateTurnOrderUI()
        {
            //resets the turn order bar, calculate turn order
            for (int i = 0; i < imageObjectPool.Count; i++)
            {
                imageObjectPool[i].SetActive(false);
            }


            //sets all the images in the panel
            for (int i = 0; i < TurnOrderController.Instance.UnitQueue.Count; i++)
            {
                GameObject image = GetImageObject();

                if(image != null)
                {
                    image.SetActive(true);

                    if (TurnOrderController.Instance.UnitQueue.ToArray()[i].IsHostile)
                    {
                        image.GetComponent<Image>().color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b, enemyTurn.a);

                    }
                    else
                    {
                        image.GetComponent<Image>().color = new Color(playerTurn.r, playerTurn.g, playerTurn.b, playerTurn.a);
                    }
                }
            }

            
        }

        GameObject GetImageObject()
        {
            for (int i = 0; i < imageObjectPool.Count; i++)
            {
                if (!imageObjectPool[i].activeInHierarchy)
                {
                    return imageObjectPool[i];
                }
            }          

            return null;
        }

        public void EnablePlayerUI(bool enable)
        {
            foreach (Button button in currentUnitButtonList)
            {
                button.interactable = enable;
            }
        }


        public void EnableInspectedUI(bool enable)
        {
            foreach (Button button in inspectedUnitButton)
            {
                button.interactable = enable;
            }

            inspectedUnitProfile.gameObject.SetActive(enable);

            if (enable)
            {
                inspectedUnitNameText.text = $"{inspectedUnit.Name}";
            }
            else
            {
                inspectedUnitNameText.text = $"";
            }

        }

        public void SettingsButton()
        {
            if (Time.timeScale > 0)
            {
                gameIsPaused = true;
                Time.timeScale = 0;
                settingsPanel.SetActive(true);
            }
            else
            {
                gameIsPaused = false;
                Time.timeScale = 1;
                settingsPanel.SetActive(false);
            }
        }
        #endregion
    }
}
