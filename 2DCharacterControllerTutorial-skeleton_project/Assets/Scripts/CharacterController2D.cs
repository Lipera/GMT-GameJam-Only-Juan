using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Holding time necessary to interpret input as a long input")]
    float longInputTime = .5f;

    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    private BoxCollider2D boxCollider;

    private Vector2 velocity;

    private bool grounded;

    private float timeBetweenInputs;

    private bool moveRight;

    private float buttonPressedTime;

    private void Awake()
    {      
        boxCollider = GetComponent<BoxCollider2D>();
        moveRight = false;
        buttonPressedTime = 0;
    }

    private void Update()
    {
        if(grounded) {
            velocity.y = 0;

            if(Input.GetButtonDown("Jump")) {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        }
        
        handleControls();

        velocity.y += Physics2D.gravity.y * Time.deltaTime;

        float acceleration = grounded ? walkAcceleration : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        float moveInput = Input.GetAxisRaw("Horizontal");
        // if(Input.GetButtonDown("Fire1")) {
        //     moveRight = true;
        // }

        if(moveRight) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed, acceleration * Time.deltaTime);
        } else if(moveInput != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }
        transform.Translate(velocity * Time.deltaTime);

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        grounded = false;
        foreach(Collider2D hit in hits) {
            if(hit == boxCollider) {
                continue;
            }

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            if(colliderDistance.isOverlapped) {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }

            if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0) {
                grounded = true;
            }
        }
    }

    private void handleControls() {
        if(Input.GetKey(KeyCode.C)) {
            // moveRight = true;
            buttonPressedTime += Time.deltaTime;

            if(grounded && buttonPressedTime >= longInputTime) {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        } else {
            buttonPressedTime = 0;
        }
    }
}
