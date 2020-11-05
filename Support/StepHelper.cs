using System;
using System.Collections;
using System.Data;
using System.Linq;
using TechTalk.SpecFlow;

namespace MigrationTesting.Support
{
    class StepHelper
    {
        public DataTable ConvertSpecFlowTableToDataTable(Table table)
        {
            DataTable dataTable = new DataTable();

            TableRow headerRow = table.Rows[0];
            int headerCellCount = headerRow.Count();

            for (int i = 0; i < headerCellCount; i++)
            {
                string columnName = table.Rows[0].AsEnumerable().ElementAt(i).Key;
                dataTable.Columns.Add(columnName);
            }

            foreach (var row in table.Rows)
            {
                var dataRow = dataTable.NewRow();

                for (int i = 0; i < headerCellCount; i++)
                {
                    string columnName = row.AsEnumerable().ElementAt(i).Key;
                    dataRow[columnName] = Convert.ChangeType(row[i], typeof(string));
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public ArrayList IsTableSame(DataTable dt1, DataTable dt2)
        {
            ArrayList result = new ArrayList();

            if (dt1 == null)
            {
                result.Add(false);
                result.Add("Datatable 1 was empty");
                return result;
            }
            if (dt2 == null)
            {
                result.Add(false);
                result.Add("Datatable 2 was empty");
                return result;
            }
            if (dt1.Rows.Count != dt2.Rows.Count)
            {
                result.Add(false);
                result.Add("Row count mismatch");
                return result;
            }

            if (dt1.Columns.Count != dt2.Columns.Count)
            {
                result.Add(false);
                result.Add("Column count mismatch");
                return result;
            }

            if (dt1.Columns.Cast<DataColumn>().Any(dc => !dt2.Columns.Contains(dc.ColumnName)))
            {
                {
                    result.Add(false);
                    result.Add("Column name mismatch");
                    return result;
                }
            }

            for (int i = 0; i <= dt1.Rows.Count - 1; i++)
            {
                if (dt1.Columns.Cast<DataColumn>().Any(dc1 => dt1.Rows[i][dc1.ColumnName].ToString() != dt2.Rows[i][dc1.ColumnName].ToString()))
                {
                    result.Add(false);
                    result.Add("Row data mismatch");
                    return result;
                }
            }

            result.Add(true);
            result.Add("Match!");
            return result;
        }
    }
}
