using Photon.Deterministic;
using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    public unsafe class AsteroidsProjectileSystem : SystemMainThreadFilter<AsteroidsProjectileSystem.Filter>, ISignalAsteroidsShipShoot, ISignalOnCollisionProjectileHitShip,ISignalOnCollisionProjectileHitAsteroid
    {
        public struct Filter
        {
            public EntityRef Entity;
            public AsteroidsProjectile* projectile;
        }
        public override void Update(Frame frame, ref Filter filter)
        {
            filter.projectile->TTL -= frame.DeltaTime;
            if (filter.projectile->TTL <= 0)
            {
                frame.Destroy(filter.Entity);
            }
        }
        public void AsteroidsShipShoot(Frame frame, EntityRef owner, FPVector2 spawnPosition, AssetRef<EntityPrototype> projectilePrototype)
        {
            EntityRef projectileEntity = frame.Create(projectilePrototype);
            Transform2D* projectileTransform = frame.Unsafe.GetPointer<Transform2D>(projectileEntity);
            Transform2D* ownerTransform = frame.Unsafe.GetPointer<Transform2D>(owner);

            projectileTransform->Rotation = ownerTransform->Rotation;
            projectileTransform->Position = spawnPosition;

            AsteroidsProjectile* projectile = frame.Unsafe.GetPointer<AsteroidsProjectile>(projectileEntity);
            var config = frame.FindAsset(projectile->ProjectileConfig);
            projectile->TTL = config.ProjectileTTL;
            projectile->Owner = owner;

            PhysicsBody2D* body = frame.Unsafe.GetPointer<PhysicsBody2D>(projectileEntity);
            body->Velocity = ownerTransform->Up * config.ProjectileInitialSpeed;
        }
        public void OnCollisionProjectileHitShip(Frame frame, CollisionInfo2D info, AsteroidsProjectile* projectile, AsteroidShip* ship)
        {
            if (projectile->Owner == info.Other)
            {
                info.IgnoreCollision = true;
                return;
            }
            frame.Destroy(info.Entity);

            // 弾の当たったシップの処理
            if (frame.Has<AsteroidsShipRespawn>(info.Other))
            {
                return;
            }
            // リスポーン時間をシップに設定
            frame.Add<AsteroidsShipRespawn>(info.Other, out var shipRespawn);
            shipRespawn->RespawnTimer = 2;

            frame.Unsafe.GetPointer<AsteroidShip>(info.Other)->Score--;
            frame.Unsafe.GetPointer<PhysicsCollider2D>(info.Other)->Enabled = false;
        }
        public void OnCollisionProjectileHitAsteroid(Frame frame, CollisionInfo2D info, AsteroidsProjectile* projectile, AsteroidsAsteroid* asteroid)
        {
            if (asteroid->ChildAsteroid != null)
            {
                frame.Signals.SpawnAsteroid(asteroid->ChildAsteroid, info.Other);
                frame.Signals.SpawnAsteroid(asteroid->ChildAsteroid, info.Other);
            }
            if (frame.Unsafe.TryGetPointer<AsteroidShip>(projectile->Owner, out var ship1))
            {
                ship1->Score++;
            }
            frame.Destroy(info.Entity);
            frame.Destroy(info.Other);
        }
    }
}
