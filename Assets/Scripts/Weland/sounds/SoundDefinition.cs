using System;
using System.Collections.Generic;
using System.IO;

namespace Weland {
    enum SoundDefinitionFlags : ushort {
        CannotBeRestarted = 0x0001,
        DoesNotSelfAbort = 0x0002,
        ResistsPitchChanges = 0x0004,
        CannotChangePitch = 0x0008,
        CannotBeObstructed = 0x0010,
        CannotBeMediaObstructed = 0x0020,
        IsAmbient = 0x0040
    };
    
    public class SoundDefinition {
        short SoundCode {
            get { return soundCode; }
        }

        short BehaviorIndex {
            get { return behaviorIndex; }
        }

        double Chance {
            get { return chance; }
        }

        double LowPitch {
            get { return lowPitch; }
        }

        double HighPitch {
            get { return highPitch; }
        }

        bool CannotBeRestarted {
            get {
                return (flags & SoundDefinitionFlags.CannotBeRestarted) != 0;
            }
        }

        bool DoesNotSelfAbort {
            get {
                return (flags & SoundDefinitionFlags.DoesNotSelfAbort) != 0;
            }
        }

        bool ResistsPitchChanges {
            get {
                return (flags & SoundDefinitionFlags.ResistsPitchChanges) != 0;
            }
        }

        bool CannotChangePitch {
            get {
                return (flags & SoundDefinitionFlags.CannotChangePitch) != 0;
            }
        }

        bool CannotBeObstructed {
            get {
                return (flags & SoundDefinitionFlags.CannotBeObstructed) != 0;
            }
        }

        bool CannotBeMediaObstructed {
            get {
                return (flags & SoundDefinitionFlags.CannotBeMediaObstructed) != 0;
            }
        }

        bool IsAmbient {
            get {
                return (flags & SoundDefinitionFlags.IsAmbient) != 0;
            }
        }
        
        public void Load(BinaryReaderBE reader) {
            long origin = reader.BaseStream.Position;
            
            soundCode = reader.ReadInt16();
            behaviorIndex = reader.ReadInt16();
            flags = (SoundDefinitionFlags) reader.ReadUInt16();
            chance = (32768.0 - reader.ReadUInt16()) / 32768.0;

            lowPitch = reader.ReadFixed();
            highPitch = reader.ReadFixed();

            short numPermutations = reader.ReadInt16();
            reader.ReadUInt16(); // permutations played

            uint groupOffset = reader.ReadUInt32();
            reader.ReadUInt32(); // single length
            reader.ReadUInt32(); // total length

            List<int> offsets = new List<int>();

            for (int i = 0; i < MAXIMUM_PERMUTATIONS_PER_SOUND; ++i) {
                offsets.Add(reader.ReadInt32());
            }

            reader.ReadUInt32(); // last played

            reader.BaseStream.Position = origin + HEADER_SIZE;

            permutations.Clear();
            long position = reader.BaseStream.Position;
            for (int i = 0; i < numPermutations; ++i) {
                reader.BaseStream.Seek(groupOffset + offsets[i], SeekOrigin.Begin);

                Permutation permutation = new Permutation();
                permutation.Load(reader);
                permutations.Add(permutation);
            }
            reader.BaseStream.Seek(position, SeekOrigin.Begin);
        }

        short soundCode;
        short behaviorIndex;
        SoundDefinitionFlags flags;
        double chance;

        double lowPitch;
        double highPitch;

        public List<Permutation> permutations = new List<Permutation>();

        static readonly int HEADER_SIZE = 64;
        
        private static readonly int MAXIMUM_PERMUTATIONS_PER_SOUND = 5;        
    }
}
