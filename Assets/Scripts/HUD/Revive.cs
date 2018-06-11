using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : MonoBehaviour {

    public static bool dead = false;
    public GameObject reviveUI;
    public GameObject dieUI;
    public GameObject player;
    private MainCharacterScript mainCharacterScript;
    
    // Use this for initialization
	void Start () {
        mainCharacterScript = player.GetComponent<MainCharacterScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (dead == true)
        {
            reviveUI.SetActive(true);
        }
        else
        {
            reviveUI.SetActive(false);
        }
	}

    public void OnDie()
    {
        if (mainCharacterScript.life > 0)
        {
            dead = true;
        }
        else
        {
            dieUI.SetActive(true);
            Time.timeScale = 0f;
            return;
        }
    }

    public void OnRevive()
    {
        dead = false;
    }
}
