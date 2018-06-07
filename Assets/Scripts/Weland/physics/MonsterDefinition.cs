namespace Weland {
    public class MonsterDefinition : ISerializableBE {
        public static readonly uint Tag = Wadfile.Chunk("MNpx");

        public short Collection;
        public short Vitality;
        
        public uint Immunities;
        public uint Weaknesses;
        public uint Flags;

        public int Class;
        public int Friends;
        public int Enemies;

        public double SoundPitch;

        public short ActivationSound;
        public short FriendlyActivationSound;
        public short ClearSound;
        public short KillSound;
        public short ApologySound;
        public short FriendlyFireSound;
        public short FlamingSound;
        public short RandomSound;
        public short RandomSoundMask;

        public short CarryingItemType;

        public short Radius;
        public short Height;
        public short PreferredHoverHeight;
        public short MinimumLedgeDelta;
        public short MaximumLedgeDelta;

        public double ExternalVelocityScale;

        public short ImpactEffect;
        public short MeleeImpactEffect;
        public short ContrailEffect;

        public short HalfVisualArc;
        public short HalfVerticalVisualArc;
        public short VisualRange;
        public short DarkVisualRange;

        public short Intelligence;
        public short Speed;
        public short Gravity;
        public short TerminalVelocity;
        public short DoorRetryMask;
        public short ShrapnelRadius;

        public DamageDefinition ShrapnelDamage;

        public short HitShapes;
        public short HardDyingShape;
        public short SoftDyingShape;
        public short HardDeadShapes;
        public short SoftDeadShapes;
        public short StationaryShape;
        public short MovingShape;
        public short TeleportInShape;
        public short TeleportOutShape;

        public short AttackFrequency;

        public AttackDefinition MeleeAttack;
        public AttackDefinition RangedAttack;

        public void Load(BinaryReaderBE reader) {
            Collection = reader.ReadInt16();
            
            Vitality = reader.ReadInt16();
            Immunities = reader.ReadUInt32();
            Weaknesses = reader.ReadUInt32();
            Flags = reader.ReadUInt32();

            Class = reader.ReadInt32();
            Friends = reader.ReadInt32();
            Enemies = reader.ReadInt32();

            SoundPitch = reader.ReadFixed();

            ActivationSound = reader.ReadInt16();
            FriendlyActivationSound = reader.ReadInt16();
            ClearSound = reader.ReadInt16();
            KillSound = reader.ReadInt16();
            ApologySound = reader.ReadInt16();
            FriendlyFireSound = reader.ReadInt16();
            FlamingSound = reader.ReadInt16();
            RandomSound = reader.ReadInt16();
            RandomSoundMask = reader.ReadInt16();

            CarryingItemType = reader.ReadInt16();

            Radius = reader.ReadInt16();
            Height = reader.ReadInt16();
            PreferredHoverHeight = reader.ReadInt16();
            MinimumLedgeDelta = reader.ReadInt16();
            MaximumLedgeDelta = reader.ReadInt16();
            ExternalVelocityScale = reader.ReadFixed();
            ImpactEffect = reader.ReadInt16();
            MeleeImpactEffect = reader.ReadInt16();
            ContrailEffect = reader.ReadInt16();

            HalfVisualArc = reader.ReadInt16();
            HalfVerticalVisualArc = reader.ReadInt16();
            VisualRange = reader.ReadInt16();
            DarkVisualRange = reader.ReadInt16();
            Intelligence = reader.ReadInt16();
            Speed = reader.ReadInt16();
            Gravity = reader.ReadInt16();
            TerminalVelocity = reader.ReadInt16();
            DoorRetryMask = reader.ReadInt16();
            ShrapnelRadius = reader.ReadInt16();
            ShrapnelDamage.Load(reader);

            HitShapes = reader.ReadInt16();
            HardDyingShape = reader.ReadInt16();
            SoftDyingShape = reader.ReadInt16();
            HardDeadShapes = reader.ReadInt16();
            SoftDeadShapes = reader.ReadInt16();
            StationaryShape = reader.ReadInt16();
            MovingShape = reader.ReadInt16();
            TeleportInShape = reader.ReadInt16();
            TeleportOutShape = reader.ReadInt16();

            AttackFrequency = reader.ReadInt16();
            MeleeAttack.Load(reader);
            RangedAttack.Load(reader);
        }

        public void Save(BinaryWriterBE writer) {

        }
    }
}
