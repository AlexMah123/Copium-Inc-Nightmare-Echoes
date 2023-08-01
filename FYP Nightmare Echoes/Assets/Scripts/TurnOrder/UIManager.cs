using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using NightmareEchoes.TurnOrder;
using System.ComponentModel;
using Unity.Collections;
using System;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;


        [Space(15), Header("Turn Order Bar")]
        int capIndex = 0;
        [SerializeField] int imagePoolCap = 6;
        [SerializeField] GameObject imagePrefab;
        [SerializeField] GameObject turnIndicator;
        [SerializeField] GameObject currentTurnOrderPanel;
        [SerializeField] TextMeshProUGUI currentTurnNum;
        [SerializeField] TextMeshProUGUI phaseText;

        List<GameObject> imageObjectPool = new List<GameObject>();
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Space(15), Header("Hotbar Info")]
        [SerializeField] List<Button> currentUnitButtonList;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        [SerializeField] TextMeshProUGUI currentUnitHealthText;
        [SerializeField] TextMeshProUGUI currentUnitSpeedText;
        [SerializeField] TextMeshProUGUI currentUnitMoveRangeText;
        [SerializeField] TextMeshProUGUI currentUnitStunResistText;
        [SerializeField] TextMeshProUGUI currentUnitResistText;
        [SerializeField] Slider currentUnitHealth;
        BaseUnit CurrentUnit { get => TurnOrderController.Instance.CurrentUnit;}

        [Space(15), Header("Inspectable Info")]
        [SerializeField] BaseUnit inspectedUnit;
        [SerializeField] List<Button> inspectedUnitButtonList;
        [SerializeField] GameObject inspectedUnitPanel;
        [SerializeField] Button inspectedUnitProfile;
        [SerializeField] TextMeshProUGUI inspectedUnitNameText;
        [SerializeField] TextMeshProUGUI inspectedUnitHealthText;
        [SerializeField] TextMeshProUGUI inspectedUnitSpeedText;
        [SerializeField] TextMeshProUGUI inspectedUnitMoveRangeText;
        [SerializeField] TextMeshProUGUI inspectedUnitStunResistText;
        [SerializeField] TextMeshProUGUI inspectedUnitResistText;
        [SerializeField] Slider inspectedUnitHealth;

        [Space(15), Header("Settings")]
        [SerializeField] Button settingButton;
        [SerializeField] GameObject settingsPanel;
        [NonSerialized] public bool gameIsPaused = false;

        [Space(15), Header("Current Unit")]
        [SerializeField] TextMeshProUGUI unitAction;
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

            //initialize objectpool
            InitImagePool(currentTurnOrderPanel);

        }

        private void Start()
        {
            inspectedUnitPanel.SetActive(false);
        }

        private void Update()
        {

            #region CurrentUnit
            if (CurrentUnit != null)
            {
                currentUnitProfile.image.sprite = CurrentUnit.Sprite;
                currentUnitNameText.text = $"{CurrentUnit.Name}";
                currentUnitHealthText.text = $"{CurrentUnit.Health}/{CurrentUnit.MaxHealth}";
                currentUnitSpeedText.text = $"Speed: {CurrentUnit.Speed}";
                currentUnitMoveRangeText.text = $"Move Range: {CurrentUnit.MoveRange}";
                currentUnitStunResistText.text = $"Stun Resist: {CurrentUnit.StunResist}%";
                currentUnitResistText.text = $"Resist: {CurrentUnit.Resist}%";

                currentUnitHealth.maxValue = CurrentUnit.MaxHealth;
                currentUnitHealth.value = CurrentUnit.Health;

            }
            #endregion

            #region current unit indicator
            if (CurrentUnit != null)
            {
                if (!indicator.activeSelf)
                {
                    indicator.SetActive(true);
                }

                if (CurrentUnit.IsHostile)
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

            #region InspectedUnit
            if (Input.GetMouseButtonDown(1)) // rightclick on an inspectable unit
            {
                bool selected = false;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                int unitMask = LayerMask.GetMask("Unit");

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, unitMask);
                
                if(hit) 
                {
                    if (hit.collider.gameObject.CompareTag("Inspectable"))
                    {
                        inspectedUnit = hit.collider.gameObject.GetComponent<BaseUnit>();
                        selected = true;
                        EnableInspectedUI(true);
                    }
                    else
                    {
                        selected = false;
                    }
                }

                if(!selected)
                {
                    EnableInspectedUI(false);
                }

            }
            #endregion

            #region TurnOrderPanel

            currentTurnNum.text = $"{ TurnOrderController.Instance.turnCount}";


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
                phaseText.text = $"Start Phase";

            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.planPhase)
            {
                phaseText.text = $"Plan Phase";

            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.playerPhase)
            {
                phaseText.text = $"Player's Phase";
                phaseText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.enemyPhase)
            {
                phaseText.text = $"Enemy's Phase";
                phaseText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.endPhase)
            {
                phaseText.text = $"End's Phase";
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
            if (TurnOrderController.Instance.CurrentUnitQueue.Count > 1)
            {
                //if the second element exist, check hostile and change accordingly, else endPhase
                if (TurnOrderController.Instance.CurrentUnitQueue.ToArray()[1].IsHostile)
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

        void InitImagePool(GameObject panel)
        {

            for (int i = 0; i < imagePoolCap; i++)
            {
                GameObject obj = Instantiate(imagePrefab, panel.transform);
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


            //sets all the images in the panel for current turn
            for (int i = 0; i < TurnOrderController.Instance.CurrentUnitQueue.Count; i++)
            {
                GameObject image = GetImageObject();

                if(image != null)
                {
                    image.SetActive(true);

                    if (TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].IsHostile)
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
            foreach (Button button in inspectedUnitButtonList)
            {
                button.interactable = enable;
            }

            inspectedUnitPanel.SetActive(enable);

            if (enable)
            {
                inspectedUnitProfile.image.sprite = inspectedUnit.Sprite;
                inspectedUnitNameText.text = $"{inspectedUnit.Name}";
                inspectedUnitHealthText.text = $"{inspectedUnit.Health}/{inspectedUnit.MaxHealth}";
                inspectedUnitSpeedText.text = $"Speed: {inspectedUnit.Speed}";
                inspectedUnitMoveRangeText.text = $"Move Range: {inspectedUnit.MoveRange}";
                inspectedUnitStunResistText.text = $"Stun Resist: {inspectedUnit.StunResist}%";
                inspectedUnitResistText.text = $"Resist: {inspectedUnit.Resist}%";

                inspectedUnitHealth.maxValue = inspectedUnit.MaxHealth;
                inspectedUnitHealth.value = inspectedUnit.Health;
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
