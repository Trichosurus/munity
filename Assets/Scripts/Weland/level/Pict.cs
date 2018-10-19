using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Weland {
public class Pict : ISerializableBE {
	public static readonly uint Tag = Wadfile.Chunk("pict");
	public static readonly uint Tag2 = Wadfile.Chunk("PICT");
	public string name = "";

	public void LoadWithName (BinaryReaderBE reader, string name) {
		this.name = name;
		Load(reader);
	}
	public void Load(BinaryReaderBE reader) {
		long start = (int)reader.BaseStream.Position;

		
		UInt32 size;
		
		size = reader.ReadUInt32();
		
		Debug.Log("pict");

	}
	public void Save(BinaryWriterBE writer) {
	}

}
}