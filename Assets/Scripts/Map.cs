using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weland;
using System.Reflection;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
public class Map : MonoBehaviour {
	public GameObject polygon;
	List<int> collectionMapping = new List<int>{};
	List<int> transferModeMapping = new List<int> {0, 4, 5, 6, 9, 15, 16, 17, 18, 19, 20};
	public List<Material> materials;
	public List<MapSegment> segments;
	public List<mapLight> lights;
	public List<bool> tags;
	public List<audioDefinition> audioDefinitions = new List<audioDefinition>();

	private string loadingText = "";
	public int mapNo = 0;

	BinaryFormatter formatter = new BinaryFormatter();

	// Use this for initialization
	void Start () {
		
		StartCoroutine(loadData());
	}
	IEnumerator loadData() {
		// loadingText += "Loading Default Physics... ";
		// Wadfile physicsFile = new Wadfile();
		loadingText += "\n";
		SoundsFile sounds = new SoundsFile();
		sounds.Load(GlobalData.soundsFilePath);
		yield return makeAudioDefinitionsFromSoundsFile(sounds);

		// AudioSource aud = GetComponent<AudioSource>();
		// aud.clip = audioDefinitions[29].sounds[0];
		// aud.Play();

		loadingText += "\n";
		GlobalData.map = this;
	    ShapesFile shapes = new ShapesFile();
		shapes.Load(GlobalData.shapesFilePath);
		yield return StartCoroutine(makeMaterialsFromShapesFile(shapes));

		loadingText += "\nLoading Map... ";
		MapFile mapfile = new MapFile();
		mapfile.Load(GlobalData.mapsFilePath);
	    foreach (var kvp in mapfile.Directory) {
			if (kvp.Value.Chunks.ContainsKey(MapInfo.Tag)) {
				int levelNumber = kvp.Key;
			}
	    }
	    Level Level = new Level();
		Level.Load(mapfile.Directory[mapNo]);

		Debug.Log(Level.Name);
		// Debug.Log(Level.Environment);

		yield return createLightsFromMarathonMap(Level);

		yield return StartCoroutine(makeWorldFromMarathonMap(Level));

		loadingText += "\nSpawing Entities... ";
		yield return StartCoroutine(spawnEntitiesFromMarathonMap(Level, shapes));

		loadingText = null;
		GameObject.Find("LoadingDisplay").SetActive(false);

		
		GameObject.Find("worldLight").SetActive(GlobalData.globalLighting);
		
	}

	// Update is called once per frame
	void Update () {
		if (loadingText != null) {
			GameObject.Find("LoadingDisplay/LoadingText").GetComponent<TextMesh>().text = loadingText;
		}
	}

	IEnumerator makeAudioDefinitionsFromSoundsFile(SoundsFile sounds) {
		//Debug.Log(sounds.SourceCount);
		string load = loadingText;
		for (int sound = 0; sound < sounds.SoundCount; sound++) {

			SoundDefinition sd = sounds.GetSound(1,sound);
			audioDefinition ad = new audioDefinition();
			ad.sounds = new List<AudioClip>();
			foreach (Permutation p in sd.permutations) {
				AudioClip clip = AudioClip.Create("sound", p.Samples.Length, 1, (int)p.SampleRate, false); 
				clip.SetData(p.Samples,0);
				ad.sounds.Add(clip);
			}
			ad.cannotBeMediaObstructed = sd.CannotBeMediaObstructed;
			ad.cannotBeRestarted = sd.CannotBeRestarted;
			ad.doesNotSelfAbort = sd.DoesNotSelfAbort;
			ad.resistsPitchChanges = sd.ResistsPitchChanges;
			ad.cannotChangePitch = sd.CannotChangePitch;
			ad.cannotBeObstructed = sd.CannotBeObstructed;
			ad.isAmbient = sd.IsAmbient;
			ad.chance = sd.Chance;
			ad.lowPitch = sd.LowPitch;
			ad.highPitch = sd.HighPitch;

			audioDefinitions.Add(ad);

			if (sound % 77 == 0 ){
				loadingText = load + "Loading Sounds... " + sound + "/" + sounds.SoundCount;
				yield return null;
			}
		}
		loadingText = load + "Loading Sounds... " + sounds.SoundCount + "/" + sounds.SoundCount;

	}
	Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
		{
			Color32[] original = originalTexture.GetPixels32();
			Color32[] rotated = new Color32[original.Length];
			int w = originalTexture.width;
			int h = originalTexture.height;
	
			int iRotated, iOriginal;
	
			for (int j = 0; j < h; ++j)
			{
				for (int i = 0; i < w; ++i)
				{
					iRotated = (i + 1) * h - j - 1;
					iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
					rotated[iRotated] = original[iOriginal];
				}
			}
	
			Texture2D rotatedTexture = new Texture2D(h, w);
			rotatedTexture.SetPixels32(rotated);
			rotatedTexture.Apply();
			return rotatedTexture;
		}

