using DataRepository.Models;
using FirstAPI.Models;
using Logic.DTOs;
using Logic.Helpers;
using Microsoft.EntityFrameworkCore;
using Repository.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class CoreService<ObjModel> where ObjModel : CoreModel
    {
        private DataContext ContextAccessor;
        private List<string> QueryIncludesList;
        public CoreService(DataContext contextAccessor)
        {
            ContextAccessor = contextAccessor;
            QueryIncludesList = new List<string>();
        }
        public virtual void QueryIncludes(List<string> include)
        {
            foreach (var incl in include)
            {
                QueryIncludesList.Add(incl);
            }

        }

        public virtual ObjModel BeforeAdd(ObjModel obj)
        {
            return obj;
        }
        public virtual Envelope<ObjModel> Add(ObjModel obj)
        {
            var collection = new Envelope<ObjModel>();
            obj = BeforeAdd(obj);
            ContextAccessor.getDBSet<ObjModel>().Add(obj);
            ContextAccessor.Entry(obj).State = EntityState.Added;
            var savedResult = 0;
            try
            {
                savedResult = ContextAccessor.SaveChanges();
            }
            catch (Exception e)
            {
                collection.Logger.AddError(e.Message, "Add");
                if (e.InnerException != null)
                {
                    collection.Logger.AddError(e.InnerException.Message, "Add");
                }
            }
            if (savedResult > 0)
            {
                collection.Collection.Add(obj);
            }
            else
            {
                collection.Logger.AddError("Error while writing Database", "Add");
            }
            return collection;
        }
        public virtual Envelope<ObjModel> AfterGet(Envelope<ObjModel> collection)
        {
            return collection;
        }
        public virtual Envelope<ObjModel> GetAll()
        {
            var collection = new Envelope<ObjModel>();
            var query = ContextAccessor.getDBSet<ObjModel>().AsQueryable();
            foreach (var include in QueryIncludesList)
            {
                query = query.Include(include);
            }
            var objectCollection = query.ToList();
            if (objectCollection.Count == 0)
            {
                collection.Logger.AddError("No Record Found", "GetAll");
            }
            else
            {
                collection.Collection = objectCollection;
                collection = AfterGet(collection);
            }
            return collection;
        }

        public virtual Envelope<ObjModel> GetByID(Guid objID)
        {
            var collection = new Envelope<ObjModel>();
            var obj = ContextAccessor.getDBSet<ObjModel>().AsNoTracking().Where(x => x.ID == objID).FirstOrDefault();
            if (obj != null)
            {
                collection.Collection.Add(obj);
            }
            else
            {
                collection.Logger.AddError("Entity not found", "GetByID");
            }
            return collection;
        }
        public virtual Envelope<ObjModel> GetByName(string name)
        {
            var collection = new Envelope<ObjModel>();
            var objects = ContextAccessor.getDBSet<ObjModel>().AsNoTracking().Where(x => x.Name.ToLower() == name.ToLower()).ToList();
            if (objects.Count > 0)
            {
                foreach (var obj in objects)
                {
                    collection.Collection.Add(obj);
                }
            }
            else
            {
                collection.Logger.AddError("Not found", "GetByName");
            }
            return collection;
        }
        public Envelope<ObjModel> Update(ObjModel obj)
        {
            var collection = new Envelope<ObjModel>();
            var objectInDB = ContextAccessor.getDBSet<ObjModel>().Where(x => x.ID == obj.ID).FirstOrDefault();
            if (objectInDB == null)
            {
                collection.Logger.AddError("Entity not found to update", "Update");
                return collection;
            }
            var savedResult = 0;
            try
            {
                ContextAccessor.Entry(objectInDB).CurrentValues.SetValues(obj);
                savedResult = ContextAccessor.SaveChanges();
            }
            catch (Exception e)
            {
                collection.Logger.AddError(e.Message, "Update");
                if (e.InnerException != null)
                {
                    collection.Logger.AddError(e.InnerException.Message, "Add");
                }
            }
            if (savedResult > 0)
            {
                collection.Collection.Add(obj);
            }
            return collection;
        }
        public virtual Envelope<ObjModel> DeleteByID(Guid objID)
        {
            var collection = new Envelope<ObjModel>();
            var obj = ContextAccessor.getDBSet<ObjModel>().Where(x => x.ID == objID).FirstOrDefault();
            if (obj == null)
            {
                collection.Logger.AddError("Entity not found to delete", "DeleteByID");
                return collection;
            }
            var savedResult = 0;
            ContextAccessor.getDBSet<ObjModel>().Remove(obj);
            ContextAccessor.Entry(obj).State = EntityState.Deleted;
            try
            {
                savedResult = ContextAccessor.SaveChanges();
            }
            catch (Exception e)
            {
                collection.Logger.AddError(e.Message, "DeleteByID");
                if (e.InnerException != null)
                {
                    collection.Logger.AddError(e.InnerException.Message, "Add");
                }
            }
            return collection;
        }
    }
}
