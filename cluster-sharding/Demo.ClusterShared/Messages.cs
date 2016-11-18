using System;
using System.Collections.Generic;

namespace Demo.ClusterShared
{

    public struct GetCurrentState
    {
        public static readonly GetCurrentState Instance = new GetCurrentState();
    }

    public struct GetHistory
    {
        public static readonly GetHistory Instance = new GetHistory();
    }

    public struct VehicleHistory
    {
        public readonly string VehicleId;
        public readonly IEnumerable<PositionChanged> Positions;

        public VehicleHistory(string vehicleId, IEnumerable<PositionChanged> positions) : this()
        {
            VehicleId = vehicleId;
            Positions = positions;
        }
    }

    public struct PositionChanged
    {
        public readonly DateTime When;
        public readonly Position Position;

        public PositionChanged(Position position, DateTime @when)
        {
            Position = position;
            When = when;
        }
    }

    public struct VehicleState
    {
        public readonly string Identifier;
        public readonly DateTime LastUpdate;
        public readonly Position Position;

        public VehicleState(string identifier, Position position, DateTime lastUpdate)
        {
            Position = position;
            LastUpdate = lastUpdate;
            Identifier = identifier;
        }
    }

    public struct IdentifiedPosition
    {
        public readonly string Identifier;
        public readonly Position Position;

        public IdentifiedPosition(string identifier, Position position)
        {
            Identifier = identifier;
            Position = position;
        }
    }

    public struct Position : IEquatable<Position>
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return (Longitude.GetHashCode() * 397) ^ Latitude.GetHashCode();
            }
        }

        public static readonly Position Zero = new Position(0, 0);

        public readonly double Longitude;
        public readonly double Latitude;

        public Position(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public bool Equals(Position other) =>
            Math.Abs(Longitude - other.Longitude) < 0.01 && Math.Abs(Latitude - other.Latitude) < 0.001;

        public override bool Equals(object obj) => obj is Position && Equals((Position)obj);

        public override string ToString() => $"({Longitude}, {Latitude})";

        public static bool operator ==(Position x, Position y) => x.Equals(y);

        public static bool operator !=(Position x, Position y) => !(x == y);
    }
}