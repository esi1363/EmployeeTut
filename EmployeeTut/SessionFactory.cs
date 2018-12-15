using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NHibernate.Cfg;
using FluentNHibernate.Automapping;

namespace EmployeeTut
{
    public class SessionFactory
    {
        private static volatile ISessionFactory sessionFactory;
        private static object syncRoot = new object();
        public static ISession OpenSession
        {
            get
            {
                if (sessionFactory == null)
                {
                    lock (syncRoot)
                    {
                        if (sessionFactory == null)
                            sessionFactory = BuildSessionFactory();
                    }
                }
                return sessionFactory.OpenSession();
            }
        }

        private static ISessionFactory BuildSessionFactory()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
                return Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(connectionString))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
                    .ExposeConfiguration(BuildSchema)
                    .BuildSessionFactory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
        }
        //Create Session
        private static AutoPersistenceModel CreateMappings()
        {
            return AutoMap.Assembly(System.Reflection.Assembly.GetCallingAssembly())
                .Where(testc => testc.Namespace == "EmployeeTut.Model");
        } 
        private static void BuildSchema(Configuration cfg)
        {
            //CreateMappings();
        }
    }
}
