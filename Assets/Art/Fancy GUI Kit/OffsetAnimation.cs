using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class OffsetAnimation : MonoBehaviour {

	public Animator[] controllers;

	public float delay, startDelay;
	// Use this for initialization
	void Start () 
	{
		controllers = GetComponentsInChildren<Animator>();

        //controllers = controllers.Where(x => x.enabled = false)as Animator[];

        for (int i = 0; i < controllers.Length; i++)
        {
			controllers[i].enabled = false;
			controllers[i].gameObject.SetActive(false);
		}

		StartCoroutine(OffsetTime());
	}

	IEnumerator OffsetTime()
    {
		yield return new WaitForSeconds(startDelay);
		for (int i = 0; i < controllers.Length; i++)
        {
			controllers[i].enabled = true;
			controllers[i].gameObject.SetActive(true);
			yield return new WaitForSeconds(delay);
        }
    }

}
