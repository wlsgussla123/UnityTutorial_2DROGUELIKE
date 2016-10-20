using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {
    public GameObject gameManager;

	// Use this for initialization
	void Awake ()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager); // null 이라면 prefabs 에 있는 gameManager를 인스턴스화 한다.  
	}
}
