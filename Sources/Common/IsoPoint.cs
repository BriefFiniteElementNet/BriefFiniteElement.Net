using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Globalization;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a Point in 3D isparametric space
    /// </summary>
    [Serializable]
    public struct IsoPoint : ISerializable
    {
        #region Properties

        private double xi;
        private double eta;
        private double lambda;

        /// <summary>
        /// Gets or sets the ξ. first dimension.
        /// </summary>
        /// <value>
        /// The ξ.
        /// </value>
        public double Xi
        {
            get { return xi; }
            set { xi = value; }
        }

        /// <summary>
        /// Gets or sets the η. second dimension.
        /// </summary>
        /// <value>
        /// The η.
        /// </value>
        public double Eta
        {
            get { return eta; }
            set { eta = value; }
        }

        /// <summary>
        /// Gets or sets the γ. third dimension.
        /// </summary>
        /// <value>
        /// The γ.
        /// </value>
        public double Lambda
        {
            get { return lambda; }
            set { lambda = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates a p from the ξ, η and γ.
        /// </summary>
        /// <param name="xi">The xi.</param>
        /// <param name="eta">The eta.</param>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        public static IsoPoint FromXiEtaLambda(double xi, double eta, double lambda)
        {
            return new IsoPoint(xi, eta, lambda);
        }


        /// <summary>
        /// Gets a p witch is on origins (0,0,0).
        /// </summary>
        public static IsoPoint Origins
        {
            get { return new IsoPoint(0, 0, 0); }
        }



        #endregion

        public IsoPoint(double xi) : this(xi,0,0)
        {
        }

        public IsoPoint(double xi, double eta) : this(xi,eta,0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public IsoPoint(double xi, double eta, double lambda) : this()
        {
            Xi = xi;
            Eta = eta;
            Lambda = lambda;
        }

        public IsoPoint(double[] array) : this()
        {
            Xi = array[0];

            if (array.Length >= 1)
                Eta = array[1];

            if (array.Length >= 2)
                Lambda = array[2];
        }

        #region Operators



        #endregion


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}, {1}, {2}", xi, eta, lambda);
        }


        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("xi", xi);
            info.AddValue("eta", eta);
            info.AddValue("lambda", lambda);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private IsoPoint(SerializationInfo info, StreamingContext context)
        {
            xi = info.GetDouble("xi");
            eta = info.GetDouble("eta");
            lambda = info.GetDouble("lambda");
        }

        #endregion

        public bool Equals(IsoPoint other)
        {
            return xi.Equals(other.xi) && eta.Equals(other.eta) && lambda.Equals(other.lambda);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is IsoPoint && Equals((IsoPoint)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = xi.GetHashCode();
                hashCode = (hashCode * 397) ^ eta.GetHashCode();
                hashCode = (hashCode * 397) ^ lambda.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(IsoPoint left, IsoPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IsoPoint left, IsoPoint right)
        {
            return !left.Equals(right);
        }
    }
}