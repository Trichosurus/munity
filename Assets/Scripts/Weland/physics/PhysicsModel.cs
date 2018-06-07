using System;
using System.Collections.Generic;
using System.IO;

namespace Weland {
    public class PhysicsModel {
        public class BadPhysicsException : Exception {
            public BadPhysicsException() { }
            public BadPhysicsException(string message) : base(message) { }
            public BadPhysicsException(string message, Exception inner) :
                base(message, inner) { }
        }
            
        public List<MonsterDefinition> MonsterDefinitions {
            get { return monsterDefinitions; }
        }

        public List<EffectDefinition> EffectDefinitions {
            get { return effectDefinitions; }
        }

        public List<ProjectileDefinition> ProjectileDefinitions {
            get { return projectileDefinitions; }
        }

        public List<PhysicsConstants> PhysicsConstants {
            get { return physicsConstants; }
        }

        public List<WeaponDefinition> WeaponDefinitions {
            get { return weaponDefinitions; }
        }

        public void Load(string filename) {
            Wadfile file = new Wadfile();
            file.Load(filename);
            if (file.Directory.Count == 1) {
                Load(file.Directory[0]);
            } else {
                throw new BadPhysicsException("Invalid physics file");
            }
        }

        public void Load(Wadfile.DirectoryEntry wad) {
            if (wad.Chunks.ContainsKey(MonsterDefinition.Tag)) {
                LoadChunkList<MonsterDefinition>(monsterDefinitions, wad.Chunks[MonsterDefinition.Tag]);
            } else {
                throw new BadPhysicsException("Invalid physics: missing monster definitions");
            }

            if (wad.Chunks.ContainsKey(EffectDefinition.Tag)) {
                LoadChunkList<EffectDefinition>(effectDefinitions, wad.Chunks[EffectDefinition.Tag]);
            } else {
                throw new BadPhysicsException("Invalid physics: missing effects definitions");
            }

            if (wad.Chunks.ContainsKey(ProjectileDefinition.Tag)) {
                LoadChunkList<ProjectileDefinition>(projectileDefinitions, wad.Chunks[ProjectileDefinition.Tag]);
            } else {
                throw new BadPhysicsException("Invalid physics: missing projectile definitions");
            }

            if (wad.Chunks.ContainsKey(Weland.PhysicsConstants.Tag)) {
                LoadChunkList<PhysicsConstants>(physicsConstants, wad.Chunks[Weland.PhysicsConstants.Tag]);
            } else {
                throw new BadPhysicsException("Invalid physics: missing physics constants");
            }

             if (wad.Chunks.ContainsKey(WeaponDefinition.Tag)) {
                LoadChunkList<WeaponDefinition>(weaponDefinitions, wad.Chunks[WeaponDefinition.Tag]);
            } else {
                throw new BadPhysicsException("Invalid physics: missing weapon definitions");
            }
        }
        
        public static void Main(string[] args) {
            if (args.Length == 1) {
                PhysicsModel model = new PhysicsModel();
                model.Load(args[0]);

                Console.WriteLine("{0} monster definitions", model.monsterDefinitions.Count);
                //Console.WriteLine(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(model));
            } else {
                Console.WriteLine("Usage: PhysicsModel <filename>");
            }
        }

        void LoadChunkList<T>(List<T> list, byte[] data) where T : ISerializableBE, new() {
	    BinaryReaderBE reader = new BinaryReaderBE(new MemoryStream(data));
	    list.Clear();
	    while (reader.BaseStream.Position < reader.BaseStream.Length) {
		T t = new T();
		t.Load(reader);
		list.Add(t);
	    }
	}

        List<MonsterDefinition> monsterDefinitions = new List<MonsterDefinition>();
        List<EffectDefinition> effectDefinitions = new List<EffectDefinition>();
        List<ProjectileDefinition> projectileDefinitions = new List<ProjectileDefinition>();
        List<PhysicsConstants> physicsConstants = new List<PhysicsConstants>();
        List<WeaponDefinition> weaponDefinitions = new List<WeaponDefinition>();
    }
}
