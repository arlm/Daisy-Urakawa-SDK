using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.unitTests.testbase;
using System.IO;

namespace urakawa.unitTests.fixtures.xukfiles.roundtrip

{
	/// <summary>
	/// Summary description for RoundTrip.
	/// </summary>
	[TestFixture]
	public class XUKRoundTrip : RoundTrip
	{
		private string mDefaultFile = "../XukWorks/roundTripTestSample.xuk";

		[SetUp]
		public void Init()
		{
			mProject = new urakawa.project.Project();
			
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, mDefaultFile);
			
			Assert.IsTrue(mProject.openXUK(fileUri), "Failed to load XUK file {0}", mDefaultFile);
		}
	}
}
