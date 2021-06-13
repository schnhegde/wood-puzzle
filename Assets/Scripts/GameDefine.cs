using System;
using UnityEngine;

public class GameDefine
{
	public static Vector2 posRight = new Vector2(2000f, 0f);

	public static Vector2 posLeft = new Vector2(-2000f, 0f);

	public static Vector2 posTop = new Vector2(0f, 2000f);

	public static Vector2 posCenter = new Vector2(0f, 0f);

	public static float pikachuNormalScale = 0.48f;

	public static float pikachuYScale = 0.15f;

	public static string pageID = "1384190174944235";

	public static string pageName = "Connect Animal Game";

	public static int startRow = 16;

	public static int startCol = 4;

	public static int selectingLayer = 5;

	public static int freeLayer = 0;

	public static float pattemDarkAlpha = 0.38f;

	public static float pattemLightAlpha = 1f;

	public static int GetScore(int numberLine)
	{
		switch (numberLine)
		{
		case 0:
			return 0;
		case 1:
			return 10;
		case 2:
			return 30;
		case 3:
			return 60;
		case 4:
			return 100;
		case 5:
			return 150;
		case 6:
			return 250;
		case 7:
			return 500;
		default:
			return 1000;
		}
	}
}
