using Newtonsoft.Json;
using TankWars;

namespace WorldSpace
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class represents a Wall. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Walls
    {
        // an int representing the wall's unique ID.
        [JsonProperty(PropertyName = "wall")]
        private int ID;

        //  a Vector2D representing one endpoint of the wall.
        [JsonProperty(PropertyName = "p1")]
        private Vector2D firstPoint;

        // a Vector2D representing the other endpoint of the wall.
        [JsonProperty(PropertyName = "p2")]
        private Vector2D secondPoint;

        /// <summary>
        /// default constructor.
        /// </summary>
        public Walls()
        {
        }

        /// <summary>
        /// Retrives wall's ID.
        /// </summary>
        /// <returns>wall's ID</returns>
        public int getID()
        {
            return ID;
        }

        /// <summary>
        /// Retrieves first point.
        /// </summary>
        /// <returns>first point</returns>
        public Vector2D getP1()
        {
            return firstPoint;
        }

        /// <summary>
        /// Retrieves second point.
        /// </summary>
        /// <returns> second point.</returns>
        public Vector2D getP2()
        {
            return secondPoint;
        }

        // ----------------------------------Methods for the server--------------------------------

        /// <summary>
        /// Sets the first point of the wall.
        /// </summary>
        /// <param name="p1"> first point</param>
        public void setP1(Vector2D p1)
        {
            firstPoint = p1;
        }

        /// <summary>
        /// Sets the second point of the wall.
        /// </summary>
        /// <param name="p2"> second point</param>
        public void setP2(Vector2D p2)
        {
            secondPoint = p2;
        }

        /// <summary>
        /// Sets the id of this wall.
        /// </summary>
        /// <param name="ID"> wall ID</param>
        public void setID(int ID)
        {
            this.ID = ID;
        }
    }
}
