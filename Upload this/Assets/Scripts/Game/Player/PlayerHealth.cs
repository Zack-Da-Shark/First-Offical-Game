using UnityEngine;
using static UnityEngine.SceneManagement.SceneManager;

public class PlayerHealth : MonoBehaviour
{
    //When player is touched by a melee enemy, die
    //When player is shot 3 times, die
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("EnemyMelee"))
        {
            //Hit by an enemy
            //Debug.Log("Contact Made");
            Restart();
        }
        else if(other.gameObject.CompareTag("Bullet"))
        {
            //Shot
            Restart();
        }
    }

    private void Restart()
    {
        //Show Popup, text should be "Press R to restart"
        LoadScene(GetActiveScene().buildIndex);
    }
}
