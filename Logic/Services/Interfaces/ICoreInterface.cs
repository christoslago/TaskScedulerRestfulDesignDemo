using DataRepository.Models;
using Logic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services.Interfaces
{
    public interface ICoreInterface<ObjModel> where ObjModel : CoreModel
    {
        Envelope<ObjModel> Add(ObjModel obj);
        Envelope<ObjModel> GetAll();
        Envelope<ObjModel> GetByName(string id);
        Envelope<ObjModel> GetByID(Guid id);
        Envelope<ObjModel> Update(ObjModel obj);
        Envelope<ObjModel> DeleteByID(Guid id);
    }
}
