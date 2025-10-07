using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    public unsafe class AsteroidsWaveSpawnerSystem : SystemSignalsOnly, ISignalSpawnAsteroid , ISignalOnComponentRemoved<AsteroidsAsteroid>
    {
        public void SpawnAsteroid(Frame frame, AssetRef<EntityPrototype> childPrototype, EntityRef parent)
        {
            AsteroidGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            EntityRef asteroid = frame.Create(childPrototype);
            Transform2D* asteroidTransform = frame.Unsafe.GetPointer<Transform2D>(asteroid);

            if (parent == EntityRef.None)
            {
                asteroidTransform->Position = GetRandomEdgePointOnCircle(frame, config.AsteroidSpawnDistanceToCenter);
            }
            else
            {
                asteroidTransform->Position = frame.Get<Transform2D>(parent).Position;
            }
            asteroidTransform->Rotation = GetRandomRotation(frame);

            if (frame.Unsafe.TryGetPointer<PhysicsBody2D>(asteroid, out var body))
            {
                body->Velocity = asteroidTransform->Up * config.AsteroidInitialSpeed;
                body->AddTorque(frame.RNG->Next(config.AsteroidInitialTorqueMin, config.AsteroidInitialTorqueMax));
            }
        }

        public static FP GetRandomRotation(Frame frame)
        {
            return frame.RNG->Next(0, 360);
        }

        public static FPVector2 GetRandomEdgePointOnCircle(Frame frame, FP radius)
        {
            return FPVector2.Rotate(FPVector2.Up * radius, frame.RNG->Next() * FP.PiTimes2);
        }

        private void SpawnAsteroidWave(Frame frame)
        {
            AsteroidGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            for (int i = 0; i < frame.Global->AsteroidsWaveCount + config.InitialAsteroidsCount; i++)
            {
                SpawnAsteroid(frame, config.AsteroidPrototype, EntityRef.None);
            }
            frame.Global->AsteroidsWaveCount++;
        }
        public override void OnInit(Frame frame)
        {
            SpawnAsteroidWave(frame);
        }

        public void OnRemoved(Frame frame, EntityRef entity, AsteroidsAsteroid* component)
        {
            Debug.Log("残りアステロイド数 = " + frame.ComponentCount<AsteroidsAsteroid>());
            if (frame.ComponentCount<AsteroidsAsteroid>() <= 1)
            {
                SpawnAsteroidWave(frame);
            }
        }
    }
}
