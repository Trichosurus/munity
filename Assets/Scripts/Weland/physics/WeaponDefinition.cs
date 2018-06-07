namespace Weland {
    public class WeaponDefinition : ISerializableBE {
        public static readonly uint Tag = Wadfile.Chunk("WPpx");

        public short ItemType;
        public short PowerupType;
        public short WeaponClass;
        public short Flags;
        
        public double FiringLightIntensity;
        public short FiringIntensityDecayTicks;
        
        public double IdleHeight;
        public double BobAmplitude;
        public double KickHeight;
        public double ReloadHeight;
        public double IdleWidth;
        public double HorizontalAmplitude;
        
        public short Collection;
        public short IdleShape;
        public short FiringShape;
        public short ReloadingShape;
        public short Unused;
        public short ChargingShape;
        public short ChargedShape;
        
        public short ReadyTicks;
        public short AwaitReloadTicks;
        public short LoadingTicks;
        public short FinishLoadingTicks;
        public short PowerupTicks;

        public TriggerDefinition[] Triggers = new TriggerDefinition[2];
        
        public void Load(BinaryReaderBE reader) {
            ItemType = reader.ReadInt16();
            PowerupType = reader.ReadInt16();
            WeaponClass = reader.ReadInt16();
            Flags = reader.ReadInt16();

            FiringLightIntensity = reader.ReadFixed();
            FiringIntensityDecayTicks = reader.ReadInt16();

            IdleHeight = reader.ReadFixed();
            BobAmplitude = reader.ReadFixed();
            KickHeight = reader.ReadFixed();
            ReloadHeight = reader.ReadFixed();
            IdleWidth = reader.ReadFixed();
            HorizontalAmplitude = reader.ReadFixed();

            Collection = reader.ReadInt16();
            IdleShape = reader.ReadInt16();
            FiringShape = reader.ReadInt16();
            ReloadingShape = reader.ReadInt16();
            Unused = reader.ReadInt16();
            ChargingShape = reader.ReadInt16();
            ChargedShape = reader.ReadInt16();

            ReadyTicks = reader.ReadInt16();
            AwaitReloadTicks = reader.ReadInt16();
            LoadingTicks = reader.ReadInt16();
            FinishLoadingTicks = reader.ReadInt16();
            PowerupTicks = reader.ReadInt16();

            for (int i = 0; i < 2; ++i) {
                Triggers[i] = new TriggerDefinition();
                Triggers[i].Load(reader);
            }
        }

        public void Save(BinaryWriterBE writer) {

        }
    }
}