	IEnumerator makeMaterialsFromShapesFile(ShapesFile shapes) {
		string load = loadingText;

		for (int col = 0; col < 32; col++) {
			//if (col != 20) {	collectionMapping.Add(0);continue;}
			//bool landscape = col >= 27;
			Collection coll = shapes.GetCollection(col);
			collectionMapping.Add(coll.BitmapCount);
			// Debug.Log(collectionMapping[col]);
			ShapeDescriptor d = new ShapeDescriptor();
			d.CLUT = 0;
			d.Collection = (byte) col;

			for (byte i = 0; i < coll.BitmapCount; ++i) {
				d.Bitmap = i;
				
				Texture2D bitmap = shapes.GetShape(d);


				if (GlobalData.landscapeCollections.Contains(col)) {
					bitmap = rotateTexture(bitmap,false);
					//extend landscape image to make skybox distortion less
					if (GlobalData.landscapeType > 0) {
						Color32[] original = bitmap.GetPixels32();
						Color32[] stretched = new Color32[bitmap.width * bitmap.width];
						int difference = stretched.Length - original.Length;
						int top = difference/2;

						if (GlobalData.landscapeType == 2) {
							Color32 fill = new Color32(255,255,255,0);
							for (int pos = 0; pos < stretched.Length; pos++) {
								stretched[pos] = fill;
							}
						}

						if (GlobalData.landscapeType == 3) {
							int row = bitmap.width;
							Color32[] fillTop = new Color32[row];
							Color32[] fillBottom = new Color32[row];
 							for (int pos = 0; pos < row; pos++) {
								fillBottom[pos] = original[pos];
								fillTop[pos] = original[pos+original.Length-row];
							}
							
							for (int y = 0; y < top/row; y++) {
								for (int x = 0; x < row; x++) {
									stretched[y * row + x + original.Length + top] = fillTop[x];	
									stretched[y*row + x] = fillBottom[x];									
								}
							}
						}
						if (GlobalData.landscapeType == 4 || GlobalData.landscapeType == 5) {
							int row = bitmap.width;
							Color32[] fillTop = new Color32[row];
							Color32[] fillBottom = new Color32[row];
							int rt = 0;
							int gt = 0;
							int bt = 0;
							int rb = 0;
							int gb = 0;
							int bb = 0;
 							for (int pos = 0; pos < row; pos++) {
								rb += original[pos].r;
								gb += original[pos].g;
								bb += original[pos].b;
								rt += original[pos+original.Length-row].r;
								gt += original[pos+original.Length-row].g;
								bt += original[pos+original.Length-row].b;
							}
							rt = rt/row;
							gt = gt/row;
							bt = bt/row;
							rb = rb/row;
							gb = gb/row;
							bb = bb/row;
							
 							for (int pos = 0; pos < row; pos++) {
								fillBottom[pos] = new Color32((byte)rb,(byte)gb,(byte)bb,255);
								fillTop[pos] = new Color32((byte)rt,(byte)gt,(byte)bt,255);
							}

							for (int y = 0; y < top/row; y++) {
								for (int x = 0; x < row; x++) {
									stretched[y * row + x + original.Length + top] = fillTop[x];	
									stretched[y*row + x] = fillBottom[x];									
								}
							}
						}
						for (int pos = 0; pos < original.Length; pos++) {
							stretched[pos + top] = original[pos];
						}

						if (GlobalData.landscapeType == 5) {
							Color32[] restretched = new Color32[bitmap.width * bitmap.width - top];
							for (int pos = top /2; pos < stretched.Length-top/2; pos++) {
								restretched[pos-top/2] = stretched[pos];
							}
							bitmap = new Texture2D(bitmap.width, bitmap.width - top/bitmap.width);
							bitmap.SetPixels32(restretched);
						} else {
							bitmap = new Texture2D(bitmap.width, bitmap.width);
							bitmap.SetPixels32(stretched);
						}
					}
					bitmap.wrapMode = TextureWrapMode.Clamp;
				}

				bitmap.Apply();

				Material  material = new Material(Shader.Find("Custom/StandardClippableV2"));

				material.SetFloat("_Glossiness",0.0f);
				if (bitmap.format == TextureFormat.ARGB32) {
					material.SetFloat("_Mode",1f);
					material.SetFloat("_Cutoff",0.85f);
					material.EnableKeyword("_ALPHABLEND_ON");
					material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
					material.EnableKeyword("_ALPHATEST_ON");
				} else {
					//Debug.Log("noalpha");
				}
				material.mainTexture = bitmap;
				material.SetTexture ("_EmissionMap", bitmap);
				material.SetColor ("_EmissionColor", Color.white);
				material.EnableKeyword("_EMISSION");
				materials.Add(material);
			//	Debug.Log(material);
			}		
			loadingText = load + "Loading Shapes... " + col + "/31";
			if (col % 7 == 0 ){
				yield return null;
			}
		}
		loadingText = load + "Loading Shapes... 31/31";


	}


