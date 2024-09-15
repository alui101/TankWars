using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TankWars;

namespace WorldSpace
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class represents a Beam. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
   public class Beams
    {
        //  an int representing the beam's unique ID.
        [JsonProperty(PropertyName = "beam")]
        private int ID;

        // a Vector2D representing the origin of the beam.
        [JsonProperty(PropertyName = "org")]
        private Vector2D loc;

        // a Vector2D representing the direction of the beam.
        [JsonProperty(PropertyName = "dir")]
        private Vector2D orientation;

        // an int representing the ID of the tank that fired the beam.
        [JsonProperty(PropertyName = "owner")]
        private int owner;

        /// <summary>
        /// Constructor to use when creating projectiles in the server.
        /// </summary>
        /// <param name="ID"> unique ID identifying a beam</param>
        /// <param name="loc"> location of the beam</param>
        /// <param name="orientation"> orientation of the beam</param>
        /// <param name="died"> indicates wheter it has died</param>
        /// <param name="owner"> contains information on who owns the projectile</param>
        public Beams(int ID, Vector2D loc, Vector2D orientation, int owner)
        {
            this.ID = ID;
            this.loc = loc;
            this.orientation = orientation;
            this.owner = owner;
        }

        /// <summary>
        /// default constructor.
        /// </summary>
        public Beams()
        {
        }

        /// <summary>
        /// Retrieves the origin.
        /// </summary>
        /// <returns>origin of beam</returns>
        public Vector2D getOrigin()
        {
            return loc;
        }
        /// <summary>
        /// Retrieves the direction.
        /// </summary>
        /// <returns>direction of beam</returns>
        public Vector2D getDirection()
        {
            return orientation;
        }

        /// <summary>
        /// Retrieves owner.
        /// </summary>
        /// <returns>owner of the beam</returns>
        public int getOwner()
        {
            return owner;
        }
        /// <summary>
        /// Retrieves an ID.
        /// </summary>
        /// <returns> an ID</returns>
        public int getID()
        {
            return ID;
        }

        // ----------------------------------Methods for the server--------------------------------


        /// <summary>
        /// Determines possible tank beam intersections.
        /// </summary>
        /// <param name="b"> beam to be processed</param>
        /// <param name="theWorld"> current world holding all components of the game</param>
        /// <param name="gameMode"> determines what gamemode is on</param>
        public static void ProcessBeam(Beams b, World theWorld,bool gameMode)
        {
            // Must check possible intersections on all tanks
            foreach (KeyValuePair<int, Tank> tank in theWorld.getTanks())
            {
                // If the game mode is turned on
                if (gameMode)
                {
                    // Checks if tanks are not on same team and if a tank is intersected
                    if (b.getOwner() % 2 != tank.Value.GetID() % 2 && Intersects(b.getOrigin(), b.getDirection(), tank.Value.GetLocation(), 50))
                    {
                        // destroys the tank
                        tank.Value.SetDeath(true);
                        tank.Value.SetHp(0);
                        theWorld.addDeadTank(tank.Value);
                        // destroyer of tank gets a point to score
                        theWorld.getTank(b.getOwner()).AddScore();
                    }
                }
                // If the game mode is turned off, then the beam is allowed to destroy any tank
                else if (Intersects(b.getOrigin(), b.getDirection(), tank.Value.GetLocation(), 50))
                {
                    // destroys the tank
                    tank.Value.SetDeath(true);
                    tank.Value.SetHp(0);
                    theWorld.addDeadTank(tank.Value);
                    // destroyer of tank gets a point to score
                    theWorld.getTank(b.getOwner()).AddScore();
                }
            }
        }

        /// <summary>
        /// Determines if a ray interescts a tank
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

    }
}
