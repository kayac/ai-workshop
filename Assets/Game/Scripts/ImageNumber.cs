using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageNumber : MonoBehaviour
{

	[SerializeField]
	private List<Sprite> _sprites = new List<Sprite>();
	
	private List<Image> _images = new List<Image>();

	public int number;

	private int _numberInternal;

	void OnValidate()
	{
		UpdateNumber();
	}

	void Awake()
	{
		_images = new List<Image>(gameObject.GetComponentsInChildren<Image>());
	}

	void Update()
	{
		UpdateNumber();
	}

	public void UpdateNumber()
	{
		if (_sprites.Count < 10 || _sprites == null|| _numberInternal == number)
		{
			return;
		}

		var str = number.ToString();

		while(_images.Count < str.Length)
		{
			GenerateImage();
		}

		for (int i = 0; i < _images.Count; i++)
		{
			
			try
			{
				var image = _images[i];
				image.gameObject.SetActive(i < str.Length);
				if (str.Length <= i)
				{
					continue;
				}

				var s = str.Substring(i, 1);
				var n = int.Parse(s);
				
				image.sprite = _sprites[n];
				image.transform.localScale = Vector3.one;
			}
			catch(System.NullReferenceException e)
			{
				Clear();
				UpdateNumber();
			}
			catch(MissingReferenceException e)
			{
				Clear();
				UpdateNumber();
			}
		}

		_numberInternal = number;
	}

	private Image GenerateImage()
	{
		var go = new GameObject();
		var image = go.AddComponent<Image>();
		image.transform.parent = transform;
		image.name = "Image";
		_images.Add(image);
		return image;
	}

	[ContextMenu("Clear")]
	private void Clear()
	{
		foreach (var image in _images)
		{
			if (image == null) continue;
			DestroyImmediate(image.gameObject);
		}

		_images.Clear();
	}
}
