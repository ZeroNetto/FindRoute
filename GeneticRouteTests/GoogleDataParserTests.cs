using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticRoute;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Internal;

namespace GeneticRouteTests
{
	[TestFixture]
	public class GoogleDataParser_Should
	{
		private const string FilesFolder 
			= "e:/programFiles/visualstudioprojects/geneticroute/geneticroutetests/files/";

		[Test]
		public void ParseFromFile_ShouldParseCorrectly()
		{
			var envData = new GoogleDataParser().ParseFromFile(
				FilesFolder + "test_managers",
				FilesFolder + "test_clients"
			);

			envData.Clients.Should().HaveCount(1);
			envData.Managers.Should().HaveCount(1);
			envData.AddressClient.Count.Should().Be(1);
		}
	}
}
