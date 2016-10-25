using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite; // can see attacked walls when player attack walls !
    public int hp = 4;
    public AudioClip chopSound1; // **
    public AudioClip chopSound2; // **

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
	
    /*
     *  수정됨
     */ 
    public void DamageWall (int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2); // **
        spriteRenderer.sprite = dmgSprite; // assign attacked walls to the sprite.
        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false); // not equal enable. enable is for component
    }
}
