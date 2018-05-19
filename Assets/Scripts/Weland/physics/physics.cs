using System;
using System.IO;
using System.Collections.Generic;

namespace Weland {

    public class PhysicsFile {



	void LoadChunkList<T>(List<T> list, byte[] data) where T : ISerializableBE, new() {
	    BinaryReaderBE reader = new BinaryReaderBE(new MemoryStream(data));
	    list.Clear();
	    while (reader.BaseStream.Position < reader.BaseStream.Length) {
		T t = new T();
		t.Load(reader);
		list.Add(t);
	    }
	}	


	public void Load(BinaryReaderBE reader) {
	    // long origin = reader.BaseStream.Position;
	    // collectionHeaders = new CollectionHeader[ShapeDescriptor.MaximumCollections];
	    // for (int i = 0; i < collectionHeaders.Length; ++i) {
		// collectionHeaders[i] = new CollectionHeader();
		// collectionHeaders[i].Load(reader);
	    // }

	    // collections = new Collection[collectionHeaders.Length];
	    // for (int i = 0; i < collectionHeaders.Length; ++i) {
		// collections[i] = new Collection();
		// if (collectionHeaders[i].Offset > 0) {
		//     reader.BaseStream.Seek(origin + collectionHeaders[i].Offset, SeekOrigin.Begin);
		//     collections[i].Load(reader);
		// }
	    // }
	}

	public void Load(string filename) {
	    try {
		BinaryReaderBE reader = new BinaryReaderBE(File.Open(filename, FileMode.Open));
		Load(reader);
	    } catch (Exception) {
		//should abort loading here and return to main menu
	    }
	}

	List<uint> ChunkFilter = new List<uint> {
	    // embedded physics
	    Wadfile.Chunk("MNpx"),
	    Wadfile.Chunk("FXpx"),
	    Wadfile.Chunk("PRpx"),
	    Wadfile.Chunk("RXpx"),
	    Wadfile.Chunk("WPpx"),
	};


	}

	public class Monster {
		
	}

	public class Effect {
	}

	public class Shot {
	}

	public class PhysicsModel {
	}

	public class Weapon {
	}




}

