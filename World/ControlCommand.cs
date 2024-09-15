using Newtonsoft.Json;
using TankWars;

namespace Model
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class is responsible for how the client will tell the server what it wants to do (moving, firing, etc). 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        // a string representing whether the player wants to move or not, and the desired direction.
        [JsonProperty(PropertyName = "moving")]
        private string moving = "none";

        // a string representing whether the player wants to fire or not, and the desired type. 
        [JsonProperty(PropertyName = "fire")]
        private string fire = "none";

        // a Vector2D representing where the player wants to aim their turret. 
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D tdir;

        /// <summary>
        /// Sets the values of the Json fields.
        /// </summary>
        /// <param name="moving">desired movement</param>
        /// <param name="fire">desired projectile</param>
        /// <param name="tdir">desired turrent direction</param>
        public ControlCommand(string moving, string fire, Vector2D tdir)
        {
            this.moving = moving;
            this.fire = fire;
            this.tdir = tdir;
        }

        // ----------------------------------Server methods--------------------------------


        /// <summary>
        /// Gets the direction the user wants to move to.
        /// </summary>
        /// <returns> direction</returns>
        public string GetMoving()
        {
            return moving;
        }

        /// <summary>
        /// Returns the state of the tank firing.
        /// </summary>
        /// <returns>state of fire</returns>
        public string GetFiring()
        {
            return fire;
        }

        /// <summary>
        /// Gets the direction of the turrent.
        /// </summary>
        /// <returns> turrent direction</returns>
        public Vector2D GetTdir()
        {
            return tdir;
        }

        /// <summary>
        /// Sets the type of projectile or the lack of one.
        /// </summary>
        /// <param name="fire"> desired projectile</param>
        public void setFiring(string fire)
        {
            this.fire = fire;
        }
    }
}