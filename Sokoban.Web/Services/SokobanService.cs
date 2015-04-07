
namespace Sokoban.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using Sokoban.DataAccessLayer;
    using Sokoban.DataTypes;


    // Implements application logic using the SokobanEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class SokobanService : DomainService 
    {
        PackagesManager _AssociatedPackagesManager = new PackagesManager();
        ProgressManager _AssociatedProgressManager = new ProgressManager();
        UsersManager _AssociatedUsersManager = new UsersManager();
         
        #region Packages Methods
        public IQueryable<Package> GetPackages()
        {
            return _AssociatedPackagesManager.GetAllPackages();
        }

        public void InsertPackage(Package package)
        {
            _AssociatedPackagesManager.InsertPackage(package);
        }
        #endregion

        #region Progress Methods
        public IQueryable<Progress> GetProgresses()
        {
            return _AssociatedProgressManager.GetAllProgress ();
        }

        public IQueryable<Progress> GetProgressByUserId(int userId)
        {
            return _AssociatedProgressManager.GetProgressByUserId(userId );
        }

        public void InsertProgress(Progress progress)
        {
            _AssociatedProgressManager.InsertProgress(progress);
        }

        public void UpdateProgress(Progress currentProgress)
        {
            _AssociatedProgressManager.UpdateProgress(currentProgress);
        }
        #endregion

        #region Users Methods
        public IQueryable<User> GetUsers()
        {
            return _AssociatedUsersManager.GetAllUsers();
        }

        public IQueryable<User> GetUsersWithUsername(string username)
        {
            return _AssociatedUsersManager.GetUserByUsername(username );
        }

        public IQueryable<UserRanking> GetUsersRanking()
        {
            return _AssociatedUsersManager.GetUsersRanking();
        }

        public void InsertUser(User user)
        {
            _AssociatedUsersManager.InsertUser(user);
        }

        public void UpdateUser(User  currentUser)
        {
            _AssociatedUsersManager.UpdateUser(currentUser);
        }
        #endregion
    }
}


