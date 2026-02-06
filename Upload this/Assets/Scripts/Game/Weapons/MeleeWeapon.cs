using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeapon : MonoBehaviour
{
    public InputActionReference grab;
    public InputActionReference attack;
    /* Weapon needs to do the following
    float up and down
    Be pickup-able by any entity
    Can be thrown (1-shot hit depends on weapon weight)
    Must be used for great violence
    */
    private readonly float force = 0.2f;
    //game is very tiny
    
    //Swing settings
    private readonly float swingAngle = 90f; // How far to swing in degrees
    private readonly float swingDuration = 0.3f; // How long the swing takes
    private readonly float swingRadius = 1f; // Distance from player to weapon during swing
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    private bool acquired = false;
    private Rigidbody rb3D;
    private Rigidbody2D rb2D;

    //Properties for different weapons
    enum Type
    {
        Blunt,
        HeavyBlunt,
        ShortBlade,
        LongBlade,
        Electric,
        Fragile
    }
    private Transform location;


    private Type type;

    void Start()
    {
        rb3D = GetComponent<Rigidbody>();
        rb2D = GetComponent<Rigidbody2D>();

        //Determine what object is the weapon via tags
        //Blunt
        //Heavy Blunt
        //Short Blade
        //Long Blade
        //Electric
        //Fragile

        if(gameObject.CompareTag("Blunt"))
            type = Type.Blunt;
        else if(gameObject.CompareTag("Heavy Blunt"))
            type = Type.HeavyBlunt;
        else if(gameObject.CompareTag("Short blade"))
            type = Type.ShortBlade;
        else if(gameObject.CompareTag("Long Blade"))
            type = Type.LongBlade;
        else if(gameObject.CompareTag("Electric"))
            type = Type.Electric;
        else if(gameObject.CompareTag("Fragile"))
            type = Type.Fragile;

        //Leave the template here
        //else if(gameObject.CompareTag(""))
        //    type = Type.
    }
    
    //If player is within range and presses 'e'
    //Pickup weapon

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        if (grab?.action != null)
            grab.action.performed += OnPickUp;
        if (attack?.action != null)
        {                        
            Debug.Log("Attack action, callling on use");
            attack.action.performed += OnUse;
        }
    }

    private void OnDisable()
    {
        if (grab?.action != null)
            grab.action.performed -= OnPickUp;
        if (attack?.action != null)
            attack.action.performed -= OnUse;
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        //Code for picking up/ throwing / swapping weapons  
        Debug.Log("Attempting to pick up weapon");
        //Distance from player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if(Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            acquired = !acquired;
            if (acquired)
            {
                Debug.Log("Weapon acquired");
                transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
                transform.localPosition = new Vector3(0.5f, -0.5f, 1f);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                originalRotation = transform.localRotation;
                originalPosition = transform.localPosition;
                // Change the physics to kinematic/supported state and stop motion
                if (rb3D != null)
                {
                    rb3D.linearVelocity = Vector3.zero;
                    rb3D.angularVelocity = Vector3.zero;
                    rb3D.isKinematic = true;
                }
                if (rb2D != null)
                {
                    rb2D.linearVelocity = Vector2.zero;
                    rb2D.angularVelocity = 0f;
                    rb2D.bodyType = RigidbodyType2D.Kinematic;
                }
                //Weapon follows player hand
                //End of weapon will always point to the top left in the png, make not of it to adjust if needed
            }
            else
            {
                Debug.Log("Weapon yeeted");
                transform.SetParent(null);
                //yeet the weapon towards mouse
                Vector3 mouseScreen = Mouse.current.position.ReadValue();
                Vector3 mouseWorld3D = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, Camera.main.nearClipPlane));
                Vector3 direction3D = (mouseWorld3D - player.transform.position).normalized;

                if (rb3D != null)
                {
                    rb3D.isKinematic = false;
                    rb3D.AddForce(direction3D * force, ForceMode.Impulse);
                }
                else if (rb2D != null)
                {
                    // For 2D, convert mouse to world with z = 0 (assumes top-down camera orthographic)
                    Vector3 mouseWorld2D = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
                    Vector2 direction2D = (mouseWorld2D - player.transform.position).normalized;
                    rb2D.bodyType = RigidbodyType2D.Dynamic;
                    rb2D.linearVelocity = Vector2.zero;
                    rb2D.AddForce(direction2D * force, ForceMode2D.Impulse);
                }
            }
        }
    }

    //This is where the fun begins
    public void OnUse(InputAction.CallbackContext context)
    {
        if(acquired)//If player has a weapon
        {
            Debug.Log("Using weapon");
            StartCoroutine(SwingWeapon());
        }
    }

    private System.Collections.IEnumerator SwingWeapon()
    {
        var player = transform.parent;
        if (player == null) yield break;
        
        // Get player's facing direction
        Vector3 playerForward = player.forward;
        
        // Calculate the arc path perpendicular to player forward
        Vector3 arcAxis = player.up; // Swing in the horizontal plane
        Vector3 arcStart = player.position + (player.right * swingRadius);
        Vector3 arcEnd = player.position + (Quaternion.AngleAxis(swingAngle, arcAxis) * player.right * swingRadius);
        
        float elapsedTime = 0f;
        bool damageDealt = false;

        // Swing out
        while (elapsedTime < swingDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (swingDuration / 2f);
            
            // Interpolate position along the arc
            Vector3 arcDirection = (arcEnd - arcStart).normalized;
            transform.position = Vector3.Lerp(arcStart, arcEnd, t);
            
            // Rotate to face outward during swing
            transform.rotation = Quaternion.LookRotation(transform.position - player.position, player.up);
            
            // Deal damage during the swing
            if (!damageDealt && t > 0.3f) // Hit at 30% through the swing
            {
                DetectAndHitEnemies();
                damageDealt = true;
            }
            
            yield return null;
        }

        // Swing back
        elapsedTime = 0f;
        while (elapsedTime < swingDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (swingDuration / 2f);
            
            transform.position = Vector3.Lerp(arcEnd, arcStart, t);
            transform.rotation = Quaternion.LookRotation(transform.position - player.position, player.up);
            
            yield return null;
        }

        // Return to original position and rotation
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    private void DetectAndHitEnemies()
    {
        //Detect enemies in front of player within a certain range
        //Only using rigidbody 2d
        if (rb2D != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position + (Vector2)transform.right, 1f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("EnemyMelee"))
                {
                    Debug.Log("Hit enemy with " + type.ToString());
                    Destroy(hit.gameObject);
                }
            }
        }
    }
}
