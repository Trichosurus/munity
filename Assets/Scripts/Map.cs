using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weland;
using System.Reflection;
using System;
using System.IO;



public class Map : MonoBehaviour {
	public GameObject polygon;
	List<int> collectionMapping = new List<int>{};
	List<int> transferModeMapping = new List<int> {0, 4, 5, 6, 9, 15, 16, 17, 18, 19, 20};
	public List<Material> materials;
	public List<MapSegment> segments;
	public List<mapLight> lights;
	public List<bool> tags;

	private string loadingText;

	// Use this for initialization
	void Start () {
		
		StartCoroutine(loadData());
	}
	IEnumerator loadData() {
		loadingText = "Loading Shapes... ";
		GlobalData.map = this;
	    ShapesFile shapes = new ShapesFile();
		shapes.Load(GlobalData.shapesFilePath);
		yield return StartCoroutine(makeMaterialsFromShapesFile(shapes));

		loadingText += "\nLoading Map... ";
		// ClearLog();
		Wadfile wadfile = new Wadfile();
		wadfile.Load(GlobalData.mapsFilePath);
		//Wadfile	wadfile = w;
	    foreach (var kvp in wadfile.Directory) {
			if (kvp.Value.Chunks.ContainsKey(MapInfo.Tag)) {
				string String = kvp.Value.LevelName;
				int levelNumber = kvp.Key;
			}
	    }
	    Level Level = new Level();
		Level.Load(wadfile.Directory[0]);
		Debug.Log(Level.Name);
		// Debug.Log(Level.Environment);

		yield return createLightsFromMarathonMap(Level);

		yield return StartCoroutine(makeWorldFromMarathonMap(Level));

		loadingText += "\nSpawing Entities... ";
		yield return StartCoroutine(spawnEntitiesFromMarathonMap(Level));

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

	IEnumerator makeMaterialsFromShapesFile(ShapesFile shapes) {
	    

		for (int col = 0; col < 32; col++) {
			//if (col != 20) {	collectionMapping.Add(0);continue;}
			bool landscape = col >= 27;
			Collection coll = shapes.GetCollection(col);
			collectionMapping.Add(coll.BitmapCount);
			// Debug.Log(collectionMapping[col]);
			ShapeDescriptor d = new ShapeDescriptor();
			d.CLUT = 0;
			d.Collection = (byte) col;
			//int textureSize = 64;
			for (byte i = 0; i < coll.BitmapCount; ++i) {
				d.Bitmap = i;
				
				Texture2D bitmap = shapes.GetShape(d);
				// if (col == 20) {Debug.Log(d.GetType().);}
				// if (landscape) {
				// 	int W = 192;
				// 	int H = (int) Math.Round((double) bitmap.height * W / bitmap.width);
				// 	bitmap.Resize(W,H);
				// } else {
				// 	//bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
				// 	//bitmap = ImageUtilities.ResizeImage(bitmap, textureSize, textureSize);
				// }
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
			loadingText = "Loading Shapes... " + col + "/31";
			if (col % 7 == 0 ){
				yield return null;
			}
		}
		loadingText = "Loading Shapes... 31/31";


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
		//for (int p = 0; p < 2; p++) {
			GameObject pol = Instantiate(polygon);
			pol.name = "Polygon" + p;
			pol.tag = "polygon";
			MapSegment seg = pol.GetComponent<MapSegment>();
			segments.Add(seg);
			seg.height = new Vector3(0,(float)(Level.Polygons[p].CeilingHeight - Level.Polygons[p].FloorHeight)/1024f,0);
			if(Level.Polygons[p].Type == Weland.PolygonType.Platform) {
				foreach (Weland.Platform pl in Level.Platforms) {
					if (pl.PolygonIndex == p) {
						if (pl.PolygonIndex ==6) {
							;
						}
						seg.platform = new Platform();
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
				Debug.Log(Level.Polygons[p].MediaIndex);
				Media media = Level.Medias[Level.Polygons[p].MediaIndex];
				seg.liquid.currentSpeed = (float)media.CurrentMagnitude/1024f;
				seg.liquid.currentDirectioin = Quaternion.Euler(0, (float)media.Direction+90, 0);
				seg.liquid.high = (float)media.High/1024f;
				seg.liquid.low = (float)media.Low/1024f;
				// media.
				//?? where is the liquid data stored???
				//switch (media.Type) {
					// case MediaType.Goo: 
					// 		seg.liquid.
							

					// 		Shapes.	
				//}
				//media.Type =
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
			
			List<Vector3> points = new List<Vector3>();

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

			List<Weland.Line> Line = new List<Weland.Line>();
			List<Weland.Side> Sides = new List<Weland.Side>();
			int currentLine = -1;
			for (int ln = 0; ln < Level.Polygons[p].VertexCount; ln++) {

				Line.Add(Level.Lines[Level.Polygons[p].LineIndexes[ln]]);
				if (Level.Polygons[p].SideIndexes[ln] >=0) {
					Sides.Add(Level.Sides[Level.Polygons[p].SideIndexes[ln]]);
				}
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

				points.Add(new Vector3(
							(float)Level.Endpoints[ei[pt]].X/1024f,
							0,
							(float)Level.Endpoints[ei[pt]].Y/1024f)
				);

				MapSegmentSide mss = new MapSegmentSide();
				mss.transparent = Line[currentLine].Transparent;
				mss.solid = Line[currentLine].Solid;
				Side side = new Side();
				if (p == 61 && Line[currentLine].Solid && Line[currentLine].Transparent ) {
					;
				}
				if (Line[currentLine].ClockwisePolygonSideIndex >= 0 ) {
					side = Level.Sides[Line[currentLine].ClockwisePolygonSideIndex];
				} else if (Line[currentLine].CounterclockwisePolygonSideIndex >= 0 ) {
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

				if (side.IsControlPanel|| side.IsPlatformSwitch() || side.IsTagSwitch() || side.IsLightSwitch()) {
						// if (p == 4) {
						// 	;
						// }
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
					// Debug.Log(p);
					// Debug.Log(mss.controlPanel.permutation);
				}

				seg.sides.Add(mss);
				if (Line[currentLine].ClockwisePolygonOwner == p) {
					mss.connectionID = Line[currentLine].CounterclockwisePolygonOwner;
				} else {
					mss.connectionID = Line[currentLine].ClockwisePolygonOwner;
				}
				pt = Mathf.Abs(pt - 1);
				lastPt = ei[pt];

				Line.RemoveAt(currentLine);

				for (int i = 0; i < Line.Count; i++) {
					if (Line[i].EndpointIndexes[0] == ei[pt] || Line[i].EndpointIndexes[1] == ei[pt]){
						currentLine = i;
					}
				}


			}
			int collectionNo = Level.Polygons[p].CeilingTexture.Collection;
			int texNo = Level.Polygons[p].CeilingTexture.Bitmap;
			// Debug.Log(collectionNo);
			// Debug.Log(texNo);
			seg.ceiling.upperMaterial = getTexture(Level.Polygons[p].CeilingTexture);
			seg.floor.upperMaterial = getTexture(Level.Polygons[p].FloorTexture);

			seg.ceiling.upperOffset = new Vector2((float)Level.Polygons[p].CeilingOrigin.X/1024f,(float)Level.Polygons[p].CeilingOrigin.Y/1024f);
			seg.floor.upperOffset = new Vector2((float)Level.Polygons[p].FloorOrigin.X/1024f,(float)Level.Polygons[p].FloorOrigin.Y/1024f);

			seg.ceiling.lightID = Level.Polygons[p].CeilingLight;
			seg.ceiling.light = lights[Level.Polygons[p].CeilingLight];
			seg.floor.lightID = Level.Polygons[p].FloorLight;
			seg.floor.light = lights[Level.Polygons[p].FloorLight];
			//seg.levelSegments = segments;
			seg.vertices = points;
			seg.centerPoint = new Vector3(0,(float)Level.Polygons[p].FloorHeight/1024f,0);
			seg.id = p;
			seg.calculatePoints();
		
			if (p % 77 == 0 ){
				loadingText = load + "\nGenerating Polygons "+p+"/"+Level.Polygons.Count;
				yield return null;
			}

		}

		load = load + "\nGenerating Polygons "+Level.Polygons.Count+"/"+Level.Polygons.Count;

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
			// if (s.id == 61) {
			// 	;
			// }
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
			s.checkIfImpossible();
			if (count % 77 == 0 ){
				loadingText = load + "\nFinding Impossible Space "+count+"/"+segments.Count;
				yield return null;
			}
		}

		load = load + "\nFinding Impossible Space "+count+"/"+segments.Count;
		count = 0;
		foreach(MapSegment s in segments) {
			count++;
			s.calculateVisibility();
			if (!GlobalData.skipOcclusion && count % 7 == 0) {
				loadingText = load + "\nOcclusion Culling "+count+"/"+segments.Count;
				yield return null;
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




	IEnumerator spawnEntitiesFromMarathonMap(Weland.Level Level) {
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

// if (i == 115) {
// 	;
// }				
				Vector3 rpos = pos;
				rpos.y += 0.1f;
				GameObject item = Instantiate(Resources.Load<GameObject>("itemObject"), rpos, facing);
				GameObject sprite = Instantiate(Resources.Load<GameObject>("spriteObject"), rpos, facing);
				sprite.transform.parent = item.transform;
				spriteController sc = sprite.GetComponent<spriteController>();
				sc.parent = item;
				sc.sideCount = 1;
				List<Material> frames = new List<Material>();
				Weland.ShapeDescriptor tex = new Weland.ShapeDescriptor();
				tex.Collection = 7;
				tex.Bitmap = (byte)(obj.Index);
				frames.Add(getTexture(tex));
				sc.frames = frames;
				item.name = "item" + i;
			}

			if (obj.Type == ObjectType.Sound ) {
			}

			if (obj.Type == ObjectType.Scenery ) {
			}

			if (obj.Type == ObjectType.Goal ) {
			}


			if (obj.Type == ObjectType.Monster ) {
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

		if (materials.Count >= retval && retval >= 0) {
			return materials[retval];
		} else {
			//return new Material(Shader.Find("Custom/StandardClippableV2"));
			return null;
		}
	}


	public void ClearLog()
	{
		var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
		var type = assembly.GetType("UnityEditorInternal.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}
}


