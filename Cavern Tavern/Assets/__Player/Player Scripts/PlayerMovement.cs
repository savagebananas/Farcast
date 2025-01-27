using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerBase playerBase;
    public HealthBarUI healthBarUI;
    private Rigidbody2D characterBody;
    private Vector2 inputMovement;
    private bool canMove = true;

    public float moveSpeed;
    public float dashSpeed;
    public float dashLength;
    public int maxDashes = 2;
    public int amountOfDashes;
    public float dashRegenLength;
    private float nextRegenTime;
    public bool isDashing = false;
    
    private float activeMoveSpeed;
    
    void Start()
    {
        playerBase = GetComponent<PlayerBase>();
        characterBody = GetComponent<Rigidbody2D>();

        activeMoveSpeed = moveSpeed;

        amountOfDashes = maxDashes;
    }

    void Update()
    {
        if (canMove)
        {
            Movement();

            // space pressed and movement key pressed -> dash
            if (Input.GetKeyDown("space") && amountOfDashes > 0 &&
                (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))) 
            {
                if (amountOfDashes == 2) nextRegenTime = Time.time + dashRegenLength;
                StartCoroutine(dash());
            }
        }

        RegenerateDash();
    }

    void Movement()
    {
        inputMovement.x = Input.GetAxisRaw("Horizontal");
        inputMovement.y = Input.GetAxisRaw("Vertical");
        inputMovement.Normalize();
        if (playerBase.isHurt == false)
        {
            characterBody.linearVelocity = inputMovement * activeMoveSpeed; 
        }
    }

    /// <summary>
    /// Changes the movement speed of the player temporarily
    /// and ignores certain collisions
    /// </summary>
    IEnumerator dash()
    {
        activeMoveSpeed = dashSpeed;
        isDashing = true;
        Physics2D.IgnoreLayerCollision(9, 10, true); // ignore projectiles and enemy collision layers
        amountOfDashes -= 1;
        healthBarUI.reduceDash();

        yield return new WaitForSeconds(dashLength);

        activeMoveSpeed = moveSpeed; //normal speed
        isDashing = false;

        Physics2D.IgnoreLayerCollision(9, 10, false);
    }

    void RegenerateDash()
    {
        if (amountOfDashes < maxDashes)
        {
            if (Time.time >= nextRegenTime)
            {
                nextRegenTime = Time.time + dashRegenLength;
                amountOfDashes++;
            }
        }
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        characterBody.linearVelocity = new Vector2(0, 0);
    }
}
