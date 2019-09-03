using DbToRest.Core.Data.Provider;
using DbToRest.Core.Domain.Data;
using DbToRest.Core.Infrastructure.SmartForm;
using DbToRest.Core.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DbToRest.Core.Data.Repository
{
    public class DbRepository<T> : IDbRepository<T> where T : class, new()
    {
        internal readonly IDataProvider _documentStore;

        public DbRepository(
            IDataProvider documentStore
            )
        {
            _documentStore = documentStore;
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var filtered = Table(where).ToList();
            filtered.Each(i => Delete(i));
        }

        public virtual void Delete(IEnumerable<object> key)
        {
            key.Each(u => Delete(u));
        }

        public virtual void Delete(T value)
        {
            if (value is TrackEntity)
            {
                (value as TrackEntity).Deleted = true;
            }
            Save(value);
        }

        public virtual void Delete(object key)
        {
            using (var session = _documentStore.Database.OpenSession())
            {
                session.Delete(key);
            }
        }

        public virtual void Save(T value)
        {
            using (var session = _documentStore.Database.OpenSession())
            {
                if (TryValidate(value))
                {
                    session.Store(value);
                    session.SaveChanges();
                }
            }
        }

        public virtual void Save(List<T> value)
        {
            value.Each(each => Save(each));
        }

        public virtual T Single(Expression<Func<T, bool>> where)
        {
            return Table(where).FirstOrDefault();
        }

        public virtual T Single(object key)
        {
            using (var session = _documentStore.Database.OpenSession())
            {
                return session.Load<T>(key.ToString());
            }
        }

        public virtual IEnumerable<T> Table(Expression<Func<T, bool>> where = null)
        {
            using (var session = _documentStore.Database.OpenSession())
            {
                var _type = typeof(T);
      
                var query = session.Query<T>().AsQueryable();

                query = query.Where("Deleted=@0", false);

                if (where != null)
                    query = query.Where(where);

                return query.ToList();
            }
        }

        public virtual IEnumerable<T> Table()
        {
            return Table(where: null).ToList();
        }

        public virtual IEnumerable<T> Table(string indexName, string whereClause)
        {
            using (var session = _documentStore.Database.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;
                var query = session.Advanced.DocumentQuery<T>(indexName);

                if (whereClause.IsNullOrEmpty() == false)
                    query = query.WhereLucene(indexName, whereClause);

                return query.ToList();
            }
        }

        //private void WriteAudit(T post)
        //{
        //    var curentUser = _authenticationService.GetAuthenticatedCustomer();
        //    if (curentUser != null)
        //    {
        //        if (post is TrackEntity)
        //        {
        //            var activeEntity = post as TrackEntity;
        //            if (activeEntity.Id == null)
        //            {
        //                activeEntity.CreatedBy = curentUser.Id;
        //                activeEntity.CreatedAt = DateTime.Now;
        //                activeEntity.CreatedByLogId = curentUser.SessionId;
        //            }
        //            else
        //            {
        //                activeEntity.ModifiedBy = curentUser.Id;
        //                activeEntity.ModifiedAt = DateTime.Now;
        //                activeEntity.ModifiedByLogId = curentUser.SessionId;
        //            }
        //        }

        //        //if (post is ICompanyEntity && curentUser.IsManager == false)
        //        //    post.SetPropertyValue("CompanyId", curentUser.CompanyId);
        //    }
        //}

        //private void WriteAudit(List<T> post)
        //{
        //    post.Each(t =>
        //    {
        //        WriteAudit(t);
        //    });
        //}

        private bool TryValidate(object @object)
        {
            /*ICollection<ValidationResult> errors = new List<ValidationResult>();
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            var result = Validator.TryValidateObject(
                @object, context, errors,
                validateAllProperties: true
            );

            if (result)
                return true;
            else
                throw new ValidationException(string.Join(",", errors.Select(f => f.MemberNames)));*/

            return true;
        }

        public IEnumerable<T> Table(SmartFilterCommand cmd)
        {
            var result = _documentStore.Query<T>(cmd);
            return result.Result;
        }
    }
}