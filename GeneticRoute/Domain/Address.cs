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

        public override int GetHashCode() => Name.GetHashCode();
	    public override bool Equals(object obj)
	    {
		    return Name == ((Address) obj)?.Name;
	    }

	    public override string ToString()
	    {
		    return Name;
	    }
    }
}