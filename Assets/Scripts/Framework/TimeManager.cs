using System;
using UnityEngine;

namespace Scaffold
{
	// TimeManager demonstrates a simple, effective design pattern to create
	// application-wide tools with an intuitive interfaces.
	//
	// This class serves as the public interface. It contains all the methods
	// and properties necessary to interact with the system.
	//
	// The implementation is hidden inside a MonoBehaviour marked as internal.
	// The MonoBehaviour provides the actual logic necessary to implement the
	// interface. It is usually attached to a game object marked as
	// DoNotDestroyOnLoad.
	//
	// To link the two together, a static variable marked as internal is placed
	// in the interface class. The interface uses this reference to forward all
	// calls directly to the implementation, where they can be handled
	// properly.

	/// <summary>
	/// Allows manual control over the current timescale.
	/// </summary>
	[Serializable]
	public class DirectTimeDilation
	{
		[Min(0)] public float factor = 0.7f;
	}

	public static class TimeManager
	{
		// References to the MonoBehaviour that provides the implementation of
		// the class. Hidden from end users via internal keyword.
		internal static TimeManagerImpl impl;

		/// <summary>
		/// Raised when the game is paused or unpaused. To check if the game is
		/// paused, use <see cref="Paused"/>.
		/// </summary>
		public static event Action PauseChanged;

		/// <summary>
		/// True if the game is paused.
		/// </summary>
		public static bool Paused
		{
			get => impl.paused;
			set => impl.SetPaused(value);
		}

		/// <summary>
		/// Begins a new effect. The duration of the effect is equivalent to
		/// the duration of the animation curve.
		/// </summary>
		public static void AddTimedDilation(AnimationCurve dilation)
		{
			impl.BeginDilation(dilation);
		}

		public static void AddDirectDilation(DirectTimeDilation dilation)
		{
			impl.AddDilation(dilation);
		}

		public static void RemoveDirectDilation(DirectTimeDilation dilation)
		{
			impl.RemoveDilation(dilation);
		}

		/// <summary>
		/// Removes all time-based effects. Game remains in previous pause
		/// state.
		/// </summary>
		public static void ResetAllEffects()
		{
			impl.ResetEffects();
		}

		// C# events can only be invoked from a method inside the parent class.
		// This function allows the implementation to remotely invoke the
		// event.
		internal static void RaisePauseEvent()
		{
			// Standard, safe approach to invoking an event.
			var cpy = PauseChanged;
			cpy?.Invoke();
		}
	}
}