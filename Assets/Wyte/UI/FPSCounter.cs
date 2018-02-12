using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FPSCounter : SingletonBaseBehaviour<FPSCounter>
{

	int cnt;
	float time;

	StringBuilder debugTexts;

	public delegate void DebugRenderingEventHandler(StringBuilder debugTexts);
	public event DebugRenderingEventHandler DebugRendering;

	bool isDebugMode;
	int fps;

	protected override void Awake()
	{
		base.Awake();
		debugTexts = new StringBuilder();
	}

	// Use this for initialization
	void Start()
	{
		DebugRendering += (d) =>
		{
			d.Append(FpsString);
		};
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F3))
			isDebugMode = !isDebugMode;

		string output = "";
		if (isDebugMode)
		{
			debugTexts.Clear();
			DebugRendering?.Invoke(debugTexts);
			output = debugTexts.ToString();
		}
		else
		{
			output = FpsString;
		}
		GetComponent<Text>().text = output;

		if (time > 1)
		{
			fps = cnt;
			cnt = 0;
			time = 0;
			return;
		}
		cnt++;
		time += Time.deltaTime;
	}

	string FpsString => $"fps{fps} ";
}
