namespace GeneticRoute
{
    public class Address
    {
	    private static int idNow;
        public readonly string Name;
        private readonly int Id;

        public Address(string name)
        {
            Id = idNow++;
            Name = name;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}