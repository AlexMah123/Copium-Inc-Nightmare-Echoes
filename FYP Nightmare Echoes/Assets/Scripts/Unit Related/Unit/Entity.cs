using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NightmareEchoes.Grid;
using System.Linq;
using NightmareEchoes.Unit.Pathfinding;
using NightmareEchoes.Unit.AI;
using NightmareEchoes.Unit.Combat;
using UnityEngine.Pool;

//created by Alex, edited by Ter
namespace NightmareEchoes.Unit
{
    [RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D)), Serializable]
    public class Entity : MonoBehaviour
    {
        public event Action<Entity> OnDestroyedEvent;
        public event Action OnAddBuffEvent;

        [Header("Unit Info")]
        [SerializeField] protected string _name;
        [SerializeField] protected Sprite sprite;
        protected SpriteRenderer spriteRenderer;
        [SerializeField] protected bool isProp;
        [SerializeField] protected bool isHostile;
        [SerializeField] protected Direction direction;
        [SerializeField] protected TypeOfUnit typeOfUnit;

        [Space(15), Header("Units Popup text")]
        [SerializeField] protected GameObject popupTextPrefab;
        
        [Space(15), Header("Stats For Units")]
        public BaseStats baseStats = new BaseStats();
        public BaseStats stats = new BaseStats();
        public ModifiersStruct modifiedStats = new();

        [Space(15), Header("Buff Debuff")]
        [SerializeField] protected List<Modifier> buffDebuffList = new List<Modifier>();
        [SerializeField] protected List<Modifier> tokenList = new List<Modifier>();

        [Space(15), Header("Positive Token Bools")]
        [SerializeField] protected bool dodgeToken;
        [SerializeField] protected bool blockToken;
        [SerializeField] protected bool strengthToken;
        [SerializeField] protected bool hasteToken;
        [SerializeField] protected bool barrierToken;
        [SerializeField] protected bool stealthToken;

        [Space(15), Header("Negative Token Bools")]
        [SerializeField] protected bool blindToken;
        [SerializeField] protected bool vulnerableToken;
        [SerializeField] protected bool weakenToken;
        [SerializeField] protected bool vertigoToken;
        [SerializeField] protected bool stunToken;
        [SerializeField] protected bool immobilizeToken;


        [Space(15), Header("Unit Skills")]
        [SerializeField] protected Skill basicAttack;
        [SerializeField] protected Skill skill1;
        [SerializeField] protected Skill skill2;
        [SerializeField] protected Skill skill3;
        [SerializeField] protected Skill passive;

        [Space(15), Header("Sprite Directions"), Tooltip("Sprites are ordered in north, south, east, west")]
        [SerializeField] protected List<Sprite> sprites = new List<Sprite>(); //ordered in NSEW
        [SerializeField] protected GameObject frontModel;
        [SerializeField] protected GameObject backModel;
        [SerializeField] protected Animator frontAnimator;
        [SerializeField] protected Animator backAnimator;


        [Space(15), Header("Tile Related")]
        [SerializeField] protected OverlayTile activeTile;
        [SerializeField] protected PolygonCollider2D tileSize;

        [Header("Popup Text Related")]
        private Queue<PopupTextData> popupTextQueue = new Queue<PopupTextData>();
        private bool isDisplayingPopupText = false;
        [SerializeField] float popupTextDelay = 0.6f;

        #region Class Properties

        #region Unit Info Properties
        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public SpriteRenderer SpriteRenderer
        {
            get => spriteRenderer;
            set => spriteRenderer = value;
        }

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public bool IsProp
        {
            get => isProp;
            set => isProp = value;
        }

        public bool IsHostile
        {
            get => isHostile;
            private set => IsHostile = value;
        }

        public Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public TypeOfUnit TypeOfUnit
        {
            get => typeOfUnit;
            set => typeOfUnit = value;
        }

        #endregion


        #region Positive Token Bool
        public bool DodgeToken
        {
            get => dodgeToken;
            set => dodgeToken = value;
        }

        public bool BlockToken
        {
            get => blockToken; 
            set => blockToken = value;
        }

        public bool StrengthToken
        {
            get => strengthToken; 
            set => strengthToken = value;
        }

        public bool HasteToken
        {
            get => hasteToken; 
            set => hasteToken = value;
        }

        public bool BarrierToken
        {
            get => barrierToken; 
            set => barrierToken = value;
        }

        public bool StealthToken
        {
            get => stealthToken;
            set
            {
                stealthToken = value;

                if(sprites.Count > 0) 
                {
                    if (value == true)
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.75f);
                    }
                    else
                    {
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
                    }
                }
                else
                {
                    if (value == true)
                    {
                        var modelSprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                        foreach (var spriteRenderer in modelSprite)
                        {
                            if (spriteRenderer.color.a != 0.5f)
                            {
                                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
                            }
                        }
                    }
                    else
                    {
                        var modelSprite = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
                        foreach (var spriteRenderer in modelSprite)
                        {
                            if (spriteRenderer.color.a != 1.0f)
                            {
                                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                            }
                        }
                    }
                }
            }
        }
        #endregion 


        #region Negative Token Bool
        public bool BlindToken
        {
            get => blindToken;
            set => blindToken = value;
        }

        public bool VulnerableToken
        {
            get => vulnerableToken; 
            set => vulnerableToken = value;
        }

        public bool WeakenToken
        {
            get => weakenToken; 
            set => weakenToken = value;
        }

        public bool VertigoToken
        {
            get => vertigoToken; 
            set => vertigoToken = value;
        }

        public bool StunToken
        {
            get => stunToken;
            set => stunToken = value;
        }

        public bool ImmobilizeToken
        {
            get => immobilizeToken;
            set => immobilizeToken = value;
        }
        #endregion


        #region Buff Debuff Token List
        public List<Modifier> BuffDebuffList
        {
            get => buffDebuffList;
            set => buffDebuffList = value;
        }

        public List<Modifier> TokenList
        {
            get => tokenList;
            set => tokenList = value;
        }
        #endregion


        #region Unit Skill Properties

        public string BasicAttackName
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack.SkillName;
                }
            }

            private set => basicAttack.SkillName = value;
        }

        public string BasicAttackDesc
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack.SkillDescription;
                }
            }

            private set => basicAttack.SkillDescription = value;
        }

        public Skill BasicAttackSkill
        {
            get
            {
                if (basicAttack == null)
                {
                    return null;
                }
                else
                {
                    return basicAttack;
                }
            }

            private set => basicAttack = value;
        }

        public string Skill1Name
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1.SkillName;
                }
            }

            private set => skill1.SkillName = value;
        }

        public string Skill1Desc
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1.SkillDescription;
                }
            }

            private set => skill1.SkillDescription = value;
        }
        
        public Skill Skill1Skill
        {
            get
            {
                if (skill1 == null)
                {
                    return null;
                }
                else
                {
                    return skill1;
                }
            }

            private set => skill1 = value;
        }

        public string Skill2Name
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2.SkillName;
                }
            }

            private set => skill2.SkillName = value;
        }

        public string Skill2Desc
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2.SkillDescription;
                }
            }
            private set => skill2.SkillDescription = value;
        }

        public Skill Skill2Skill
        {
            get
            {
                if (skill2 == null)
                {
                    return null;
                }
                else
                {
                    return skill2;
                }
            }

            private set => skill2 = value;
        }

        public string Skill3Name
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3.SkillName;
                }
            }

            private set => skill3.SkillName = value;
        }

        public string Skill3Desc
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3.SkillDescription;
                }
            }
            private set => skill3.SkillDescription = value;
        }

        public Skill Skill3Skill
        {
            get
            {
                if (skill3 == null)
                {
                    return null;
                }
                else
                {
                    return skill3;
                }
            }

            private set => skill3 = value;
        }
        public string PassiveName
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive.SkillName;
                }
            }

            private set => passive.SkillName = value;
        }

        public string PassiveDesc
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive.SkillDescription;
                }
            }

            private set => passive.SkillDescription = value;
        }

        public Skill PassiveSkill
        {
            get
            {
                if (passive == null)
                {
                    return null;
                }
                else
                {
                    return passive;
                }
            }

            private set => passive = value;
        }
        #endregion


        #region Tile Related
        public OverlayTile ActiveTile
        {
            get => activeTile;
            set => activeTile = value;
        }
        #endregion


        #region Sprites Directions
        public GameObject FrontModel
        {
            get => frontModel;
            private set => frontModel = value;
        }

        public GameObject BackModel
        {
            get => backModel;
            private set => backModel = value;
        }

        public Animator FrontAnimator
        {
            get => frontAnimator;
            private set => frontAnimator = value;
        }

        public Animator BackAnimator
        {
            get => backAnimator;
            private set => backAnimator = value;
        }

        #endregion

        #endregion

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            //collider presets
            PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();
            polyCollider.points = tileSize.points;
            polyCollider.isTrigger = true;

            //rb2d presets
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.freezeRotation = true;
            rb2D.gravityScale = 0f;

            //MANDATORY, DO NOT REMOVE
            baseStats.Reset();
            AwakeAllStatusEffects();
            UpdateStatsWithoutEndCycleEffect();
        }

        protected virtual void Start()
        {
            stats.Health = stats.MaxHealth;
        }

        protected virtual void Update()
        {
            if (stats.Health <= 0)
            {
                if (!IsHostile)
                {
                    PathfindingManager.Instance.HideTilesInRange(PathfindingManager.Instance.playerTilesInRange);
                }
                else if (!isProp)
                {
                    PathfindingManager.Instance.HideTilesInRange(GetComponent<BasicEnemyAI>().TilesInRange);
                }

                OnDestroyedEvent?.Invoke(this);
                Destroy(gameObject);
            }

            #region Sprite/Animation Updates
            if (isProp) return;
            if (sprites.Count > 0)
            {
                switch (Direction)
                {
                    case Direction.NORTH: //back facing

                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.NORTH];
                        }
                        break;

                    case Direction.SOUTH: //front facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.SOUTH];
                        }
                        break;

                    case Direction.EAST: //front facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.EAST];
                        }
                        break;

                    case Direction.WEST: //back facing
                        if (SpriteRenderer != null)
                        {
                            SpriteRenderer.sprite = sprites[(int)Direction.WEST];
                        }
                        break;
                }
            }
            else
            {
                switch (Direction)
                {
                    case Direction.NORTH: //back facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(false);
                            backModel.SetActive(true);
                        }

                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Direction.SOUTH: //front facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(true);
                            backModel.SetActive(false);
                        }

                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Direction.EAST: //front facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(true);
                            backModel.SetActive(false);
                        }

                        transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;

                    case Direction.WEST: //back facing
                        if (frontModel != null && backModel != null)
                        {
                            frontModel.SetActive(false);
                            backModel.SetActive(true);
                        }

                        transform.localRotation = Quaternion.Euler(0, 180, 0);
                        break;

                }
            }
            #endregion
        }


        #region Override Functions
        public virtual void Move()
        {
            
        }

        public virtual void BasicAttack()
        {

        }


        public virtual void Passive()
        {

        }


        public virtual void Skill1()
        {

        }


        public virtual void Skill2()
        {

        }


        public virtual void Skill3()
        {

        }


        public virtual void TakeDamage(int damage, bool checkDodge = true)
        {
            if (frontModel != null && frontModel.activeSelf && frontAnimator != null)
            {
                frontAnimator.SetBool("GettingHit", true);
            }
            else if (backModel != null && backModel.activeSelf && backAnimator != null)
            {
                backAnimator.SetBool("GettingHit", true);
            }


            #region Token Checks Before Taking Dmg
            if (dodgeToken && checkDodge)
            {
                if (FindModifier(STATUS_EFFECT.DODGE_TOKEN).genericValue > UnityEngine.Random.Range(0, 101))
                {
                    ShowPopUpText($"Dodged!", Color.red);
                }
                else if (barrierToken)
                {
                    ShowPopUpText($"Failed to dodge!", Color.red);
                    ShowPopUpText($"Damage was negated!", Color.red);
                    UpdateTokenLifeTime(STATUS_EFFECT.BARRIER_TOKEN);
                }
                else
                {
                    ShowPopUpText($"Failed to dodge!", Color.red);

                    if (blockToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 0.5f);
                        stats.Health -= newDamage;

                        ShowPopUpText($"Damage was reduced!", Color.red);
                        ShowPopUpText($"-{newDamage}", Color.yellow);
                        UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
                    }
                    else if (vulnerableToken)
                    {
                        int newDamage = Mathf.RoundToInt(damage * 1.5f);
                        stats.Health -= newDamage;

                        ShowPopUpText($"Damage was increased!", Color.red);
                        ShowPopUpText($"-{newDamage}", Color.yellow);
                        UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
                    }
                    else
                    {
                        stats.Health -= damage;

                        ShowPopUpText($"-{damage}", Color.yellow);
                    }
                }

                UpdateTokenLifeTime(STATUS_EFFECT.DODGE_TOKEN);
            }
            else if(barrierToken)
            {
                ShowPopUpText($"Damage was negated!", Color.red);
                UpdateTokenLifeTime(STATUS_EFFECT.BARRIER_TOKEN);
            }
            else if(blockToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 0.5f);
                stats.Health -= newDamage;

                ShowPopUpText($"Damage was reduced!", Color.red);
                ShowPopUpText($"-{newDamage}", Color.yellow);
                UpdateTokenLifeTime(STATUS_EFFECT.BLOCK_TOKEN);
            }
            else if(vulnerableToken)
            {
                int newDamage = Mathf.RoundToInt(damage * 1.5f);
                stats.Health -= newDamage;

                ShowPopUpText($"Damage was increased!", Color.red);
                ShowPopUpText($"-{newDamage}", Color.yellow);
                UpdateTokenLifeTime(STATUS_EFFECT.VULNERABLE_TOKEN);
            }
            else
            {
                stats.Health -= damage;

                ShowPopUpText($"-{damage}", Color.yellow);
            }

            #endregion            
        }


        [ContextMenu("Destroy Object")]
        public void DestroyObject()
        {
            stats.Health = 0;
        }

        #endregion


        #region Utility
        public void ShowPopUpText(string text, Color color)
        {
            if (popupTextPrefab)
            {
                popupTextQueue.Enqueue(new PopupTextData(text, color));

                if (!isDisplayingPopupText)
                {
                    StartCoroutine(DisplayNextPopupText());
                }
            }
        }

        IEnumerator DisplayNextPopupText()
        {
            isDisplayingPopupText = true;

            while(popupTextQueue.Count > 0)
            {
                var tempData = popupTextQueue.Dequeue();

                GameObject prefab = Instantiate(popupTextPrefab, transform.position + Vector3.up + (Vector3.left * 0.25f), Quaternion.identity);
                TextMeshPro textMeshPro = prefab.GetComponentInChildren<TextMeshPro>();
                textMeshPro.text = tempData.popupTextData;
                textMeshPro.color = tempData.textColor;

                yield return new WaitForSeconds(popupTextDelay);

            }

            isDisplayingPopupText = false;
            
        }

        public void UpdateLocation()
        {
            var hitTile = Physics2D.Raycast(transform.position, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Overlay Tile"));
            if (hitTile)
            {
                activeTile = hitTile.collider.gameObject.GetComponent<OverlayTile>();
                PathfindingManager.Instance.SetUnitPositionOnTile(activeTile, this);
            }
        }
        #endregion


        #region Status Effects Updates
        public void UpdateStatusEffectEvent()
        {
            OnAddBuffEvent?.Invoke();
        }

        public Modifier FindModifier(STATUS_EFFECT enumIndex)
        {
            List<Modifier> tempList = BuffDebuffList.Concat(TokenList).ToList();

            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                if (tempList[i].statusEffect == enumIndex)
                {
                    return tempList[i];
                }
            }

            return null;
        }

        public void ClearAllStatusEffect(List<Modifier> statusEffectList, ModifierType modifierEnum)
        {
            switch(modifierEnum)
            {
                case ModifierType.BUFF:
                case ModifierType.DEBUFF:
                    buffDebuffList.Clear();
                    break;

                case ModifierType.POSITIVETOKEN:
                    foreach(var statusEffect in statusEffectList)
                    {
                        if(statusEffect.modifierType == ModifierType.POSITIVETOKEN)
                        {
                            while (statusEffect.ReturnLifeTime() > 0)
                            {
                                UpdateTokenLifeTime(statusEffect.statusEffect);
                            }
                        }
                        
                    }
                    break;

                case ModifierType.NEGATIVETOKEN:
                    foreach (var statusEffect in statusEffectList)
                    {
                        if (statusEffect.modifierType == ModifierType.NEGATIVETOKEN)
                        {
                            while (statusEffect.ReturnLifeTime() > 0)
                            {
                                UpdateTokenLifeTime(statusEffect.statusEffect);
                            }
                        }
                        
                    }
                    break;
            }

            UpdateStatusEffectEvent();
            UpdateStatsWithoutEndCycleEffect();
        }


        //call to add buff to unit
        public void AddBuff(Modifier buff)
        {
            switch (buff.modifierType)
            {
                //if buff or debuff
                case ModifierType.BUFF:
                case ModifierType.DEBUFF:
                    buff.AwakeStatusEffect();
                    BuffDebuffList.Add(buff);
                    break;

                //if positive token or negative token
                case ModifierType.POSITIVETOKEN:
                case ModifierType.NEGATIVETOKEN:
                    buff.AwakeStatusEffect();
                    TokenList.Add(buff);
                    buff.ApplyEffect(this);
                    break;
            }


            UpdateStatusEffectEvent();
            UpdateStatsWithoutEndCycleEffect();
        }

        //call only on instantiation of object
        public void AwakeAllStatusEffects()
        {
            List<Modifier> totalStatusEffects = BuffDebuffList.Concat(TokenList).ToList();

            foreach (Modifier statusEffect in totalStatusEffects)
            {
                statusEffect.AwakeStatusEffect();
            }
        }

        public void ApplyAllBuffDebuffs()
        {
            for (int i = 0; i < BuffDebuffList.Count; i++)
            {
                BuffDebuffList[i].ApplyEffect(this);
            }
        }

        public void ApplyAllTokenEffects()
        {
            for (int i = 0; i < TokenList.Count; i++)
            {
                TokenList[i].ApplyEffect(this);
            }
        }

        public void UpdateBuffDebuffLifeTime()
        {
            for (int i = BuffDebuffList.Count - 1; i >= 0 ; i--)
            {
                BuffDebuffList[i].UpdateLifeTime();

                if (BuffDebuffList[i].ReturnLifeTime() <= 0)
                {
                    BuffDebuffList.RemoveAt(i);
                }
            }
        }

        //only call in class properties
        public void UpdateTokenLifeTime(STATUS_EFFECT enumIndex)
        {
            for (int i = TokenList.Count - 1; i >= 0; i--)
            {
                if (TokenList[i].statusEffect == enumIndex)
                {
                    TokenList[i].UpdateLifeTime(this);

                    if (TokenList[i].ReturnLifeTime() <= 0)
                    {
                        TokenList.RemoveAt(i);
                    }
                }
            }
            
        }

        public void UpdateStatsWithoutEndCycleEffect()
        {
            modifiedStats = ApplyModifiersWithoutEndCycleEffect(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        public void UpdateStatsWithEndCycleEffect()
        {
            modifiedStats = ApplyModifiersWithEndCycleEffect(modifiedStats);

            stats.MaxHealth = baseStats.MaxHealth + modifiedStats.healthModifier;
            stats.Health = stats.Health;
            stats.Speed = baseStats.Speed + modifiedStats.speedModifier;
            stats.MoveRange = baseStats.MoveRange + modifiedStats.moveRangeModifier;
            stats.StunResist = baseStats.StunResist + modifiedStats.stunResistModifier;
            stats.Resist = baseStats.Resist + modifiedStats.resistModifier;
        }

        //used in UpdateStats, do not directly call
        protected ModifiersStruct ApplyModifiersWithoutEndCycleEffect(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for (int i = 0; i < buffDebuffList.Count; i++)
            {
                temp = buffDebuffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                if (tokenList[i].statusEffect == STATUS_EFFECT.HASTE_TOKEN || tokenList[i].statusEffect == STATUS_EFFECT.VERTIGO_TOKEN)
                {
                    continue;
                }

                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        protected ModifiersStruct ApplyModifiersWithEndCycleEffect(ModifiersStruct modifiers)
        {
            ModifiersStruct temp = new();

            for (int i = 0; i < BuffDebuffList.Count; i++)
            {
                temp = BuffDebuffList[i].ApplyModifier(temp);
            }

            for (int i = 0; i < tokenList.Count; i++)
            {
                temp = tokenList[i].ApplyModifier(temp);
            }

            modifiers = temp;

            return modifiers;
        }

        public bool CheckImmobilize()
        {
            foreach (var mod in tokenList.Where(mod => mod.statusEffect == STATUS_EFFECT.IMMOBILIZE_TOKEN))
            {
                mod.TriggerEffect(this);
                return true;
            }

            return false;
        }

        public bool CheckCrippled()
        {
            foreach (var mod in buffDebuffList.Where(mod => mod.statusEffect == STATUS_EFFECT.CRIPPLED_DEBUFF))
            {
                mod.TriggerEffect(this);
                return true;
            }
            return false;
        }

        #endregion
    }

    public enum Direction
    {
        NONE = -1,
        NORTH = 0,
        SOUTH = 1,
        EAST = 2,
        WEST = 3,
    }


    public enum TypeOfUnit
    {
        RANGED_UNIT = 0,
        MELEE_UNIT = 1,
    }

    public class PopupTextData
    {
        public string popupTextData;
        public Color textColor;

        public PopupTextData(string textData, Color color)
        {
            popupTextData = textData;
            textColor = color;
        }
    }

    [Serializable]
    public class BaseStats
    {
        [Header("Unit Info")]
        [SerializeField] protected int _health;
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _speed;
        [SerializeField] protected int _moveRange;
        [SerializeField] protected float _stunResist;
        [SerializeField] protected float _resist;

        #region Unit Info Properties
        public int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, _maxHealth);
            }
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public int MoveRange
        {
            get => _moveRange;
            set => _moveRange = value;
        }

        public float StunResist
        {
            get => _stunResist;
            set => _stunResist = value;
        }

        public float Resist
        {
            get => _resist;
            set => _resist = value;
        }
        #endregion


        public void Reset()
        {
            MaxHealth = _maxHealth;
            Health = _health;
            Speed = _speed;
            MoveRange = _moveRange;
            StunResist = _stunResist;
            Resist = _resist;
        }

    }
}
