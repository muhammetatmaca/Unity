using System;
using FluentAssertions;
using NUnit.Framework;
using GitHub.Unity;

namespace IntegrationTests
{
    class BaseIntegrationTest
    {
        protected NPath TestBasePath { get; private set; }
        protected ILogging Logger { get; private set; }
        protected IEnvironment Environment { get; private set; }
        protected IFileSystem FileSystem { get; private set; }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            Logger = Logging.GetLogger(GetType());
        }

        [SetUp]
        public void SetUp()
        {
            OnSetup();
        }

        protected virtual void OnSetup()
        {
            Environment = new DefaultEnvironment();
            FileSystem = new FileSystem(TestBasePath);

            NPathFileSystemProvider.Current.Should().BeNull("Test should run in isolation");
            NPathFileSystemProvider.Current = FileSystem;

            TestBasePath = NPath.CreateTempDirectory("integration-tests");
            FileSystem.SetCurrentDirectory(TestBasePath);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                Logger.Debug("Deleting TestBasePath: {0}", TestBasePath.ToString());
                TestBasePath.Delete();
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Error deleting TestBasePath: {0}", TestBasePath.ToString());
            }

            FileSystem = null;
            NPathFileSystemProvider.Current = null;
        }

        protected void CreateFilePaths(NPath[] dirs)
        {
            foreach (var file in dirs)
            {
                TestBasePath.Combine(file).CreateFile();
            }
        }
    }
}