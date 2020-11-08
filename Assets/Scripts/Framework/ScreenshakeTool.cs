using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scaffold
{
	/// <summary>
	/// Allows fine-grained manual control over the current screenshake amount.
	/// Only necessary if screenshake effect is based on events instead of
	/// time.
	/// </summary>
	[Serializable]
	public class DirectScreenshakeEffect
	{
		[Min(0)] public float strength = 0.2f;
	}

	[Serializable]
	public class ScreenshakeTool
	{
		public float offsetAmount = 0.4f;
		public float rotationAmountDeg = 0.6f;
		public float power = 1.7f;
		public float maxStrength = 7;
		public float offsetNoiseSpeed = 16;
		public float rotationNoiseSpeed = 8;
		public bool logStrength;

		private Vector2 output;
		private float outputDeg;
		private readonly List<TimedAnimationCurve> timed = new List<TimedAnimationCurve>();
		private readonly HashSet<DirectScreenshakeEffect> directs = new HashSet<DirectScreenshakeEffect>();

		/// <summary>
		/// How far to offset the camera from its original position.
		/// </summary>
		public Vector2 CurrentOffset
		{
			get => output;
		}

		/// <summary>
		/// How far to rotate the camera from its original rotation.
		/// </summary>
		public float CurrentRotationDeg
		{
			get => outputDeg;
		}

		/// <summary>
		/// Add an object that manually controls how intense the screenshake
		/// is. When finished, remove the shake using <see
		/// cref="RemoveDirectShake(DirectScreenshakeEffect)"/> to prevent
		/// lingering effects.
		/// </summary>
		public void AddDirectShake(DirectScreenshakeEffect shake)
		{
			directs.Add(shake);
		}

		public void RemoveDirectShake(DirectScreenshakeEffect shake)
		{
			directs.Remove(shake);
		}

		/// <summary>
		/// Begins a new screenshake driven by the values of an animation curve.
		/// </summary>
		/// <param name="profile">Specifies intensity of the shake over time</param>
		public void AddTimedShake(AnimationCurve profile)
		{
			timed.Add(new TimedAnimationCurve(profile));
		}

		/// <summary>
		/// Remove all screenshake effects and reset offsets.
		/// </summary>
		public void ClearAllShakes()
		{
			timed.Clear();
			directs.Clear();
			output = Vector2.zero;
			outputDeg = 0;
		}

		private float GetPerlin(float x, float y)
		{
			return 2 * Mathf.PerlinNoise(x, y) - 1;
		}

		private float CalculateStrength()
		{
			float strength = 0f;
			strength += timed.Sum(x => x.Current);
			strength += directs.Sum(x => Mathf.Max(0, x.strength));
			if (strength > maxStrength)
			{
				strength = maxStrength;
			}

			if (logStrength)
			{
				Debug.Log("Strength: " + strength);
			}

			return strength;
		}

		/// <summary>
		/// Calculates the new screenshake offsets. This method must be called
		/// once per frame in Update() or LateUpdate().
		/// </summary>
		public void Update()
		{
			timed.ForEach(x => x.timer.Update(Time.deltaTime));
			timed.RemoveAll(x => x.timer.Done);

			float factor = Mathf.Pow(CalculateStrength(), power);

			output = factor * offsetAmount * new Vector2(
				GetPerlin(Time.time * offsetNoiseSpeed, 0),
				GetPerlin(0, Time.time * offsetNoiseSpeed));
			outputDeg = factor * rotationAmountDeg * GetPerlin(Time.time * rotationNoiseSpeed + 100, Time.time * rotationNoiseSpeed);
		}
	}
}