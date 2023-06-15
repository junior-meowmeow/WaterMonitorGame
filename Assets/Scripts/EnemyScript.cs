using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemyScript : MonoBehaviour, IDamagable
{
    public Transform stage;

    public Transform player;

    public MovementScript movementController;
    public AttackRangeScript attackRange;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public int maxHealth = 100;
    public int health;
    public bool isInvicible;
    public bool isControllable;

    private bool isStagger;
    private float lastStaggerTime;
    private float currentStaggerDuration;

    [SerializeField] private bool isFloating;
    [SerializeField] private bool isKnockedBack;
    [SerializeField] private bool isDead;

    private float lastActionTime;
    public float actionCooldown = 0.5f;

    public float moveSpeed = 6f;
    public bool isFacingRight = true;

    private Vector2 velocity;
    private float walkingDuration;

    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isAttacking = false;

    public float attackCooldown = 5f;
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

        health = maxHealth;
        isInvicible = false;
        isControllable = true;

        velocity = Vector2.zero;
        walkingDuration = 0f;

        lastActionTime = -100f;
        lastAttackTime = -100f;
        attackCount = 0;

        isFloating = false;
        isKnockedBack = false;
        isDead = false;

        player = GameManagerScript.instance.player;
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

        if (attackerPosition.x > this.transform.position.x)
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
        isControllable = false;
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

        // ControllableState
        if (isAttacking && Time.timeSinceLevelLoad - lastAttackTime > attackCooldown)
        {
            isAttacking = false;
        }

        bool isInterrupted = isStagger || isFloating || isKnockedBack || isDead;

        if (isInterrupted || isAttacking)
        {
            isControllable = false;
        }
        else
        {
            isControllable = true;
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

        if (!isControllable)
        {
            return;
        }

        bool isActionCooldownReady = (Time.timeSinceLevelLoad - lastActionTime) > actionCooldown;

        bool isAttackCooldownReady = (Time.timeSinceLevelLoad - lastAttackTime) * attackSpeed > attackCooldown;

        //print(isAttackCooldownReady + " " + isComboCooldownReady);

        if (isActionCooldownReady)
        {
            if (attackRange.isPlayerInAttackRange && isAttackCooldownReady)
            {
                PerformAttack();
            }
            else
            {
                CalculateMovement();
            }
        }

        PerformMovement();
    }

    private void CalculateMovement()
    {
        Vector2 currentPosition2D = this.transform.position;
        Vector2 targetPosition2D = player.position;
    }

    private void PerformMovement()
    {



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
                    int damage = 5;
                    Vector2 horizontalKnockback = 0.75f * direction;
                    Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                    CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                }
                break;
            case 1:
                {
                    Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                    int damage = 5;
                    Vector2 horizontalKnockback = 0.75f * direction;
                    Vector3 hitBoxSize = new Vector3(1, 1, 0.5f);
                    CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                }
                break;
            case 2:
                {
                    Vector3 attackBoxPosition = currentPosition + 0.5f * direction;
                    int damage = 15;
                    Vector2 horizontalKnockback = 1f * direction;
                    float verticalKnockback = 5f;
                    Vector3 hitBoxSize = new Vector3(1.25f, 1f, 1f);
                    CreateAttackBox(attackBoxPosition, damage, 0.75f, horizontalKnockback, verticalKnockback, hitBoxSize);
                }
                break;
            default:
                print("INVALID ENEMY ATTACK COUNT");
                break;
        }

        lastAttackTime = Time.timeSinceLevelLoad;
        isAttacking = true;
        attackCount++;

        if (attackCount > 2)
        {
            attackCount = 0;
        }

    }

    private void UpdateSprite()
    {
        spriteRenderer.flipX = isFacingRight;

        bool isAttacked = isStagger || isKnockedBack;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isAttacked", isAttacked);
        animator.SetBool("isFalling", isFloating);
        animator.SetBool("isDead", isDead);
    }

    private void CreateAttackBox(Vector3 position, int damage, float staggerDuration, Vector2 horizontalKnockback, float verticalKnockback, Vector3 hitBoxSize)
    {
        GameObject attackBoxObject = Instantiate(attackHitboxPrefab, position, Quaternion.identity);
        attackBoxObject.transform.SetParent(stage.transform, true);
        AttackHitboxScript attackBox = attackBoxObject.GetComponent<AttackHitboxScript>();
        attackBox.targetTag = "PlayerHitbox";
        attackBox.attackerPosition = this.transform.position;
        attackBox.damage = damage;
        attackBox.staggerDuration = staggerDuration;
        attackBox.horizontalKnockbackVelocity = horizontalKnockback;
        attackBox.verticalKnockbackVelocity = verticalKnockback;
        attackBox.boxCollider.size = hitBoxSize;
    }

}
