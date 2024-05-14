using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointController : MonoBehaviour
{
    public string cpName;
    // Start is called before the first frame update
    void Start()
    {   
        if(PlayerPrefs.HasKey(SceneManager.GetActiveScene().name + "_cp"))
        {
            if(PlayerPrefs.GetString(SceneManager.GetActiveScene().name + "_cp") == cpName)
            {
                PlayerController.instance.transform.position = transform.position;
                Debug.Log("Player start " + cpName);
                Debug.Log(transform.position);
                Debug.Log(PlayerController.instance.transform.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetString(SceneManager.GetActiveScene().name + "_cp", cpName);
            Debug.Log("Player hit " + cpName);
            
            AudioManager.instance.PlaySFX(1);
        }
    }
}
