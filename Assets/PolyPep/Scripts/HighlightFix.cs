﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
	public RectTransform myRT;
	public Vector3 myStartScale;
	public Vector3 myTargetScale;
	public Vector3 myCurrentScale;

	public Toggle myToggle;

	public float selectScaleFactor = 1.45f;
	public float toggleOnScaleFactor = 1.3f;

	public Color normalColor;

	void Start()
	{
		// some very brittle code here
		// makes assumptions about how UI is set up

		// a slider
		myRT = this.transform.Find("Slide_Area/Handle") as RectTransform;

		if (myRT)
		{
			//Debug.Log("-> Slider");
		}

		if (!myRT)
		{
			// a button
			myRT = this.transform.Find("Background") as RectTransform;
			selectScaleFactor = 1.25f;
		}

		if (!myRT)
		{
			// a toggle
			myRT = this.transform as RectTransform;
		}

		if (!myRT)
		{
			Debug.Log("---> Failed to find Rect Transform for UI Element");
		}

		myStartScale = myRT.localScale;
		myTargetScale = myStartScale;
		myCurrentScale = myStartScale;

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//if (!EventSystem.current.alreadySelecting)
		//	EventSystem.current.SetSelectedGameObject(this.gameObject);
		//Debug.Log("highlight fix enter");

		//if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
			//Debug.Log(myRT.localScale);

			myTargetScale.x = myStartScale.x * selectScaleFactor;
			myTargetScale.y = myStartScale.y * selectScaleFactor;
			myTargetScale.z = 1f;
		}
		//myRT.localScale = myTargetScale;
		//myCurrentScale = myTargetScale;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		//Debug.Log("highlight fix leave");
		//if (EventSystem.current.alreadySelecting)

		//{
		//	Debug.Log("1");

		//}

		if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
			//Debug.Log("2");
			EventSystem.current.SetSelectedGameObject(null);
		}

		{
			myTargetScale = myStartScale;
			if (myToggle)
			{
				if (myToggle.isOn)
				{
					{
						//Debug.Log(myRT.localScale);

						myTargetScale.x = myStartScale.x * toggleOnScaleFactor;
						myTargetScale.y = myStartScale.y * toggleOnScaleFactor;
						myTargetScale.z = 1f;


						ColorBlock colors = myToggle.colors;
						colors.normalColor = myToggle.colors.highlightedColor;
						myToggle.colors = colors;
					}
				}
				else
				{
					ColorBlock colors = myToggle.colors;
					colors.normalColor = normalColor;
					myToggle.colors = colors;
				}
			}

			//Debug.Log(myRT.localScale);
			
			//myRT.localScale = myTargetScale;
			//myCurrentScale = myTargetScale;
		}

		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	void Update()
	{
		//Debug.Log("update");
		myCurrentScale = Vector3.Lerp(myCurrentScale, myTargetScale, ((Time.deltaTime / 0.01f) * 0.2f));
		myRT.localScale = myCurrentScale;
	}
}
//
//https://forum.unity.com/threads/button-keyboard-and-mouse-highlighting.294147/
//