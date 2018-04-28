namespace GeneticRoute
{
    public class Address
    {
	    private static int idNow;

        public readonly int Id;

        public Address()
        {
            Id = idNow++;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}