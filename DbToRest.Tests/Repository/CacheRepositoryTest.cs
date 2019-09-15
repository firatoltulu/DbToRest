using DbToRest.Core;
using DbToRest.Core.Data.Repository;
using DbToRest.Core.Domain.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbToRest.Tests.Repository
{
    [TestFixture]
    public class CacheRepositoryTest : BaseTest
    {
        private readonly ICacheRepository<Project> _projectRepository;

        public CacheRepositoryTest()
        {
            _projectRepository = DbToRestContext.Current.Resolve<ICacheRepository<Project>>();
        }

        [Test, Order(0)]
        public void Given_ProjectData_When_Save_Then_MustBeIdNotNull()
        {
            Project project = new Project();
            project.Name = "Test";

            _projectRepository.Save(project);

            Assert.IsNotNull(project.Id);
        }

        [Test, Order(1)]
        public void Given_LastInsterted_When_Delete_Then_Success()
        {
            var result = _projectRepository.Table(f => f.Name == "Test").FirstOrDefault();
            Assert.IsNotNull(result);

            _projectRepository.Delete(result);
        }

    }
}
