using GeneticRoute.Util;

namespace GeneticRoute
{
    public class Address : ValueType<Address>
    {
        public string Name { get;}

        public Address(string name)
        {
            Name = name;
        }
    }
}