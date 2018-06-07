namespace Weland {
    public struct DamageDefinition {
        public short Type;
        public short Flags;
        public short Base;
        public short Random;
        public double Scale;
        
        public void Load(BinaryReaderBE reader) {
            Type = reader.ReadInt16();
            Flags = reader.ReadInt16();
            Base = reader.ReadInt16();
            Random = reader.ReadInt16();
            Scale = reader.ReadFixed();
        }
    }
}
