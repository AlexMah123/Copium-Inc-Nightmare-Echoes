using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using NightmareEchoes.TurnOrder;

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
        [Space(15)]
        [SerializeField] List<Button> currentUnitButton;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        BaseUnit currentUnit { get => TurnOrderController.Instance.CurrentUnit; set { }}

        [Header("Inspectable Info")]
        [SerializeField] BaseUnit inspectedUnit;
        [Space(15)]
        [SerializeField] List<Button> inspectedUnitButton;
        [SerializeField] Button inspectedUnitProfile;
        [SerializeField] TextMeshProUGUI inspectedUnitNameText;

        [Space(15)]
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
            GameObject obj = Instantiate(ImagePrefab, turnOrderPanel.transform);
            obj.SetActive(false);
            obj.name = $"{obj.name} {capIndex}";
            imageObjectPool.Add(obj);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                ShuffleTurnOrder();
            }

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

            }

            if (currentUnit != null)
            {
                currentUnitNameText.text = $"{currentUnit.Name}";
            }
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



        #region Hotbar Functions
        public void AttackButton()
        {
            TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.enemyPhase);
            currentUnit.BasicAttack();
        }

        public void Skill1Button()
        {
            TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.enemyPhase);
            currentUnit.Skill1();
        }

        public void Skill2Button()
        {
            TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.enemyPhase);
            currentUnit.Skill2();
        }

        public void Skill3Button()
        {
            TurnOrderController.Instance.ChangePhase(TurnOrderController.Instance.enemyPhase);
            currentUnit.Skill3();
        }
        #endregion



        #region UI Function

        public void UpdateTurnOrderUI()
        {
            //resets values, clears list, calculate turn order
            for (int i = 0; i < imageObjectPool.Count; i++)
            {
                imageObjectPool[i].SetActive(false);
            }


            //sets all the images in the panel
            for (int i = 0; i < TurnOrderController.Instance.TurnOrderList.Count; i++)
            {
                GameObject image = GetImageObject();

                if(image != null)
                {
                    image.SetActive(true);

                    if (TurnOrderController.Instance.TurnOrderList[i].IsHostile)
                    {
                        image.GetComponent<Image>().color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b);

                    }
                    else
                    {
                        image.GetComponent<Image>().color = new Color(playerTurn.r, playerTurn.g, playerTurn.b);
                    }
                }
            }

            
        }

        public void ShuffleTurnOrder()
        {
            //stores the image to add and removes it from the first index of list
            GameObject firstImage = imageObjectPool[0];
            imageObjectPool.RemoveAt(0);


            //add the stored image to the end of the list and sets that image as the last in the rect transform list
            firstImage.GetComponent<RectTransform>().SetAsLastSibling();
            imageObjectPool.Add(firstImage);
        }

        GameObject GetImageObject()
        {
            if(imageObjectPool.Count < imagePoolCap)
            {
                bool found = false;
                for (int i = 0; i < imageObjectPool.Count; i++)
                {
                    if (!imageObjectPool[i].activeInHierarchy)
                    {
                        found = true;
                        return imageObjectPool[i];
                    }
                }

                if(!found)
                {
                    GameObject obj = Instantiate(ImagePrefab, turnOrderPanel.transform);
                    obj.SetActive(false);
                    capIndex++;
                    obj.name = $"{obj.name} {capIndex}";
                    imageObjectPool.Add(obj);
                }
            }
            

            return null;
        }

        public void EnablePlayerUI(bool enable)
        {
            foreach (Button button in currentUnitButton)
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
