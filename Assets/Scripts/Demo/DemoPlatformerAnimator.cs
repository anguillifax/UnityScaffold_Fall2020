using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scaffold.Demo
{
	internal class DemoPlatformerAnimator : MonoBehaviour
	{
		private SpriteRenderer sr;
		private Animator anim;

		public GameObject diePrefab = default;

		private void Awake()
		{
			sr = GetComponent<SpriteRenderer>();
			anim = GetComponent<Animator>();
		}

		public void PlayClip(DemoPlatformerAnimationId clip, bool facingRight)
		{
			anim.Play(clip.ToString());
			sr.flipX = !facingRight;
		}

		public void PlayDieAnimation()
		{
			Instantiate(diePrefab, transform.position, transform.rotation);
		}
	}
}