using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Novel.Exceptions;

[System.Serializable]
public struct MusicData
{
	[SerializeField]
	private string id;
	[SerializeField]
	private AudioClip clip;
	[SerializeField]
	private bool loop;

	public string Id => id;
	public AudioClip Clip => clip;
	public bool Loop => loop;
}

[RequireComponent(typeof(AudioSource))]
public class MusicManager : SingletonBaseBehaviour<MusicManager>
{
	public MusicData[] Songs;

	AudioSource source;

	protected override void Awake()
	{
		base.Awake();
		source = GetComponent<AudioSource>();
	}

	void Start()
	{
	}

	public void Play(string id)
	{
		var targetSong = Songs.FirstOrDefault(m => m.Id == id);
		Stop();
		source.loop = targetSong.Loop;
		source.clip = targetSong.Clip;
		source.volume = 1;
		source.Play();
	}

	public void Play(int id)
	{
		if (Songs.Length <= id)
		{
			Debug.LogWarning($"Music ID {id} is not found. This command will be ignored.");
			return;
		}
		Play(Songs[id].Id);
	}

	public IEnumerator Play(string t, string[] a)
	{
		Play(UnityNRuntime.CombineAll(a));
		yield break;
	}

	public IEnumerator Stop(string t, string[] a)
	{
		if (a.Length == 0)
		{
			Stop();
		}
		else
		{
			float i;
			if (!float.TryParse(UnityNRuntime.CombineAll(a), out i))
				throw new NRuntimeException("時間が不正です。");
			yield return Stop(i);
		}
	}

	public IEnumerator StopAsync(string t, string[] a)
	{
		StartCoroutine(Stop(t, a));
		yield break;
	}

	public void Stop()
	{
		source.Stop();
	}

	/// <summary>
	/// フェードアウトしながら音楽を停止します。
	/// </summary>
	/// <param name="time">フェードアウトを行う秒単位の時間。</param>
	public IEnumerator Stop(float time)
	{
		float t = 0;
		while (t < time)
		{
			t += Time.deltaTime;
			// (time - t) / time = 1 - (t / time)
			source.volume = 1 - (t / time);
			yield return null;
		}
		Stop();
		source.volume = 1;
	}
}
