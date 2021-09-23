using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : LivingEntity
{
    public NavMeshAgent navMeshAgent;
    public LootScript lootScript;
    public GameManager manager;
    public GameObject dieEffect;
    public BombLuncher bombLuncher;
    public Weapon currentWeapon;
    public Transform target;
    public EnemyStats stats;

    public LayerMask whatIsGround, whatIsObstacle, whatIsPlayer;

    [Header("Health")]
    [SerializeField] HealthBar healthBar;

    private AiStateMachine stateMachine = new AiStateMachine();

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        bombLuncher = GetComponentInChildren<BombLuncher>();
        manager = FindObjectOfType<GameManager>();
        lootScript = GetComponent<LootScript>();

        stateMachine = new AiStateMachine();

        var patrol = new AiPatrolingState(this);
        var chase = new AiChasePlayerState(this);
        var attak = new AiAttackPlayerState(this);

        At(patrol, chase, TargetInViewRange());
        At(chase, patrol, TargetLost());
        At(chase, attak, TargetInAttakRange());
        At(attak, patrol, TargetLost());

        stateMachine.SetState(patrol);

        void At(AiState from, AiState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        Func<bool> TargetLost() => () => target == null;
        Func<bool> TargetInViewRange() => () => {
            if (target != null)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (Vector3.Angle(transform.forward, dirToTarget) < stats.viewAngle / 2 
                && distToTarget <= stats.sightRange)
                {
                    return !Physics.Raycast(transform.position, dirToTarget, distToTarget, whatIsObstacle);
                }
            }
            return false;
        };
        Func<bool> TargetInAttakRange() => () => target != null &&
            Vector3.Distance(transform.position, target.position) <= stats.attackRange;
    }

    protected override void Start()
    {
        base.Start();
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        stateMachine.Update();
        healthBar.SetHealth(health);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Enemy/Death", transform.position);
        lootScript.dropLoot();

        manager.killedEnemies++;
        Destroy(Instantiate(dieEffect, transform.position, transform.rotation) as GameObject, 1);

        base.Die();

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.sightRange);
    }
}
