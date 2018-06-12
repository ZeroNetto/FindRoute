namespace GeneticRoute
{
	public interface IEnvironmentDataParser
	{
		EnvironmentData ParseFromFile(string pathToManagersFile, string pathToClientsFile);
	}
}