using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float waitAfterDeath = 2f;
    [HideInInspector] public bool ending;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void PlayerDied()
    {
        Debug.Log("Player died");
        StartCoroutine(PlayerDiedCoroutine());
    }

    public IEnumerator PlayerDiedCoroutine()
    {
        yield return new WaitForSeconds(waitAfterDeath);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void PauseUnpause()
    {
        if (UIController.instance.pauseScreen.activeInHierarchy)
        {
            UIController.instance.pauseScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            PlayerController.instance.footstepSlow.Play();
            PlayerController.instance.footstepFast.Play();
        }
        else
        {
            UIController.instance.pauseScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            PlayerController.instance.footstepSlow.Stop();
            PlayerController.instance.footstepFast.Stop();

        }
    }
}
