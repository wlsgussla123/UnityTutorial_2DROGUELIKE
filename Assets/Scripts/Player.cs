﻿using UnityEngine;
using System.Collections;
using System;

// Player inherits from MovingObjects, our base class for objects that can move,
// Enemy also inherits from this.

public class Player : MovingObject {
    public int wallDamage = 1; // damage when player chop to wall
    public int pointsPerFood = 10; 
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f; // Delay time to restart level.

    private Animator animator;
    private int food;
    
	// Use this for initialization
	protected override void Start () {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        base.Start(); // initialize by using MovingObject
	}

    // Update is called once per frame
    void Update()
    {
        // If it's not player's turn, exit the function.
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal"); // 키보드와 조이스틱 입력값에 대해 -1, 0, 1 중 하나의 값을 갖는다.
        vertical = (int)Input.GetAxisRaw("Vertical"); // 마찬가지

        // 대각선으로 이동하는 것을 막기 위하여
        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical); // player 가 가고자 하는 방향
        }
    }

    // OnDisable : This function is called when player is disable.
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();
        GameManager.instance.playersTurn = false; // 이 시기에 Update에서 playersTurn을 확인
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);

            // disable the player object since level is over.
            enabled = false;
        }
        else if(other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall; // as는 형변환을 의미. (normal casting과 다른 점 : 변환이 불가능하면 예외가 아닌 null 반환

        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");

        throw new NotImplementedException();
    }

    // If player reached the exit, set a new level 
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}