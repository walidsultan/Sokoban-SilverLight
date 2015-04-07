using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.CS.Config;
using System.Web;

namespace Sokoban.DataAccessLayer
{
    public class DataManager
    {
        private static IObjectServer _Server;

        private static IObjectServer Server
        {
            get
            {
                if (_Server == null)
                {
                    IServerConfiguration serverConfiguraiton = Db4objects.Db4o.CS.Db4oClientServer.NewServerConfiguration();
                    _Server = Db4objects.Db4o.CS.Db4oClientServer.OpenServer(serverConfiguraiton, HttpContext.Current.Request.PhysicalApplicationPath + @"App_Data\Sokoban.db4o", 0);
                }
                return _Server;
            }
        }

        private IObjectContainer _Client;

        protected  IObjectContainer Client
        {
            get { return _Client; }
        }

        protected DataManager()
        {
            lock (Server)
            {
                _Client = Server.OpenClient();
            }
        }

        ~DataManager()
        {
            if (_Client != null)
            {
                _Client.Close();
            }
        }

        public static  void StopServer() {
            if (_Server != null)
            {
                _Server.Close();
                _Server = null;
            }
        }
    
    }
}
