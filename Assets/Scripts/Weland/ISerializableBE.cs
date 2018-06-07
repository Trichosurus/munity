namespace Weland {
    interface ISerializableBE {
	void Load(BinaryReaderBE reader);
	void Save(BinaryWriterBE writer);
    }
}
