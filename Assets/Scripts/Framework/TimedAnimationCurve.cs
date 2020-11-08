using UnityEngine;

namespace Scaffold
{
	// Evaluates a curve using a timer.
	//
	// This helper class is used in a couple framework classes and exists for
	// convenience.

	public class TimedAnimationCurve
	{
		public readonly AnimationCurve curve;
		public readonly ManualTimer timer;

		public float Current => curve.Evaluate(timer.ElapsedTime);

		public TimedAnimationCurve(AnimationCurve curve)
		{
			this.curve = curve;
			timer = new ManualTimer(curve.keys[curve.length - 1].time);
			timer.Start();
		}
	}
}