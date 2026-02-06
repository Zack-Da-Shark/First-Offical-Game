using UnityEngine;

public class Chase : MonoBehaviour
{
    private Transform target;
    public LayerMask obstacleMask;

    public float speed = 5f;
    public float sightAngle = 170f;
    public float viewDistance = 20f;
    public float hearDistance = 2f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (TargetSeen(target) || TargetHeard(target))
        {
            // Move toward player
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            // Face player
            transform.up = target.position - transform.position;
        }
    }

    public bool TargetSeen(Transform target)
    {
        //This function dictates the visibility


        Vector2 origin = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;

        //How far away is the player?
        float distance = Vector2.Distance(origin, target.position);
        if (distance > viewDistance) return false;

        //Is the player within viewing range?
        float angleToTarget = Vector2.Angle(transform.up, direction);
        if (angleToTarget > sightAngle / 2f) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            obstacleMask
        );

        //Don't have x-ray vision
        if (hit.collider != null)
            return false;

        return true;
    }

    public bool TargetHeard(Transform target)
    {
        //This block dictates if the player is within the enemy's hearing range
        
        Vector2 origin = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;

        float distance = Vector2.Distance(origin, target.position);
        if (distance > hearDistance) return false;
        
        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            obstacleMask
        );

        //Can't hear through walls
        if (hit.collider != null)
            return false;

        return true;
    }

}
