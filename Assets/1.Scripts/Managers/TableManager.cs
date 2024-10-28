using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : Singleton<TableManager>
{
    private List<Table> tableList = new List<Table>();
    
    public void AddTable(Table table)
    {
        tableList.Add(table);
    }

    public Table GetTableAvailable()
    {
        foreach (Table table in tableList)
        {
            if (table.IsAvailable)
            {
                return table;
            }
        }
        return null;
    }
}
