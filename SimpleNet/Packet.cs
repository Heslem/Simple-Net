using System;

namespace SimpleNet
{
    public struct Packet : IEquatable<Packet>
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public Packet(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public bool Equals(Packet other)
        {
            return other.Name == Name && other.Value == Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Packet packet && Equals(packet);
        }

        public override int GetHashCode() => 0;

        public static bool operator ==(Packet left, Packet right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Packet left, Packet right)
        {
            return !(left == right);
        }
    }
}
