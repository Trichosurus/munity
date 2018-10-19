using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Weland {
public class Terminal : ISerializableBE {
	public static readonly uint Tag = Wadfile.Chunk("term");
	public short TotalLength;
	public short GroupingCount;
	public short Flags;
	public short LinesPerPage;
	public short FontChangeCount;
	public List<Grouping> Groupings = new List<Grouping>();
	public List<FontChange> FontChanges = new List<FontChange>();
	private byte[] textBytes;
	public string[] Texts;

	public class Grouping {
		public short Flags;
		public short Permutation;
		public short Type;
		public short StartIndex;
		public short Length;
		public short MaximumLineCount;

		public void Load(BinaryReaderBE reader) {
			Flags = reader.ReadInt16();
			Type = reader.ReadInt16();
			Permutation = reader.ReadInt16();
			StartIndex = reader.ReadInt16();
			Length = reader.ReadInt16();
			MaximumLineCount = reader.ReadInt16();
		}
	}

	public class FontChange {
		public short Index;
		public short Face;
		public short Colour;

		public void Load(BinaryReaderBE reader) {
			Index = reader.ReadInt16();
			Face = reader.ReadInt16();
			Colour = reader.ReadInt16();
		}
	}



	public void Load(BinaryReaderBE reader) {
		long start = (int)reader.BaseStream.Position;

	    TotalLength = reader.ReadInt16();
	    Flags = reader.ReadInt16();
	    LinesPerPage = reader.ReadInt16();
	    GroupingCount = reader.ReadInt16();
	    FontChangeCount = reader.ReadInt16();

		Groupings = new List<Grouping>();
		FontChanges = new List<FontChange>();
		

		for (int i = 0; i < GroupingCount; ++i) {
			Grouping grouping = new Grouping();
			grouping.Load(reader);
			Groupings.Add(grouping);
		}

		for (int i = 0; i < FontChangeCount; ++i)	{
			FontChange fontChange = new FontChange();
			fontChange.Load(reader);
			FontChanges.Add(fontChange);
		}


		int textLength = TotalLength - (int)(reader.BaseStream.Position - start);

		textBytes = reader.ReadBytes(textLength);
		
		if (Flags == 1) {

		//decode text
			for (int i = 0; i < textBytes.Length; ++i) {
				i++;
				if (textBytes.Length > ++i) {
					textBytes[i] ^= 0xfe;
				}
				if (textBytes.Length > ++i) {	
					textBytes[i] ^= 0xed;
				}
			}

		}


		
		Encoding macRoman = Encoding.GetEncoding(10000);
		Texts = macRoman.GetString(textBytes).Split('\0');
	}
	public void Save(BinaryWriterBE writer) {
	}

}
}