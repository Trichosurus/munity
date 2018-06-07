namespace Weland {
    public class PhysicsConstants : ISerializableBE {
        public static readonly uint Tag = Wadfile.Chunk("PXpx");

        public double MaximumForwardVelocity;
        public double MaximumBackwardVelocity;
        public double MaximumPerpendicularVelocity;

        public double Acceleration;
        public double Deceleration;
        public double AirborneDeceleration;

        public double GravitationalAcceleration;
        public double ClimbingAcceleration;
        public double TerminalVelocity;

        public double ExternalAcceleration;

        public double AngularAcceleration;
        public double AngularDeceleration;
        public double MaximumAngularVelocity;
        public double AngularRecenteringVelocity;

        public double FastAngularVelocity;
        public double FastAngularMaximum;

        public double MaximumElevation;
        public double ExternalAngularDeceleration;

        public double StepDelta;
        public double StepAmplitude;

        public double Radius;
        public double Height;
        public double DeadHeight;
        public double CameraHeight;
        public double SplashHeight;

        public double HalfCameraSeparation;

        public void Load(BinaryReaderBE reader) {
            MaximumForwardVelocity = reader.ReadFixed();
            MaximumBackwardVelocity = reader.ReadFixed();
            MaximumPerpendicularVelocity = reader.ReadFixed();

            Acceleration = reader.ReadFixed();
            Deceleration = reader.ReadFixed();
            AirborneDeceleration = reader.ReadFixed();

            GravitationalAcceleration = reader.ReadFixed();
            ClimbingAcceleration = reader.ReadFixed();
            TerminalVelocity = reader.ReadFixed();

            ExternalAcceleration = reader.ReadFixed();

            AngularAcceleration = reader.ReadFixed();
            AngularDeceleration = reader.ReadFixed();
            MaximumAngularVelocity = reader.ReadFixed();
            AngularRecenteringVelocity = reader.ReadFixed();

            FastAngularVelocity = reader.ReadFixed();
            FastAngularMaximum = reader.ReadFixed();

            MaximumElevation = reader.ReadFixed();
            ExternalAngularDeceleration = reader.ReadFixed();

            StepDelta = reader.ReadFixed();
            StepAmplitude = reader.ReadFixed();

            Radius = reader.ReadFixed();
            Height = reader.ReadFixed();
            DeadHeight = reader.ReadFixed();
            CameraHeight = reader.ReadFixed();
            SplashHeight = reader.ReadFixed();

            HalfCameraSeparation = reader.ReadFixed();
        }

        public void Save(BinaryWriterBE writer) {

        }
    }
}
