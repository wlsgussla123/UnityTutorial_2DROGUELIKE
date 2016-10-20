using UnityEngine;
using System.Collections;

/*
 * abstract class로 설정함으로써 drived class에서 반드시 구현해야만 하도록 만든다. 
 */
public abstract class MovingObject : MonoBehaviour {
    public float moveTime = 0.1f; // 유닛이 움직이는데 걸리는 시간
    public LayerMask blockingLayer; // 충돌을 감지하는 layer (이동할 수 있는가 없는가를 결정)

    private BoxCollider2D boxCollider; 
    private Rigidbody2D rb2D;
    private float inverseMoveTime; // make movement more efficient

	// Use this for initialization, protected virtual : (오버라이드 될 것임)
	protected virtual void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}

    /* 이 경우에는 리턴이 두 개가 된다고 볼 수 있다. (반환형 bool과 매개변수의 out)
     * @ out RaycastHit2D hit : call by reference와 기능은 같지만 call by reference는 전달되는 인자가 초기화 되어야 하는 것과 달리 out은 초기화 되지 않아도 된다.
     */
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false; // Disable the boxCollider so that linecast doesn't hit this object's own collider.
        hit = Physics2D.Linecast(start, end, blockingLayer); // cast a line from start point to end point checking collision on blockinglayer.
        boxCollider.enabled = true;

        // blocking layer가 없으면 move 할 수 있다는 의미
        if(hit.transform == null)
        {
            // 맞은 것이 없다면, 이동을 하자
            StartCoroutine (SmoothMovement(end));

            return true;
        }

        return false;
    }
    /* Generic 함수를 사용하는 이유? : movingObject로 부터 player와 enemy가 상호작용 함. (Player는 Walls와 Enemy는 Player와 상호작용하기 때문에 hitComponent가 무엇인지 알 수가 없다.)
     * constraint : component, why ? we expect our unit to interact with if blocked. In the case of enemies, 
     *                               this going to be a player and in the case of the player this is going to be walls so that the player can attack and destroy the walls.
     *                                
     */ 
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit; // hit will store whatever our linecast hits when Move is called.
        bool canMove = Move(xDir, yDir, out hit); // hit은 Move를 호출하면서 값이 저장 되어진다.

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }
    /* Coroutine for moving units from one space to next
     * @ end : to specify where to move to. 
     */
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // Epsilon : 거의 0에 가까운 값, anyValue + Epsilon = anyValue, anyValue - Epsilon = anyValue
        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime); // find a new position closer to end
            rb2D.MovePosition(newPosition);

            // Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null; //  //Return and loop until sqrRemainingDistance is close enough to zero to end the function
        }
    }

    // The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
    // where T : The type argument must be a reference type; this applies also to any class, interface, delegate, or array type.
    // because this function is abstract class, it hasn't a brace
    protected abstract void OnCantMove <T> (T component)
        where T : Component; // 제약이라고 보면 된다. 이 경우에는 T에 올 수 있는 것이 Component 타입이어야 한다.
}
