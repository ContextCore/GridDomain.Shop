using System;
using System.Data.SqlClient;
using Shop.Infrastructure;
using Shop.Tests.Unit;

namespace Shop.Tests.Acceptance
{
    public class SqlSequence_tests : Sequence_provider_tests,
                                     IDisposable
    {
        public SqlSequence_tests()
        {
            var prov = new SqlSequenceProvider(ConnectionString);
            prov.Connect();
            Provider = prov;
        }
        protected override ISequenceProvider Provider { get; }

        public void Dispose()
        {
            DeleteCreatedSequences("global");
            DeleteCreatedSequences(CreatedSequences.ToArray());
            CreatedSequences.Clear();
        }
//for local docker-based sql server on linux/mac
        private const string ConnectionString =
            "Server = localhost,1400; Database = ShopWrite; User = sa; Password=P@ssw0rd1; MultipleActiveResultSets = True";
//for local windows-based sql server        
//        private const string ConnectionString =
//            "Server = localhost;  Database = ShopWrite; User = sa; Password=P@ssw0rd1;  MultipleActiveResultSets = True";

        private void DeleteCreatedSequences(params string[] sequences)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                foreach (var sequence in sequences)
                    try
                    {
                        var cmd = new SqlCommand("DROP SEQUENCE " + sequence) {Connection = connection};
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        Console.WriteLine("error was occured while deleting sequence " + sequence);
                    }
            }
        }
    }
}