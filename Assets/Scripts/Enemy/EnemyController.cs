using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public StatsData enemyStats = new StatsData();

    private Animator enemyAnimator;

    [SerializeField]
    private GameObject weapon;
    private float range;
    private int damage;

    public GameObject player;
    private bool isAttacking = false;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private float moveSpeed = 3f; // Added speed factor

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();

        //small values for easier testing
        enemyStats.health = 50;
        damage = 10;

        healthBar.maxValue = enemyStats.health;
        healthBar.value = enemyStats.health;

        range = weapon.GetComponent<WeaponController>().weaponData.range;
        //damage = weapon.GetComponent<WeaponController>().weaponData.damage;
    }

    void Update()
    {
        if (GetComponent<EnemyViewController>().isSeeing && player)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.transform.LookAt(new Vector3(player.transform.position.x, healthBar.transform.position.y, player.transform.position.z));

            if (Vector3.Distance(transform.position, player.transform.position) > range)
            {
                // Face the player
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

                // Move towards the player with speed factor
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z),
                    moveSpeed * Time.deltaTime
                );

                enemyAnimator.SetBool("isWalking", true); // Trigger walking animation
            }
            else if (!isAttacking)
            {
                Attack();
            }
        }
        else
        {
            healthBar.gameObject.SetActive(false);
            enemyAnimator.SetBool("isWalking", false); // Stop walking animation
        }
    }

    private void Attack()
    {
        var playerToHit = player;

        isAttacking = true;
        enemyAnimator.SetBool("isAttacking", true);

        StartCoroutine(Utility.TimedEvent(() =>
        {
            playerToHit.GetComponent<CombatController>().TakeDamage(damage);
        }, 1f));

        StartCoroutine(Utility.TimedEvent(() =>
        {
            isAttacking = false;
            enemyAnimator.SetBool("isAttacking", false);
        }, 2.5f));
    }

    public void TakeDamage(int damage)
    {
        healthBar.value -= damage;
        enemyStats.health -= damage;
        enemyAnimator.SetBool("isHit", true);

        if (enemyStats.health <= 0)
        {
            GetComponent<Collider>().enabled = false;
            enemyAnimator.SetBool("isDead", true);
            player = null;

            Destroy(gameObject, 4f);
        }
    }
}
