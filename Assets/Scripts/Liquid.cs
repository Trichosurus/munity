using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid  {
	public float currentSpeed = 0;
	public Quaternion currentDirectioin = new Quaternion();
	public float high = 0;
	public float low = 0;
	//public Material tint = new Material(Shader.Find("Custom/StandardClippableV2"));
	public Material surface = new Material(Shader.Find("Custom/StandardClippableV2"));
	public float damage = 0;
	public mapLight mediaLight;

	public GameObject volume = null;

	public Color colour = new Color(210,210,210);
	public float density = 0.1f;

	public MapSegment parent;



}