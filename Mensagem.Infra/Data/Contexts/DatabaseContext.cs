using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Dapper.FluentMap.Dommel.Mapping;
using System.Data.Common;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;


namespace Mensagem.Infra.Data.Contexts
{
    public class DatabaseContext
    {
        private readonly DbConnectionStringBuilder _dbConnectionStringBuilder;
        public DatabaseContext(DbConnectionStringBuilder dbConnectionStringBuilder)
        {
            _dbConnectionStringBuilder = dbConnectionStringBuilder;
            Register();
        }

        private static IEnumerable<Type> Mappers { get; set; }
        public static void Register()
        {
            if (Mappers == null)
            {
                Mappers = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.DeclaringType != null && t.DeclaringType.BaseType != null && t.DeclaringType.BaseType.IsGenericType)
                    .Select(t => t.DeclaringType.BaseType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(DommelEntityMap<>))
                    .SelectMany(c => c.GetGenericArguments());
            }

            if (FluentMapper.EntityMaps == null || !FluentMapper.EntityMaps.Any())
            {
                FluentMapper.Initialize(config =>
                {
                    var mappedClasses = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(x => x.GetInterfaces()
                            .SelectMany(y => y.GenericTypeArguments)
                            .Any(z => Mappers.Contains(z))
                            && !x.Name.Contains("Repository")
                        );

                    foreach (var type in mappedClasses)
                    {
                        dynamic configurationInstance = Activator.CreateInstance(type);
                        config.AddMap(configurationInstance);
                    }

                    config.ForDommel();
                });
            }
        }
        internal IDbConnection GetConnection()
        {
            var conn = SqlClientFactory.Instance.CreateConnection();
            conn.ConnectionString = _dbConnectionStringBuilder.ConnectionString;
            return conn;
        }

    }
}
