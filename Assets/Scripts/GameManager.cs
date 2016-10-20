using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null; // 다른 클래스가 접근할 수 있또록 public.

    public BoardManager boardScript;

    private int level = 3;

    // get and store a component reference
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject); // 이미 인스턴스가 존재한다면 이것을 파괴하자. (GameManager 인스턴스는 오로지 하나만 있을 수 있음을 알 수 있음)

        DontDestroyOnLoad(gameObject); // when we load a new scene, normally all of the game objects in the hierarchy will be destroyed.
                                       // 즉, 쉽게 말해서 씬이 reloading 될 때 파괴되지 않도록 하기 위한 것.
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene(level); // boardScript를 참조하여 SetUpScene 함수를 호출하여 적의 개수를 정할 수 있음.
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
