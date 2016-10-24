using UnityEngine;
using System.Collections;
using System.Collections.Generic; // we can use lists, which we're going to use to keep track of our enemies.

public class GameManager : MonoBehaviour {
    public float levelStartDelay = 2f;  // ** Time to wait before starting level, in seconds
    public float turnDelay = .1f; // ** Dealy between each Player turn.
    public static GameManager instance = null; // 다른 클래스가 접근할 수 있또록 public.

    public BoardManager boardScript; 
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private int level = 3;
    private List<Enemy> enemies; // ** List of all enemy units, used to used to issue them move commands. 
    private bool enemiesMoving; // ** Boolean to check if enemies are moving.

    // get and store a component reference
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject); // 이미 인스턴스가 존재한다면 이것을 파괴하자. (GameManager 인스턴스는 오로지 하나만 있을 수 있음을 알 수 있음)

        DontDestroyOnLoad(gameObject); // when we load a new scene, normally all of the game objects in the hierarchy will be destroyed.
                                       // 즉, 쉽게 말해서 씬이 reloading 될 때 파괴되지 않도록 하기 위한 것.

        enemies = new List<Enemy>(); // ** Assign enemies to a new List of Enemy objects.
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear(); // ** Clear any Enemy objects in our List to prepare for next level.
        boardScript.SetupScene(level); // boardScript를 참조하여 SetUpScene 함수를 호출하여 적의 개수를 정할 수 있음.
    }

    public void GameOver()
    {
        enabled = false;
    }
	
	// Update is called once per frame
    /*
     *  새로 수정됨. 
     */
	void Update ()
    {
        // player's turn or enemiesMoving 
	    if(playersTurn || enemiesMoving)
        {
            return;
        }

        // if neither are true we're going to start our coroutine MoveEnemies.
        StartCoroutine(MoveEnemies());
	}

    /*
     *  새로 추가됨. 
     */
     // Call this to add the passed in Enemy to the List of Enemy Lists.
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    /*
     * 새로 추가됨.
     */
    IEnumerator MoveEnemies()
    {
        // While enemiesMoving is true player is unable to move.
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay); // WaitForSeconds : 주어진 시간(초)동안, co-routine의 수행을 중단.
        
        // If there are no enemies spawned ex) level = 1;
        if(enemies.Count == 0)
        {
            // Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        // Loop through List of enemy objects.
        for(int i = 0; i < enemies.Count; i++)
        {
            // Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            // Wait for enemy's moveTime before moving the next Enemy
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        // Once enemies are done moving, set playersTurn to true so player can move,
        playersTurn = true;

        // Enemies are done moving, set enemiesMoving = false
        enemiesMoving = false;
    }
}
