using System.Collections.Generic;
using System.IO;

namespace Weland {
    public class Permutation {
        public double SampleRate {
            get { return sampleRate; }
        }
        
        public int Channels {
            get { return channels; }
        }

        public float[] Samples {
            get { return samples.ToArray(); }
        }

        public uint LoopStart {
            get { return loopStart; }
        }

        public uint LoopEnd {
            get { return loopEnd; }
        }

        // I think this is a MIDI key code; but I don't have my Apple
        // SoundManager reference guide handy; I think it's unused anyway
        public byte Frequency {
            get { return frequency; }
        }
        
        public void Load(BinaryReaderBE reader) {
            samples.Clear();
            
            int depth = 8;
            bool signed_8bit = false;
            uint frames = 0;
            
            long origin = reader.BaseStream.Position;

            reader.BaseStream.Seek(20, SeekOrigin.Current);
            byte headerType = reader.ReadByte();
            reader.BaseStream.Seek(origin, SeekOrigin.Begin);
            
            switch (headerType) {
            case StandardSoundHeader:
                depth = 8;
                signed_8bit = false;
                channels = 1;

                reader.ReadUInt32(); // sample pointer
                frames = reader.ReadUInt32();
                sampleRate = reader.ReadFixed();
                loopStart = reader.ReadUInt32();
                loopEnd = reader.ReadUInt32();
                reader.ReadByte(); // type
                frequency = reader.ReadByte(); // I think this is a MIDI note?
                break;
            case ExtendedSoundHeader:
            case CompressedSoundHeader: 
                reader.ReadUInt32(); // sample pointer
                channels = reader.ReadUInt32() == 2 ? 2 : 1;
                sampleRate = reader.ReadFixed();
                loopStart = reader.ReadUInt32();
                loopEnd = reader.ReadUInt32();

                reader.ReadByte(); // type
                frequency = reader.ReadByte(); // I think this is MIDI?

                frames = reader.ReadUInt32();

                if (headerType == CompressedSoundHeader) {
                    reader.BaseStream.Seek(10, SeekOrigin.Current);
                    reader.ReadUInt32(); // marker chunk
                    uint format = reader.ReadUInt32();
                    reader.ReadUInt32(); // future use
                    reader.ReadUInt32(); // stateVars
                    reader.ReadUInt32(); // leftOverSamples
                    short compressionId = reader.ReadInt16();
                    if (format != SoundsFile.FourCharsToInt('t', 'w', 'o', 's') || compressionId != -1) {
                        // fail somehow
                        return;
                    }
                    signed_8bit = true;
                    reader.ReadInt16(); // packet size
                    reader.ReadInt16(); // unused
                    depth = (reader.ReadInt16() == 16 ? 16 : 8);
                } else {
                    signed_8bit = false;
                    reader.BaseStream.Seek(22, SeekOrigin.Current);
                    depth = (reader.ReadInt16() == 16 ? 16 : 8);
                    reader.BaseStream.Seek(14, SeekOrigin.Current);
                }
                break;
            }

            for (int i = 0; i < frames; ++i) {
                for (int j = 0; j < channels; ++j) {
                    float sample;
                    if (depth == 16) {
                        sample = (reader.ReadInt16() + 0.5f) / 32767.5f;
                    } else if (signed_8bit) {
                        sample = (reader.ReadSByte() + 0.5f) / 127.5f;
                    } else {
                        sample = reader.ReadByte() / 127.5f - 1.0f;
                    }
                    
                    samples.Add(sample);
                }
            }
        }

        double sampleRate;
        int channels;
        uint loopStart;
        uint loopEnd;
        byte frequency;

        List<float> samples = new List<float>();

        private const byte StandardSoundHeader = 0x00;
        private const byte ExtendedSoundHeader = 0xFF;
        private const byte CompressedSoundHeader = 0xFE;
    }
}
