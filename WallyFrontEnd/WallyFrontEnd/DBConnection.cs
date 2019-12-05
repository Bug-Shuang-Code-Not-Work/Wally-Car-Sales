/*
 File: DBConnection.cs
 Project: RDB A-04
 Programmer: Shuang Liang 7492259
 First Version: 2018-12-01
 Description: This is the mysql database class
 */
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace WallyFrontEnd
{

    //class: DBConnection
    //Description: This class contains all logic making select, update and insert queries 
    class DBConnection
    {
        //database connection
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;




        //Function: DBConnection
        //Parameter: none
        //Return: none
        //Description: constructor
        public DBConnection()
        {
            Initialize();
        }



        //Description:Initialize connection
        private void Initialize()
        {
            server = "localhost";
            database = "SLWally";
            uid = "root";
            password = "940726";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);


        }


        //Function: OpenConnection
        //Parameter: none
        //Return: true if connected, false otherwise
        //Description: open connection before each query
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }




        //Function: CloseConnection
        //Parameter: none
        //Return: true if closed, false otherwise
        //Description:  close connection after each query
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }




        //Function: SelectItem
        //Parameter: List<string> columns, List<string> tables, List<string> conditions
        //Return: List<string[]> all data being returned from the query
        //Description: select item   every string in condition is a pair of field and value
        public List<string[]>SelectItem(List<string> columns, List<string> tables, List<string> conditions)
        {


            string query = "SELECT "; 

            //columns selected
            for(int i = 0; i < columns.Count; i++)
            {
                //not the last one
                if( i != columns.Count - 1)
                {
                    query += columns[i] + ", ";
                }
                else
                {
                    query += columns[i] + " FROM ";
                }
            }

            //tables selected
            for (int i = 0; i < tables.Count; i++)
            {
                if(i != tables.Count -1)
                {
                    query += tables[i] + ", ";
                }
                else
                {
                    query += tables[i];
                }
          
            }



            //conditions after where clause
            if(conditions[0] != "none")
            {


                query += " WHERE ";

                //string[] FieldValue = new string[2];
                for (int i = 0; i < conditions.Count; i++)
                {
                    string[] FieldValue = conditions[i].Split('|');

                    //not the last one
                    if(i != conditions.Count -1)
                    {
                        query += FieldValue[0] + " = " + FieldValue[1] + " AND ";
                    }
                    else
                    {
                        query += FieldValue[0] + " = " + FieldValue[1] + "";
                    }
                }

            }


            //Create a list to store the result
            List<string[]> list = new List<string[]>();

            //Open connection
            if (OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    //make string[] the size of the row being returned
                    string[] row = new string[dataReader.FieldCount];
                    //fill sting[] with row data
                    for (int i = 0; i < row.Length; i++)
                        row[i] = dataReader[i].ToString();
                    //add string[] to return list
                    list.Add(row);
                }

                //close stuff
                dataReader.Close();
                CloseConnection();

                //return list to be displayed
                return list;
            }
            else
                return list;



        }






        //Function: SelectType
        //Parameter: string column, string table, bool isDistinct
        //Return: List <string[]> all types in a list
        //Description: get item types
        public List<string[]> SelectType(string column, string table, bool isDistinct)
        {
            string query;

            //generate the query
           if(isDistinct == true)
            {
                query = "Select distinct " + column + " from " + table + ";";
            }
            else
            {
                query = "Select " + column + " from " + table + ";";
            }
            
       
            //Create a list to store the result
            List<string[]> list = new List<string[]>();

            //Open connection
            if (OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    //make string[] the size of the row being returned
                    string[] row = new string[dataReader.FieldCount];
                    //fill sting[] with row data
                    for (int i = 0; i < row.Length; i++)
                        row[i] = dataReader[i].ToString();
                    //add string[] to return list
                    list.Add(row);
                }

                //close stuff
                dataReader.Close();
                CloseConnection();

                //return list to be displayed
                return list;
            }
            else
                return list;

        }


        //Function: GetLastInsertID
        //Parameter:  none
        //Return: string last insert ID
        //Description: get last insert ID
        public string GetLastInsertID()
        {
            string query = "SELECT LAST_INSERT_ID()";

            string lastInsertID = "";

            //Create a list to store the result
            List<string[]> list = new List<string[]>();

            //Open connection
            if (OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    //make string[] the size of the row being returned
                    string[] row = new string[dataReader.FieldCount];
                    //fill sting[] with row data
                    for (int i = 0; i < row.Length; i++)
                        row[i] = dataReader[i].ToString();
                    //add string[] to return list
                    list.Add(row);
                }

                //close stuff
                dataReader.Close();
                CloseConnection();

                if(list[0][0] != null)
                {
                    lastInsertID = list[0][0];
                }

                 else
                {
                    lastInsertID = "0";
                }
            }

                return lastInsertID;
        }



        //Function: CountInStockVehicle
        //Parameter: string arg1, string arg2
        //Return: int count 
        //Description:   count in stock vehicle
        public int CountInStockVehicle(string arg1, string arg2)
        {
            string query = "SELECT Count(*) FROM vehicle where " + arg1 + " = " + "'" + arg2 + "' and inStock = 'Yes';";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Function: Insert
        //Parameter: string tableName, string fields, string value
        //return: None
        //Description:  this function creates insert query
        //Insert   fields column|column|....        value|value|value|....
        public void Insert(string tableName, string fields, string values)
        {


            string query = "INSERT INTO " + tableName + " (";



            string[] splitFields = fields.Split('|');
       
            //fields in the table
          for(int i = 0; i < splitFields.Length; i++)
            {   
                // not the last one
                if(i != splitFields.Length - 1)
                {
                    query += (" " + splitFields[i] + ",");
                }
                else
                {
                    query += (" " + splitFields[i]);
                }

            }

            query += ") VALUES (";

            // actual values
            string[] splitValues = values.Split('|');

            for (int i = 0; i < splitValues.Length; i++)
            {
                // not the last one
                if( i != splitValues.Length - 1)
                {
                    query += ( splitValues[i] +  ", ");
                }
                else
                {
                    query += (splitValues[i] );
                }
            }

            //end of query
            query += ");";

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();

            }
        }


        //Function: Update
        //Parameter: string table, List<string> fields, List<string> conditions
        //Return: None
        //Description: This function creates update query
        public void Update(string table, List<string> fields, List<string> conditions)
        {
            string query = "UPDATE " + table + " SET ";


            //append update fields
            for(int i = 0; i < fields.Count; i++)
            {
                string[] FieldValue = fields[i].Split('|');

                if (i != fields.Count - 1)
                {
                    query += FieldValue[0] + " = " + FieldValue[1] + ", "; 


                }
                else
                {
                    query += FieldValue[0] + " = " + FieldValue[1];
                }

            }

            query += " WHERE ";

            //append conditions
            for(int i = 0; i < conditions.Count; i++)
            {
                string[] FieldValue = conditions[i].Split('|');

                //not the last one
                if (i != conditions.Count - 1)
                {
                    query += FieldValue[0] + "=" + FieldValue[1] + " AND ";
                }
                else
                {
                    query += FieldValue[0] + " = " + FieldValue[1];
                }
            }

            //send the query
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();

            }



        }


    }



}
