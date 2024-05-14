using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    public int currentHealth = 5;
    public GameObject deathAnimation;

    public int score;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageEnemy(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            Instantiate(deathAnimation, transform.position + (transform.forward * (-1 * Time.deltaTime)), transform.rotation);
            AudioManager.instance.PlaySFX(2);
            UIController.instance.score.text = (int.Parse(UIController.instance.score.text) + score).ToString();
            
        }
    }
}
