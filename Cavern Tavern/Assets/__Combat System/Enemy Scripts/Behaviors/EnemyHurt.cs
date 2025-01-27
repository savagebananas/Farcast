using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnemyHurt : State
{
    /*
    When state is set, script calls OnStart() method which sets all components and calls the hurt() function once
    Hurt() function: 
        -Subtracts damage enemy health
        -Calls the knockback function based on knockback power and the Vector difference from the attackingCollider (arrow come from left, enemy knockback right)
    Knockback() function:        
        -Adds Force to certain vector direction and starts coroutine, which counts down a number of seconds before stopping the velocity (stopping knockback)
        -After knockback, sets state back to follow state
    */

    [Header("Enemy References")]
    [Space(5)]
    [SerializeField] private EnemyBase enemyBase;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Renderer enemySpriteRenderer;
    [SerializeField] private State followState;

    [Header("Null for non-melee enemies")]
    [Space(1)]
    [SerializeField] private AttackPlayer meleeAttackState;

    [Header("VFX")]
    [Space(5)]
    public GameObject bloodParticles;
    public GameObject deathParticles;
    public GameObject smokeParticles;
    public Animator squashAnimator;
    public CinemachineImpulseSource impulse;
    [HideInInspector] public float weaponMultiplier;

    [Header("Knockback Stats")]
    [Space(5)]
    [HideInInspector] public Vector2 attackingColliderToEnemyVector;
    [HideInInspector] public float damageTaken;
    [HideInInspector] public float knockbackPower;
    private float knockbackDistance;
    private float knockbackDuration;
    

    public override void OnStart()
    {
        rb = enemyBase.GetComponent<Rigidbody2D>();
        knockbackDistance = enemyBase.knockbackDistance;
        knockbackDuration = enemyBase.knockbackDuration;

        hurt(damageTaken, knockbackPower, attackingColliderToEnemyVector);
    }

    public override void OnUpdate()
    {
        //To Update Distance From Player and Stop Enemy From Attacking Outside Of Range
        if (meleeAttackState != null) meleeAttackState.enemyToPlayerDistance = enemyBase.enemyDistanceFromPlayer(); 
    }
    public override void OnLateUpdate(){}

    public void hurt(float damage, float knockbackPower, Vector2 attackingColliderToEnemyVector)
    {
        enemyBase.health -= damage;
        
        if (enemyBase.health > 0) knockback(knockbackPower, attackingColliderToEnemyVector); //Enemy hurt
        else knockback(1.5f * knockbackPower, attackingColliderToEnemyVector); //Enemy dead
    }

    void knockback(float power, Vector2 attackingColliderToEnemyVector)
    {
        rb.AddForce(attackingColliderToEnemyVector.normalized * knockbackDistance * power, ForceMode2D.Impulse);
        enemySpriteRenderer.material.color = new Color(255, 255, 255, 255);

        StartCoroutine(knockbackCo());
        StartCoroutine(Flash());
    }

    private IEnumerator knockbackCo()
    {
        if (enemyBase.health <= 0) //enemy dead
        {
            //Camera Shake
            impulse.GenerateImpulse(0.5f);
            //squash stretch animation
            squashAnimator.SetTrigger("SquashAndStretch");
            //particles
            var particle = Instantiate(deathParticles, transform.position, Quaternion.identity);
            particle.transform.localScale = new Vector2(particle.transform.localScale.x, particle.transform.localScale.y);
            particle.transform.parent = enemyBase.transform;

            yield return new WaitForSeconds(knockbackDuration * weaponMultiplier);
            
            rb.linearVelocity = Vector2.zero; //stop velocity (knockback)

            squashAnimator.SetTrigger("Idle"); //stop squishing enemy

            particle.transform.parent = null; //Move blood particles out of enemy object so it will not get destroyed early
            
            //smoke death particles
            var smoke = Instantiate(smokeParticles, transform.position, Quaternion.identity);
            smoke.transform.localScale = new Vector2(smoke.transform.localScale.x * weaponMultiplier, smoke.transform.localScale.y * weaponMultiplier);
            
            //Destroy entire enemy object if dead
            Destroy(enemyBase.gameObject); 
        }
        else //enemy hurt
        {
            //animation
            squashAnimator.SetTrigger("SquashAndStretch");
            //particles
            var particle = Instantiate(bloodParticles, transform.position, Quaternion.identity);
            particle.transform.localScale = new Vector2(particle.transform.localScale.x, particle.transform.localScale.y);
            particle.transform.parent = enemyBase.transform;

            yield return new WaitForSeconds(knockbackDuration * weaponMultiplier);
            
            rb.linearVelocity = Vector2.zero; //stop velocity (knockback)
            squashAnimator.SetTrigger("Idle"); //stop squishing enemy
            stateMachineManager.setNewState(followState); //Enemy changes to attack state after hurt
        }
    }

    private IEnumerator Flash()
    {
        yield return new WaitForSeconds(knockbackDuration * weaponMultiplier);
        enemySpriteRenderer.material.color = Color.white;
    }           
    
}
