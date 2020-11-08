using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scaffold.Demo
{
	internal class DemoCameraController : MonoBehaviour
	{
		public static DemoCameraController instance;

		public ScreenshakeTool screenshake = new ScreenshakeTool();
		public Transform trackTarget = default;

		private Vector2 currentBasePos;

		/// <summary>
		/// Sets the current camera position in the XY plane without affecting
		/// the z offset.
		/// </summary>
		public Vector2 Position
		{
			get => transform.position;
			set
			{
				transform.position = new Vector3(value.x, value.y, transform.position.z);
			}
		}

		/// <summary>
		/// Sets the current camera rotation in degrees.
		/// </summary>
		public float RotationDeg
		{
			get => transform.eulerAngles.z;
			set
			{
				transform.eulerAngles = new Vector3(0, 0, value);
			}
		}

		private void Awake()
		{
			instance = this;
		}

		private void OnEnable()
		{
			if (trackTarget != null)
			{
				currentBasePos = trackTarget.position;
			}
		}

		private void LateUpdate()
		{
			// If there is no assigned object to follow, try and follow the
			// player.
			if (trackTarget == null)
			{
				var player = GameObject.FindWithTag("Player");
				if (player != null)
				{
					trackTarget = player.transform;
				}
			}

			if (trackTarget != null)
			{
				currentBasePos = trackTarget.position;
			}

			screenshake.Update();

			Position = currentBasePos + screenshake.CurrentOffset;
			RotationDeg = screenshake.CurrentRotationDeg;
		}
	}
}