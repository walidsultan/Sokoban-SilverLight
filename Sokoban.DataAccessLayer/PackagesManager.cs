using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.DataTypes;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o;

namespace Sokoban.DataAccessLayer
{
    public class PackagesManager:DataManager 
    {
        public IQueryable<Package> GetAllPackages()
        {
            return (from Package p in Client select p).AsQueryable<Package>();
        }

        public void InsertPackage(Package package)
        {
            package.Id = (from Package p in Client select p).Count() + 1;
            Client.Store(package);
            Client.Commit();
        }

    }
}
