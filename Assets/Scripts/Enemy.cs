using UnityEngine;
using System.Collections;
using System;

// inherit from MovingObject
public class Enemy : MovingObject {
    public int playerDamage; // the food points that are going to be substracted 
                             // when the enemy attacks the player.
    private Animator animator;
    private Transform target; 
    private bool skipMove;  //Boolean to determine whether or not enemy should skip a turn or move this turn.

    // Use this for initialization
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        // check if skip is true, if so set it to false and skip this turn.
        if(skipMove)
        {
            skipMove = false;
            return; 
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = false; // Now that Enemy has moved, set skipMove to true to skip the move.

    }
	
    // MoveEnemy is called by the GameManager each turn to tell each Enemy to try to move towards the player.
    public void MoneyEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // enemy를 player에게 다가가기 위한 if문 (Epsilon은 0에 가까운 숫자)
        if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
        {
            // If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            // Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
        AttemptMove<Player> (xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        // Declare hitPlayer and set it to equal the encountered component
        Player hitPlayer = component as Player;

        // Call the LoseFood function of hitPlayer passing it playerDamage
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack"); // Set the attack trigger of animator to trigger Enemy attack animation

        throw new NotImplementedException();
    }

}
