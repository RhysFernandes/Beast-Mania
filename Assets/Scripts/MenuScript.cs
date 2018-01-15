using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	public Canvas quitMenu;
	public Button startGame;
	public Button exitGame;

	// Use this for initialization
	void Start () {
		quitMenu = quitMenu.GetComponent<Canvas> ();
		startGame = startGame.GetComponent<Button> ();
		exitGame = exitGame.GetComponent<Button> ();
		quitMenu.enabled = false;
	}

	public void ExitPress(){
		quitMenu.enabled = true;
		startGame.enabled = false;
		exitGame.enabled = false;
	}

	public void NoPress(){
		quitMenu.enabled = false;
		startGame.enabled = true;
		exitGame.enabled = true;
	}

	public void startLevel(){
		SceneManager.LoadScene (1);
	}

	public void exit(){
		Application.Quit ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
