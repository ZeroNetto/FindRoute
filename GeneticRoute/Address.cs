namespace GeneticRoute
{
    public class Address
    {
        private readonly int I;

        public Address(int i)
        {
            this.I = i;
        }

        public override int GetHashCode() => this.I.GetHashCode();
    }
}