	IEnumerator createLightsFromMarathonMap(Weland.Level Level) {
		string load = loadingText;
		for (int i = 0; i < Level.Lights.Count; i++) {

			Weland.Light light = Level.Lights[i];
			mapLight ml = gameObject.AddComponent<mapLight>();
			ml.id = i;
			ml.mapTag = light.TagIndex;
			ml.stateless = light.Stateless;
			ml.initiallyActive = light.InitiallyActive;
			ml.phase = light.Phase;
			ml.becomingActive.setFromMarathonObject(light.BecomingActive);
			ml.primaryActive.setFromMarathonObject(light.PrimaryActive);
			ml.secondaryActive.setFromMarathonObject(light.SecondaryActive);
			ml.becomingInactive.setFromMarathonObject(light.BecomingInactive);
			ml.primaryInactive.setFromMarathonObject(light.PrimaryInactive);
			ml.secondaryInactive.setFromMarathonObject(light.SecondaryInactive);
			if (light.Type == Weland.LightType.Strobe) {ml.type = 1;}
			if (light.Type == Weland.LightType.Media) {ml.type = 2;}

			lights.Add(ml);
		}
		yield return null;
	}



	IEnumerator makeWorldFromMarathonMap(Weland.Level Level) {
		string load = loadingText;
		//marathon maps have y +/- directions swapped so fix that
		for (int i = 0; i < Level.Endpoints.Count; i++) {
			Level.Endpoints[i] = new Point(Level.Endpoints[i].X, (short)(0-Level.Endpoints[i].Y));
		}
		//now we can generate mapsegment objects from each map polygon
		for (int p = 0; p < Level.Polygons.Count; p++) {
			GameObject pol = Instantiate(polygon);
			pol.name = "Polygon" + p;
			pol.tag = "polygon";
			MapSegment seg = pol.GetComponent<MapSegment>();
			segments.Add(seg);
			seg.height = new Vector3(0,(float)(Level.Polygons[p].CeilingHeight - Level.Polygons[p].FloorHeight)/1024f,0);
			if(Level.Polygons[p].Type == Weland.PolygonType.Platform) {
				foreach (Weland.Platform pl in Level.Platforms) {
					if (pl.PolygonIndex == p) {
						seg.platform = new PlatformObject();
						seg.platform.comesFromCeiling = pl.ComesFromCeiling;
						seg.platform.comesFromFloor = pl.ComesFromFloor;
						seg.platform.initiallyExtended = pl.InitiallyExtended;
						seg.platform.maximumHeight = (float)pl.MaximumHeight/1024f;
						seg.platform.minimumHeight = (float)pl.MinimumHeight/1024f;
						seg.platform.speed = (float)pl.Speed/64f;
						seg.platform.usesNativePolygonHeights = pl.UsesNativePolygonHeights;
						seg.platform.door = pl.IsDoor;
						seg.platform.initiallyActive = pl.InitiallyActive;
						seg.platform.parent = seg;
						seg.platform.activatesAdjacantPlatformsAtEachLevel = pl.ActivatesAdjacantPlatformsAtEachLevel;
						seg.platform.activatesAdjacentPlatformsWhenActivating = pl.ActivatesAdjacentPlatformsWhenActivating;
						seg.platform.activatesAdjacentPlatformsWhenDeactivating = pl.ActivatesAdjacentPlatformsWhenDeactivating;
						seg.platform.activatesLight = pl.ActivatesLight;
						seg.platform.activatesOnlyOnce = pl.ActivatesOnlyOnce;
						seg.platform.cannotBeExternallyDeactivated = pl.CannotBeExternallyDeactivated;
						seg.platform.causesDamage = pl.CausesDamage;
						seg.platform.contractsSlower = pl.ContractsSlower;
						seg.platform.deactivatesAdjacentPlatformsWhenActivating = pl.DeactivatesAdjacentPlatformsWhenActivating;
						seg.platform.deactivatesAdjacentPlatformsWhenDeactivating = pl.DeactivatesAdjacentPlatformsWhenDeactivating;
						seg.platform.deactivatesAtEachLevel = pl.DeactivatesAtEachLevel;
						seg.platform.deactivatesAtInitialLevel = pl.DeactivatesAtInitialLevel;
						seg.platform.deactivatesLight = pl.DeactivatesLight;
						seg.platform.delay = pl.Delay/30f;
						seg.platform.delaysBeforeActivation = pl.DelaysBeforeActivation;
						seg.platform.doesNotActivateParent = pl.DoesNotActivateParent;
						seg.platform.extendsFloorToCeiling = pl.ExtendsFloorToCeiling;
						seg.platform.isMonsterControllable = pl.IsMonsterControllable;
						seg.platform.isPlayerControllable = pl.IsPlayerControllable;
						seg.platform.locked = pl.IsLocked;
						seg.platform.reversesDirectionWhenObstructed = pl.ReversesDirectionWhenObstructed;
						seg.platform.secret = pl.IsSecret;
						seg.platform.mapTag = pl.Tag;
						seg.platform.usesNativePolygonHeights = pl.UsesNativePolygonHeights;
					}
				}
			}


			if (Level.Polygons[p].MediaIndex >= 0) {
				seg.liquid = new Liquid();

				Media media = Level.Medias[Level.Polygons[p].MediaIndex];
				seg.liquid.currentSpeed = (float)media.CurrentMagnitude/1024f;
				seg.liquid.currentDirectioin = Quaternion.Euler(0, (float)media.Direction+90, 0);
				seg.liquid.high = (float)media.High/1024f;
				seg.liquid.low = (float)media.Low/1024f;
				seg.liquid.mediaLight = lights[media.LightIndex];
				//?? where is the liquid data stored???
				// Material mat = new Material(Shader.Find("Custom/StandardClippableV2"));

				Weland.ShapeDescriptor tex = new Weland.ShapeDescriptor();
				int mediaType = 0;
				switch (media.Type) {
					case MediaType.Water: 
						mediaType = 0;
						break;
					case MediaType.Lava: 
						mediaType = 1;
						break;
					case MediaType.Goo: 
						mediaType = 2;
						break;
					case MediaType.Sewage: 
						mediaType = 3;
						break;
					case MediaType.Jjaro: 
						mediaType = 4;
						break;
					default: 
						mediaType = 0;
						break;
				}

				tex.Collection = (byte)GlobalData.mediaCollections[mediaType];
				tex.Bitmap = (byte)GlobalData.mediaBitmaps[mediaType];
				seg.liquid.surface = getTexture(tex);
				seg.liquid.colour = GlobalData.mediaColours[mediaType];
				seg.liquid.density = GlobalData.mediaDensities[mediaType];


				seg.liquid.parent = seg;
			}

			switch (Level.Polygons[p].Type) {
				case Weland.PolygonType.AutomaticExit : 
					seg.automaticExit = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.Base : 
					seg.mapBase = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.DualMonsterTrigger : 
					seg.dualMonsterTrigger = true;
				break;	
				case Weland.PolygonType.Glue : 
					seg.glue = true;
				break;	
				case Weland.PolygonType.GlueTrigger : 
					seg.glueTrigger = true;
				break;	
				case Weland.PolygonType.Goal :
					seg.goal = true;
				break;	
				case Weland.PolygonType.Hill : 
					seg.hill = true;
				break;	
				case Weland.PolygonType.InvisibleMonsterTrigger : 
					seg.invisibleMonsterTrigger = true;
				break;	
				case Weland.PolygonType.ItemImpassable : 
					seg.itemImpassable = true;
				break;	
				case Weland.PolygonType.ItemTrigger : 
					seg.itemTrigger = true;
				break;	
				case Weland.PolygonType.LightOffTrigger : 
					seg.lightOffTrigger = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.LightOnTrigger : 
					seg.lightOnTrigger = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.MajorOuch : 
					seg.damage = 7;
				break;	
				case Weland.PolygonType.MinorOuch : 
					seg.damage = 3;
				break;	
				case Weland.PolygonType.MonsterImpassable : 
					seg.monsterImpassable = true;
				break;	
				case Weland.PolygonType.MustBeExplored : 
					seg.mustBeExplored = true;
				break;	
				case Weland.PolygonType.PlatformOffTrigger : 
					seg.platformOffTrigger = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.PlatformOnTrigger : 
					seg.platformOnTrigger = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.Superglue : 
					seg.superglue = true;
				break;	
				case Weland.PolygonType.Teleporter : 
					seg.teleporter = Level.Polygons[p].Permutation;
				break;	
				case Weland.PolygonType.VisibleMonsterTrigger : 
					seg.visibleMonsterTrigger = true;
				break;	
				case Weland.PolygonType.ZoneBorder : 
					seg.zoneBorder = true;
				break;	
			}
			
			//get the map points that make up the polygon
			List<Vector3> points = new List<Vector3>();
			//find the top most point so we can sort them clockwise to make
			int zPt = 0;
			int xPt = 0;
			for (int ep = 0; ep <  Level.Polygons[p].VertexCount; ep++) {
				int x = Level.Endpoints[Level.Polygons[p].EndpointIndexes[ep]].X;
				int z = Level.Endpoints[Level.Polygons[p].EndpointIndexes[ep]].Y;

				if (z > Level.Endpoints[Level.Polygons[p].EndpointIndexes[zPt]].Y ||
					(z == Level.Endpoints[Level.Polygons[p].EndpointIndexes[zPt]].Y && x > Level.Endpoints[Level.Polygons[p].EndpointIndexes[zPt]].X)) {
						zPt = ep;
				}
				if (x < Level.Endpoints[Level.Polygons[p].EndpointIndexes[xPt]].X ||
					(x == Level.Endpoints[Level.Polygons[p].EndpointIndexes[xPt]].X && z > Level.Endpoints[Level.Polygons[p].EndpointIndexes[xPt]].Y)) {
						xPt = ep;
				}
			}

			//add the lines and sides for the polygon
			List<Weland.Line> Line = new List<Weland.Line>();
			List<Weland.Side> Sides = new List<Weland.Side>();
			int currentLine = -1;
			for (int ln = 0; ln < Level.Polygons[p].VertexCount; ln++) {
				Line.Add(Level.Lines[Level.Polygons[p].LineIndexes[ln]]);
				if (Level.Polygons[p].SideIndexes[ln] >=0) {
					Sides.Add(Level.Sides[Level.Polygons[p].SideIndexes[ln]]);
				}
				//which lines are attached to the vertex
				int ep = -1;
				if (Line[Line.Count-1].EndpointIndexes[0] == Level.Polygons[p].EndpointIndexes[zPt]) {ep = 0;}
				if (Line[Line.Count-1].EndpointIndexes[1] == Level.Polygons[p].EndpointIndexes[zPt]) {ep = 1;}
				if (ep >= 0) {
					if (currentLine < 0) {
						currentLine = ln;
					} else {
						int lep = 0;
						if (Line[currentLine].EndpointIndexes[1] == Line[Line.Count-1].EndpointIndexes[0] ||
							Line[currentLine].EndpointIndexes[1] == Line[Line.Count-1].EndpointIndexes[1]) {
							lep = 1;
						}
						Point point = Level.Endpoints[Line[currentLine].EndpointIndexes[lep]];
						Vector2 a = new Vector2(point.X, point.Y);
						
						ep = Line[Line.Count-1].EndpointIndexes[Mathf.Abs(ep-1)];
						lep = Line[currentLine].EndpointIndexes[Mathf.Abs(lep-1)];
						//make sure we are going clockwise
						Vector2 b = new Vector2(Level.Endpoints[lep].X, Level.Endpoints[lep].Y);
						Vector2 c = new Vector2(Level.Endpoints[ep].X, Level.Endpoints[ep].Y);
						if (Mathf.Atan2(b.y - a.y, b.x - a.x) < Mathf.Atan2(c.y - a.y, c.x - a.x)) {
							currentLine = ln;
						}
					}
				}
			}
			
			int lastPt = -1;
			while (Line.Count > 0) {
				//get the correct point for each line
				int pt = 0;
				int[] ei= {Line[currentLine].EndpointIndexes[0], Line[currentLine].EndpointIndexes[1]};
				if (lastPt == -1) {
					if (Level.Endpoints[ei[1]].Y > Level.Endpoints[ei[0]].Y) {
						pt = 1;
					}
					if (Level.Endpoints[ei[1]].Y == Level.Endpoints[ei[0]].Y) {
						if (Level.Endpoints[ei[1]].X < Level.Endpoints[ei[0]].X) {
							pt = 1;
						}
					}
				}
				if (ei[1] == lastPt) {pt = 1;}
				// map segment vertices must be in clockwise order
				points.Add(new Vector3(
							(float)Level.Endpoints[ei[pt]].X/1024f,
							0,
							(float)Level.Endpoints[ei[pt]].Y/1024f)
				);

				MapSegmentSide mss = new MapSegmentSide();
				mss.transparent = Line[currentLine].Transparent;
				mss.solid = Line[currentLine].Solid;
				Side side = new Side();
				//get texture + lighting information for side
				if ((Line[currentLine].ClockwisePolygonSideIndex >= 0  && Line[currentLine].ClockwisePolygonOwner == p ) 
					|| (Level.Polygons[p].Type == PolygonType.Platform && Line[currentLine].ClockwisePolygonSideIndex >= 0) ) {
					side = Level.Sides[Line[currentLine].ClockwisePolygonSideIndex];
				} else if ((Line[currentLine].CounterclockwisePolygonSideIndex >= 0   && Line[currentLine].CounterclockwisePolygonOwner == p)
						||(Level.Polygons[p].Type == PolygonType.Platform && Line[currentLine].CounterclockwisePolygonSideIndex >= 0) ) {
					side = Level.Sides[Line[currentLine].CounterclockwisePolygonSideIndex];
				}
					mss.upperMaterial = getTexture(side.Primary.Texture);
					mss.lowerMaterial = getTexture(side.Secondary.Texture);
					mss.middeMaterial = getTexture(side.Transparent.Texture);

					mss.upperOffset = new Vector2((float)side.Primary.X/1024f,(float)side.Primary.Y/1024f);
					mss.middleOffset = new Vector2((float)side.Transparent.X/1024f,(float)side.Transparent.Y/1024f);
					mss.lowerOffset = new Vector2((float)side.Secondary.X/1024f,(float)side.Secondary.Y/1024f);

					mss.upperLight = lights[side.PrimaryLightsourceIndex];
					mss.lowerLight = lights[side.SecondaryLightsourceIndex];
					mss.middleLight = lights[side.TransparentLightsourceIndex];


				if (mss.lowerMaterial == null) {
					mss.lowerMaterial = mss.upperMaterial;
					mss.lowerOffset = mss.upperOffset;
					mss.lowerLight = mss.upperLight;
				}

				//get control panel information if needed
				if (side != null && (side.IsControlPanel || side.IsPlatformSwitch() || side.IsTagSwitch() || side.IsLightSwitch())) {
					mss.controlPanel = new ControlPanel();
					mss.controlPanel.permutation = side.ControlPanelPermutation;
					mss.controlPanel.type = side.ControlPanelType;
					mss.controlPanel.controlPanel = side.IsControlPanel;
					if (side.IsPlatformSwitch()) {mss.controlPanel.platformSwitch = side.ControlPanelPermutation;}
					if (side.IsTagSwitch()) {mss.controlPanel.tagSwitch = side.ControlPanelPermutation;}
					if (side.IsLightSwitch()) {mss.controlPanel.lightSwitch = side.ControlPanelPermutation;}
					mss.controlPanel.inactiveMat =  mss.upperMaterial;
					for (int t = 0 ; t < materials.Count; t++) {
						if (materials[t] == mss.controlPanel.inactiveMat) {
							mss.controlPanel.activeMat =  materials[t-1];
						}
					}
					
					switch(side.Flags) {
						// case SideFlags.None:
						// 	break;
						case SideFlags.ControlPanelStatus:
							mss.controlPanel.controlPanelStatus = 1;//?? what is this for?
							break;
						case SideFlags.Dirty:
							mss.controlPanel.dirty = true;
							break;
						case SideFlags.IsDestructiveSwitch:
							mss.controlPanel.destructiveSwitch = true;
							break;
						// case SideFlags.IsLightedSwitch:
						// 	mss.controlPanel.active = true;
						// 	break;
						case SideFlags.IsRepairSwitch:
							mss.controlPanel.repairSwitch = true;
							break;
						case SideFlags.IsControlPanel:
							//mss.controlPanel.controlPanel = true;
							break;
						case SideFlags.SwitchCanBeDestroyed:
							mss.controlPanel.canBeDestroyed = true;
							break;
						case SideFlags.SwitchCanOnlyBeHitByProjectiles:
							mss.controlPanel.canOnlyBeHitByProjectiles = true;
							break;
					}
				}

				//connedtion information is used for occlusion culling
				seg.sides.Add(mss);
				if (Line[currentLine].ClockwisePolygonOwner == p) {
					mss.connectionID = Line[currentLine].CounterclockwisePolygonOwner;
				} else {
					mss.connectionID = Line[currentLine].ClockwisePolygonOwner;
				}
				pt = Mathf.Abs(pt - 1);
				lastPt = ei[pt];

				//find next line that connects to the endpoint of this one to make the next side
				Line.RemoveAt(currentLine);
				for (int i = 0; i < Line.Count; i++) {
					if (Line[i].EndpointIndexes[0] == ei[pt] || Line[i].EndpointIndexes[1] == ei[pt]){
						currentLine = i;
					}
				}
			}
			 
			//get floor and ceiling texture data
			seg.ceiling.upperMaterial = getTexture(Level.Polygons[p].CeilingTexture);
			seg.floor.upperMaterial = getTexture(Level.Polygons[p].FloorTexture);

			seg.ceiling.upperOffset = new Vector2((float)Level.Polygons[p].CeilingOrigin.X/1024f,(float)Level.Polygons[p].CeilingOrigin.Y/1024f);
			seg.floor.upperOffset = new Vector2((float)Level.Polygons[p].FloorOrigin.X/1024f,(float)Level.Polygons[p].FloorOrigin.Y/1024f);

			seg.ceiling.lightID = Level.Polygons[p].CeilingLight;
			seg.ceiling.light = lights[Level.Polygons[p].CeilingLight];
			seg.floor.lightID = Level.Polygons[p].FloorLight;
			seg.floor.light = lights[Level.Polygons[p].FloorLight];
			seg.vertices = points;
			seg.centerPoint = new Vector3(0,(float)Level.Polygons[p].FloorHeight/1024f,0);
			seg.id = p;

			//convert points to be relative to the polygon average point
			seg.calculatePoints();
		
			if (p % 77 == 0 ){
				loadingText = load + "\nGenerating Polygons "+p+"/"+Level.Polygons.Count;
				yield return null;
			}

		}
		load = load + "\nGenerating Polygons "+Level.Polygons.Count+"/"+Level.Polygons.Count;

		//if a polygon is a platform we need to change its floor/ceiling heights to actually be
		//the volume that the platform would be moving up and down in if it were an actual 
		//object - which it will be. 
		int count = 0;		
		foreach(Weland.Platform pl in Level.Platforms) {
			count++;
			segments[pl.PolygonIndex].recalculatePlatformVolume();
			if (count % 7 == 0 ){
				loadingText = load + "\nRecalculate Platform Volumes "+count+"/"+Level.Platforms.Count;
				yield return null;
			}
		}
		load = load + "\nRecalculate Platform Volumes "+count+"/"+Level.Platforms.Count;
		count = 0;

		foreach(MapSegment s in segments) {
			count++;
			s.generateMeshes();
			if (count % 77 == 0 ){
				loadingText = load + "\nGenerating Meshes "+count+"/"+segments.Count;
				yield return null;
			}
		}
		load = load + "\nGenerating Meshes "+count+"/"+segments.Count;
		count = 0;

		foreach(MapSegment s in segments) {
			count++;
			s.showHide(false);
			s.checkIfImpossible();
			s.showHide(true);
			if (count % 77 == 0 ){
				loadingText = load + "\nFinding Impossible Space "+count+"/"+segments.Count;
				yield return null;
			}
		}
		load = load + "\nFinding Impossible Space "+count+"/"+segments.Count;
		count = 0;

		List<List<int>> collisionsList = new List<List<int>>();
		for(int s = 0; s < segments.Count; s++) {
			if (segments[s].impossible){
				impossibleVolume iv = new impossibleVolume();
				iv.parent = segments[s];
				List<int> connList = iv.getConnectedVolumes(segments[s], true);
				segments[s].impossibleVolumes = new List<impossibleVolume>();
				iv.collisionPolygonsSelf = connList;
				segments[s].impossibleVolumes.Add(iv); 
			}
		}
		for(int s = 0; s < segments.Count; s++) {
			if (segments[s].impossible){
				segments[s].impossibleVolumes[0].getConnectedVolumes(segments[s], false);
				for (int imp = 0; imp <  segments[s].impossibleVolumes.Count; imp++) {
					foreach (int seg in segments[s].impossibleVolumes[imp].collisionPolygonsSelf) {
						HashSet<int> hsSelf = new HashSet<int>(segments[s].impossibleVolumes[imp].collisionPolygonsOther);
						bool exists = false;
						foreach (impossibleVolume iv in segments[seg].impossibleVolumes) {
							HashSet<int> hsOther = new HashSet<int>(iv.collisionPolygonsOther);
							if (hsSelf.SetEquals(hsOther)) {
								exists = true;				
							}
						}
						if (!exists) {
							segments[seg].impossibleVolumes.Add(segments[s].impossibleVolumes[imp]);
						}
					}
				}
			}
		}

		for(int s = 0; s < segments.Count; s++) {
			if (segments[s].impossible){
				foreach(impossibleVolume iv in segments[s].impossibleVolumes) {
					iv.assembleVolumeSides(segments[s], true);
					iv.assembleVolumeSides(segments[s], false);
					iv.calculateCollisionPoints();
				}
			}
		}

		string mapHash = CalculateMD5(GlobalData.mapsFilePath);
		mapHash = Application.persistentDataPath + "/" + mapHash  + "-" + mapNo + ".cache";
		if (!File.Exists(mapHash)) {
			activePolygonList apl = new activePolygonList();
			apl.activePolygons = new List<bool[]>();
			for (int i = 0; i < segments.Count; i++) {
				count++;
				segments[i].calculateVisibility();

				apl.activePolygons.Add(segments[i].activePolygons);

				if (!GlobalData.skipOcclusion && count % 7 == 0) {
					loadingText = load + "\nOcclusion Culling "+count+"/"+segments.Count;
					yield return null;
				}
			}
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = File.Open(mapHash, FileMode.Create);
			bf.Serialize(fs, apl);
			fs.Close();
		} else {
			
			FileStream fs = File.Open(mapHash, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			activePolygonList apl = (activePolygonList)bf.Deserialize(fs);
			fs.Close();

			for (int i = 0; i < apl.activePolygons.Count; i++) {
				if (segments.Count > i) {
					segments[i].activePolygons = apl.activePolygons[i];
				}
			}
		}
		load = load + "\nOcclusion Culling "+count+"/"+segments.Count;
		count = 0;
	
		foreach(Weland.Platform pl in Level.Platforms) {
			count++;
			segments[pl.PolygonIndex].showHide(true);
			segments[pl.PolygonIndex].makePlatformObjects();
			if (count % 27 == 0 ){
				loadingText = load + "\nMaking Platforms "+count+"/"+Level.Platforms.Count;
				yield return null;
			}
		}
		
		foreach(Weland.Platform pl in Level.Platforms) {
			if (segments[pl.PolygonIndex].platform.initiallyActive) {
				segments[pl.PolygonIndex].platform.activate();
			}
		}

	}




		
	//struct for saving occlusion data cache
	[System.Serializable]
	struct activePolygonList {
		public List<bool[]> activePolygons;	
	}

	string CalculateMD5(string filename) {
		using (var md5 = MD5.Create()) {
			using (var stream = File.OpenRead(filename)) {
				var hash = md5.ComputeHash(stream);
				return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
			}
		}
	}


	IEnumerator spawnEntitiesFromMarathonMap(Weland.Level Level, Weland.ShapesFile shapes) {
		string load = loadingText;
		bool playerSpawned = false;
		for (int i = 0; i < Level.Objects.Count; i++) {
			MapObject obj = Level.Objects[i];
			Quaternion facing = Quaternion.Euler(0, (float)obj.Facing+90, 0);

			Vector3 pos = new Vector3(pos.x = (float)obj.X/1024f,
									pos.y = (float)obj.Z/1024f + segments[obj.PolygonIndex].centerPoint.y,
									pos.z = 0f-(float)obj.Y/1024f
									);	

			if (obj.Type == ObjectType.Player && !playerSpawned) {
				playerSpawned = true;

				GameObject player = Instantiate(Resources.Load<GameObject>("player"), pos, facing);
				player.gameObject.name = "player";
				player.GetComponent<playerController>().currentPolygon = obj.PolygonIndex;
			}

			if (obj.Type == ObjectType.Item ) {
				GameObject item = createMapItemFromSpriteSequence(7,GlobalData.itemSequences[obj.Index-1],shapes, "itemObject");
				Vector3 rpos = pos;
				item.transform.position = rpos;
				item.transform.rotation = facing;
				item.name = "item" + i;
			}

			if (obj.Type == ObjectType.Sound ) {

			}

			if (obj.Type == ObjectType.Scenery ) {
				GameObject item = createMapItemFromSpriteSequence(GlobalData.sceneryCollections[obj.Index],
																	GlobalData.scenerySequences[obj.Index],
																	shapes, "sceneryObject");

				Vector3 rpos = pos;
				if (obj.FromCeiling) {
					rpos.y = segments[obj.PolygonIndex].centerPoint.y + segments[obj.PolygonIndex].height.y;
				}
				item.transform.position = rpos;
				item.transform.rotation = facing;
				item.name = "scenery" + i;
				item.transform.Find("sprite").GetComponent<spriteController>().fromCeiling = obj.FromCeiling;

			}

			if (obj.Type == ObjectType.Goal ) {
			}

			if (obj.Type == ObjectType.Monster ) {
				Debug.Log("monster");
				Debug.Log(obj.Index);
			}

			if (i % 7 == 0 ){
				loadingText = load + i+"/"+Level.Objects.Count;
				yield return null;
			}

		}
	}

	


	Material getTexture(Weland.ShapeDescriptor texture) {
		int retval = 0;

		for (int i = 0; i < texture.Collection; i++) {
			retval += collectionMapping[i];
		}
		retval += texture.Bitmap;

		if (materials.Count > retval && retval >= 0) {

			if (GlobalData.landscapeCollections.Contains(texture.Collection)) {
				RenderSettings.skybox.mainTexture = materials[retval].mainTexture;
				return  Resources.Load<Material>("Materials/transparent");
			} else {
				return materials[retval];
			}
		} else {
			//return new Material(Shader.Find("Custom/StandardClippableV2"));
			return null;
		}
	}

	GameObject createMapItemFromSpriteSequence (int collectionID, int sequenceID, Weland.ShapesFile shapes, string objectName) {
		GameObject item = Instantiate(Resources.Load<GameObject>(objectName));
		GameObject sprite = Instantiate(Resources.Load<GameObject>("spriteObject"));
		sprite.name = "sprite";
		sprite.transform.parent = item.transform;
		spriteController sc = sprite.GetComponent<spriteController>();
		sc.parent = item;
		sc.type = GlobalData.spriteType;

		// Debug.Log(sequenceID);
		Collection coll = shapes.GetCollection(collectionID);
		if (sequenceID > coll.sequences.Count) {sequenceID = coll.sequences.Count -1;}
		Collection.ShapeSequence sequence = coll.sequences[sequenceID];

		float scale = sequence.PixelsToWorld;
		if (scale == 0) {scale = coll.pixelsToWorld ;}
		sc.scale = scale;
		// Debug.Log(coll.frames[0].OriginX);
		sc.sideCount = sequence.getRealViewCount(sequence.NoOfViews);
		List<Material> frames = new List<Material>();
		Weland.ShapeDescriptor tex = new Weland.ShapeDescriptor();
		
		for (int i = 0; i < sequence.FrameIndexes.Count; i++) {
			tex.Collection = (byte)collectionID;
			tex.Bitmap = (byte)coll.frames[sequence.FrameIndexes[i]].BitmapIndex;
			frames.Add(getTexture(tex));

		}

		sc.frames = frames;


		return item;
	}


}


