using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Weland {
public class TextResource : ISerializableBE {
	public static readonly uint Tag = Wadfile.Chunk("text");
	public static readonly uint Tag2 = Wadfile.Chunk("TEXT");
	
	public void Load(BinaryReaderBE reader) {
		Debug.Log("clut");

	}
	public void Save(BinaryWriterBE writer) {
	}

}
}