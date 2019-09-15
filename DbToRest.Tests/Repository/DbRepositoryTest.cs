using DbToRest.Core;
using DbToRest.Core.Data.Repository;
using DbToRest.Core.Domain.Data;
using NUnit.Framework;
using System.Linq;

namespace DbToRest.Tests.Repository
{
    [TestFixture]
    public class DbRepositoryTest : BaseTest
    {
        private readonly IDbRepository<Project> _projectRepository;

        public DbRepositoryTest()
        {
            _projectRepository = DbToRestContext.Current.Resolve<IDbRepository<Project>>();
        }

        [Test, Order(0)]
        public void Given_ProjectData_When_Save_Then_MustBeIdNotNull()
        {
            Core.Domain.Data.Project project = new Core.Domain.Data.Project();
            project.Name = "Test";

            _projectRepository.Save(project);

            Assert.IsNotNull(project.Id);
        }

        [Test,Order(1)]
        public void Given_LastInsterted_When_Delete_Then_Success()
        {
            var result = _projectRepository.Table(f => f.Name == "Test").FirstOrDefault();
            Assert.IsNotNull(result);

            _projectRepository.Delete(result);
        }
    }
}