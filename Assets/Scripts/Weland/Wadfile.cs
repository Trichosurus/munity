using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace Weland {
    enum WadfileVersion {
	PreEntryPoint,
	HasDirectoryEntry,
	SupportsOverlays,
	HasInfinityStuff
    }

    enum WadfileDataVersion {
	Marathon,
	MarathonTwo
    }

    public class Wadfile {
	const int headerSize = 128;

	public static uint Chunk(string s) {
	    return (uint) ((((byte) s[0]) << 24) | (((byte) s[1]) << 16) | (((byte) s[2]) << 8) | ((byte) s[3]));
	}

	public short Version {
	    get {
		return version;
	    }
	}
	short version;

	public short DataVersion;
	public string Filename;
	const int maxFilename = 64;
	public uint Checksum {
	    get {
		return checksum;
	    }
	}
	uint checksum;

	int directoryOffset;
	public int WadCount {
	    get {
		return Directory.Count;
	    }
	}

	protected short applicationSpecificDirectoryDataSize;
	short entryHeaderSize;
	short directoryEntryBaseSize;
	public uint ParentChecksum;
	public List<Pict> pictResources = new List<Pict>();
	public List<Clut> clutResources = new List<Clut>();
	public List<SoundResource> soundResources = new List<SoundResource>();
	public List<TextResource> textResources = new List<TextResource>();
	
	public class BadMapException : Exception {
	    public BadMapException()
		{ 
		}
			
	    public BadMapException(string message) : base(message)
		{
		}
			
	    public BadMapException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class DirectoryEntry {
	    internal const short BaseSize = 10;

	    public Dictionary<uint, byte[]> Chunks = new Dictionary<uint, byte[]> ();
	    internal const short HeaderSize = 16;
	    internal int Offset;
	    internal short Index;
	    public int Size {
		get {
		    int total = 0;
		    foreach (var kvp in Chunks) {
			total += kvp.Value.Length + HeaderSize;
		    }
		    return total;
		}
	    }

 	    internal void LoadEntry(BinaryReaderBE reader) {
			Offset = reader.ReadInt32();
			reader.ReadInt32(); // size
			Index = reader.ReadInt16();
	    }
	    
	    internal void LoadChunks(BinaryReaderBE reader) {
		long position = reader.BaseStream.Position;
		int nextOffset;
		do {
		    uint tag = reader.ReadUInt32();
		    nextOffset = reader.ReadInt32();
		    int length = reader.ReadInt32();
		    reader.ReadInt32(); // offset

					
		    Chunks[tag] = reader.ReadBytes(length);
					
		    if (nextOffset > 0) 
			reader.BaseStream.Seek(position + nextOffset, SeekOrigin.Begin);
		} while (nextOffset > 0);
	    }

	    internal void SaveEntry(BinaryWriterBE writer) {
		writer.Write(Offset);
		writer.Write((int) Size);
		writer.Write(Index);
	    }	

	    internal void SaveChunks(BinaryWriterBE writer, uint[] tagOrder) {
		// build a list of tags to write in order
		HashSet<uint> Used = new HashSet<uint>();
		List<uint> Tags = new List<uint>();

		foreach (uint tag in tagOrder) {
		    if (Chunks.ContainsKey(tag)) {
			Tags.Add(tag);
			Used.Add(tag);
		    }
		}

		foreach (var kvp in Chunks) {
		    if (!Used.Contains(kvp.Key)) {
			Tags.Add(kvp.Key);
			Used.Add(kvp.Key);
		    }
		}

		int offset = 0;

		foreach (uint tag in Tags) {
		    writer.Write(tag);
		    if (tag == Tags[Tags.Count - 1]) {
			writer.Write((uint) 0);
		    } else {
			writer.Write((int) offset + HeaderSize + Chunks[tag].Length);
		    }
		    writer.Write((int) Chunks[tag].Length);
		    writer.Write((int) 0);
		    writer.Write(Chunks[tag]);
		    offset += Chunks[tag].Length + HeaderSize;
		}
	    }

	    public DirectoryEntry Clone() {
		DirectoryEntry clone = (DirectoryEntry) MemberwiseClone();
		clone.Chunks = new Dictionary<uint, byte[]>();
		foreach (var kvp in Chunks) {
		    clone.Chunks[kvp.Key] = (byte[]) kvp.Value.Clone();
		}
		return clone;
	    }
	}

	int MacBinaryHeader(byte[] header) {
	    if (header[0] != 0 || header[1] > 64 || header[74] != 0 || header[123] > 0x81)
		return 0;

	    ushort crc = 0;
	    for (int i = 0; i < 124; ++i) {
		ushort data = (ushort) (header[i] << 8);
		for (int j = 0; j < 8; ++j) {
		    if (((data ^ crc) & 0x8000) == 0x8000)
			crc = (ushort) ((crc << 1) ^ 0x1021);
		    else
			crc <<= 1;
		    data <<= 1;
		}
	    }

		int data_length = (header[83] << 24) | (header[84] << 16) | (header[85] << 8) | header[86];
		int resource_length = (header[87] << 24) | (header[88] << 16) | (header[89] << 8) | header[90];
		int resourceOffset = 128 + ((data_length + 0x7f) & ~0x7f);

	    if (crc != ((header[124] << 8) | header[125]))
		return 0;

	    return data_length;
	}

	public SortedDictionary<int, DirectoryEntry> Directory = new SortedDictionary<int, DirectoryEntry> ();

        protected virtual void LoadApplicationSpecificDirectoryData(BinaryReaderBE reader, int index) {
            reader.ReadBytes(applicationSpecificDirectoryDataSize);
        }

        protected virtual void SaveApplicationSpecificDirectoryData(BinaryWriterBE writer, int index) {

        }

        protected virtual void SetApplicationSpecificDirectoryDataSize() {
            applicationSpecificDirectoryDataSize = 0;
        }

        protected virtual uint[] GetTagOrder() {
            return new uint[] { };
        }

	public virtual void Load(string filename) {
	    BinaryReaderBE reader = new BinaryReaderBE(File.Open(filename, FileMode.Open));
	    try {
		// is it MacBinary?
		int fork_start = 0;
		int resourceOffset = MacBinaryHeader(reader.ReadBytes(128));
		if (resourceOffset > 0) {
		    fork_start = 128;
			resourceOffset = 128 + ((resourceOffset + 0x7f) & ~0x7f);
		}
		reader.BaseStream.Seek(fork_start, SeekOrigin.Begin);
		
		// read the header
		version = reader.ReadInt16();
		DataVersion = reader.ReadInt16();
		Filename = reader.ReadMacString(maxFilename);
		checksum = reader.ReadUInt32();
		directoryOffset = reader.ReadInt32();
		short wadCount = reader.ReadInt16();
		applicationSpecificDirectoryDataSize = reader.ReadInt16();
		entryHeaderSize = reader.ReadInt16();
		
		directoryEntryBaseSize = reader.ReadInt16();

		// sanity check the map
		if (Version < 2 || entryHeaderSize != 16 || directoryEntryBaseSize != 10) {
		    throw new BadMapException("Only Marathon 2 and higher maps are supported");
		}
		
		ParentChecksum = reader.ReadUInt32();
		reader.ReadBytes(2 * 20); // unused
		
		// load the directory
		reader.BaseStream.Seek(directoryOffset + fork_start, SeekOrigin.Begin);
		for (int i = 0; i < wadCount; ++i) {
		    DirectoryEntry entry = new DirectoryEntry();
		    entry.LoadEntry(reader);
		    Directory[entry.Index] = entry;

            LoadApplicationSpecificDirectoryData(reader, entry.Index);
		}
		
		// load all the wads(!)
		foreach (KeyValuePair<int, DirectoryEntry> kvp in Directory) {
		    reader.BaseStream.Seek(kvp.Value.Offset + fork_start, SeekOrigin.Begin);
		    kvp.Value.LoadChunks(reader);
		}

		if (resourceOffset > 0) {
			reader.BaseStream.Seek(resourceOffset, SeekOrigin.Begin);
			UInt32 dataOffset = reader.ReadUInt32();
			UInt32 mapOffset = reader.ReadUInt32();
			UInt32 dataLength = reader.ReadUInt32();
			UInt32 mapLength = reader.ReadUInt32();
			Debug.Log(resourceOffset);

			Debug.Log(dataOffset);
			Debug.Log(mapOffset);
			Debug.Log(dataLength);
			Debug.Log(mapLength);

			dataOffset += (UInt32)resourceOffset;
			mapOffset += (UInt32)resourceOffset;


			ResourceMap resourceMap = new ResourceMap();
			resourceMap.load(ref reader, mapOffset);

			for (int i = 0; i < resourceMap.refs.Count; i++) {
				string name = "";
				if (resourceMap.refs[i].nameListOffset > 0){
					reader.BaseStream.Seek(resourceMap.refs[i].nameListOffset + resourceMap.nameListOffset + mapOffset, SeekOrigin.Begin );
					int nameLength = reader.ReadByte();
					name = reader.ReadMacString(nameLength);
				}
				if (resourceMap.refs[i].type == Pict.Tag || resourceMap.refs[i].type == Pict.Tag2) {
					Pict pict = new Pict();
					reader.BaseStream.Seek(resourceMap.refs[i].dataOffset + dataOffset, SeekOrigin.Begin );
					pict.LoadWithName(reader, name);
					pictResources.Add(pict);
				}
				if (resourceMap.refs[i].type == Clut.Tag) {
					Clut clut = new Clut();
					reader.BaseStream.Seek(resourceMap.refs[i].dataOffset + dataOffset, SeekOrigin.Begin );
					clut.Load(reader);
					clutResources.Add(clut);
				}
				if (resourceMap.refs[i].type == SoundResource.Tag) {
					SoundResource snd = new SoundResource();
					reader.BaseStream.Seek(resourceMap.refs[i].dataOffset + dataOffset, SeekOrigin.Begin );
					snd.Load(reader);
					soundResources.Add(snd);
				}
				if (resourceMap.refs[i].type == TextResource.Tag || resourceMap.refs[i].type == TextResource.Tag2) {
					TextResource txt = new TextResource();
					reader.BaseStream.Seek(resourceMap.refs[i].dataOffset + dataOffset, SeekOrigin.Begin );
					txt.Load(reader);
					textResources.Add(txt);
				}


			}

		}


	    } finally {
		reader.Close();
	    }
	}

	class ResourceMap {
		public Int16 typeListOffset;
		public Int16 nameListOffset;
		public Int16 numTypes;
		public List<Type> types = new List<Type>();
		public List<ResRef> refs = new List<ResRef>();

		public void load (ref BinaryReaderBE reader, UInt32 mapOffset) {

			reader.BaseStream.Seek(mapOffset + 24, SeekOrigin.Begin);		

			typeListOffset = reader.ReadInt16();
			nameListOffset = reader.ReadInt16();
			numTypes = reader.ReadInt16();


			for (int i = 0; i <= numTypes; i++) {
				Type type = new Type();
				type.load(ref reader);
				types.Add(type);
			}

			for (int t = 0; t < types.Count; t ++) {
				for (int r = 0; r < types[t].numRefs; r ++) {
					ResRef resref = new ResRef();
					
					resref.load(ref reader);
					resref.type = types[t].type;
					refs.Add(resref);

				}
			}

		}

		public class Type {
			public UInt32 type;
			public Int16 numRefs;
			public Int16 refListOffset;

			public void load(ref BinaryReaderBE reader) {
				type = reader.ReadUInt32();
				numRefs = reader.ReadInt16();
				numRefs++;
				refListOffset = reader.ReadInt16();

				Debug.Log(type);
				Debug.Log(numRefs);
				Debug.Log(refListOffset);
			}
		}

		public class ResRef {
			public Int16 id;
			public Int16 nameListOffset;
			public UInt32 dataOffset;
			public UInt32 type;
			public void load(ref BinaryReaderBE reader) {
					id = reader.ReadInt16();
					nameListOffset = reader.ReadInt16();
					dataOffset = reader.ReadUInt32();
					dataOffset &= 0x00ffffff; //ignore attribures data
	    			reader.BaseStream.Seek(4, SeekOrigin.Current);

					Debug.Log(id);
					Debug.Log(nameListOffset);
					Debug.Log(dataOffset);
							
			}
		}
	
	}

	public void Save(string filename) {
	    using (FileStream fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write)) {
		CrcStream crcStream = new CrcStream(fs);
		BinaryWriterBE writer = new BinaryWriterBE(crcStream);

		// set up the header
		if (Directory.Count == 1) {
		    version = (short) WadfileVersion.SupportsOverlays;
		} else {
		    version = (short) WadfileVersion.HasInfinityStuff;
		}
	    
		DataVersion = (short) WadfileDataVersion.MarathonTwo;
		checksum = 0;
		directoryOffset = headerSize;
		foreach (var kvp in Directory) {
		    kvp.Value.Offset = directoryOffset;
		    kvp.Value.Index = (short) kvp.Key;
		    directoryOffset += kvp.Value.Size;
		}

                SetApplicationSpecificDirectoryDataSize();
		entryHeaderSize = DirectoryEntry.HeaderSize;
		directoryEntryBaseSize = DirectoryEntry.BaseSize;
		ParentChecksum = 0;
	    
		// write the header
		writer.Write(version);
		writer.Write(DataVersion);
		writer.WriteMacString(filename.Split('.')[0], maxFilename);
		writer.Write(checksum);
		writer.Write(directoryOffset);
		writer.Write((short) Directory.Count);
		writer.Write(applicationSpecificDirectoryDataSize);
		writer.Write(entryHeaderSize);
		writer.Write(directoryEntryBaseSize);
		writer.Write(ParentChecksum);
		writer.Write(new byte[2 * 20]);

		// write wads
		foreach (var kvp in Directory) {
		    kvp.Value.SaveChunks(writer, GetTagOrder());
		}

		// write directory
		foreach (var kvp in Directory) {
		    kvp.Value.SaveEntry(writer);
                    SaveApplicationSpecificDirectoryData(writer, kvp.Value.Index);
                }

		// fix the checksum!
		checksum = crcStream.GetCRC();
		fs.Seek(68, SeekOrigin.Begin);
		writer.Write(checksum);
	    }
	}


    public static uint FourCharsToInt(char a, char b, char c, char d) {
		return (uint) ((a << 24) | (b << 16) | (c << 8) | d);
	}

	static public void Main(string[] args) {
	    if (args.Length == 2) {
		Wadfile wadfile = new Wadfile();
		wadfile.Load(args[0]);

		Wadfile export = new Wadfile();
		export.Directory[0] = wadfile.Directory[0];
		export.Save(args[1]);
		Console.WriteLine("DirectoryOffset: {0}", export.directoryOffset);
	    } else {
		Console.WriteLine("Test usage: wadfile.exe <wadfile> <export>");
	    }
	}
    }
}
