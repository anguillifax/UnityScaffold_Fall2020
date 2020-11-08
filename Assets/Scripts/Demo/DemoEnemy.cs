using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scaffold.Demo
{
	internal class DemoEnemy : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D col)
		{
			IDemoPlayerDamageTarget iface = col.GetComponent<IDemoPlayerDamageTarget>();
			if (iface != null)
			{
				iface.Attack();
			}
		}
	}
}