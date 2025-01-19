using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float detectionRange = 10f; // Range within which the enemy detects the player
    public float attackRange = 2f; // Range within which the enemy attacks
    public float moveSpeed = 3f; // Speed of the enemy's movement
    public Animator animator; // Reference to the enemy's Animator
    public int health = 100; // Enemy's health

    private bool isAttacking = false; // Is the enemy currently attacking?
    private bool isDead = false; // Is the enemy dead?

    void Update()
    {
        if (isDead) return; // Stop all actions if the enemy is dead

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within detection range but not in attack range
        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            FollowPlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            StopMovement();
        }
    }

    void FollowPlayer()
    {
        if (!isAttacking)
        {
            // Look at the player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Ignore y-axis for 2D-like movement
            transform.LookAt(player);

            // Move towards the player
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Play walk animation
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
    }

    void AttackPlayer()
    {
        StopMovement();

        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

            // Simulate damage to the player (replace with actual logic)
            Debug.Log("Enemy is attacking the player!");
        }
    }

    void StopMovement()
    {
        animator.SetBool("isWalking", false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead"); // Trigger the death animation
        Debug.Log("Enemy is dead!");

        // Disable collider and movement
        GetComponent<Collider>().enabled = false;

        // Optionally destroy the object after the animation
        Destroy(gameObject, 5f); // Adjust the delay as needed
    }

    // Called by animation event at the end of the attack animation
    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
