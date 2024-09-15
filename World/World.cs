using System;
using System.Collections.Generic;
using WorldSpace;

namespace Model
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class represents the game world, holding all of the objects that are assigned to it. 
    /// </summary>
    public class World
    {
        /// <summary>
        /// contains all of the walls
        /// </summary>
        private Dictionary<int, Walls> walls;

        /// <summary>
        /// contains all of the beams
        /// </summary>
        private Dictionary<int, Beams> beams;

        /// <summary>
        /// contains all of the tanks
        /// </summary>
        private Dictionary<int, Tank> tanks;

        /// <summary>
        /// contains all of the projectiles
        /// </summary>
        private Dictionary<int, Projectile> projectiles;

        /// <summary>
        /// contains all of the powerups
        /// </summary>
        private Dictionary<int, Powerups> powerups;

        /// <summary>
        /// contains all of the explosions
        /// </summary>
        private HashSet<Explosion> explosions;

        /// <summary>
        /// List of powerups that have died(been picked up)
        /// </summary>
        private HashSet<Powerups> deadPowerups;

        /// <summary>
        /// List of the tanks that have died.
        /// </summary>
        private HashSet<Tank> deadTanks;

        /// <summary>
        /// determines if it was a beam shot
        /// </summary>
        private bool isBeamShot;

        /// <summary>
        /// determines vitality of tank
        /// </summary>
        private bool dead;

        /// <summary>
        /// Helps us assign powerup's IDs.
        /// </summary>
        private int powerupID;

        /// <summary>
        /// determines ID
        /// </summary>
        private int ID;

        /// <summary>
        /// How many frames to wait before spawning a new powerup.
        /// </summary>
        private long powerupDelay;

        /// <summary>
        /// initializing fields.
        /// </summary>
        public World()
        {
            walls = new Dictionary<int, Walls>();
            beams = new Dictionary<int, Beams>();
            tanks = new Dictionary<int, Tank>();
            projectiles = new Dictionary<int, Projectile>();
            powerups = new Dictionary<int, Powerups>();
            explosions = new HashSet<Explosion>();
            deadPowerups = new HashSet<Powerups>();
            deadTanks = new HashSet<Tank>();

            isBeamShot = false;
            dead = false;
            ID = 0;
            powerupID = 0;
        }

        /// <summary>
        /// Determines the powerupID.
        /// </summary>
        /// <param name="powerupID">provided ID</param>
        public void setPowerupID(int powerupID)
        {
            this.powerupID = powerupID;
        }

        /// <summary>
        /// Retrieves the powerupID
        /// </summary>
        /// <returns>the powerupID</returns>
        public int getPowerupID()
        {
            return powerupID;
        }
        
        /// <summary>
        /// Removes the provided tank from a list of dead tanks.
        /// </summary>
        /// <param name="tank"> tank to be removed</param>
        public void removeDeadTank(Tank tank)
        {
            deadTanks.Remove(tank);
        }

        /// <summary>
        /// Includes a dead tank.
        /// </summary>
        /// <param name="tank"> tank to be included</param>
        public void addDeadTank(Tank tank)
        {
            deadTanks.Add(tank);
        }

        /// <summary>
        /// Retrieves the dead tanks.
        /// </summary>
        /// <returns> list of dead tanks</returns>
        public HashSet<Tank> getDeadTanks()
        {
            return deadTanks;
        }

        /// <summary>
        /// Gets rid of all the dead tanks.
        /// </summary>
        public void ClearDeadTanks()
        {
            deadTanks.Clear();
        }

        /// <summary>
        /// Removes a provided powerup.
        /// </summary>
        /// <param name="p"> provided powerup</param>
        public void removeDeadPowerUp(Powerups p)
        {
            deadPowerups.Remove(p);
        }

        /// <summary>
        /// Includes a dead powerup.
        /// </summary>
        /// <param name="p"> a dead powerup</param>
        public void addDeadPowerUp(Powerups p)
        {
            deadPowerups.Add(p);
        }

        /// <summary>
        /// Retrieves the dead powerups.
        /// </summary>
        /// <returns> dead powerups</returns>
        public HashSet<Powerups> getDeadPowerUps()
        {
            return deadPowerups;
        }

        /// <summary>
        /// Gets rid of all the dead powerups.
        /// </summary>
        public void ClearDeadPowerUps()
        {
            deadPowerups.Clear();
        }

        /// <summary>
        /// Gets the powerup delay.
        /// </summary>
        /// <returns> number of frames to wait before spawning a new powerup.</returns>
        public long GetPowerupDelay()
        {
            return powerupDelay;
        }

        /// <summary>
        /// Sets the spawn delay of a powerup.
        /// </summary>
        public void SetPowerupDelay()
        {
            Random rand = new Random();

            powerupDelay = rand.Next(1651);
        }

        /// <summary>
        /// Sets the vitality of a powerup.
        /// </summary>
        /// <param name="ID"> ID of powerup</param>
        /// <param name="death"> determines whether powerup exists</param>
        public void setPowerUpDeath(int ID,bool death)
        {
            powerups[ID].SetDeath(death);
        }

        /// <summary>
        /// Retrieves information on whether it was a beam shot or not.
        /// </summary>
        /// <returns> information on projectile</returns>
        public bool getIsBeamShot()
        {
            return isBeamShot;
        }
        /// <summary>
        /// Determines whether it is a beam shot.
        /// </summary>
        /// <param name="isBeamShot"> is a beam shot</param>
        public void setIsBeamShot(bool isBeamShot)
        {
            this.isBeamShot = isBeamShot;
        }

        /// <summary>
        /// Determines the vitality of a tank.
        /// </summary>
        /// <param name="dead"> vitality of tank</param>
        public void setDeath(bool dead)
        {
            this.dead = dead;
        }

        /// <summary>
        /// Retrieves the information on vitality of tank.
        /// </summary>
        /// <returns> information of vitality of tank</returns>
        public bool getDeath()
        {
            return dead;
        }

        /// <summary>
        /// Determines the ID.
        /// </summary>
        /// <param name="ID"> the ID</param>
        public void setID(int ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Retrieves the ID.
        /// </summary>
        /// <returns> the ID</returns>
        public int getID()
        {
            return ID;
        }

        /// <summary>
        /// Includes a powerup.
        /// </summary>
        /// <param name="ID">its ID</param>
        /// <param name="p">the powerup</param>
        public void addPowerUp(int ID, Powerups p)
        {
            powerups.Add(ID, p);
        }

        /// <summary>
        /// Includes an explosion.
        /// </summary>
        /// <param name="e"> an explosion</param>
        public void addExplosion(Explosion e)
        {
            explosions.Add(e);
        }
        /// <summary>
        /// Removes an explosion.
        /// </summary>
        /// <param name="e"> the explosion</param>
        public void removeExplosion(Explosion e)
        {
            explosions.Remove(e);
        }

        /// <summary>
        /// Includes the beam with an ID.
        /// </summary>
        /// <param name="ID"> the ID</param>
        /// <param name="b"> the beam</param>
        public void addBeam(int ID, Beams b)
        {
            beams.Add(ID, b);
        }

        /// <summary>
        /// Removes a beam.
        /// </summary>
        /// <param name="ID"> the ID</param>
        public void removeBeam(int ID)
        {
            beams.Remove(ID);
        }
        /// <summary>
        /// Includes a wall
        /// </summary>
        /// <param name="ID"> the ID</param>
        /// <param name="w"> the wall</param>
        public void addWall(int ID, Walls w)
        {
            walls.Add(ID, w);
        }
        /// <summary>
        /// Includes a projectile.
        /// </summary>
        /// <param name="ID"> the ID</param>
        /// <param name="p"> the projectile</param>
        public void addProjectile(int ID, Projectile p)
        {
            projectiles.Add(ID, p);
        }
        /// <summary>
        /// Includes a tank.
        /// </summary>
        /// <param name="id"> the ID</param>
        /// <param name="t"> the tank</param>
        public void addTank(int id, Tank t)
        {

            tanks.Add(id, t);
        }
        /// <summary>
        /// Retrieves the powerups
        /// </summary>
        /// <returns> powerups</returns>
        public Dictionary<int, Powerups> getPowerUps()
        {
            return powerups;
        }

        /// <summary>
        /// Retrieves the explosions
        /// </summary>
        /// <returns>explosions</returns>
        public HashSet<Explosion> getExplosion()
        {
            return explosions;
        }
        /// <summary>
        /// Retrieves the projectiles.
        /// </summary>
        /// <returns> projectiles</returns>
        public Dictionary<int, Projectile> getProjectiles()
        {
            return projectiles;
        }

        /// <summary>
        /// Retrieves the tanks.
        /// </summary>
        /// <returns> the tanks</returns>
        public Dictionary<int, Tank> getTanks()
        {
            return tanks;
        }

        /// <summary>
        /// Retrieves the walls.
        /// </summary>
        /// <returns> the walls</returns>
        public Dictionary<int, Walls> getWalls()
        {
            return walls;
        }

        /// <summary>
        /// Determines the walls.
        /// </summary>
        /// <param name="walls"> the walls</param>
        public void setWalls(Dictionary<int, Walls> walls)
        {
            this.walls = walls;
        }

        /// <summary>
        /// Retrieves the beams.
        /// </summary>
        /// <returns> the beams</returns>
        public Dictionary<int, Beams> getBeams()
        {
            return beams;
        }

        /// <summary>
        /// Determines the beams.
        /// </summary>
        /// <param name="beams"> the beams</param>
        public void setBeams(Dictionary<int, Beams> beams)
        {
            this.beams = beams;
        }

        /// <summary>
        /// Removes tank by ID.
        /// </summary>
        /// <param name="ID">specified ID for the removal of tank</param>
        public void removeTankID(int ID)
        {
            tanks.Remove(ID);
        }

        /// <summary>
        /// Retrieve tank by ID.
        /// </summary>
        /// <param name="id"> specified ID for retrieval of tank</param>
        /// <returns> a tank</returns>
        public Tank getTank(int id)
        {
            if (tanks.Count == 0)
            {
                return null;
            }

            if (tanks.ContainsKey(id))
            {
                return tanks[id];
            }
            return null;

        }

        /// <summary>
        /// Retrieve projectile by ID.
        /// </summary>
        /// <param name="ID"> specified ID for retrieval of projectile</param>
        /// <returns>a projectile</returns>
        public Projectile getProj(int ID)
        {
            if (projectiles.Count == 0)
            {
                return null;
            }

            if (projectiles.ContainsKey(ID))
            {
                return projectiles[ID];
            }
            return null;
        }

        /// <summary>
        /// Remove projectile.
        /// </summary>
        /// <param name="ID"> the ID</param>
        public void RemoveProj(int ID)
        {
            projectiles.Remove(ID);
        }
        /// <summary>
        /// Retrieve powerup with ID.
        /// </summary>
        /// <param name="ID"> the ID</param>
        /// <returns></returns>
        public Powerups getPowerUp(int ID)
        {
            if (powerups.Count == 0)
            {
                return null;
            }

            if (powerups.ContainsKey(ID))
            {
                return powerups[ID];
            }
            return null;
        }

        /// <summary>
        /// Remove the powerup with the ID.
        /// </summary>
        /// <param name="ID"> the ID</param>
        public void removePowerUp(int ID)
        {
            powerups.Remove(ID);
        }

    }
}
