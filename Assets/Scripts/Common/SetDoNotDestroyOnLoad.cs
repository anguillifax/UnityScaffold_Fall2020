using UnityEngine;

namespace Scaffold
{
	// Objects marked as DoNotDestroyOnLoad are preserved between scenes.
	//
	// Most framework objects should have this script attached.

	/// <summary>
	/// Sets the current game object as DoNotDestroyOnLoad.
	/// </summary>
	public class SetDoNotDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}