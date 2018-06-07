namespace Weland {
    public class ProjectileDefinition : ISerializableBE {
        public static readonly uint Tag = Wadfile.Chunk("PRpx");

        public short Collection;
        public short Shape;
        
        public short DetonationEffect;
        public short MediaDetonationEffect;
        
        public short ContrailEffect;
        public short TicksBetweenContrails;
        public short MaximumContrails;

        public short MediaProjectilePromotion;

        public short Radius;
        public short AreaOfEffect;
        public DamageDefinition Damage;

        public uint Flags;

        public short Speed;
        public short MaximumRange;

        public double SoundPitch;
        public short FlybySound;
        public short ReboundSound;

        public void Load(BinaryReaderBE reader) {
            Collection = reader.ReadInt16();
            Shape = reader.ReadInt16();

            DetonationEffect = reader.ReadInt16();
            MediaDetonationEffect = reader.ReadInt16();

            ContrailEffect = reader.ReadInt16();
            TicksBetweenContrails = reader.ReadInt16();
            MaximumContrails = reader.ReadInt16();

            MediaProjectilePromotion = reader.ReadInt16();

            Radius = reader.ReadInt16();
            AreaOfEffect = reader.ReadInt16();
            Damage.Load(reader);

            Flags = reader.ReadUInt32();

            Speed = reader.ReadInt16();
            MaximumRange = reader.ReadInt16();

            SoundPitch = reader.ReadFixed();
            FlybySound = reader.ReadInt16();
            ReboundSound = reader.ReadInt16();
        }

        public void Save(BinaryWriterBE writer) {

        }
    }
}
