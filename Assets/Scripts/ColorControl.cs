using System;
using System.IO;
using UnityEngine;

public class ColorControl : MonoBehaviour
{
	public Sprite[] preloadSprite;

	public int preload;

	public Texture2D spriteTexs;

	public int width;

	public int height;

	public Color whiteAlpha;

	public int space;

	private void Awake()
	{
		this.Load();
	}

	private void CreateAliasBlock()
	{
		Texture2D texture2D = new Texture2D(116 * this.preload + 116, 116);
		for (int i = 0; i < this.preload + 1; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				for (int k = 0; k < this.height; k++)
				{
					Color pixel = this.spriteTexs.GetPixel(j, k);
					float b = 0.2126f * pixel.r + 0.7152f * pixel.g + 0.0722f * pixel.b;
					Color color = new Color(Mathf.Lerp(pixel.r, b, (float)i / (float)this.preload), Mathf.Lerp(pixel.g, b, (float)i / (float)this.preload), Mathf.Lerp(pixel.b, b, (float)i / (float)this.preload), pixel.a);
					if (i == 0)
					{
						texture2D.SetPixel(j + i * this.width, k, pixel + new Color(0.12f, 0.12f, 0.12f, 0f));
					}
					else if (i == 1)
					{
						texture2D.SetPixel(j + i * this.width, k, pixel);
					}
					else
					{
						texture2D.SetPixel(j + i * this.width, k, color);
					}
				}
			}
		}
		texture2D.Apply();
		this.SaveTextureToFile(texture2D, "BlockAlias");
	}

	private void SaveTextureToFile(Texture2D texture, string name)
	{
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/" + name + ".png", bytes);
	}

	private void Load()
	{
		// String blockAlias = "BlockAlias_" + UnityEngine.Random.Range(1, 4);
		String blockAlias = "BlockAlias_1";
		this.preloadSprite = Resources.LoadAll<Sprite>(blockAlias);
	}

	public Sprite GetSpriteGray(float t)
	{
		int a = Mathf.Min(Mathf.RoundToInt(t * (float)this.preload), this.preload - 1);
		return this.preloadSprite[Mathf.Max(a, 1)];
	}

	public Sprite GetSpriteData(int index)
	{
		return this.preloadSprite[index];
	}
}
