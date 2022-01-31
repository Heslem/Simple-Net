namespace SimpleNetworking
{
    public struct Packet
    {
        public readonly string Name;
        public readonly string Value;

        public Packet(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
