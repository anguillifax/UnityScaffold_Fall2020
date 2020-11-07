using UnityEngine;

namespace Scaffold
{
	// Stores when a button was pressed.
	//
	// This class is necessary in a physics-based game. Input must be read in
	// Update() to prevent dropping keystrokes. However, input must be used in
	// FixedUpdate() to be compatible with the physics engine. This class
	// serves as the bridge between the two.
	//
	// This class abstracts the most bug-prone parts of working between two
	// different update loops.
	//
	// As a side benefit, it enhances user experience by making buffered inputs
	// the default.

	/// <summary>
	/// Stores a button press for a brief period of time.
	/// </summary>
	public class BufferedButtonPress
	{
		/// <summary>
		/// Maximum time a button can be pressed before it is needed and still
		/// register as pressed. This number must be large enough to account
		/// for the offset between Update() and FixedUpdate().
		/// </summary>
		private const float DefaultBufferDuration = 0.1f;

		private float timestamp;

		/// <summary>
		/// Create a new buffered input with no input stored.
		/// </summary>
		public BufferedButtonPress()
		{
			Clear();
		}

		/// <summary>
		/// Store the current time in the buffered input.
		/// 
		/// <para>
		/// This class stores the current unscaled delta time. Pauses and
		/// timescale changes do not affect how long a buffer is valid.
		/// </para>
		/// 
		/// </summary>
		public void Store()
		{
			timestamp = Time.unscaledTime;
		}

		/// <summary>
		/// Returns true if the buffer was written to less than maxBuffer
		/// seconds before this method was called. If there is no input stored,
		/// then this method will always return false.
		/// 
		/// <para>
		/// This class reads from the current unscaled total time. Pauses and
		/// timescale changes do not affect how long a buffer is valid.
		/// </para>
		/// 
		/// </summary>
		/// <param name="maxBuffer">The maximum time the buffer is valid for.</param>
		/// <returns>True if buffer is still valid.</returns>
		public bool Peek(float maxBuffer)
		{
			if (float.IsNaN(timestamp))
			{
				return false;
			}
			else
			{
				return (Time.unscaledTime - timestamp) <= maxBuffer;
			}
		}

		/// <summary>
		/// Returns true if the button was pressed recently. Leaves the buffer
		/// untouched.
		/// </summary>
		public bool Peek()
		{
			return Peek(DefaultBufferDuration);
		}

		/// <summary>
		/// Clears the input saved in the buffer.
		/// 
		/// <para>
		/// Until a new input is stored, reading from the buffer will always
		/// return false.
		/// </para>
		/// </summary>
		public void Clear()
		{
			timestamp = float.NaN;
		}

		/// <summary>
		/// Returns true if the button was cached less than maxBuffer seconds
		/// ago. After reading, the buffer is cleared.
		/// 
		/// <para>
		/// For details, see <see cref="Peek(float)"/> and <see cref="Clear"/>;
		/// </para>
		/// 
		/// </summary>
		/// <param name="maxBuffer">The maximum time the buffer is valid for</param>
		/// <returns>True if the buffer was still valid. False if the buffer expired.</returns>
		public bool PeekThenClear(float maxBuffer)
		{
			if (Peek(maxBuffer))
			{
				Clear();
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Return true if the button was pressed recently, then clear the
		/// buffer.
		/// </summary>
		public bool PeekThenClear()
		{
			return PeekThenClear(DefaultBufferDuration);
		}
	}
}