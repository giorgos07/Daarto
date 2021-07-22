using System;
using SqlKata;
using SqlKata.Compilers;

namespace AspNetCore.Identity.Dapper.Tables
{
    /// <summary>
    /// A base class for all identity tables.
    /// </summary>
    public class IdentityTable : TableBase
    {
      

        /// <summary>
        /// Query compliler, default set to <see cref="SqlServerCompiler"/>
        /// </summary>
        private readonly Compiler _sqlCompiler;

        /// <summary>
        /// Creates a new instance of with a sql kata compiler <see cref="IdentityTable"/>.
        /// </summary>
        /// <param name="dbConnectionFactory"></param>
        /// <param name="sqlCompiler"></param>
        public IdentityTable(IDbConnectionFactory dbConnectionFactory,Compiler sqlCompiler):this(dbConnectionFactory)
        {
            _sqlCompiler = sqlCompiler;
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="IdentityTable"/>.
        /// </summary>
        /// <param name="dbConnectionFactory"></param>
        protected IdentityTable(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory;
            _sqlCompiler = new SqlServerCompiler();
        }

        
        /// <summary>
        /// Compile the given query to raw sql
        /// </summary>
        /// <param name="query"> <see cref="Query"/></param>
        /// <returns>raw sql</returns>
        protected string CompileQuery(Query query)
        {
         return   _sqlCompiler.Compile(query).ToString();
        }
        /// <summary>
        /// The type of the database connection class used to access the store.
        /// </summary>
        //protected IDbConnection DbConnection { get; init; }
        protected readonly IDbConnectionFactory DbConnectionFactory;

        /// <inheritdoc />
        protected override void OnDispose()
        {
            
        }
    }
}
