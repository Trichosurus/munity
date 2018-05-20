using System;
// using System.Drawing;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Weland {
    class CollectionHeader {
	public short Status;
	public ushort Flags;
	public int Offset;
	public int Length;
	public int Offset16;
	public int Length16;
	
	public void Load(BinaryReaderBE reader) {
	    Status = reader.ReadInt16();
	    Flags = reader.ReadUInt16();
	    Offset = reader.ReadInt32();
	    Length = reader.ReadInt32();
	    Offset16 = reader.ReadInt32();
	    Length16 = reader.ReadInt32();
	    reader.BaseStream.Seek(12, SeekOrigin.Current);
	}
    }

    public enum CollectionType : short {
	Unused,
	Wall,
	Object,
	Interface,
	Scenery
    }

	public enum SequenceType : short {
        Animated1 = 1,		// simple isotropic animation
		Animated2TO8 = 2,	// 8 view animation
		Animated3TO4 = 3,	// 4 view animation
		Animated4 = 4,		// 4 view animation
		Animated5TO8 = 5,	// 8 view animation
		Animated8 = 8,		// 8 view animation
		Animated3TO5 = 9,	// 5 view animation
		NotAnimated = 10,	// no animation, choose a random frame
		Animated5 = 11		// 5 view animation
	};

    public class Collection {
#pragma warning disable 0414
	public short Version;
	public CollectionType Type;
	public ushort Flags;
	
	short colorCount;
	short colorTableCount;
	int colorTableOffset;

	short highLevelShapeCount;
	int highLevelShapeOffsetTableOffset;

	short lowLevelShapeCount;
	int lowLevelShapeOffsetTableOffset;

	public short BitmapCount {
	    get {
		return bitmapCount;
	    }
	}

	public int ColorTableCount {
	    get {
		return colorTables.Count;
	    }
	}

	short bitmapCount;
	int bitmapOffsetTableOffset;

	public short pixelsToWorld;
	int size;

#pragma warning restore 0414

	struct ColorValue {
	    public byte Flags;
	    public byte Value;
	    
	    public ushort Red;
	    public ushort Green;
	    public ushort Blue;

	    public void Load(BinaryReaderBE reader) {
		Flags = reader.ReadByte();
		Value = reader.ReadByte();
		
		Red = reader.ReadUInt16();
		Green = reader.ReadUInt16();
		Blue = reader.ReadUInt16();
	    }
	}

	public struct ShapeSequence {
	    public ushort Flags;
	    public short Type;
	    
	    public string Name;
	    public short NoOfViews;
	    public short FramesPerView;
	    public short TickePerFrame;
	    public short KeyFrame;
	    public short TrasferMode;
	    public short TransferModePeriod;
	    public short FirstFrameSound;
	    public short KeyFrameSound;
	    public short LastFrameSound;
	    public short PixelsToWorld;
	    public short LoopFrame;

		public List<int> FrameIndexes;
		// private short nameLen;
	    public void Load(BinaryReaderBE reader) {
		reader.ReadBytes(1);
		// reader.ReadBytes(1);
		Type = reader.ReadInt16();
		// reader.ReadBytes(1);
		Flags = reader.ReadUInt16();

		// long position = reader.BaseStream.Position;
		// position += 1;//???
		// reader.BaseStream.Seek(position, SeekOrigin.Begin);

		// reader.ReadBytes(1);
		Name = reader.ReadMacString(33);
		NoOfViews = reader.ReadInt16();
		FramesPerView = reader.ReadInt16();
		TickePerFrame = reader.ReadInt16();
		KeyFrame = reader.ReadInt16();
		TrasferMode = reader.ReadInt16();
		TransferModePeriod = reader.ReadInt16();
		FirstFrameSound = reader.ReadInt16();
		KeyFrameSound = reader.ReadInt16();
		LastFrameSound = reader.ReadInt16();
		PixelsToWorld = reader.ReadInt16();
		LoopFrame = reader.ReadInt16();

		// long position = reader.BaseStream.Position;
		// position += 28;//???
		// reader.BaseStream.Seek(position, SeekOrigin.Begin);
		reader.ReadBytes(28);
		short nov = NoOfViews;
		int idxCount = getRealViewCount(nov) * FramesPerView;
		FrameIndexes = new List<int>();
		for (int i = 0; i < idxCount; i++) {
			short frameIndex = reader.ReadInt16();
			FrameIndexes.Add(frameIndex);
		}		
	    }

		public int getRealViewCount(short sequenceType) {
			switch (sequenceType) {
				case (short)SequenceType.NotAnimated:
					return 1;
				case (short)SequenceType.Animated1:
					return 1;
				case (short)SequenceType.Animated3TO4:
					return 4;
				case (short)SequenceType.Animated4:
					return 4;
				case (short)SequenceType.Animated3TO5:
					return 5;
				case (short)SequenceType.Animated5:
					return 5;
				case (short)SequenceType.Animated2TO8:
					return 8;
				case (short)SequenceType.Animated5TO8:
					return 8;
				case (short)SequenceType.Animated8:
					return 8;
			}
			return -1;
		}

		
	}

	public struct ShapeFrame {

		public enum ShapeFrameFlags : ushort {
		XMirror = 0x8000,
		YMirror = 0x4000,
		KeypointObscured = 0x2000

		}

	    public ushort Flags;
	    public short BitmapIndex;
	    public bool XMirror;
	    public bool YMirror;
	    public bool KeypointObscured;
	    
	    public float MinimumLightIntensity;
	    public short OriginX;
	    public short OriginY;
	    public short KeypointX;
	    public short KeypointY;
	    public short WorldY;
	    public short WorldX;
	    public short WorldTop;
	    public short WorldBottom;
	    public short WorldLeft;
	    public short WorldRight;

	    public void Load(BinaryReaderBE reader) {
		Flags = reader.ReadUInt16();
		XMirror = (Flags & (ushort)ShapeFrameFlags.XMirror) != 0;
		YMirror = (Flags & (ushort)ShapeFrameFlags.YMirror) != 0;
		KeypointObscured = (Flags & (ushort)ShapeFrameFlags.KeypointObscured) != 0;
		MinimumLightIntensity = (float)reader.ReadInt32()/65535f;
		BitmapIndex = reader.ReadInt16();
		OriginX = reader.ReadInt16();
		OriginY = reader.ReadInt16();
		WorldLeft = reader.ReadInt16();
		WorldRight = reader.ReadInt16();
		WorldTop = reader.ReadInt16();
		WorldBottom = reader.ReadInt16();
		WorldX = reader.ReadInt16();
		WorldY = reader.ReadInt16();
	    }
	}


	List<ColorValue[]> colorTables = new List<ColorValue[]>();
	List<Bitmap> bitmaps = new List<Bitmap>();

	public List<ShapeFrame> frames = new List<ShapeFrame>();
	public List<ShapeSequence> sequences = new List<ShapeSequence>();

	public void Load(BinaryReaderBE reader) {
	    long origin = reader.BaseStream.Position;

	    Version = reader.ReadInt16();
	    Type = (CollectionType) reader.ReadInt16();
	    Flags = reader.ReadUInt16();
	    colorCount = reader.ReadInt16();
	    colorTableCount = reader.ReadInt16();
	    colorTableOffset = reader.ReadInt32();
	    highLevelShapeCount = reader.ReadInt16();
	    highLevelShapeOffsetTableOffset = reader.ReadInt32();
	    lowLevelShapeCount = reader.ReadInt16();
	    lowLevelShapeOffsetTableOffset = reader.ReadInt32();
	    bitmapCount = reader.ReadInt16();
	    bitmapOffsetTableOffset = reader.ReadInt32();
	    pixelsToWorld = reader.ReadInt16();
	    size = reader.ReadInt32();
	    reader.BaseStream.Seek(253 * 2, SeekOrigin.Current);

	    colorTables.Clear();
	    reader.BaseStream.Seek(origin + colorTableOffset, SeekOrigin.Begin);
	    for (int i = 0; i < colorTableCount; ++i) {
		ColorValue[] table = new ColorValue[colorCount];
		for (int j = 0; j < colorCount; ++j) {
		    table[j].Load(reader);
		}
		colorTables.Add(table);
	    }

	    reader.BaseStream.Seek(origin + bitmapOffsetTableOffset, SeekOrigin.Begin);
	    bitmaps.Clear();
	    for (int i = 0; i < bitmapCount; ++i) {
		int offset = reader.ReadInt32();
		long position = reader.BaseStream.Position;
		reader.BaseStream.Seek(origin + offset, SeekOrigin.Begin);
		Bitmap bitmap = new Bitmap();
		bitmap.Load(reader);
		bitmaps.Add(bitmap);
		reader.BaseStream.Seek(position, SeekOrigin.Begin);
	    }

	    reader.BaseStream.Seek(origin + lowLevelShapeOffsetTableOffset, SeekOrigin.Begin);
	    frames.Clear();
	    for (int i = 0; i < lowLevelShapeCount; ++i) {
		int offset = reader.ReadInt32();
		long position = reader.BaseStream.Position;
		reader.BaseStream.Seek(origin + offset, SeekOrigin.Begin);
		ShapeFrame frame = new ShapeFrame();
		frame.Load(reader);
		frames.Add(frame);
		reader.BaseStream.Seek(position, SeekOrigin.Begin);
	    }

	    reader.BaseStream.Seek(origin + highLevelShapeOffsetTableOffset, SeekOrigin.Begin);
	    sequences.Clear();
	    for (int i = 0; i < highLevelShapeCount; ++i) {
		int offset = reader.ReadInt32();
		long position = reader.BaseStream.Position;
		reader.BaseStream.Seek(origin + offset, SeekOrigin.Begin);
		ShapeSequence sequence = new ShapeSequence();
		sequence.Load(reader);
		sequences.Add(sequence);
		reader.BaseStream.Seek(position, SeekOrigin.Begin);
	    }




	}

	public Texture2D GetShape(byte ColorTableIndex, byte BitmapIndex) {
	    Bitmap bitmap = bitmaps[BitmapIndex];
	    ColorValue[] colorTable = colorTables[ColorTableIndex];
	    Color[] colors = new Color[colorTable.Length];
		bool hasAlpha = false;
	    for (int i = 0; i < colorTable.Length; i++) {
			ColorValue color = colorTable[i];
		 	colors[i].r = (float)color.Red/65535f;
		 	colors[i].g = (float)color.Green/65535f;
		 	colors[i].b = (float)color.Blue/65535f;
			if (colors[i].r != 0.0f || colors[i].g != 0.0f || colors[i].b != 1.0f){
		 		colors[i].a = 1;
			} else {
		 		colors[i].a = 0;
			}
		}
		Texture2D result;
		for (int i = 0; i < bitmap.Data.Length; i++) {
			if (bitmap.Data[i] == 0) {
				hasAlpha = true;
			}
		}
		if (hasAlpha) {
			result = new Texture2D(bitmap.Width, bitmap.Height,TextureFormat.ARGB32,true);
		} else {
			result = new Texture2D(bitmap.Width, bitmap.Height,TextureFormat.RGB24,true);
		}
		if (!bitmap.ColumnOrder) {
			for (int y = 0; y < bitmap.Height; y++) {
				for (int x = 0; x < bitmap.Width; x++) {
					result.SetPixel(x, bitmap.Height-y-1, colors[bitmap.Data[x + y * bitmap.Width]]);

				}
			}
		} else {
			for (int x = 0; x < bitmap.Width; x++) {
				for (int y = 0; y < bitmap.Height; y++) {
					result.SetPixel(x,bitmap.Height-y-1, colors[bitmap.Data[x * bitmap.Height + y]]);

				}
			}

		}
		return result;
	}
    }
}