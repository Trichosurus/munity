using System;
using System.Collections.Generic;
using System.IO;

namespace Weland {
    public class SoundsFile {
        public ushort SourceCount {
            get { return sourceCount; }
        }

        public ushort SoundCount {
            get { return soundCount; }
        }
        
        public SoundDefinition GetSound(int source, int index) {
            return sounds[source][index];
        }
        
        public void Load(BinaryReaderBE reader) {
            long origin = reader.BaseStream.Position;

            uint version = reader.ReadUInt32();
            uint tag = reader.ReadUInt32();

            if (version != 0 && version != 1) {
                Console.WriteLine("Bad version: {0}", version);
            }

            if (tag != FourCharsToInt('s', 'n', 'd', '2')) {
                Console.WriteLine("Bad tag!");
            }

            sourceCount = reader.ReadUInt16();
            soundCount = reader.ReadUInt16();

            reader.BaseStream.Position = origin + HEADER_SIZE;

            for (int source = 0; source < sourceCount; ++source) {
                List<SoundDefinition> list = new List<SoundDefinition>();
                for (int definition = 0; definition < soundCount; ++definition) {
                    SoundDefinition sound = new SoundDefinition();
                    sound.Load(reader);
                    list.Add(sound);
                }
                sounds.Add(list);
            }
        }
        
        public void Load(string filename) {
            BinaryReaderBE reader = new BinaryReaderBE(File.Open(filename, FileMode.Open));
            Load(reader);
        }

        public static uint FourCharsToInt(char a, char b, char c, char d) {
            return (uint) ((a << 24) | (b << 16) | (c << 8) | d);
        }
        
        ushort sourceCount;
        ushort soundCount;
        List<List<SoundDefinition>> sounds = new List<List<SoundDefinition>>();

        static readonly int HEADER_SIZE = 260;

        public static void Main(string[] args) {
            if (args.Length == 1 || args.Length == 5) {
                SoundsFile sf = new SoundsFile();
                sf.Load(args[0]);

                Console.WriteLine("Source count: {0}", sf.SourceCount);
                Console.WriteLine("Sound count: {0}", sf.SoundCount);

                if (args.Length == 5) {
                    int source = Int32.Parse(args[1]);
                    int index = Int32.Parse(args[2]);
                    int permutation = Int32.Parse(args[3]);

                    using (BinaryWriter writer = new BinaryWriter(File.Open(args[4], FileMode.Create))) {
                        Permutation p = sf.GetSound(source, index).permutations[permutation];
                        Console.WriteLine("Rate: {0}", p.SampleRate);
                        Console.WriteLine("Channels: {0}", p.Channels);
                        foreach (float f in p.Samples) {
                            if (f >= 1.0f || f <= -1.0f) {
                                Console.WriteLine("Sample was {0}", f);
                            }
                            writer.Write(f);
                        }

                        // you can play the file by running it through sox:
                        // sox -r <rate> -e floating-point -b 32 -c <channels> ,input> <output.wav>
                    }
                }
            } else {
                Console.WriteLine("Usage: soundsfile <filename> [<source> <index> <permutation> <file>]");
            }
        }
    }
}
