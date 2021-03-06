using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

[DefaultExecutionOrder(-1000)]
public class GamePadBehaviour : SingletonBaseBehaviour<GamePadBehaviour>
{
	[SerializeField]
	RectTransform left;
	[SerializeField]
	RectTransform right;
	[SerializeField]
	RectTransform action;
	[SerializeField]
	RectTransform menu;
	[SerializeField]
	RectTransform sliderHandle;
	[SerializeField]
	RectTransform itemSub1;
	[SerializeField]
	RectTransform itemSub2;
	[SerializeField]
	RectTransform item;
	[SerializeField]
	RectTransform escape;

	[SerializeField]
	Image sliderFill;

	[SerializeField]
	Sprite sliderFillDisabled;
	[SerializeField]
	Sprite sliderFillEnabled;

	public bool ShotToggled { get; set; }

	float baseYPos;

	float sliderDisabledPos;

	// Use this for initialization
	void Start()
	{
		if (!IsSmartDevice)
			gameObject.SetActive(false);
		Input.multiTouchEnabled = true;
		Input.simulateMouseWithTouches = true;
		baseYPos = left.anchoredPosition.y;
		sliderDisabledPos = sliderHandle.anchoredPosition.x;
	}

	// Update is called once per frame
	void Update()
	{
		ButtonAnimation(left, GamePadButtons.Left);
		ButtonAnimation(right, GamePadButtons.Right);
		ButtonAnimation(action, GamePadButtons.Action);

		// トグル
		ShotToggled ^= Get(GamePadButtons.Slider, true);

		// トグル表示
		if (ShotToggled)
		{
			sliderFill.sprite = sliderFillEnabled;
			sliderHandle.anchoredPosition = new Vector2(sliderDisabledPos + 16, sliderHandle.anchoredPosition.y);
		}
		else
		{
			sliderFill.sprite = sliderFillDisabled;
			sliderHandle.anchoredPosition = new Vector2(sliderDisabledPos, sliderHandle.anchoredPosition.y);
		}
	}

	public void ButtonAnimation(RectTransform r, GamePadButtons judgement)
	{
		r.anchoredPosition = new Vector2(r.anchoredPosition.x, baseYPos - (Get(judgement) ? 2 : 0));
	}

	public bool Get(GamePadButtons gpb, bool down = false)
	{
		foreach (var t in Input.touches)
		{
			// down = true の場合，タッチ開始以外のフレームは無視
			if (down && t.phase != TouchPhase.Began)
				continue;

			var rt = default(RectTransform);

			switch (gpb)
			{
				case GamePadButtons.Left:
					rt = left;
					break;
				case GamePadButtons.Right:
					rt = right;
					break;
				case GamePadButtons.Action:
					rt = action;
					break;
				case GamePadButtons.Slider:
					rt = sliderHandle;
					break;
				case GamePadButtons.Menu:
					rt = menu;
					break;
				case GamePadButtons.Escape:
					rt = escape;
					break;
				case GamePadButtons.ItemFrameSub1:
					rt = itemSub1;
					break;
				case GamePadButtons.ItemFrameSub2:
					rt = itemSub2;
					break;
				case GamePadButtons.ItemFrame:
					rt = item;
					break;
				case GamePadButtons.Screen:
					// パッドに触れていればtrue => falseならスクリーンに触れている => notをとる
					if (!GetComponent<RectTransform>().Overlaps(t.position)) return true;
					break;
				default:
					throw new ArgumentException("予期しないボタンの判定を試みました。");
			}

			if (rt == null)
				return false;

			if (rt.Overlaps(t.position))
				return true;
		}
		return false;
	}

}


public enum GamePadButtons
{
	Left,
	Right,
	Action,
	Menu,
	ItemFrameSub1,
	ItemFrameSub2,
	ItemFrame,
	Slider,
	Escape,
	Screen
}



static class Extension
{
	public static bool Overlaps(this RectTransform r, Vector2 point)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(r, point);
		//return (rect.xMin < point.x) && (point.x < rect.xMax) && (rect.yMin < point.y) && (point.y < rect.yMax);
	}
}
