using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForAudioSource : CustomYieldInstruction
{
	private AudioSource audioSource;

	public WaitForAudioSource(AudioSource audioSource)
	{
		this.audioSource = audioSource;
	}

	public override bool keepWaiting
	{
		get
		{
            if (audioSource == null)
                return false;

			return audioSource.isPlaying;
		}
	}
}
