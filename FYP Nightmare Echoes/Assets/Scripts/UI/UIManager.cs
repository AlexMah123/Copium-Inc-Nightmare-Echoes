using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using NightmareEchoes.TurnOrder;

//created by Alex
namespace NightmareEchoes.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Test Buttons")] // basic attack
        [SerializeField] Button testButton;

        [Header("Turn Order Bar")]
        [SerializeField] GameObject currentTurnIndicator;
        [SerializeField] List<Image> turnOrderImageList;
        [SerializeField] TextMeshProUGUI turnOrderText;
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Header("Hotbar Info")]
        BaseUnit currentUnit;
        [SerializeField] List<Button> currentUnitButton;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI currentUnitNameText;

        [Header("Inspectable Info")]
        BaseUnit inspectedUnit;
        [SerializeField] List<Button> inspectedUnitButton;
        [SerializeField] Button inspectedUnitProfile;
        [SerializeField] TextMeshProUGUI inspectedUnitNameText;

        [Header("Settings")]
        [SerializeField] Button settingButton;
        [SerializeField] GameObject settingsPanel;
        public bool gameIsPaused = false;

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
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            #region TurnOrderPanel

            if(Input.GetKeyDown(KeyCode.Space)) 
            {
                UpdateTurnOrderUI();
            }

            switch (TurnOrderManager.Instance.GameState)
            {
                case GameState.PlayerTurn:
                    EnablePlayerUI(true);
                    currentUnit = TurnOrderManager.Instance.CurrentUnit;

                    if(currentUnit != null)
                    {
                        //Debug.Log(currentUnit.Name);
                    }

                    turnOrderText.text = $"Player's Turn";
                    turnOrderText.color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);

                    break;

                case GameState.EnemyTurn:
                    EnablePlayerUI(false);


                    turnOrderText.text = $"Enemy's Turn";
                    turnOrderText.color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);

                    break;

                case GameState.CheckEffects:
                    UpdateTurnOrderUI();
                    break;
            }

            currentUnitNameText.text = $"{currentUnit.Name}";
            #endregion


            #region InspectedUnit
            if (Input.GetMouseButtonDown(0)) // rightclick on an inspectable unit
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

        void UpdateTurnOrderUI()
        {
            int i = 0;
            TurnOrderManager.Instance.CalculatedTurnOrder();

            foreach(BaseUnit unit in TurnOrderManager.Instance.TurnOrderList)
            {
                //turnOrderImageList[i] = unit.UnitScriptable.Image;
            }


            //sets indicator to the first image on the list
            currentTurnIndicator.transform.position = turnOrderImageList[0].transform.position;

            //stores the image to add and removes it from the first index of list
            Image imageToAdd = turnOrderImageList[0];
            turnOrderImageList.RemoveAt(0);

            //add the stored image to the end of the list and sets that image as the last in the rect transform list
            turnOrderImageList.Add(imageToAdd);
            imageToAdd.rectTransform.SetAsLastSibling();
        }

        #region Hotbar Functions
        public void PlayerAttackButton()
        {
            TurnOrderManager.Instance.GameState = GameState.EnemyTurn;
            currentUnit.BasicAttack();
        }

        

        #endregion


        #region UI Function

        void EnablePlayerUI(bool enable)
        {
            foreach (Button button in currentUnitButton)
            {
                button.interactable = enable;
            }
        }


        void EnableInspectedUI(bool enable)
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