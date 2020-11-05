using MigrationTesting.Support;
using NUnit.Framework;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using TechTalk.SpecFlow;

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

        [Then(@"verify the data in the first row of the (.*) table is")]
        public void ThenVerifyTheDataInTheFirstRowOfTheTableIs(string name, Table table)
        {
            string query = $"select top 1 * from {name}";

            SqlCommand sqlCmd = new SqlCommand(query, conn);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            DataTable tableConverted = stepHelper.ConvertSpecFlowTableToDataTable(table);

            ArrayList match = stepHelper.IsTableSame(dt, tableConverted);
            Assert.IsTrue((bool)match[0], $"Table '{table}' schema did not match expected result because {match[1]}.");
        }

        // Alternative to the above method
        //[Then(@"verify the data in the first row of the (.*) table is")]
        //public void ThenVerifyTheDataInTheFirstRowOfTheTableIs(string name, Table table)
        //{
        //    string query = $"select top 1 * from {name}";

        //    SqlCommand sqlCmd = new SqlCommand(query, conn);
        //    SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
        //    DataTable dt = new DataTable();
        //    dataAdapter.Fill(dt);

        //    for (int i = 0; i < table.Rows[0].Count(); i++)
        //    {
        //        string columnName = table.Rows[0].AsEnumerable().ElementAt(i).Key;
        //        string tableRowValue = table.Rows[0].AsEnumerable().ElementAt(i).Value;
        //        string queryRowValue = dt.Rows[0][columnName].ToString();
        //        Assert.AreEqual(tableRowValue, queryRowValue, $"The data in the first row of '{table}' table did not match expected result.");
        //    }
        //}

        [Then(@"verify the data in the last row of the (.*) table is")]
        public void ThenVerifyTheDataInTheLastRowOfTheTableIs(string name, Table table)
        {
            string query = $"select top 1 * from {name} order by {name}ID desc";

            SqlCommand sqlCmd = new SqlCommand(query, conn);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            DataTable tableConverted = stepHelper.ConvertSpecFlowTableToDataTable(table);

            ArrayList match = stepHelper.IsTableSame(dt, tableConverted);
            Assert.IsTrue((bool)match[0], $"Table '{table}' values did not match expected result because {match[1]}.");
        }

        [Then(@"verify the data in row (.*) of the (.*) table is")]
        public void ThenVerifyTheDataInRowOfTheTableIs(int p0, string name, Table table)
        {
            string query = $"declare @id int;select top 2 @id={name}ID from {name};select * from {name} where {name}ID=@id";

            SqlCommand sqlCmd = new SqlCommand(query, conn);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            DataTable tableConverted = stepHelper.ConvertSpecFlowTableToDataTable(table);

            ArrayList match = stepHelper.IsTableSame(dt, tableConverted);
            Assert.IsTrue((bool)match[0], $"Table '{table}' values did not match expected result because {match[1]}.");
        }

        [Then(@"verify the data with (.*) equal to (.*) in the (.*) table is")]
        public void ThenVerifyTheDataWithEqualToInTheTableIs(string column, int p0, string name, Table table)
        {
            string query = $"select * from {name} where {column} = {p0}";

            SqlCommand sqlCmd = new SqlCommand(query, conn);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            DataTable tableConverted = stepHelper.ConvertSpecFlowTableToDataTable(table);

            ArrayList match = stepHelper.IsTableSame(dt, tableConverted);
            Assert.IsTrue((bool)match[0], $"Table '{table}' values did not match expected result because {match[1]}.");
        }

        [Then(@"verify the count of records in the (.*) table with a (.*) of (.*) is (.*)")]
        public void ThenVerifyTheCountOfRecordsInTherTableWithAOfIs(string name, string column, string value, int count)
        {
            string query = $"select count(*) from {name} where {column} = '{value}'";
            SqlCommand sqlCmd = new SqlCommand(query, conn);
            int actualCount = (int)sqlCmd.ExecuteScalar();

            Assert.AreEqual(count, actualCount, $"'{name}' table count - expected '{count}' but was actually '{actualCount}'.");
        }

        [Then(@"verify the sum of all values in the (.*) column of the (.*) table is (.*)")]
        public void ThenVerifyTheSumOfAllValuesInTheColumnOfTheTableIs(string column, string name, int count)
        {
            string query = $"select sum({column}) from {name}";
            SqlCommand sqlCmd = new SqlCommand(query, conn);
            int actualCount = (int)sqlCmd.ExecuteScalar();

            Assert.AreEqual(count, actualCount, $"'{name}' table count - expected '{count}' but was actually '{actualCount}'.");

        }

        [Then(@"verify the (.*) in the (.*) table always has the format (.*)")]
        [Then(@"verify the (.*) in the (.*) table always start with (.*)")]
        public void ThenVerifyTheTableAlwaysStartsWith(string column, string name, string format)
        {
            bool match;

            string query = $"SELECT count(*) FROM {name} where {column} not like ('{format}%')";
            SqlCommand sqlCmd = new SqlCommand(query, conn);
            match = (int)sqlCmd.ExecuteScalar() == 0;

            Assert.IsTrue(match, $"Table '{name}', column name '{column}' - expected all records to start / have the format with {format}.");
        }

        //[Then(@"verify the the (.*) in the (.*) table always has the format dd-mm-yy")]
        //public void ThenVerifyTheTheBirthdateInTheCustomerTableAlwaysHasTheFormatDd_Mm_Yy()
        //{
        //    bool match;

        //    string query = $"SELECT count(*) FROM {name} where {column} not like ('{startingWith}%')";
        //    SqlCommand sqlCmd = new SqlCommand(query, conn);
        //    match = (int)sqlCmd.ExecuteScalar() == 0;

        //    Assert.IsTrue(match, $"Table '{name}', column name '{column}' - expected all records to start with {startingWith}.");

        //}



    }
}

