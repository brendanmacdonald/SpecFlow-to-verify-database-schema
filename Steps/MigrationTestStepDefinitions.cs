using NUnit.Framework;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using TechTalk.SpecFlow;
using MigrationTesting.Support;

namespace MigrationTesting.Steps
{
    [Binding]
    public sealed class MigrationTestStepDefinitions
    {
        private readonly StepHelper stepHelper = new StepHelper();
        private static SqlConnection conn;

        [AfterScenario]
        public void CloseConnection()
        {
            conn.Close();
        }

        [Given(@"there is a connection to the Dynamics database")]
        public void GivenThereIsAConnectionToTheDynamicsDb()
        {
            try
            {
                conn = new SqlConnection(@"Data Source=Plaster\LRSERVER1;Initial Catalog=LRDatabase;Integrated Security=True");
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught when connecting to the database.", e);
            }
        }

        [Given(@"the (.*) table exists")]
        public void TheTableExists(string name)
        {
            bool exists;

            string query = $"select case when exists((select * from information_schema.tables where table_name = '{name}')) then 1 else 0 end";
            SqlCommand sqlCmd = new SqlCommand(query, conn);
            exists = (int)sqlCmd.ExecuteScalar() == 1;

            Assert.IsTrue(exists, $"Table '{name}' was not found in database.");
        }

        [Then(@"verify the schema of the (.*) table")]
        public void ThenVerifyTheSchemaOfTheTable(string name, Table table)
        {
            string query = $"SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{name}'";

            SqlCommand sqlCmd = new SqlCommand(query, conn);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            DataTable tableConverted = stepHelper.ConvertSpecFlowTableToDataTable(table);

            ArrayList match = stepHelper.IsTableSame(dt, tableConverted);
            Assert.IsTrue((bool)match[0], $"Table '{table}' schema did not match expected result because {match[1]}.");
        }

        [Then(@"verify the (.*) record count is (.*)")]
        public void ThenVerifyRecordCount(string name, int count)
        {
            string query = $"select count(*) from {name}";
            SqlCommand sqlCmd = new SqlCommand(query, conn);
            int actualCount = (int)sqlCmd.ExecuteScalar();

            Assert.AreEqual(count, actualCount, $"'{name}' table count - expected '{count}' but was actually '{actualCount}'.");
        }

    }
}

