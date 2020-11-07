using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scaffold
{
	// Counterpart to the TimeManager interface class. See TimeManager for
	// explanation.

	internal class TimeManagerImpl : MonoBehaviour
	{
		[Serializable]
		public class AnimationTimerPair
		{
			public AnimationCurve curve;
			public ManualTimer timer;
		}

		public List<AnimationTimerPair> dilations = new List<AnimationTimerPair>();
		public bool paused;

		private void Awake()
		{
			TimeManager.impl = this;
			SceneManager.sceneLoaded += OnSceneLoad;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoad;
		}

		private void OnSceneLoad(Scene scene, LoadSceneMode mode)
		{
			// Remove any effects when changing to a new scene.
			if (mode == LoadSceneMode.Single)
			{
				ResetEffects();
			}
		}

		private void UpdateTimescale()
		{
			if (paused)
			{
				Time.timeScale = 0;
			}
			else
			{
				float value = 1f;
				foreach (var x in dilations)
				{
					value *= x.curve.Evaluate(x.timer.ElapsedTime);
				}
				Time.timeScale = value;
			}
		}

		private void Update()
		{
			dilations.ForEach(x => x.timer.Update(Time.unscaledDeltaTime));
			dilations.RemoveAll(x => x.timer.Done);
			UpdateTimescale();
		}

		// Internal Interface Implementation

		internal void BeginDilation(AnimationCurve curve)
		{
			dilations.Add(new AnimationTimerPair()
			{
				curve = curve,
				timer = new ManualTimer(curve.keys[curve.length - 1].time)
			});
		}

		internal void ResetEffects()
		{
			dilations.Clear();
			UpdateTimescale();
		}

		internal void SetPaused(bool value)
		{
			if (paused != value)
			{
				paused = value;
				UpdateTimescale();
				TimeManager.RaisePauseEvent();
			}
		}
	}
}