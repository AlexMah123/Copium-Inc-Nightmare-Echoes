using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NightmareEchoes.Unit;
using NightmareEchoes.TurnOrder;
using System.Linq;

//created by Alex
namespace NightmareEchoes.TurnOrder
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;


        [Header("Turn Order Bar")]
        [SerializeField] int initImagePool = 6;
        int capIndex = 0;
        [SerializeField] GameObject turnOrderSpritePrefab;
        [SerializeField] GameObject turnIndicator;
        [SerializeField] GameObject currentTurnOrderPanel;
        [SerializeField] TextMeshProUGUI currentTurnNum;
        [SerializeField] TextMeshProUGUI phaseText;

        List<GameObject> turnOrderSpritePool = new List<GameObject>();
        [SerializeField] Color playerTurn;
        [SerializeField] Color enemyTurn;

        [Space(20), Header("Current Unit Info")]
        [SerializeField] List<Button> currentUnitButtonList;
        [SerializeField] Button currentUnitProfile;
        [SerializeField] TextMeshProUGUI BasicAttackText;
        [SerializeField] TextMeshProUGUI Skill1Text;
        [SerializeField] TextMeshProUGUI Skill2Text;
        [SerializeField] TextMeshProUGUI Skill3Text;
        [SerializeField] TextMeshProUGUI PassiveText;
        [SerializeField] TextMeshProUGUI currentUnitNameText;
        [SerializeField] TextMeshProUGUI currentUnitHealthText;
        [SerializeField] TextMeshProUGUI currentUnitMoveRangeText;
        [SerializeField] TextMeshProUGUI currentUnitSpeedText;
        [SerializeField] TextMeshProUGUI currentUnitStunResistText;
        [SerializeField] TextMeshProUGUI currentUnitResistText;
        [SerializeField] Slider currentUnitHealth;
        [SerializeField] GameObject currentUnitStatusEffectsPanel;
        public List<Modifier> currentUnitTotalStatusEffectList = new List<Modifier>();

        Units CurrentUnit { get => TurnOrderController.Instance.CurrentUnit;}

        [Space(20), Header("Inspectable Info")]
        public Units inspectedUnit;
        [SerializeField] List<Button> inspectedUnitButtonList;
        [SerializeField] GameObject inspectedUnitPanel;
        [SerializeField] Button inspectedUnitProfile;
        [SerializeField] TextMeshProUGUI inspectedUnitNameText;
        [SerializeField] TextMeshProUGUI inspectedUnitHealthText;
        [SerializeField] TextMeshProUGUI inspectedUnitMoveRangeText;
        [SerializeField] TextMeshProUGUI inspectedUnitSpeedText;
        [SerializeField] TextMeshProUGUI inspectedUnitStunResistText;
        [SerializeField] TextMeshProUGUI inspectedUnitResistText;
        [SerializeField] Slider inspectedUnitHealth;
        [SerializeField] GameObject inspectedUnitStatusEffectsPanel;
        public List<Modifier> inspectedUnitTotalStatusEffectList = new List<Modifier>();

        [Space(20), Header("Current/Inspected Status Effects")]
        [SerializeField] int statusEffectCap = 8;
        [SerializeField] GameObject statusEffectPrefab;
        List<GameObject> inspectedUnitStatusEffectPool = new List<GameObject>();
        List<GameObject> currentUnitStatusEffectPool = new List<GameObject>();


        [Space(20), Header("Character Glossary")]
        [SerializeField] GameObject glossaryPanel;
        [SerializeField] GameObject glossaryContainer;
        [SerializeField] Image glossaryImage;
        [SerializeField] TextMeshProUGUI glossaryName;
        [SerializeField] TextMeshProUGUI glossaryHealthText;
        [SerializeField] TextMeshProUGUI glossaryMoveRangeText;
        [SerializeField] TextMeshProUGUI glossarySpeedText;
        [SerializeField] TextMeshProUGUI glossaryStunResistText;
        [SerializeField] TextMeshProUGUI glossaryResistText;
        [SerializeField] Slider glossaryHealth;
        [Space(10)]
        [SerializeField] GameObject glossaryPrefab;
        [SerializeField] int initGlossaryPool = 5;
        List<GameObject> glossaryPrefabPool = new List<GameObject>();


        [Space(20), Header("Settings")]
        [SerializeField] Button settingButton;
        [SerializeField] GameObject settingsPanel;
        [NonSerialized] public bool gameIsPaused = false;


        [Space(20), Header("Current Unit Indicator")]
        //[SerializeField] TextMeshProUGUI unitAction;
        [SerializeField] GameObject unitIndicator;
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
            InitTurnOrderSpritePool(currentTurnOrderPanel);
            InitStatusEffectPool(currentUnitStatusEffectsPanel);
            InitStatusEffectPool(inspectedUnitStatusEffectsPanel);
            InitGlossaryPool(glossaryContainer);
        }

        private void Start()
        {
            inspectedUnitPanel.SetActive(false);
            glossaryPanel.SetActive(false);
        }

        private void Update()
        {

            #region Current Unit text
            if (CurrentUnit != null)
            {
                currentUnitProfile.image.sprite = CurrentUnit.Sprite;
                currentUnitProfile.image.color = new Color(CurrentUnit.SpriteRenderer.color.r,
                    CurrentUnit.SpriteRenderer.color.g, CurrentUnit.SpriteRenderer.color.b, CurrentUnit.SpriteRenderer.color.a);
                currentUnitNameText.text = $"{CurrentUnit.Name}";
                currentUnitHealthText.text = $"{CurrentUnit.stats.Health}/{CurrentUnit.stats.MaxHealth}";
                currentUnitSpeedText.text = $"Speed: {CurrentUnit.stats.Speed}";
                currentUnitMoveRangeText.text = $"Move Range: {CurrentUnit.stats.MoveRange}";
                currentUnitStunResistText.text = $"Stun Resist: {CurrentUnit.stats.StunResist}%";
                currentUnitResistText.text = $"Resist: {CurrentUnit.stats.Resist}%";

                if(!CurrentUnit.IsHostile)
                {
                    BasicAttackText.text = CurrentUnit.BasicAttackName;
                    Skill1Text.text = CurrentUnit.Skill1Name;
                    Skill2Text.text = CurrentUnit.Skill2Name;
                    Skill3Text.text = CurrentUnit.Skill3Name;
                    PassiveText.text = CurrentUnit.PassiveName;
                }
               
                currentUnitHealth.maxValue = CurrentUnit.stats.MaxHealth;
                currentUnitHealth.value = CurrentUnit.stats.Health;

            }
            #endregion

            #region Inspected Unit text
            if(inspectedUnit != null)
            {
                inspectedUnitProfile.image.sprite = inspectedUnit.Sprite;
                inspectedUnitProfile.image.color = new Color(inspectedUnit.SpriteRenderer.color.r,
                     inspectedUnit.SpriteRenderer.color.g, inspectedUnit.SpriteRenderer.color.b, inspectedUnit.SpriteRenderer.color.a);
                inspectedUnitNameText.text = $"{inspectedUnit.Name}";
                inspectedUnitHealthText.text = $"{inspectedUnit.stats.Health}/{inspectedUnit.stats.MaxHealth}";
                inspectedUnitSpeedText.text = $"Speed: {inspectedUnit.stats.Speed}";
                inspectedUnitMoveRangeText.text = $"Move Range: {inspectedUnit.stats.MoveRange}";
                inspectedUnitStunResistText.text = $"Stun Resist: {inspectedUnit.stats.StunResist}%";
                inspectedUnitResistText.text = $"Resist: {inspectedUnit.stats.Resist}%";

                inspectedUnitHealth.maxValue = inspectedUnit.stats.MaxHealth;
                inspectedUnitHealth.value = inspectedUnit.stats.Health;
            }

            if (Input.GetMouseButtonDown(1)) // rightclick on an inspectable unit
            {
                bool selected = false;
                int unitMask = LayerMask.GetMask("Unit");

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, unitMask);

                if (hit)
                {
                    if (hit.collider.gameObject.CompareTag("Inspectable"))
                    {
                        inspectedUnit = hit.collider.gameObject.GetComponent<Units>();
                        selected = true;
                        EnableInspectedUI(true);
                    }
                }

                //UNCOMMENT IF WE WANT TO HIDE WHEN PLAYERS DRAG
                /*if (!selected)
                {
                    EnableInspectedUI(false);
                }*/

            }
            #endregion

            #region current unit indicator 
            if (CurrentUnit != null)
            {
                if (!unitIndicator.activeSelf)
                {
                    unitIndicator.SetActive(true);
                }

                if (CurrentUnit.IsHostile)
                {
                    unitIndicator.GetComponent<SpriteRenderer>().color = new Color(enemyTurn.r, enemyTurn.g, enemyTurn.b, enemyTurn.a);
                }
                else
                {
                    unitIndicator.GetComponent<SpriteRenderer>().color = new Color(playerTurn.r, playerTurn.g, playerTurn.b, playerTurn.a);
                }

                unitIndicator.transform.position = new Vector3(CurrentUnit.transform.position.x, CurrentUnit.transform.position.y + offset, CurrentUnit.transform.position.z)
                    + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
            }
            else
            {
                unitIndicator.SetActive(false);
            }

            #endregion

            #region TurnOrderPanel

            currentTurnNum.text = $"{ TurnOrderController.Instance.turnCount}";


            //sets indicator to the first image on the list
            if (turnOrderSpritePool[0].activeSelf)
            {
                if(!turnIndicator.activeSelf)
                {
                    turnIndicator.SetActive(true);
                }

                turnIndicator.transform.position = turnOrderSpritePool[0].transform.position;
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
                phaseText.color = Color.white;
            }
            else if (TurnOrderController.Instance.currentPhase == TurnOrderController.Instance.planPhase)
            {
                phaseText.text = $"Plan Phase";
                phaseText.color = Color.white;
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
                phaseText.color = Color.white;
            }


            #endregion

        }


        #region Hotbar Functions
        public void AttackButton()
        {
            CurrentUnit.BasicAttack();
        }

        public void Skill1Button()
        {
            CurrentUnit.Skill1();
        }

        public void Skill2Button()
        {
            CurrentUnit.Skill2();
        }

        public void Skill3Button()
        {
            CurrentUnit.Skill3();
        }

        #endregion


        #region UI Pool
        void InitTurnOrderSpritePool(GameObject panel)
        {
            capIndex = 0;
            for (int i = 0; i < initImagePool; i++)
            {
                GameObject obj = Instantiate(turnOrderSpritePrefab, panel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";
                turnOrderSpritePool.Add(obj);
            }
        }

        void InitStatusEffectPool(GameObject panel)
        {
            capIndex = 0;
            for (int i = 0; i < statusEffectCap; i++)
            {
                GameObject obj = Instantiate(statusEffectPrefab, panel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";

                if(panel == currentUnitStatusEffectsPanel)
                {
                    currentUnitStatusEffectPool.Add(obj);
                }
                else if(panel == inspectedUnitStatusEffectsPanel)
                {
                    inspectedUnitStatusEffectPool.Add(obj);
                }
            }
        }

        void InitGlossaryPool(GameObject panel)
        {
            capIndex = 0;
            for (int i = 0; i < initGlossaryPool; i++)
            {
                GameObject obj = Instantiate(glossaryPrefab, panel.transform);
                obj.SetActive(false);
                obj.name = $"{obj.name} {capIndex++}";
                glossaryPrefabPool.Add(obj);
            }
        }

        GameObject GetImageObject()
        {
            for (int i = 0; i < turnOrderSpritePool.Count; i++)
            {
                if (!turnOrderSpritePool[i].activeInHierarchy)
                {
                    return turnOrderSpritePool[i];
                }
            }

            GameObject obj = Instantiate(turnOrderSpritePrefab, currentTurnOrderPanel.transform);
            obj.SetActive(false);
            obj.name = $"{obj.name} {capIndex++}";
            turnOrderSpritePool.Add(obj);
            return obj;
        }

        GameObject GetStatusEffectObject(GameObject panel)
        {
            if(panel == currentUnitStatusEffectsPanel)
            {
                for (int i = 0; i < currentUnitStatusEffectPool.Count; i++)
                {
                    if (!currentUnitStatusEffectPool[i].activeInHierarchy)
                    {
                        return currentUnitStatusEffectPool[i];
                    }
                }
            }
            else if(panel == inspectedUnitStatusEffectsPanel)
            {
                for (int i = 0; i < inspectedUnitStatusEffectPool.Count; i++)
                {
                    if (!inspectedUnitStatusEffectPool[i].activeInHierarchy)
                    {
                        return inspectedUnitStatusEffectPool[i];
                    }
                }
            }

            return null;
        }

        GameObject GetGlossaryPrefab()
        {
            for (int i = 0; i < glossaryPrefabPool.Count; i++)
            {
                if (!glossaryPrefabPool[i].activeInHierarchy)
                {
                    return glossaryPrefabPool[i];
                }
            }

            GameObject obj = Instantiate(glossaryPrefab, glossaryContainer.transform);
            obj.SetActive(false);
            obj.name = $"{obj.name} {capIndex++}";
            glossaryPrefabPool.Add(obj);
            return obj;
        }
        #endregion


        #region Update UI Functions
        public void UpdateTurnOrderUI()
        {
            //resets the turn order bar, calculate turn order
            for (int i = 0; i < turnOrderSpritePool.Count; i++)
            {
                turnOrderSpritePool[i].SetActive(false);
            }


            //sets all the images in the panel for current turn
            for (int i = 0; i < TurnOrderController.Instance.CurrentUnitQueue.Count; i++)
            {
                GameObject image = GetImageObject();

                if(image != null)
                {
                    image.GetComponent<Image>().sprite = TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].Sprite;
                    image.GetComponent<Image>().color =
                        new Color(TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.r,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.g,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.b,
                        TurnOrderController.Instance.CurrentUnitQueue.ToArray()[i].SpriteRenderer.color.a);

                    image.SetActive(true);
                }
            }
        }

        public void UpdateStatusEffectUI()
        {
            if(CurrentUnit != null)
            {
                //total list of current unit status effect
                currentUnitTotalStatusEffectList = CurrentUnit.BuffList.Concat(CurrentUnit.DebuffList).Concat(CurrentUnit.TokenList).ToList();

                //reset pool
                for (int i = 0; i < currentUnitStatusEffectPool.Count; i++)
                {
                    currentUnitStatusEffectPool[i].SetActive(false);
                }

                //foreach in total list
                for (int i = 0; i < currentUnitTotalStatusEffectList.Count; i++)
                {
                    //get obj from pool
                    GameObject statusEffectObj = GetStatusEffectObject(currentUnitStatusEffectsPanel);

                    if (statusEffectObj != null)
                    {
                        //sets image component and scriptable object component
                        Image statusEffectImage = statusEffectObj.GetComponent<Image>();
                        Modifier modifier = currentUnitTotalStatusEffectList[i];

                        //sets image to icon
                        //statusEffectImage.sprite = modifier.icon;

                        //sets image color to respective color
                        switch (modifier.modifierType)
                        {
                            case ModifierType.BUFF:
                                statusEffectImage.color = Color.green;
                                break;

                            case ModifierType.DEBUFF:
                                statusEffectImage.color = Color.red;

                                break;

                            case ModifierType.POSITIVETOKEN:
                                statusEffectImage.color = new Color(0, 0.7f, 1, 1);
                                break;

                            case ModifierType.NEGATIVETOKEN:
                                statusEffectImage.color = Color.yellow;
                                break;
                        }


                        statusEffectObj.SetActive(true);
                    }
                }
            }
           
            //inspected unit
            if (inspectedUnit != null)
            {
                //total list of status effect
                //inspectedUnitStatusEffectList.Clear();
                inspectedUnitTotalStatusEffectList = inspectedUnit.BuffList.Concat(inspectedUnit.DebuffList).Concat(inspectedUnit.TokenList).ToList();

                //reset pool
                for (int i = 0; i < inspectedUnitStatusEffectPool.Count; i++)
                {
                    inspectedUnitStatusEffectPool[i].SetActive(false);
                }

                //foreach in total list
                for (int i = 0; i < inspectedUnitTotalStatusEffectList.Count; i++)
                {
                    //get obj from pool
                    GameObject statusEffectObj = GetStatusEffectObject(inspectedUnitStatusEffectsPanel);

                    if (statusEffectObj != null)
                    {
                        //sets image component and scriptable object component
                        Image statusEffectImage = statusEffectObj.GetComponent<Image>();
                        Modifier modifier = inspectedUnitTotalStatusEffectList[i];

                        //sets image to icon
                        //statusEffectImage.sprite = modifier.icon;

                        //sets image color to respective color
                        switch (modifier.modifierType)
                        {
                            case ModifierType.BUFF:
                                statusEffectImage.color = Color.green;
                                break;

                            case ModifierType.DEBUFF:
                                statusEffectImage.color = Color.red;

                                break;

                            case ModifierType.POSITIVETOKEN:
                                statusEffectImage.color = new Color(0, 0.7f, 1, 1);
                                break;

                            case ModifierType.NEGATIVETOKEN:
                                statusEffectImage.color = Color.yellow;
                                break;
                        }


                        statusEffectObj.SetActive(true);
                    }
                }
            }
}

        public void UpdateGlossaryUI(string text)
        {
            //resets glossary UI
            for (int i = 0; i < glossaryPrefabPool.Count; i++)
            {
                glossaryPrefabPool[i].SetActive(false);
            }

            if (text == "Current")
            {
                glossaryImage.sprite = CurrentUnit.Sprite;
                glossaryImage.color = new Color(
                    CurrentUnit.SpriteRenderer.color.r, CurrentUnit.SpriteRenderer.color.g,
                    CurrentUnit.SpriteRenderer.color.b, CurrentUnit.SpriteRenderer.color.a);
                glossaryName.text = CurrentUnit.Name;

                glossaryMoveRangeText.text = $"Move Range: {CurrentUnit.baseStats.MoveRange} + (<color=yellow>{CurrentUnit.modifiedStats.moveRangeModifier}</color>)";
                glossarySpeedText.text = $"Speed: {CurrentUnit.baseStats.Speed} + (<color=yellow>{CurrentUnit.modifiedStats.speedModifier}</color>)";
                glossaryStunResistText.text = $"Stun Resist: {CurrentUnit.baseStats.StunResist}% + (<color=yellow>{CurrentUnit.modifiedStats.stunResistModifier}%</color>)";
                glossaryResistText.text = $"Resist: {CurrentUnit.baseStats.Resist}% + (<color=yellow>{CurrentUnit.modifiedStats.resistModifier}%</color>)";

                glossaryHealthText.text = $"{CurrentUnit.stats.Health}/{CurrentUnit.stats.MaxHealth}";
                glossaryHealth.maxValue = CurrentUnit.stats.MaxHealth;
                glossaryHealth.value = CurrentUnit.stats.Health;

                for (int i = 0; i < currentUnitTotalStatusEffectList.Count; i++)
                {
                    GameObject glossaryObj = GetGlossaryPrefab();

                    if (glossaryObj != null)
                    {
                        glossaryObj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = 
                            currentUnitTotalStatusEffectList[i].icon;

                        TextMeshProUGUI glossaryObjText = glossaryObj.GetComponentInChildren<TextMeshProUGUI>();


                        if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.BUFF)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=green>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.DEBUFF)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=red>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.POSITIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=#00BAFF>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (currentUnitTotalStatusEffectList[i].modifierType == ModifierType.NEGATIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{currentUnitTotalStatusEffectList[i].description} " +
                            $"(<color=yellow>{currentUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {currentUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }


                        glossaryObj.SetActive(true);
                    }
                }
            }
            else if (text == "Inspected")
            {
                glossaryImage.sprite = inspectedUnit.Sprite;
                glossaryImage.color = new Color(
                    inspectedUnit.SpriteRenderer.color.r, inspectedUnit.SpriteRenderer.color.g,
                    inspectedUnit.SpriteRenderer.color.b, inspectedUnit.SpriteRenderer.color.a);
                glossaryName.text = inspectedUnit.Name;

                glossaryMoveRangeText.text = $"Move Range: {inspectedUnit.baseStats.MoveRange} + (<color=yellow>{inspectedUnit.modifiedStats.moveRangeModifier}</color>)";
                glossarySpeedText.text = $"Speed: {inspectedUnit.baseStats.Speed} + (<color=yellow>{inspectedUnit.modifiedStats.speedModifier}</color>)";
                glossaryStunResistText.text = $"Stun Resist: {inspectedUnit.baseStats.StunResist}% + (<color=yellow>{inspectedUnit.modifiedStats.stunResistModifier}%</color>)";
                glossaryResistText.text = $"Resist: {inspectedUnit.baseStats.Resist}% + (<color=yellow>{inspectedUnit.modifiedStats.resistModifier}%</color>)";

                glossaryHealthText.text = $"{inspectedUnit.stats.Health}/{inspectedUnit.stats.MaxHealth}";
                glossaryHealth.maxValue = inspectedUnit.stats.MaxHealth;
                glossaryHealth.value = inspectedUnit.stats.Health;

                for (int i = 0; i < inspectedUnitTotalStatusEffectList.Count; i++)
                {
                    GameObject glossaryObj = GetGlossaryPrefab();

                    if (glossaryObj != null)
                    {
                        glossaryObj.transform.GetChild(0).GetComponentInChildren<Image>().sprite =
                            inspectedUnitTotalStatusEffectList[i].icon;

                        TextMeshProUGUI glossaryObjText = glossaryObj.GetComponentInChildren<TextMeshProUGUI>();

                        if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.BUFF)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=green>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.DEBUFF)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=red>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";

                        }
                        else if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.POSITIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=#00BAFF>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }
                        else if (inspectedUnitTotalStatusEffectList[i].modifierType == ModifierType.NEGATIVETOKEN)
                        {
                            glossaryObjText.text =
                            $"{inspectedUnitTotalStatusEffectList[i].description} " +
                            $"(<color=yellow>{inspectedUnitTotalStatusEffectList[i].genericValue}</color>)" +
                            $" (<color=#A5FFF1>Duration: {inspectedUnitTotalStatusEffectList[i].ReturnLifeTime()}</color>)";
                        }

                        glossaryObj.SetActive(true);
                    }
                }
            }

        }
        #endregion


        #region Hotbar UI
        public void EnableCurrentUI(bool enable)
        {
            //first button is profile
            for(int i = 1; i < currentUnitButtonList.Count; i++) 
            {
                currentUnitButtonList[i].interactable = enable;

                if (CurrentUnit.IsHostile)
                {
                    currentUnitButtonList[i].gameObject.SetActive(false);
                }
                else
                {
                    currentUnitButtonList[i].gameObject.SetActive(true);
                }
            }

            UpdateStatusEffectUI();
        }

        public void EnableInspectedUI(bool enable)
        {
            foreach (Button button in inspectedUnitButtonList)
            {
                button.interactable = enable;
            }

            inspectedUnitPanel.SetActive(enable);
            UpdateStatusEffectUI();
        }

        public void CharacterGlossary(string text)
        {
            SettingsButton();

            if (!glossaryPanel.activeSelf)
            {
                settingButton.gameObject.SetActive(false);
                glossaryPanel.SetActive(true);
                UpdateGlossaryUI(text);
            }
            else
            {
                settingButton.gameObject.SetActive(true);
                glossaryPanel.SetActive(false);
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
