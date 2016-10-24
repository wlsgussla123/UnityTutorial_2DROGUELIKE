using UnityEngine;
using System.Collections;
using System.Collections.Generic; // we can use lists, which we're going to use to keep track of our enemies.
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public float levelStartDelay = 2f;  // ** Time to wait before starting level, in seconds
    public float turnDelay = .1f; //  Dealy between each Player turn.
    public static GameManager instance = null; // 다른 클래스가 접근할 수 있또록 public.
    public BoardManager boardScript; 
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1; // ** level = 1 부터 시작
    private List<Enemy> enemies; //  List of all enemy units, used to used to issue them move commands. 
    private bool enemiesMoving; //  Boolean to check if enemies are moving.
    private bool doingSetup; // check if we're setting up the board and prevent the player from moving during setup.

    // get and store a component reference
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject); // 이미 인스턴스가 존재한다면 이것을 파괴하자. (GameManager 인스턴스는 오로지 하나만 있을 수 있음을 알 수 있음)

        DontDestroyOnLoad(gameObject); // when we load a new scene, normally all of the game objects in the hierarchy will be destroyed.
                                       // 즉, 쉽게 말해서 씬이 reloading 될 때 파괴되지 않도록 하기 위한 것.

        enemies = new List<Enemy>(); // Assign enemies to a new List of Enemy objects.
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    /*
     *  새로 추가됨
     */
     // This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        // Add one to our level number
        level++;
        // Call initgame to initialize our level
        InitGame();
    }

    /*
     * 수정됨. (UI 요소들과 각 레벨을 설정하기 위하여)
     */ 
    void InitGame()
    {
        doingSetup = true; // ** the player won't be able to move while the title card is up.

        levelImage = GameObject.Find("LevelImage"); //**
        levelText = GameObject.Find("LevelText").GetComponent<Text>(); // **
        levelText.text = "Day " + level; // **
        levelImage.SetActive(true); // **
        Invoke("HideLevelImage", levelStartDelay); // ** delay time 뒤에 HideLevelImage 실행 

        enemies.Clear(); //  Clear any Enemy objects in our List to prepare for next level.
        boardScript.SetupScene(level); // boardScript를 참조하여 SetUpScene 함수를 호출하여 적의 개수를 정할 수 있음.
    }

    /*
     * 새로 추가됨
     */
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    /*
     * 수정됨
     */ 
    public void GameOver()
    {
        levelText.text = "After " + level + "days, you starved."; //**
        levelImage.SetActive(true); // **
        enabled = false;
    }
	
	// Update is called once per frame
    /*
     * 수정됨
     */ 
	void Update ()
    {
        // player's turn or enemiesMoving 
        //**
	    if(playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        // if neither are true we're going to start our coroutine MoveEnemies.
        StartCoroutine(MoveEnemies());
	}

     // Call this to add the passed in Enemy to the List of Enemy Lists.
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

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
