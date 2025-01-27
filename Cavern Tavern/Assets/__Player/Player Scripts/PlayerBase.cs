using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Stats")]
    public float maxHealth = 100;
    public float health;
    public float gold;
    public InventoryHolder playerInventory;

    [Header("Managers")]
    public AudioManager audioManager;

    [Header("Combat")]
    public float knockbackDistance;
    public float knockbackDuration;
    [HideInInspector] public bool isHurt = false;
    private bool isDead = false;

    
    private HealthBarUI healthBarUI;

    public static PlayerBase instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthBarUI = GetComponent<HealthBarUI>();
        health = maxHealth;
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    #region hurt/knockback
    public void hurt(float damage, float knockbackPower, Vector2 attackingColliderToPlayerVector)
    {
        isHurt = true;
        health -= damage;
        healthBarUI.lerpTimer = 0f;

        if (health > 0)
        {
            //animator.SetTrigger("hurt");
            knockback(knockbackPower, attackingColliderToPlayerVector);
        }
        else
        {
            Debug.Log("Player Dead");
            isDead = true;
            //animator.SetTrigger("dead");
            knockback(5f, attackingColliderToPlayerVector);
        }
    }

    void knockback(float power, Vector2 attackingColliderToPlayerVector)
    {
        rb.AddForce(attackingColliderToPlayerVector.normalized * knockbackDistance * power, ForceMode2D.Impulse);
        StartCoroutine(knockbackCo());
    }

    private IEnumerator knockbackCo()
    {
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        isHurt = false;
    }

    #endregion

    public void RestoreHealth(float amount)
    {
        health += amount;
        healthBarUI.lerpTimer = 0f;
    }

    #region Purchase Items

    public void BuyItem(Component sender, object data)
    {
        InventoryItemData itemData = (InventoryItemData) data;
        bool canPurchaseItem = gold >= itemData.goldValue;
        if (canPurchaseItem)
        {
            gold -= itemData.goldValue;
            playerInventory.InventorySystem.AddToInventory(itemData, 1);
            audioManager.PlaySound("moneySFX");
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    #endregion
}
