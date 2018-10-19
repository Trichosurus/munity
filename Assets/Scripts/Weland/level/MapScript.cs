using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Weland {
public class MapScript : ISerializableBE {
	public static readonly uint Tag = Wadfile.Chunk("MMLS");
	public static readonly uint Tag2 = Wadfile.Chunk("LUAS");
	public static readonly int ScriptNameLength = 64 + 2;
	public short NumScripts;

	public List<Script> Scripts = new List<Script>();

	public class Script {
		public int Flags;
		public int Length;
		public string Name;
		public string Text;

		public void Load(BinaryReaderBE reader) {

			Flags = reader.ReadInt32();
			Name = reader.ReadMacString(ScriptNameLength);
			Length = reader.ReadInt32();

			byte[] textBytes = reader.ReadBytes(Length);

			Text = Encoding.UTF8.GetString(textBytes);

		}
	}
	public void Load(BinaryReaderBE reader) {

		NumScripts = reader.ReadInt16();
		

		for (int i = 0; i < NumScripts; ++i) {
			Script s = new Script();
			s.Load(reader);
			Scripts.Add(s);
		}

	}
	public void Save(BinaryWriterBE writer) {
	}

}
}