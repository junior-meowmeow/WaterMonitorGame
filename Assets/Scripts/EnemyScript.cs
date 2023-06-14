using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemyScript : MonoBehaviour, IDamagable
{
    public Transform stage;

    public MovementScript movementController;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public int startingHealth = 100;
    public int health;
    public bool isInvicible;
    public bool isControllable;

    private bool isStagger;
    private float lastStaggerTime;
    private float currentStaggerDuration;

    private bool isFloating;
    private bool isKnockedBack;
    private bool isDead;

    private float lastActionTime;
    public float actionCooldown = 0.5f;

    public float moveSpeed = 6f;
    public bool isFacingRight = true;

    private bool isPlayerInAttackRange = false;

    [SerializeField]private bool isWalking = false;
    private bool isAttacking = false;

    public float attackCooldown = 2f;
    public float comboCooldown = 1f;
    public float attackSpeed = 1f;
    public float resetDuration = 4f;
    private int attackCount;
    private float lastAttackTime;

    public GameObject attackHitboxPrefab;


    void Start()
    {
        stage = this.transform.parent;

        movementController = this.GetComponent<MovementScript>();

        health = startingHealth;
        isInvicible = false;
        isControllable = true;

        lastActionTime = -100f;
        lastAttackTime = -100f;
        attackCount = 0;

        isFloating = false;
        isKnockedBack = false;
        isDead = false;
    }

    void Update()
    {
        UpdateState();
        UpdateCooldown();
        UpdateAction();
        UpdateSprite();
    }

    public void RecieveDamage(Vector3 attackerPosition, int damage, float staggerDuration, Vector2 horizontalKnockbackVelocity, float verticalKnockbackVelocity)
    {
        if (damage > 0)
        {
            health -= damage;
            if (health <= 0)
            {
                OnDead();
            }
        }

        if (staggerDuration > 0)
        {
            // play stagger animation
            isStagger = true;
            if (lastStaggerTime + currentStaggerDuration < Time.timeSinceLevelLoad + staggerDuration)
            {
                lastStaggerTime = Time.timeSinceLevelLoad;
                currentStaggerDuration = staggerDuration;
            }
        }

        if (verticalKnockbackVelocity > 0)
        {
            //knock up type
            movementController.SetVerticalVelocity(verticalKnockbackVelocity);
            movementController.AddFloatingHorizontalVelocity(horizontalKnockbackVelocity);
            movementController.isGrounded = false;
            isFloating = true;
            isKnockedBack = true;
        }
        else if (horizontalKnockbackVelocity != Vector2.zero)
        {
            //knock back type
            movementController.AddDecayableHorizontalVelocity(horizontalKnockbackVelocity);
            isKnockedBack = true;
        }

        if(attackerPosition.x > this.transform.position.x)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }

    }

    public void OnDead()
    {
        isDead = true;
        Invoke(nameof(DestroySelf), 2.0f);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void UpdateState()
    {
        if (isFloating && movementController.isGrounded)
        {
            isFloating = false;
            print("Landed");
        }

        if (isKnockedBack && !movementController.hasDecayableVelocity)
        {
            isKnockedBack = false;
        }

        if (Time.timeSinceLevelLoad > lastStaggerTime + currentStaggerDuration)
        {
            isStagger = false;
        }

    }

    private void UpdateCooldown()
    {
        bool isInterrupted = isStagger && isFloating && isKnockedBack;
        if (isInterrupted)
        {
            lastActionTime += Time.deltaTime;
            lastAttackTime += Time.deltaTime;
        }
    }

    private void UpdateAction()
    {

        bool isAttackCooldownReady = (Time.timeSinceLevelLoad - lastAttackTime) * attackSpeed > attackCooldown;

        //print(isAttackCooldownReady + " " + isComboCooldownReady);

        if (isPlayerInAttackRange && isAttackCooldownReady)
        {
            PerformAttack();
        }

    }

    private void PerformAttack()
    {

        Vector3 currentPosition = this.transform.position;
        Vector3 direction = (isFacingRight ? 1 : -1) * Vector3.right;

        switch (attackCount)
        {
            case 0:
                {
                    Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                    int damage = 15;
                    Vector2 horizontalKnockback = 1f * direction;
                    Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                    CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                }
                break;
            case 1:
                {
                    Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                    int damage = 15;
                    Vector2 horizontalKnockback = 1f * direction;
                    Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                    CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                }
                break;
            case 2:
                {
                    Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                    int damage = 15;
                    Vector2 horizontalKnockback = 1f * direction;
                    Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                    CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                }
                break;
            default:
                print("INVALID ENEMY ATTACK COUNT");
                break;
        }

        lastAttackTime = Time.timeSinceLevelLoad;
        attackCount++;

        if (attackCount > 2)
        {
            attackCount = 0;
        }

    }

    private void UpdateSprite()
    {
        spriteRenderer.flipX = isFacingRight;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isFalling", isFloating);
        animator.SetBool("isDead", isDead);
    }

    private void CreateAttackBox(Vector3 position, int damage, float staggerDuration, Vector2 horizontalKnockback, float verticalKnockback, Vector3 hitBoxSize)
    {
        GameObject attackBoxObject = Instantiate(attackHitboxPrefab, position, Quaternion.identity);
        attackBoxObject.transform.SetParent(stage.transform, true);
        AttackHitboxScript attackBox = attackBoxObject.GetComponent<AttackHitboxScript>();
        attackBox.targetTag = "EnemyHitbox";
        attackBox.attackerPosition = this.transform.position;
        attackBox.damage = damage;
        attackBox.staggerDuration = staggerDuration;
        attackBox.horizontalKnockbackVelocity = horizontalKnockback;
        attackBox.verticalKnockbackVelocity = verticalKnockback;
        attackBox.boxCollider.size = hitBoxSize;
    }

}
