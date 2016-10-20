using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    public Sprite dmgSprite; // can see attacked walls when player attack walls !
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
	
    public void DamageWall (int loss)
    {
        spriteRenderer.sprite = dmgSprite; // assign attacked walls to the sprite.
        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false); // not equal enable. enable is for component
    }
}
