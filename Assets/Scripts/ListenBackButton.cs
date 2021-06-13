using System;
using UnityEngine;

public class ListenBackButton : MonoBehaviour
{
	private float time = -1f;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && Time.time - this.time > 0.2f)
		{
			switch (MainState.GetState)
			{
			case MainState.State.Home:
				this.HomeBack();
				break;
			case MainState.State.Ingame:
				this.IngameBack();
				break;
			case MainState.State.GameOver:
				this.GameOverBack();
				break;
			case MainState.State.Pause:
				this.PauseBack();
				break;
			}
			this.time = Time.time;
		}
	}

	private void HomeBack()
	{
		Application.Quit();
	}

	private void IngameBack()
	{
		MainCanvas.Main.pauseScript.PauseGame();
	}

	private void GameOverBack()
	{
		MainCanvas.Main.lostScript.TryAgainButton();
	}

	private void PauseBack()
	{
		MainCanvas.Main.pauseScript.UnPause();
	}
}
