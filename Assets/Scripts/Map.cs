﻿using System.Collections;
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
	public List<GameObject> segments;

	// Use this for initialization
	void Start () {
	    ShapesFile shapes = new ShapesFile();
		shapes.Load(GlobalData.shapesFilePath);
	    //shapes.Load("/home/alex/marathon/makePolys/Assets/Resources/Marathon Infinity/Shapes.shpA");
	    //shapes.Load("/home/alex/marathon/makePolys/Assets/Resources/Marathon 2/Shapes.shpA");
	    //shapes.Load("/home/alex/marathon/makePolys/Assets/Resources/M1A1/M1A1 Shapes.shpA");
		makeMaterialsFromShapesFile(shapes);

		// ClearLog();
		Wadfile wadfile = new Wadfile();
		wadfile.Load(GlobalData.mapsFilePath);
		//wadfile.Load("/home/alex/Desktop/porting/Aleph One/Marathon Infinity/Ne Cede Malis.sceA");
		//wadfile.Load("/home/alex/marathon/makePolys/Assets/Resources/Marathon Infinity/Map.sceA");
		//wadfile.Load("/home/alex/marathon/makePolys/Assets/Resources/Marathon 2/Map.sceA");
		//wadfile.Load("/home/alex/marathon/makePolys/Assets/Resources/M1A1/M1A1 Map.sceA");
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
		
		makeWorldFromMarathonMap(Level);
		spawnEntitiesFromMarathonMap(Level);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void makeMaterialsFromShapesFile(ShapesFile shapes) {
	    

		for (int col = 0; col < 32; col++) {
			//if (col != 20) {	collectionMapping.Add(0);continue;}
			bool landscape = col >= 27;
			Collection coll = shapes.GetCollection(col);
			collectionMapping.Add(coll.BitmapCount);
			// Debug.Log(collectionMapping[col]);
			ShapeDescriptor d = new ShapeDescriptor();
			d.CLUT = 0;
			d.Collection = (byte) col;
			int textureSize = 64;

			for (byte i = 0; i < coll.BitmapCount; ++i) {
				d.Bitmap = i;
				
				Texture2D bitmap = shapes.GetShape(d);
				// if (landscape) {
				// 	int W = 192;
				// 	int H = (int) Math.Round((double) bitmap.height * W / bitmap.width);
				// 	bitmap.Resize(W,H);
				// } else {
				// 	//bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
				// 	//bitmap = ImageUtilities.ResizeImage(bitmap, textureSize, textureSize);
				// }
				bitmap.Apply();

				Material  material = new Material(Shader.Find("Standard"));

				material.SetFloat("_Glossiness",0.0f);
				if (bitmap.format == TextureFormat.ARGB32) {
					material.SetFloat("_Mode",1f);
					material.SetFloat("_Cutoff",0.85f);
					material.EnableKeyword("_ALPHABLEND_ON");
					material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
					material.EnableKeyword("_ALPHATEST_ON");
				} else {
					Debug.Log("noalpha");
				}
				material.mainTexture = bitmap;
				materials.Add(material);
			//	Debug.Log(material);
			}		
		}


	}

	void makeWorldFromMarathonMap(Weland.Level Level) {
		//marathon maps have y +/- directions swapped so fix that
		for (int i = 0; i < Level.Endpoints.Count; i++) {
			Level.Endpoints[i] = new Point(Level.Endpoints[i].X, (short)(0-Level.Endpoints[i].Y));
		}
		//now we can generate mapsegment objects from each map polygon
		for (int p = 0; p < Level.Polygons.Count; p++) {
		//for (int p = 0; p < 2; p++) {
			GameObject pol = Instantiate(polygon);
			segments.Add(pol);
			MapSegment seg = pol.GetComponent<MapSegment>();
			seg.height = new Vector3(0,(float)(Level.Polygons[p].CeilingHeight - Level.Polygons[p].FloorHeight)/1024f,0);
			if(Level.Polygons[p].Type == Weland.PolygonType.Platform) {
				foreach (Weland.Platform pl in Level.Platforms) {
					if (pl.PolygonIndex == p) {
						seg.platform = new Platform();
						seg.platform.ComesFromCeiling = pl.ComesFromCeiling;
						seg.platform.ComesFromFloor = pl.ComesFromFloor;
						seg.platform.InitiallyExtended = pl.InitiallyExtended;
						seg.platform.MaximumHeight = (float)pl.MaximumHeight/1024f;
						seg.platform.MinimumHeight = (float)pl.MinimumHeight/1024f;
						seg.platform.Speed = (float)pl.Speed/1024f;
						seg.platform.UsesNativePolygonHeights = pl.UsesNativePolygonHeights;
						seg.platform.IsDoor = pl.IsDoor;
						seg.platform.InitiallyActive = pl.InitiallyActive;
						// if (p == 6 || p == 4|| p == 431) {
						// 	Debug.Log(p);
						// 	Debug.Log(seg.platform.InitiallyExtended);
						// 	Debug.Log(seg.platform.MaximumHeight);
						// 	Debug.Log(seg.platform.MinimumHeight);
						// 	Debug.Log(seg.platform.Speed);
						// 	Debug.Log(seg.platform.UsesNativePolygonHeights);
							
						// }
					}
				}
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
				
						if (Math.Atan2(b.y - a.y, b.x - a.x) < Math.Atan2(c.y - a.y, c.x - a.x)) {
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

				if (mss.lowerMaterial == null) {
					mss.lowerMaterial = mss.upperMaterial;
					mss.lowerOffset = mss.upperOffset;
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
			seg.Ceiling.upperMaterial = getTexture(Level.Polygons[p].CeilingTexture);
			seg.Floor.upperMaterial = getTexture(Level.Polygons[p].FloorTexture);

			seg.Ceiling.upperOffset = new Vector2((float)Level.Polygons[p].CeilingOrigin.X/1024f,(float)Level.Polygons[p].CeilingOrigin.Y/1024f);
			seg.Floor.upperOffset = new Vector2((float)Level.Polygons[p].FloorOrigin.X/1024f,(float)Level.Polygons[p].FloorOrigin.Y/1024f);
			seg.levelSegments = segments;
			seg.vertices = points;
			seg.centerPoint = new Vector3(0,(float)Level.Polygons[p].FloorHeight/1024f,0);
			seg.id = p;
			seg.calculatePoints();
		}

		foreach(Weland.Platform pl in Level.Platforms) {
			segments[pl.PolygonIndex].GetComponent<MapSegment>().recalculatePlatformVolume();
			segments[pl.PolygonIndex].GetComponent<MapSegment>().makePlatformObjects();
		}
		foreach(GameObject s in segments) {
			s.GetComponent<MapSegment>().generateMeshes();
		}
		// foreach(Weland.Platform pl in Level.Platforms) {
		// }

	}


	void spawnEntitiesFromMarathonMap(Weland.Level Level) {
		bool playerSpawned = false;
		foreach (MapObject obj in Level.Objects) {
			if (obj.Type == ObjectType.Player) {
				playerSpawned = true;
				Debug.Log(obj.X);
				Debug.Log(obj.Y);
				Debug.Log(obj.Z);
				Debug.Log(obj.Facing);
			
				Vector3 pos = new Vector3(pos.x = (float)obj.X/1024f,
										pos.y = (float)obj.Z/1024f,
										pos.z = 0f-(float)obj.Y/1024f
										);			
				Debug.Log(pos);

				Quaternion facing = Quaternion.Euler(0, (float)obj.Facing+90, 0);
				GameObject player = Instantiate(Resources.Load<GameObject>("player"), pos, facing);
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
			//return new Material(Shader.Find("Standard"));
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
