﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboneUnit : MonoBehaviour {

	public int residue = -1;

	Shader shaderStandard;
	Shader shaderToonOutline;

	public bool activeSequenceSelect = false;
	private bool activeSequenceSelectLast = false;
	public bool controllerHoverOn = false;
	public bool controllerSelectOn = false;
	public bool remoteGrabSelectOn = false;

	public Residue myResidue;
	public PolyPepBuilder myPolyPepBuilder;
	public PolyPepManager myPolyPepManager;

	// 
	private Renderer rendererPhi;	// amide only
	private Renderer rendererPsi;   // calpha only
	private Renderer rendererPeptide; // carbonyl only

	//private Renderer[] rendererAtoms;
	private List<Renderer> renderersAtoms = new List<Renderer>();

	// Use this for initialization
	void Awake () {

		// init my references to renderers
		{
			Renderer[] allChildRenderers = gameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer childRenderer in allChildRenderers)
			{
				//Debug.Log(childRenderer.transform.gameObject.name);
				//Debug.Log(childRenderer.transform.gameObject.layer);

				//bonds
				if (childRenderer.transform.gameObject.name == "bond_N_CA")
				{
					rendererPhi = childRenderer;
				}
				if (childRenderer.transform.gameObject.name == "bond_CA_CO")
				{
					rendererPsi = childRenderer;
				}
				if (childRenderer.transform.gameObject.name == "bond_CO_N")
				{
					rendererPeptide = childRenderer;
				}

				//atoms
				if (childRenderer.transform.gameObject.layer == LayerMask.NameToLayer("Atom"))
				{
					//Debug.Log("got one!");
					renderersAtoms.Add(childRenderer);
				}
			}
			//Debug.Log("bu " + gameObject + " --> " + gameObject.transform.parent.parent.gameObject);

		}

		// init parent script references
		myResidue = (gameObject.transform.parent.gameObject.GetComponent("Residue") as Residue);
		myPolyPepBuilder = (gameObject.transform.parent.parent.gameObject.GetComponent("PolyPepBuilder") as PolyPepBuilder);
		GameObject manager = GameObject.Find("PolyPepManager");
		myPolyPepManager = manager.GetComponent("PolyPepManager") as PolyPepManager;

		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");
	}

	void Start()
	{
		//duplication of PolyPepManager UI code 

		myPolyPepBuilder.ScaleVDW(myPolyPepManager.vdwScale);
		myPolyPepBuilder.SetAllColliderIsTrigger(!myPolyPepManager.collidersOn);

		myPolyPepBuilder.NudgeHbondSprings();
		myPolyPepBuilder.UpdatePhiPsiDrives();

		UpdateRenderMode();
	}

	public void SetBackboneUnitControllerHover(bool value)
	{
		if (controllerHoverOn != value)
		{
			controllerHoverOn = value;
			myResidue.residueHovered = value;

			BackboneUnit buAmide = myResidue.amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCalpha = myResidue.calpha_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCarbonyl = myResidue.carbonyl_pf.GetComponent("BackboneUnit") as BackboneUnit;

			BackboneUnit thisBBU = gameObject.GetComponent<BackboneUnit>();
			if (thisBBU != buAmide) { buAmide.SetBackboneUnitControllerHover(value); }
			if (thisBBU != buCalpha) { buCalpha.SetBackboneUnitControllerHover(value); }
			if (thisBBU != buCarbonyl) { buCarbonyl.SetBackboneUnitControllerHover(value); }

			UpdateRenderMode();
		}

		//controllerHoverOn = flag;
		//myResidue.residueHovered = flag;
		//UpdateRenderMode();
	}

	public void SetMyResidueSelect(bool flag)
	{
		//Residue res = (gameObject.transform.parent.gameObject.GetComponent("Residue") as Residue);
		if (myResidue)
		{
			//Debug.Log("             " + res);
			BackboneUnit buAmide = myResidue.amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCalpha = myResidue.calpha_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCarbonyl = myResidue.carbonyl_pf.GetComponent("BackboneUnit") as BackboneUnit;

			buAmide.SetBackboneUnitSelect(flag);
			buCalpha.SetBackboneUnitSelect(flag);
			buCarbonyl.SetBackboneUnitSelect(flag);

			//Debug.Log(res + " " + flag);
			myResidue.residueSelected = flag;
		}
	}

	public void SetBackboneUnitSelect (bool flag)
	{
		controllerSelectOn = flag;
		UpdateRenderMode();
	}

	public void SetActiveSequenceSelect(bool flag)
	{
		activeSequenceSelect = flag;
		if (activeSequenceSelect != activeSequenceSelectLast)
		{
			UpdateRenderMode();
			activeSequenceSelectLast = activeSequenceSelect;	
		}
	}

	public void SetRemoteGrabSelect(bool value)
	{
		if (remoteGrabSelectOn != value)
		{
			remoteGrabSelectOn = value;
			myResidue.residueGrabbed = value;

			BackboneUnit buAmide = myResidue.amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCalpha = myResidue.calpha_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCarbonyl = myResidue.carbonyl_pf.GetComponent("BackboneUnit") as BackboneUnit;

			BackboneUnit thisBBU = gameObject.GetComponent<BackboneUnit>();
			if (thisBBU != buAmide) { buAmide.SetRemoteGrabSelect(value); }
			if (thisBBU != buCalpha) { buCalpha.SetRemoteGrabSelect(value); }
			if (thisBBU != buCarbonyl) { buCarbonyl.SetRemoteGrabSelect(value); }

			UpdateRenderMode();
		}
	}

	private void SetRenderingMode(GameObject go, string shaderName)
	{

		//Renderer[] allChildRenderers = go.GetComponentsInChildren<Renderer>();
		//if (_type.ToString() != "UnityEngine.ParticleSystemRenderer")

		foreach (Renderer _rendererAtom in renderersAtoms)
		{
			{
				switch (shaderName)

				{
					case "ToonOutlineGreen":
						{
							_rendererAtom.material.shader = shaderStandard;
						}
						{
							_rendererAtom.material.shader = shaderToonOutline;
							_rendererAtom.material.SetColor("_OutlineColor", Color.green);
							_rendererAtom.material.SetFloat("_Outline", myPolyPepManager.toonRenderScale * 2.0f);
						}
						break;

					case "ToonOutlineRed":
						{
							_rendererAtom.material.shader = shaderToonOutline;
							_rendererAtom.material.SetColor("_OutlineColor", Color.red);
							_rendererAtom.material.SetFloat("_Outline", myPolyPepManager.toonRenderScale * 1.5f);
						}
						break;

					case "ToonOutlineYellow":
						{
							_rendererAtom.material.shader = shaderToonOutline;
							_rendererAtom.material.SetColor("_OutlineColor", Color.yellow);
							_rendererAtom.material.SetFloat("_Outline", myPolyPepManager.toonRenderScale);
						}
						break;

					case "ToonOutlineCyan":
						{
							_rendererAtom.material.shader = shaderToonOutline;
							_rendererAtom.material.SetColor("_OutlineColor", Color.cyan);
							_rendererAtom.material.SetFloat("_Outline", myPolyPepManager.toonRenderScale * 0.8f);
						}
						break;

					case "Standard":
						{
							_rendererAtom.material.shader = shaderStandard;
						}
						break;
				}
			}
		}

		//bool doBondCartoonRendering = true;
		if (myPolyPepManager.doCartoonBondRendering)
		{
			float bondToonRenderScale = 0.0f;
			if (myResidue.drivePhiPsiTorqValue > 0)
			{
					bondToonRenderScale = 2.0f + ((myResidue.drivePhiPsiTorqValue / 400.0f) * 5.0f);
			}
			
			if (rendererPhi)
			{
				if (myPolyPepManager.showDrivenBondsOn)
				{
					rendererPhi.material.shader = shaderToonOutline;
					rendererPhi.material.SetColor("_OutlineColor", Color.cyan);
					rendererPhi.material.SetFloat("_Outline", myPolyPepManager.toonRenderScale * bondToonRenderScale);
				}
				else
				{
					rendererPhi.material.shader = shaderStandard;
				}
			}

			if (rendererPsi)
			{
				if (myPolyPepManager.showDrivenBondsOn)
				{
					rendererPsi.material.shader = shaderToonOutline;
					rendererPsi.material.SetColor("_OutlineColor", Color.magenta);
					rendererPsi.material.SetFloat("_Outline", myPolyPepManager.toonRenderScale
						* bondToonRenderScale);
				}
				else
				{
					rendererPsi.material.shader = shaderStandard;
				}
			}

			if (rendererPeptide)
			{
				if (myPolyPepManager.showDrivenBondsOn)
				{
					rendererPeptide.material.shader = shaderToonOutline;
					rendererPeptide.material.SetColor("_OutlineColor", Color.black);
					rendererPeptide.material.SetFloat("_Outline", 0.005f);
				}
				else
				{
					rendererPeptide.material.shader = shaderStandard;
				}
			}
		}


	}


	public void UpdateRenderMode()
	{
		if (remoteGrabSelectOn)
		{
			SetRenderingMode(gameObject, "ToonOutlineGreen");
		}
		else if (controllerHoverOn)
		{
			SetRenderingMode(gameObject, "ToonOutlineRed");
		}
		else if (controllerSelectOn)
		{
			SetRenderingMode(gameObject, "ToonOutlineYellow");
		}
		else if (myResidue.residueFrozen)
		{
			//BackboneUnit buCalpha = myResidue.calpha_pf.GetComponent("BackboneUnit") as BackboneUnit;
			//if (buCalpha)
			{
				SetRenderingMode(gameObject, "ToonOutlineCyan");
			}
		}
		else
		{
			SetRenderingMode(gameObject, "Standard");
		}
	}

	// Update is called once per frame
	void Update () {
		//UpdateRenderMode();
	}
}
