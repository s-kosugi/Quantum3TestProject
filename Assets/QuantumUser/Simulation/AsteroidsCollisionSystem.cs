using UnityEngine.Scripting;

namespace Quantum.Asteroids
{
    [Preserve]
    public unsafe class AsteroidsCollisionSystem : SystemSignalsOnly, ISignalOnCollisionEnter2D
    {
        public void OnCollisionEnter2D(Frame frame,CollisionInfo2D info) {
            if(frame.Unsafe.TryGetPointer<AsteroidsProjectile>(info.Entity, out var projectile))
            {
                if (frame.Unsafe.TryGetPointer<AsteroidShip>(info.Other, out var ship))
                {
                    // projectile Hit Ship
                    frame.Signals.OnCollisionProjectileHitShip(info, projectile, ship);
                }
                else if (frame.Unsafe.TryGetPointer<AsteroidsAsteroid>(info.Other, out var asteroid))
                {
                    // projectile Hit Asteroid
                    frame.Signals.OnCollisionProjectileHitAsteroid(info, projectile, asteroid);
                }
            }else if (frame.Unsafe.TryGetPointer<AsteroidShip>(info.Entity, out var ship))
            {
                if (frame.Unsafe.TryGetPointer<AsteroidsAsteroid>(info.Other, out var asteroid))
                {
                    // Asteroid Hit Ship
                    frame.Signals.OnCollisionAsteroidHitShip(info, ship, asteroid);
                }
            }
        }
    }
}