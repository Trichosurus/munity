namespace Weland {
    public class TriggerDefinition {
        public short RoundsPerMagazine;
        public short AmmunitionType;
        public short TicksPerRound;
        public short RecoveryTicks;
        public short ChargingTicks;
        public short RecoilMagnitude;
        public short FiringSound;
        public short ClickSound;
        public short ChargingSound;
        public short ShellCasingSound;
        public short ReloadingSound;
        public short ChargedSound;
        public short ProjectileType;
        public short ThetaError;
        public short Dx;
        public short Dz;
        public short ShellCasingType;
        public short BurstCount;

        public void Load(BinaryReaderBE reader) {
            RoundsPerMagazine = reader.ReadInt16();
            AmmunitionType = reader.ReadInt16();
            TicksPerRound = reader.ReadInt16();
            RecoveryTicks = reader.ReadInt16();
            ChargingTicks = reader.ReadInt16();
            RecoilMagnitude = reader.ReadInt16();
            FiringSound = reader.ReadInt16();
            ClickSound = reader.ReadInt16();
            ChargingSound = reader.ReadInt16();
            ShellCasingSound = reader.ReadInt16();
            ReloadingSound = reader.ReadInt16();
            ChargedSound = reader.ReadInt16();
            ProjectileType = reader.ReadInt16();
            ThetaError = reader.ReadInt16();
            Dx = reader.ReadInt16();
            Dz = reader.ReadInt16();
            ShellCasingType = reader.ReadInt16();
            BurstCount = reader.ReadInt16();
        }
    }
}
