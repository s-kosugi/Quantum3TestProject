using UnityEngine.Scripting;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum.Asteroids
{
    [Preserve]
    public unsafe class AsteroidSHipSystem : SystemMainThreadFilter<AsteroidSHipSystem.Filter>,ISignalOnCollisionAsteroidHitShip,ISignalAsteroidSpawnShip
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform2D* Transform;
            public PhysicsBody2D* Body;
            public AsteroidShip* asteroidShip;
            public PlayerLink* playerLink;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            if (frame.Unsafe.TryGetPointer<AsteroidsShipRespawn>(filter.Entity, out var shipRespawn))
            {
                shipRespawn->RespawnTimer -= frame.DeltaTime;
                if (shipRespawn->RespawnTimer <= 0)
                {
                    frame.Signals.AsteroidSpawnShip(filter.Entity);
                }
                return;
            }
            AsteroidGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            Input* input = input = frame.GetPlayerInput(filter.playerLink->PlayerRef);

            UpdateShipMovement(frame, ref filter, input);
            UpdateShipFire(frame, ref filter, input);
        }
        private void UpdateShipMovement(Frame frame, ref Filter filter, Input* input)
        {
            var config = frame.FindAsset(filter.asteroidShip->ShipConfig);
            FP shipAcceleration = config.ShipAceleration;
            FP turnSpeed = config.ShipTurnSpeed;

            if (input->Up)
            {
                filter.Body->AddForce(filter.Transform->Up * shipAcceleration);
            }
            if (input->Left)
            {
                filter.Body->AddTorque(turnSpeed);
            }
            if (input->Right)
            {
                filter.Body->AddTorque(-turnSpeed);
            }
            filter.Body->AngularVelocity = FPMath.Clamp(filter.Body->AngularVelocity, -turnSpeed, turnSpeed);
        }
        private void UpdateShipFire(Frame frame, ref Filter filter, Input* input)
        {
            var config = frame.FindAsset(filter.asteroidShip->ShipConfig);
            if (input->Fire && filter.asteroidShip->FireInterval <= 0)
            {
                filter.asteroidShip->FireInterval = config.FireInterval;
                var relativeOffset = FPVector2.Up * config.ShotOffset;
                var spawnPosition = filter.Transform->TransformPoint(relativeOffset);
                frame.Signals.AsteroidsShipShoot(filter.Entity, spawnPosition, config.ProjectilePrototype);
            }
            else
            {
                filter.asteroidShip->FireInterval -= frame.DeltaTime;
            }
        }
        public void AsteroidSpawnShip(Frame frame, EntityRef owner)
        {
            frame.Remove<AsteroidsShipRespawn>(owner);

            AsteroidGameConfig config = frame.FindAsset(frame.RuntimeConfig.GameConfig);
            Transform2D* transform = frame.Unsafe.GetPointer<Transform2D>(owner);
            transform->Position = FPVector2.Rotate(FPVector2.Up * config.AsteroidSpawnDistanceToCenter, frame.RNG->Next() * FP.PiTimes2);
            transform->Teleport(frame, transform);

            frame.Unsafe.GetPointer<PhysicsBody2D>(owner)->Velocity = default;
            frame.Unsafe.GetPointer<PhysicsBody2D>(owner)->AngularVelocity = default;
            // ※弾関連
            //frame.Unsafe.GetPointer<AsteroidsShip>(shipEntity)->AmmoCount = config.maxAmmo;
            frame.Unsafe.GetPointer<PhysicsCollider2D>(owner)->Enabled = true;
        }
        public void OnCollisionAsteroidHitShip(Frame frame, CollisionInfo2D info, AsteroidShip* ship, AsteroidsAsteroid* asteroid)
        {
            if (frame.Has<AsteroidsShipRespawn>(info.Entity))
            {
                return;
            }
            // リスポーン時間をシップに設定
            frame.Add<AsteroidsShipRespawn>(info.Entity, out var shipRespawn);
            shipRespawn->RespawnTimer = 2;

            frame.Unsafe.GetPointer<AsteroidShip>(info.Entity)->Score--;
            frame.Unsafe.GetPointer<PhysicsCollider2D>(info.Entity)->Enabled = false;
            //frame.Destroy(info.Entity);
        }
    }
}