using UnityEditor;
using UnityEngine;

namespace Scaffold
{
	// Dropdown menus such as File/Open and Edit/Undo are called "menu items."
	//
	// The MenuItem attribute allows you to create new buttons in your editor.
	// The function below adds a new button to open the game's save directory.

	public static class EditorMenuBarCommands
	{
		[MenuItem("Scaffold/Open Persistent Save Path")]
		static void OpenPersistentSave()
		{
			EditorUtility.RevealInFinder(Application.persistentDataPath);
		}
	}
}