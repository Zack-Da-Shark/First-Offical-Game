using UnityEngine;
using UnityEngine.InputSystem;

public class RangedWeapon : MonoBehaviour
{
    public InputActionReference grab;
    public InputActionReference attack;
    /* Weapon needs to do the following
    float up and down
    Be pickup-able by any entity
    Can be thrown (1-shot hit depends on weapon weight)
    Must be used for great violence
    */

    private bool acquired = false;

    private Rigidbody2D rb2D;

    //Properties for different weapons
    enum Type
    {
        Automatic,
        SemiAutomatic,
        Shotgun,
        Sniper,
        Explosive
    }
    private Transform location;


    private Type type;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        //Determine what object is the weapon via tags
        //Automatic
        //SemiAutomatic
        //Shotgun
        //Sniper
        //Explosive


        if(gameObject.CompareTag("Automatic"))
            type = Type.Automatic;
        else if(gameObject.CompareTag("SemiAutomatic"))
            type = Type.SemiAutomatic;
        else if(gameObject.CompareTag("Shotgun"))
            type = Type.Shotgun;
        else if(gameObject.CompareTag("Sniper"))
            type = Type.Sniper;
        else if(gameObject.CompareTag("Explosive"))
            type = Type.Explosive;

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
                if (rb2D != null)
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
            //Determine correct bullet hell type
        }
    }
}
