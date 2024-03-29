﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
	public enum ESfxType
	{
		CUT_CHANGE = 0,
		STAGE_CLEAR = 1,
		DRAG = 2,
		END_DRAG = 3,
	}

	[SerializeField] private float sceneChangeTime = 3f;
	[SerializeField] private AudioSource bgmAudioSource;

	[Header("SFX Audio Sources")] [SerializeField]
	private AudioSource cutChangeAudioSource;

	[SerializeField] private AudioSource stageClearAudioSource;
	[SerializeField] private AudioSource dragAudioSource;

	[Header("Volume")] [SerializeField] [Range(0.0f, 1.0f)]
	private float sfxVolume = .1f;

	[SerializeField] [Range(0.0f, 1.0f)] private float bgmVolume = .3f;

	private List<AudioSource> sfxAudioSources = new List<AudioSource>();

	public void PlayCutChangeAudio() => cutChangeAudioSource.Play();
	public void FadingAudio(bool fade) => StartCoroutine(FadeAudio(fade));

	private void Start()
	{
		sfxAudioSources.AddRange(transform.GetComponentsInChildren<AudioSource>());

		if (!bgmAudioSource)
		{
			bgmAudioSource = this.GetComponent<AudioSource>();
		}
	}

	public void AudioInit()
	{
		bgmAudioSource.volume = 0.1f;
		this.SetSfxVolume(sfxVolume);
		StartCoroutine(FadeAudio(true));
	}

	/// <summary>
	/// Set BGM Volume, vol is float, between 0 to 1 
	/// </summary>
	/// <param name="vol"></param>
	public void SetBgmVolume(float vol)
	{
		bgmVolume = vol;

		bgmAudioSource.volume = vol;
	}

	/// <summary>
	/// Set SFX Volume, vol is float, between 0 to 1
	/// </summary>
	/// <param name="vol"></param>
	public void SetSfxVolume(float vol)
	{
		sfxVolume = vol;

		foreach (var source in sfxAudioSources)
		{
			source.volume = vol;
		}
	}

	public void PlayStageClearAudio()
	{
		stageClearAudioSource.Play();
		StartCoroutine(FadeAudio(false));
	}

	/// <summary>
	/// type is AudioManager.SfxType, play the type of sound
	/// </summary>
	/// <param name="type"></param>
	public void PlaySfx(ESfxType type)
	{
		switch (type)
		{
			case ESfxType.STAGE_CLEAR:
				stageClearAudioSource.Play();
				StartCoroutine(FadeAudio(false));
				break;

			case ESfxType.CUT_CHANGE:
				cutChangeAudioSource.Play();
				break;

			case ESfxType.DRAG:
				dragAudioSource.loop = true;
				if (!dragAudioSource.isPlaying)
					dragAudioSource.Play();
				break;

			case ESfxType.END_DRAG:
				dragAudioSource.Stop();
				break;

			default:
				break;
		}
	}

	private IEnumerator FadeAudio(bool fade)
	{
		float dirTime = Time.time + sceneChangeTime;

		while (Time.time < dirTime)
		{
			bgmAudioSource.volume += (fade) ? 0.01f : -0.018f;

			if (fade)
			{
				if (bgmAudioSource.volume > bgmVolume)
					StopCoroutine(nameof(FadeAudio));
			}

			yield return new WaitForSeconds(0.1f);
		}
	}
}