using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float health = 100f;     //Default health, set health in inspector 
    public float maxHealth = 100f;
    public float armour = 15f;
    public float maxArmour = 15f;
    [SerializeField] public Slider healthBar;
    [SerializeField] public Slider armourBar;


    private void Awake()
    {
        try { armourBar.value = armour / maxArmour; }//Armour
        catch { }
        try { healthBar.value = health / maxHealth; } //Health
        catch { }
    }
    
    public void Hit(float damage)
    {
        if (!CompareTag("Player"))
        { 
            health -= damage;
        }

        if (armour > 0)
        {
            armour -= damage;
            return;

        }
        else if (armour > 0 && damage > armour)
        {
            damage -= armour;
            armour = 0;
            health -= damage;
            return;
        }
        else
        {
            health -= damage;
        }
        try { armourBar.value = armour / maxArmour; }
        catch { }
        try { healthBar.value = health / maxHealth; }
        catch { }
    }

    public void Die()
    {
        if(!CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, transform.position);
            
            StopAllCoroutines();
            Destroy(gameObject);
            return;
        }
        StartCoroutine(DoDeath());
    }

    IEnumerator DoDeath()
    {
        Debug.Log("You Died");
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
