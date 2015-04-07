using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o;
using Sokoban.DataTypes;

namespace Sokoban.DataAccessLayer
{
   public  class ProgressManager:DataManager 
    {
       public IQueryable<Progress > GetAllProgress()
       {
           return (from Progress p in Client select p).AsQueryable<Progress>();
       }

       public IQueryable<Progress> GetProgressByUserId(int userId)
       {
           return (from Progress p in Client where p.UserId ==userId select p ).OrderBy(p=>p.Id).AsQueryable<Progress>();
       }

       public void InsertProgress(Progress progress)
       {
           progress.Id = (from Progress p in Client select p).Count() + 1;
           Client.Store(progress);
           Client.Commit();
       }

       public void UpdateProgress(Progress progress)
       {
           Client.Store(progress);
           Client.Commit();
       }

    }
}
