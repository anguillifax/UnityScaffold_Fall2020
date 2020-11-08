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

		public List<TimedAnimationCurve> timed = new List<TimedAnimationCurve>();
		public List<DirectTimeDilation> directs = new List<DirectTimeDilation>();
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
				foreach (var x in timed)
				{
					value *= x.Current;
				}
				foreach (var x in directs)
				{
					value *= x.factor;
				}
				Time.timeScale = value;
			}
		}

		private void Update()
		{
			timed.ForEach(x => x.timer.Update(Time.unscaledDeltaTime));
			timed.RemoveAll(x => x.timer.Done);
			UpdateTimescale();
		}

		// Internal Interface Implementation

		internal void BeginDilation(AnimationCurve curve)
		{
			timed.Add(new TimedAnimationCurve(curve));
		}

		internal void AddDilation(DirectTimeDilation dilation)
		{
			directs.Add(dilation);
		}

		internal void RemoveDilation(DirectTimeDilation dilation)
		{
			directs.Remove(dilation);
		}

		internal void ResetEffects()
		{
			timed.Clear();
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