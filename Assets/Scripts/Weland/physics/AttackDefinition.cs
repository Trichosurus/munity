namespace Weland {
    public struct AttackDefinition {
        public short Type;
        public short Repetitions;
        public double Angle;
        public short Range;
        public short AttackShape;

        public short Dx;
        public short Dy;
        public short Dz;

        public void Load(BinaryReaderBE reader) {
            Type = reader.ReadInt16();
            Repetitions = reader.ReadInt16();
            Angle = Weland.Angle.ToDouble(reader.ReadInt16());
            Range = reader.ReadInt16();
            AttackShape = reader.ReadInt16();

            Dx = reader.ReadInt16();
            Dy = reader.ReadInt16();
            Dz = reader.ReadInt16();
        }
    }
}
