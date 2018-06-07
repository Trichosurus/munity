namespace Weland {
    public class EffectDefinition : ISerializableBE {
        public static readonly uint Tag = Wadfile.Chunk("FXpx");

        public short Collection;
        public short Shape;
        public double SoundPitch;
        public ushort Flags;
        public short Delay;
        public short DelaySound;

        public void Load(BinaryReaderBE reader) {
            Collection = reader.ReadInt16();
            Shape = reader.ReadInt16();
            
            SoundPitch = reader.ReadFixed();

            Flags = reader.ReadUInt16();

            Delay = reader.ReadInt16();
            DelaySound = reader.ReadInt16();
        }

        public void Save(BinaryWriterBE writer) {
            
        }
    }
}
