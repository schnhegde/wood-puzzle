using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlaneView : MonoBehaviour
{
	public enum State
	{
		Free,
		Drag
	}

	private sealed class _ScaleUpBlock_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		internal float _timer___0;

		internal float _newScale___1;

		internal Vector2 _currentMousePos___1;

		internal PlaneView _this;

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

		public _ScaleUpBlock_c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this._PC;
			this._PC = -1;
			switch (num)
			{
			case 0u:
				this._this.isScaling = true;
				this._timer___0 = 0f;
				break;
			case 1u:
				break;
			default:
				return false;
			}
			if (this._timer___0 < this._this.duration && this._this.listBlock.Count > 0)
			{
				this._timer___0 += Time.deltaTime;
				this._newScale___1 = Mathf.Lerp(this._this.scaleSmall, this._this.scale, this._timer___0 / this._this.duration);
				this._this.ScaleBlock(this._newScale___1);
				this._currentMousePos___1 = this._this.GetFixedMousePos();
				this._this.SetBlockPos(this._currentMousePos___1, this._newScale___1);
				this._current = null;
				if (!this._disposing)
				{
					this._PC = 1;
				}
				return true;
			}
			this._currentMousePos___1 = this._this.GetFixedMousePos();
			this._this.SetBlockPos(this._currentMousePos___1, this._this.scale);
			this._this.isScaling = false;
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

	public Camera camera;

	public float y;

	public float scale;

	public float scaleSmall;

	public float distanceTouch;

	public float duration;

	public Transform cam;

	private global::Types myType;

	public List<CubeUnit> listBlock;

	private Vector2[] listBlockLocalPos;

	private Vector3 touchPos;

	private Vector3 startPos;

	public int selected;

	private int row;

	private int col;

	private GroundView groundView;

	private Grid grid;

	private NextViewerControl nextViewerCtr;

	private PattemCreater pattemCreater;

	private IEnumerator ScaleUpAnim;

	private bool isScaling;

	private bool groundAcepted;

	private Vec2 cellAcepted = new Vec2();

	private Vec2 lastResetFillCel;

	public Vector2 faceMousePos;

	public bool isAuto;

	public float speedMoveDrop;

	public PlaneView.State state;

	private ColorControl colorCtr;

	private void Start()
	{
		this.listBlock = new List<CubeUnit>();
		this.row = MainObjControl.Instant.grid.numberRow;
		this.col = MainObjControl.Instant.grid.numberCol;
		this.groundView = MainObjControl.Instant.groundView;
		this.grid = MainObjControl.Instant.grid;
		this.nextViewerCtr = MainObjControl.Instant.nextViewerCtr;
		this.pattemCreater = MainObjControl.Instant.pattemCreater;
		this.colorCtr = MainObjControl.Instant.colorControl;
	}

	public void SetPattem(List<CubeUnit> listBlock0, Vector2 pos, int select, float viewScale)
	{
		this.state = PlaneView.State.Drag;
		this.selected = select;
		this.startPos = pos;
		this.groundAcepted = false;
		this.SetAllBlock();
		this.listBlockLocalPos = new Vector2[listBlock0.Count];
		for (int i = 0; i < listBlock0.Count; i++)
		{
			int indexRow = listBlock0[i].indexRow;
			int indexCol = listBlock0[i].indexCol;
			Vector2 vector = listBlock0[i].transform.position;
			this.listBlock.Add(this.pattemCreater.CreateABlock(vector, this.scale));
			this.listBlock[i].indexRow = indexRow;
			this.listBlock[i].indexCol = indexCol;
			this.listBlock[i].SetLayer(GameDefine.selectingLayer);
			this.listBlockLocalPos[i] = (vector - pos) / viewScale;
		}
		this.ScaleBlock(this.scaleSmall);
		this.SetBlockPos(this.GetFixedMousePos(), this.scaleSmall);
		if (this.ScaleUpAnim != null && this.isScaling)
		{
			base.StopCoroutine(this.ScaleUpAnim);
		}
		this.ScaleUpAnim = this.ScaleUpBlock();
		base.StartCoroutine(this.ScaleUpAnim);
		MainObjControl.Instant.helpCtr.HideAllBlock();
	}

	private IEnumerator ScaleUpBlock()
	{
		PlaneView._ScaleUpBlock_c__Iterator0 _ScaleUpBlock_c__Iterator = new PlaneView._ScaleUpBlock_c__Iterator0();
		_ScaleUpBlock_c__Iterator._this = this;
		return _ScaleUpBlock_c__Iterator;
	}

	private void Update()
	{
		this.Drag();
		if (this.isAuto)
		{
			return;
		}
		this.CheckPlace();
	}

	private void Drag()
	{
		if (this.state == PlaneView.State.Drag && this.listBlock.Count > 0 && !this.isScaling)
		{
			this.SetBlockPos(this.GetFixedMousePos(), this.scale);
			this.CheckGround();
		}
	}

	private void CheckPlace()
	{
		if (Input.GetMouseButtonUp(0) && this.listBlock.Count > 0)
		{
			this.PlacePattemGround();
		}
	}

	public void PlacePattemGround()
	{
		this.CheckSelectedBlock();
		this.SetAllBlock();
		this.groundView.HideFromBlock(0);
		MainObjControl.Instant.nextViewerCtr.CheckHelp();
		this.state = PlaneView.State.Free;
	}

	private void CheckSelectedBlock()
	{
		if (this.groundAcepted && !this.isScaling && this.CheckPlaceInTuto())
		{
			int r = this.cellAcepted.R;
			int c = this.cellAcepted.C;
			Vector2 b = new Vector2((float)c, (float)r);
			Vector2 a = this.listBlock[0].transform.position;
			float num = (a - b).magnitude / this.speedMoveDrop;
			num = Mathf.Min(num, 0.09f);
			List<CubeUnit> list = new List<CubeUnit>();
			for (int i = 0; i < this.listBlock.Count; i++)
			{
				int num2 = this.cellAcepted.R - (this.listBlock[0].indexRow - this.listBlock[i].indexRow);
				int num3 = this.cellAcepted.C - (this.listBlock[0].indexCol - this.listBlock[i].indexCol);
				Vector2 targetPos = new Vector2((float)num3, (float)num2);
				Vector2 pos = this.listBlock[i].transform.position;
				CubeUnit cubeUnit = this.pattemCreater.CreateABlock(pos, this.scale);
				this.grid.grid[num2, num3] = cubeUnit;
				cubeUnit.col = num3;
				cubeUnit.row = num2;
				list.Add(cubeUnit);
				cubeUnit.DropDown(pos, targetPos, num);
			}
			bool flag = false;
			this.grid.CheckGrid(list, num, ref flag);
			this.nextViewerCtr.listView[this.selected].SetAllBlock();
			this.nextViewerCtr.listView[this.selected].state = NextViewer.State.Null;
			if (MainState.typePlay == MainState.TypePlay.Tutorial)
			{
				MainObjControl.Instant.tutorial.Next();
			}
			else
			{
				MainObjControl.Instant.nextViewerCtr.CheckUpdateNewPattem();
			}
			if (flag)
			{
				MainAudio.Main.PlaySound(TypeAudio.SounCollect1);
			}
			else
			{
				MainAudio.Main.PlaySound(TypeAudio.SoundStop);
			}
		}
		else
		{
			this.nextViewerCtr.listView[this.selected].ShowAllBlock(this.listBlock);
			if (MainState.typePlay == MainState.TypePlay.Tutorial)
			{
				MainObjControl.Instant.tutorial.StartFinger();
				this.grid.TurnOffAllFillLine();
			}
		}
	}

	private bool CheckPlaceInTuto()
	{
		if (MainState.typePlay == MainState.TypePlay.Tutorial)
		{
			int indexRow = this.listBlock[0].indexRow;
			int indexCol = this.listBlock[0].indexCol;
			for (int i = 0; i < this.listBlock.Count; i++)
			{
				int r = this.cellAcepted.R - (indexRow - this.listBlock[i].indexRow);
				int c = this.cellAcepted.C - (indexCol - this.listBlock[i].indexCol);
				if (!this.grid.IsRowFillWith(r, c) && !this.grid.IsColFillWith(r, c))
				{
					return false;
				}
			}
		}
		return true;
	}

	private void CheckGround()
	{
		if (this.IsInvalidGrid())
		{
			this.groundAcepted = true;
			this.groundView.SetPattem(this.listBlock, this.cellAcepted.R, this.cellAcepted.C);
			if (this.lastResetFillCel == null || this.lastResetFillCel.R != this.cellAcepted.R || this.lastResetFillCel.C != this.cellAcepted.C)
			{
				for (int i = 0; i < this.listBlock.Count; i++)
				{
					this.listBlock[i].SetSprite(this.colorCtr.GetSpriteData(1), 1);
				}
				this.grid.TurnOffAllFillLine();
				this.grid.CheckGridFillTest(this.listBlock, this.cellAcepted.R, this.cellAcepted.C);
				this.lastResetFillCel = new Vec2(this.cellAcepted.R, this.cellAcepted.C);
			}
		}
		else
		{
			this.groundAcepted = false;
			this.grid.TurnOffAllFillLine();
			this.groundView.HideFromBlock(0);
			this.lastResetFillCel = null;
			for (int j = 0; j < this.listBlock.Count; j++)
			{
				if (this.listBlock[j].ID != 1)
				{
					this.listBlock[j].SetSprite(this.colorCtr.GetSpriteData(1), 1);
				}
			}
		}
	}

	private bool IsInvalidGrid()
	{
		Vector2 vector = this.listBlock[0].transform.position;
		this.cellAcepted.C = Mathf.RoundToInt(vector.x);
		this.cellAcepted.R = Mathf.RoundToInt(vector.y);
		return !this.grid.InvalidPoint(this.listBlock, this.cellAcepted.R, this.cellAcepted.C);
	}

	private Vector2 GetFixedMousePos()
	{
		if (!this.isAuto)
		{
			Vector2 result = this.camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			result = new Vector2(result.x, result.y + this.distanceTouch);
			return result;
		}
		return this.faceMousePos;
	}

	private void SetBlockPos(Vector3 pos, float newScale)
	{
		for (int i = 0; i < this.listBlock.Count; i++)
		{
			float x = pos.x + this.listBlockLocalPos[i].x * Mathf.Min(newScale, this.scale) / this.scale;
			float num = pos.y + this.listBlockLocalPos[i].y * Mathf.Min(newScale, this.scale) / this.scale;
			this.listBlock[i].transform.position = new Vector2(x, num);
		}
	}

	private void ScaleBlock(float newScale)
	{
		for (int i = 0; i < this.listBlock.Count; i++)
		{
			this.listBlock[i].transform.localScale = new Vector3(newScale, newScale, newScale);
		}
	}

	public void SetAllBlock()
	{
		for (int i = 0; i < this.listBlock.Count; i++)
		{
			this.listBlock[i].SetLayer(GameDefine.freeLayer);
			MainObjControl.Instant.pattemCreater.SetCube(this.listBlock[i]);
		}
		this.listBlock.Clear();
	}
}
