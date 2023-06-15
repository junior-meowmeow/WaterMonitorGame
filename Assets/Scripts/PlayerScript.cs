using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamagable
{
    public Transform stage;

    public MovementScript movementController;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public int maxHealth = 100;
    public int health;
    public bool isInvicible;
    public bool isControllable;

    public float moveSpeed = 6f;
    public float dashSpeed = 8f;
    public bool isFacingRight = true;

    private bool isStagger;
    private float lastStaggerTime;
    private float currentStaggerDuration;

    private bool isWalking;

    private bool isFloating;
    private bool isKnockedBack;
    [SerializeField] private bool isDead;
    private bool isDashing;
    private bool isAttacking;
    private bool isHeavyAttack;

    private float attackCooldown = 0.15f;
    public float lightAttackCooldown = 0.15f;
    public float heavyAttackCooldown = 0.30f;
    public float comboCooldown = 1f;
    public float attackSpeed = 1f;
    public float attackResetDuration = 1f;
    private int attackCount;
    private float lastAttackTime;
    private float lastComboTime;

    public GameObject attackHitboxPrefab;
    public CatchEnemyScript lastHeavyAttack;

    public float dashMultiplier = 2f;
    public float dashCooldown = 1f;
    private float lastDashTime;


    void Start()
    {
        GameManagerScript.instance.player = this.transform;

        stage = this.transform.parent;

        movementController = this.GetComponent<MovementScript>();

        health = maxHealth;
        isInvicible = false;
        isControllable = true;

        isWalking = false;
        isDead = false;
        isAttacking = false;

        lastAttackTime = -100f;
        lastComboTime = -100f;
        attackCount = 0;
        lastDashTime = -100f;
    }

    void Update()
    {
        UpdateInterruptedState();
        UpdateMovement();
        UpdateDashState();
        UpdateAttackState();
        UpdateSprite();
    }

    public void RecieveDamage(Vector3 attackerPosition, int damage, float staggerDuration, Vector2 horizontalKnockbackVelocity, float verticalKnockbackVelocity)
    {
        movementController.SetHorizontalVelocity(Vector2.zero);
        isWalking = false;

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
        animator.SetTrigger("Dead");
        //Invoke(nameof(DestroySelf), 2.0f);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void UpdateInterruptedState()
    {
        // Interrupted State
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

        if (isAttacking && Time.timeSinceLevelLoad - lastAttackTime > attackCooldown)
        {
            isAttacking = false;
        }

        bool isInterrupted = isStagger || isFloating || isKnockedBack || isDead;

        if (isInterrupted || isAttacking || isDashing)
        {
            isControllable = false;
        }
        else
        {
            isControllable = true;
        }
    }

    private void UpdateMovement()
    {
        if (!isControllable)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal > 0)
        {
            isFacingRight = true;
        }
        else if (moveHorizontal < 0)
        {
            isFacingRight = false;
        }

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        if (movement != Vector2.zero)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        //print(movement);

        movementController.SetHorizontalVelocity(moveSpeed * movement);
        movementController.ClearAdditionVelovity();
    }

    private void UpdateDashState()
    {

        if (movementController.isHittingGround)
        {
            lastHeavyAttack.OnLanding();
        }

        if (movementController.isGrounded)
        {
            // Landing
            isInvicible = false;
            isFloating = false;
            isDashing = false;
        }

        if (!isControllable)
        {
            return;
        }

        bool isDashCooldownReady = (Time.timeSinceLevelLoad - lastDashTime) * attackSpeed > dashCooldown;

        if (movementController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            if (isDashCooldownReady)
            {
                // Dash
                movementController.SetHorizontalVelocity(Vector2.zero);
                movementController.AddVerticalVelocity(dashSpeed);
                movementController.AddFloatingHorizontalVelocity(dashMultiplier * dashSpeed * (isFacingRight ? 1 : -1) * Vector2.right);
                movementController.isGrounded = false;
                isDashing = true;
                isInvicible = true;
                lastDashTime = Time.timeSinceLevelLoad;
            }
            else
            {
                print("DASH ON COOLDOWN");
            }
        }

    }

    private void UpdateAttackState()
    {

        if (!isControllable)
        {
            return;
        }

        bool isAttackCooldownReady = (Time.timeSinceLevelLoad - lastAttackTime) * attackSpeed > attackCooldown;
        bool isComboCooldownReady = (Time.timeSinceLevelLoad - lastComboTime) * attackSpeed > comboCooldown;

        bool lightAttackPressed = Input.GetKeyDown(KeyCode.J);
        bool heaveyAttackPressed = Input.GetKeyDown(KeyCode.K);

        //print(isAttackCooldownReady + " " + isComboCooldownReady);

        if (isAttackCooldownReady && isComboCooldownReady)
        {

            Vector3 currentPosition = this.transform.position;
            Vector3 direction = (isFacingRight ? 1 : -1) * Vector3.right;

            if (lightAttackPressed)
            {
                print("LIGHT ATTACK");
                switch (attackCount)
                {
                    case 0:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.75f * direction;
                            int damage = 10;
                            Vector2 horizontalKnockback = 0.75f * direction;
                            Vector3 hitBoxSize = new Vector3(1.5f, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 1:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.75f * direction;
                            int damage = 10;
                            Vector2 horizontalKnockback = 0.75f * direction;
                            Vector3 hitBoxSize = new Vector3(1.5f, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 2:
                        {
                            Vector3 attackBoxPosition = currentPosition + 2.0f * direction;
                            int damage = 10;
                            float staggerDuration = 0.6f;
                            Vector3 hitBoxSize = new Vector3(4f, 0.5f, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, staggerDuration, Vector2.zero, 0, hitBoxSize);
                        }
                        break;
                    case 3:
                        {
                            Vector3 attackBoxPosition = currentPosition + 1.2f * direction;
                            int damage = 10;
                            float verticalKnockback = 6.5f;
                            Vector3 hitBoxSize = new Vector3(1.25f, 1.2f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, Vector2.zero, verticalKnockback, hitBoxSize);
                        }
                        break;
                    case 4:
                        {
                            Vector3 attackBoxPosition = currentPosition + 1.2f * direction;
                            int damage = 10;
                            Vector2 horizontalKnockback = 5 * direction;
                            float verticalKnockback = 5;
                            Vector3 hitBoxSize = new Vector3(1.25f, 1.5f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, horizontalKnockback, verticalKnockback, hitBoxSize);
                        }
                        break;
                    default:
                        print("INVALID PLAYER ATTACK COUNT");
                        break;
                }
                AfterAttack(false);

            }
            else if (heaveyAttackPressed)
            {
                print("HEAVY ATTACK");
                switch (attackCount)
                {
                    case 0:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.75f * direction;
                            int damage = 20;
                            Vector2 horizontalKnockback = 7.5f * direction;
                            Vector3 hitBoxSize = new Vector3(1.5f, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 0.3f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 1:
                        {
                            Vector3 attackBoxPosition = currentPosition + 1.0f * direction;
                            int damage = 15;
                            float verticalKnockback = 5;
                            Vector3 hitBoxSize = new Vector3(1.5f, 1f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, Vector2.zero, verticalKnockback, hitBoxSize);
                        }
                        break;
                    case 2:
                        {
                            Vector3 attackBoxPosition = currentPosition + 1.2f * direction;
                            Vector3 secondAttackBoxPosition = currentPosition - 1.2f * direction;
                            int damage = 25;
                            Vector2 horizontalKnockback = 7.5f * direction;
                            float verticalKnockback = 5;
                            Vector3 hitBoxSize = new Vector3(1.25f, 1f, 1f);
                            CreateAttackBox(attackBoxPosition, damage, 0, horizontalKnockback, verticalKnockback, hitBoxSize);
                            CreateAttackBox(secondAttackBoxPosition, damage, 0, horizontalKnockback, verticalKnockback, hitBoxSize);
                        }
                        break;
                    case 3:
                        {
                            Vector3 attackBoxPosition = currentPosition + 0.75f * direction;
                            int damage = 20;
                            Vector2 horizontalKnockback = 11f * direction;
                            Vector3 hitBoxSize = new Vector3(1.5f, 1, 0.5f);
                            CreateAttackBox(attackBoxPosition, damage, 1f, horizontalKnockback, 0, hitBoxSize);
                        }
                        break;
                    case 4:
                        {
                            movementController.AddVerticalVelocity(6);
                            movementController.AddFloatingHorizontalVelocity(9 * direction);
                            movementController.isGrounded = false;
                            lastHeavyAttack.StartCatchEnemy(direction);
                            isDashing = true;
                        }
                        break;
                    default:
                        print("INVALID PLAYER ATTACK COUNT");
                        break;
                }
                AfterAttack(true);
            }

        }
        else if (lightAttackPressed || heaveyAttackPressed)
        {
            print("ON COOLDOWN");
        }

        // Reset Combo after a certain amount of time
        if (Time.timeSinceLevelLoad - lastAttackTime > attackResetDuration)
        {
            attackCount = 0;
            print("reset combo");
            lastAttackTime = Time.timeSinceLevelLoad;
        }

    }

    private void AfterAttack(bool isHeavyAttack)
    {
        print(attackCount);
        movementController.SetHorizontalVelocity(Vector2.zero);
        isWalking = false;
        isAttacking = true;
        lastAttackTime = Time.timeSinceLevelLoad;

        this.isHeavyAttack = isHeavyAttack;
        if (isHeavyAttack)
        {
            attackCooldown = heavyAttackCooldown;
        }
        else
        {
            attackCooldown = lightAttackCooldown;
        }

        attackCount++;

        // Check Combo End
        if (attackCount > 4)
        {
            lastComboTime = Time.timeSinceLevelLoad;
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
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isHeavyAttack", isHeavyAttack);
        animator.SetInteger("attackCount", attackCount);
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
