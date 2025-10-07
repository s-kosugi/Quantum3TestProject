using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum.Asteroids
{
    public class AsteroidGameConfig : AssetObject
    {
        [Header("Asteroids configuration")]
        [Tooltip("Prototype reference to spawn asteroid")]
        public AssetRef<EntityPrototype> AsteroidPrototype;
        [Tooltip("Speed applied to the asteroid when spawned")]
        public FP AsteroidInitialSpeed = 8;
        [Tooltip("Minimum torque applied to the asteroid when spawned")]
        public FP AsteroidInitialTorqueMin = 7;
        [Tooltip("Maximum torque applied to the asteroid when spawned")]
        public FP AsteroidInitialTorqueMax = 20;
        [Tooltip("Distance to the center of the map. This value is the radius in a random circular location where the asteroid is spawned")]
        public FP AsteroidSpawnDistanceToCenter = 20;
        [Tooltip("Amount of asteroids spawned in level 1. In each level, the number of asteroids spawned is increased by one")]
        public int InitialAsteroidsCount = 5;
        [Header("Map Configration")]
        [Tooltip("Total size of the map. This is used to calculate when an entity is outside de gameplay area and then wrap it to the other side")]
        public FPVector2 GameMapsize = new FPVector2(25, 25);
        public FPVector2 MapExtends => _mapExtends;
        private FPVector2 _mapExtends;

        public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
        {
            base.Loaded(resourceManager, allocator);
            _mapExtends = GameMapsize / 2;
        }

    }
}
