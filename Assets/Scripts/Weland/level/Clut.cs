using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Weland {
public class Clut : ISerializableBE {
	public static readonly uint Tag = Wadfile.Chunk("clut");
	public void Load(BinaryReaderBE reader) {
		long start = (int)reader.BaseStream.Position;

		
		UInt32 size;
		
		size = reader.ReadUInt32();
		
		Debug.Log("clut");

	}
	public void Save(BinaryWriterBE writer) {
	}

}
}