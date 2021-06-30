using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InGameScript : MonoBehaviour
{
	private sealed class _ScoreUpAnim_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		internal int fromScore;

		internal int _unitPlus___0;

		internal InGameScript _this;

		internal object _current;

		internal bool _disposing;

		internal int _PC;

		object IEnumerator<object>.Current
		{
			get
			{
				return this._current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._current;
			}
		}

		public _ScoreUpAnim_c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this._PC;
			this._PC = -1;
			switch (num)
			{
			case 0u:
				this._this.scoreRuning = true;
				this._unitPlus___0 = Mathf.Max(1, (this._this.scoreInt - this.fromScore) / 12);
				this._current = new WaitForSeconds(0.2f);
				if (!this._disposing)
				{
					this._PC = 1;
				}
				return true;
			case 1u:
				break;
			case 2u:
				this.fromScore = Mathf.Min(this._this.scoreInt, this.fromScore + this._unitPlus___0);
				this._this.scoreTxt.text = this.fromScore.ToString();
				if (this.fromScore > this._this.bestInt)
				{
					this._this.bestInt = this.fromScore;
					this._this.bestTxt.text = this._this.bestInt.ToString();
				}
				break;
			default:
				return false;
			}
			if (this.fromScore < this._this.scoreInt)
			{
				this._current = new WaitForSeconds(0.08f);
				if (!this._disposing)
				{
					this._PC = 2;
				}
				return true;
			}
			this._this.scoreTxt.text = this._this.scoreInt.ToString();
			if (this._this.bestInt > this._this.lastBest)
			{
				Settings.SetBest(this._this.bestInt);
				this._this.lastBest = this._this.bestInt;
				this._this.isOverBest = true;
			}
			this._this.scoreRuning = false;
			this._PC = -1;
			return false;
		}

		public void Dispose()
		{
			this._disposing = true;
			this._PC = -1;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	public RectTransform rec;

	public GameObject cup;

	public Text scoreTxt;

	public Text multiplierTxt;

	public Text bestTxt;

	public int scoreInt;

	public int bestInt;

	public int lastBest;

	public CanvasGroup group;

	public GameObject bgGrid;

	public bool isOverBest;

	private bool scoreRuning;

	private IEnumerator scoreUp;

	public void Preload()
	{
		this.SetActive(false);
	}

	public void Reset()
	{
		this.ResetScore();
		this.bestInt = Settings.GetBest;
		this.lastBest = this.bestInt;
		this.bestTxt.text = this.bestInt.ToString();
		this.scoreInt = 0;
		//resetScoreMultiplier();
		this.isOverBest = false;
	}

	public void ResetScore()
	{
		if (this.scoreRuning)
		{
			base.StopCoroutine(this.scoreUp);
		}
		this.scoreInt = 0;
		this.scoreTxt.text = this.scoreInt.ToString();
	}

	public void ResetBest()
	{
		this.bestInt = 0;
		Settings.SetBest(0);
	}

	public void SetActive(bool isActive)
	{
		base.gameObject.SetActive(isActive);
	}

	public void Pause()
	{
		MainCanvas.Main.pauseScript.PauseGame();
	}

	public void SetNewScore(int scoreFillLine, int numberUnit)
	{
		int value = scoreFillLine + numberUnit;
		this.UpScoreTxt(value);
	}

	public void setNewMulitplier(int multiplier)
    {
		string multTxt = "x" + multiplier;
		multiplierTxt.text = multTxt;
		multiplierTxt.GetComponent<Animator>().Play("ani_combo");
	}

	public void resetMultiplier()
	{
		multiplierTxt.GetComponent<Animator>().Play("ani_combo_collapse");
	}

	public void SetScoreContinue(int value)
	{
		this.scoreInt = value;
		this.scoreTxt.text = this.scoreInt.ToString();
	}

	public void UpScoreTxt(int value)
	{
		if (value > 0)
		{
			int fromScore = this.scoreInt;
			this.scoreInt += value;
			if (this.scoreRuning)
			{
				base.StopCoroutine(this.scoreUp);
				this.scoreTxt.text = this.scoreInt.ToString();
			}
			this.scoreUp = this.ScoreUpAnim(fromScore);
			base.StartCoroutine(this.scoreUp);
		}
	}

	private IEnumerator ScoreUpAnim(int fromScore)
	{
		InGameScript._ScoreUpAnim_c__Iterator0 _ScoreUpAnim_c__Iterator = new InGameScript._ScoreUpAnim_c__Iterator0();
		_ScoreUpAnim_c__Iterator.fromScore = fromScore;
		_ScoreUpAnim_c__Iterator._this = this;
		return _ScoreUpAnim_c__Iterator;
	}
}
