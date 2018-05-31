using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFunction {
	public int mode = 0;
	public float period = 0;
	public float periodDelta = 0;
	public float intensity = 0;
	public float intensityDelta = 0;

	public float totalPeriod = 0;
	public float totalIntensity = 0;

	private float flickertime = 0;
	private float initialIntensity = 0;

	public void initialise(float currentIntensity = 0) {
		initialIntensity = currentIntensity;
		totalPeriod = period + Random.Range(0f-periodDelta,periodDelta);
		totalIntensity = intensity + Random.Range(0f-intensityDelta*intensity,intensityDelta*intensity);
		flickertime = 0;
	}

	public float lightIntensity (float time, bool hightToLow = false) {
		float value = 0;
		float high, low, delta, amt;
		amt = 0;
		switch (mode) {
		case 0: //constant
			value = totalIntensity;
			break;
		case 1: //linear
			if (hightToLow) {
				high = initialIntensity;
				low = totalIntensity;
			} else {
				low = initialIntensity;
				high = totalIntensity;
			}
			delta = high-low;
			if (time > 0) {
				value = delta * (time / totalPeriod);
			} else {
				value = initialIntensity;
			}
			if (hightToLow) {
				value = initialIntensity - value;
			} else {
				value = initialIntensity + value;
			}
			break;
		case 2: //smooth
			if (hightToLow) {
				high = initialIntensity;
				low = totalIntensity;
			} else {
				low = initialIntensity;
				high = totalIntensity;
			}
			delta = high-low;
			if (time > 0) {
				amt = time /totalPeriod;
				amt *= 2f;
				amt -= 1f;
				value = Mathf.Sin(amt * 90f * Mathf.Deg2Rad);
				value += 1f;
				value /= 2f;
				value *= delta;
			} else {
				value = initialIntensity;
			}

			if (hightToLow) {
				value = initialIntensity - value;
			} else {
				value = initialIntensity + value;
			}

			break;
		case 3: //flicker
			value = totalIntensity;
			if (time > flickertime) {
				flickertime = time + Random.Range(0f,0.2f);
				if (Random.Range(0, 1) < 0.5f) {
					value = initialIntensity;
				}
			}
			break;
		}

		//if (value > totalIntensity) {value = totalIntensity;}
		if (value < 0) {value = 0;}
		return value;
	} 

	public void setFromMarathonObject (Weland.Light.Function state) {
		switch (state.LightingFunction) {
		case Weland.LightingFunction.Constant:
			mode = 0;
			break;
		case Weland.LightingFunction.Linear:
			mode = 1;
			break;
		case Weland.LightingFunction.Smooth:
			mode = 2;
			break;
		case Weland.LightingFunction.Flicker:
			mode = 3;
			break;	
		}
		intensityDelta = (float)state.DeltaIntensity;
		period = (float)state.Period/30f;
		intensity = (float)state.Intensity;
		periodDelta = (float)state.DeltaPeriod/30f;
	}

}