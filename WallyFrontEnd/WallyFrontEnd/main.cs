/*
 File: Program.cs
 Project: RDB A-04
 Programmer: Shuang Liang 7492259
 First Version: 2018-12-01
 Description: This project is a front end point of sale program based on console to allow
 sales person to make and update order 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace WallyFrontEnd
{
    partial class Program
    {
        public DBConnection SLWally;

        private static string selectMenu;



        //---------------------------------------------------------
        //variables needed for make an order
        //---------------------------------------------------------
        public string fk_customerID { get; set; }
        public string orderDate { get; set; }

        public string orderStatus { get; set; }

        public  string fk_orderID { get; set; }
        public  string fk_delearshipID { get; set; }

        public  string fk_VIN { get; set; }
        public string sPrice { get; set; }
        public string tradeIn { get; set; }




        static void Main(string[] args)
        {

            Program program = new Program();
       
            program.start();
           
        }


        //Function: start
        //Parameter: none
        //Return: none
        //Description: entry point of application
        public void start()
        {
            SLWally = new DBConnection();

            Console.WriteLine("Welcome to Wally Point of Sale System");



            selectMenu = "";
            while(selectMenu != "9")
            {
         
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("1. Make Order");
                Console.WriteLine("2. Find Order");
                Console.WriteLine("3. View Current Inventory in Warehouse");
                Console.WriteLine("9. Quit");
                Console.WriteLine("-------------------------------------");
                Console.Write("Select menu:");

                selectMenu = Console.ReadLine();
                if (selectMenu == "1") MakeOrder();
                if (selectMenu == "2") FindUpdateOrder();
                if (selectMenu == "3") DisplayInventoryLevel();

            }
            

         


        }



        //Function: MakeOrder
        //Parameter: none
        //Return: none
        //Description:  make order subroutine
        public void MakeOrder()
        {


            Console.WriteLine("-------------------------------------");

            string arg1 = "Location";
            string arg2 = "Dealership";

            List<string[]> dealerList = SLWally.SelectType(arg1, arg2, true);

            List<string> dealers = new List<string>();

            foreach(string[] row in dealerList)
            {
                dealers.Add(row[0]);
            }

  
            //display dealers in the database

            for (int i = 0;  i < dealers.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i+1, dealers[i]);
            }
            Console.Write("Select a dealer:");
            string option = Console.ReadLine();

            int dealerID = 0;
            if(!Int32.TryParse(option, out dealerID))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            //--------------------------------------------------------
            // Get fk_delearshipID
            //--------------------------------------------------------
            fk_delearshipID = dealerID.ToString();

            //ask if new customer
            Console.Write("New customer?[Y/N]");
            string choice = Console.ReadLine();



            //new customer

            if (choice.ToUpper() == "Y")
            {


                //ask user to input all customer information
                Console.Write("Please input first name:");
                string firstName = Console.ReadLine();
                while(String.IsNullOrWhiteSpace(firstName))
                {
                    Console.WriteLine("Invalid input format! First Name cannot by empty");
                    Console.Write("Please input first name:");
                     firstName = Console.ReadLine();
                }

                Console.Write("Please input last name:");
                string lastName = Console.ReadLine();

                Console.Write("Please input phone number:");
                string phoneNum = Console.ReadLine();
                while (!ValidatePhone(phoneNum))
                {
                    Console.WriteLine("Invalid input format! Phone number must follow xxx-xxx-xxxx format");
                    Console.Write("Please input phone number:");
                     phoneNum = Console.ReadLine();
                }

                //insert into customer table
                string table = "Customer";
                string fields = "firstName|lastName|phoneNumber";
                string values ="'" + firstName + "'"+ "|" + "'"+lastName + "'"+"|" + "'"+phoneNum+ "'";
                SLWally.Insert(table, fields, values);

                //------------------------------------------------
                //get fk_customerID new customer
                //-------------------------------------------------
                fk_customerID = SLWally.GetLastInsertID();
            }



            //existing customer
       
            else if (choice.ToUpper() == "N")
            {
       
                //find customer in the customer table
                Console.Write("Please input your phone number: ");
                string input = Console.ReadLine();

       
                List<string> column = new List<string>();
                column.Add("customerID");
               
                List<string> table = new List<string>();
                table.Add("customer");

     
                    //find by phone
                    List<string> condition = new List<string>();
                    condition.Add("phoneNumber|" + "'"+input+ "'");

                   List<string[]> customerList = SLWally.SelectItem(column, table, condition);                  
               
                if(customerList.Count == 0)
                {
                    Console.WriteLine("Customer not found in the database.");
                    return;
                  
                }
    
                else
                {
                    //--------------------------------------------------------
                    // Get fk_customerID existing customer
                    //--------------------------------------------------------
                    fk_customerID = customerList[0][0];
                }

            }

            else
            {
                Console.WriteLine("Invalid choice!");
                return;
            }


            //Time to choose the vehicle to purchase
            bool retValue = SelectVehicle();

            //terminate ordering
            if(retValue == false)
            {
                return;
            }

            //Time to insert into order table
            Console.Write("Please input Order date:");
            string inputDate = Console.ReadLine();

            while(!ValidateDate(inputDate))
            {
                Console.WriteLine("Invalid Date format! The format is yyyy-mm-dd.");
                Console.Write("Please input Order date:");
                inputDate = Console.ReadLine();

            }

            //--------------------------------------------------------
            // Get orderDate
            //--------------------------------------------------------
            orderDate = inputDate;

            Console.Write("Please input order status:");
            string inputStatus = Console.ReadLine();

             while(!ValidateOrderStatus(inputStatus))
            {

                Console.WriteLine("Invalid order status!");
                Console.Write("Please input order status:");
                inputStatus = Console.ReadLine();

            }


            //--------------------------------------------------------
            // Get orderStatus
            //--------------------------------------------------------
            orderStatus = inputStatus.ToUpper();

            //Insert data into order table
            string ordersTable = "orders";
            string ordersFields = "fk_customerID|lastUpdateDate|orderStatus";        
            string ordersvalues = fk_customerID + "|" + "'" +orderDate + "'" + "|" + "'"+ orderStatus+ "'";

            SLWally.Insert(ordersTable, ordersFields, ordersvalues);


            //--------------------------------------------------------
            // Get orderID
            //--------------------------------------------------------
            fk_orderID = SLWally.GetLastInsertID();

            //Ask if there is a trade in 
            TradeIn();


            //insert into orderline 
            string orderlineTable = "orderline";
            string orderlineFields = "fk_orderID|fk_dealershipID|fk_VIN|sPrice|tradeIn";
            string orderlineValues = fk_orderID + "|" + fk_delearshipID + "|" + "'"+ fk_VIN + "'"+ "|" + sPrice + "|" + tradeIn;
            SLWally.Insert(orderlineTable, orderlineFields, orderlineValues);



            //update vehicle status
            string updateTable = "Vehicle";
            string updateFields = "inStock|" + "'"+ orderStatus + "'";
            string conditions = "VIN|" + "'"+ fk_VIN + "'";

       
            List<string> UpdateFields = new List<string>();
            List<string> Conditions = new List<string>();
            UpdateFields.Add(updateFields);
            Conditions.Add(conditions);

            SLWally.Update(updateTable, UpdateFields, Conditions);




            Console.Write("Order is succussfully made!(press any key to go back to main menu)...");
            Console.ReadKey();
            Console.Clear();
        }




        //Function: SelectVehicle
        //Parameter: none
        //Return: true of continue, false to terminate ordering
        //Description: dispaly and select vehicle for purchase
        public bool SelectVehicle()
        {
            //display all individual car
            Console.Clear();


            List<string> columns = new List<string>();
            List<string> tables = new List<string>();
            List<string> conditions = new List<string>();

            columns.Add("*");
            tables.Add("vehicle");
            conditions.Add("inStock|'YES'");

            List<string[]> vehicleList = SLWally.SelectItem(columns, tables, conditions);

            Console.WriteLine("     VIN     versionYear    Make    Model        Colour     Kms      Price");

            int whichCar;
            for (whichCar = 0; whichCar < vehicleList.Count; whichCar++)
            {
                int sellPrice = int.Parse(vehicleList[whichCar][6]) * 14 / 10;

                Console.WriteLine("{0}. {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10}",
                    whichCar + 1, vehicleList[whichCar][0], vehicleList[whichCar][1], vehicleList[whichCar][2], vehicleList[whichCar][3],
                    vehicleList[whichCar][4], vehicleList[whichCar][5], sellPrice);
            }

            Console.Write("Select a vehicle(press X to quit order):");
            string select = Console.ReadLine();

            if(select.ToUpper() == "X")
            {
                return false;
            }

            int customerChoice = 0;
            if (!Int32.TryParse(select, out customerChoice) || (customerChoice < 1 || customerChoice > vehicleList.Count))
            {
                Console.WriteLine("Invalid Choice!");
                Console.Write("Select a vehicle(press X to quit order):");
                select = Console.ReadLine();

                if (select.ToUpper() == "X")
                {
                    return false;
                }

            }


            //--------------------------------------------------------
            // Get fk_VIN
            //--------------------------------------------------------
            fk_VIN = vehicleList[customerChoice -1][0];
            //--------------------------------------------------------
            // Get sPrice
            //--------------------------------------------------------
            sPrice = (int.Parse(vehicleList[customerChoice -1][6]) * 1.4).ToString();

            return true;

        }




        //Function: TradeIn
        //Parameter:  none
        //Return: none 
        //Description: Apply a trade in
        public void TradeIn()
        {
            
            Console.Write("Apply a Trade In?[Y/N]:");
            string choice = Console.ReadLine();

            if(choice.ToUpper() == "N")
            {

                tradeIn = "0";
                return;
            }

            while(choice.ToUpper() != "Y" && choice.ToUpper() != "N")
            {
                Console.Write("Apply a Trade In?[Y/N]:");
                choice = Console.ReadLine();
                if (choice.ToUpper() == "N")
                {
                    tradeIn = "0";
                    return;
                }
            }

            Console.Write("Please input VIN:");
            string VIN = Console.ReadLine();



            Console.Write("Please input Year:");
            string year = Console.ReadLine();
            int testyear;
            while(!int.TryParse(year, out testyear))
            {
                Console.WriteLine("Year has to be a digit!");
                Console.Write("Please input Year:");
                year = Console.ReadLine();
            }

            Console.Write("Please input Manufactuer:");
            string made = Console.ReadLine();
            Console.Write("Please input Model:");
            string model = Console.ReadLine();
            Console.Write("Please input Colour:");
            string colour = Console.ReadLine();

            Console.Write("Please input Kms:");
            string Kms = Console.ReadLine();
            int testKms;
            while (!int.TryParse(year, out testKms))
            {
                Console.WriteLine("Kms has to be a digit!");
                Console.Write("Please input Kms:");
                Kms = Console.ReadLine();
            }

            Console.Write("Please input Price:");
            string wPrice = Console.ReadLine();
            int testPrice;
            while (!int.TryParse(year, out testPrice))
            {
                Console.WriteLine("Price has to be a digit!");
                Console.Write("Please input Price:");
                wPrice = Console.ReadLine();
            }

            string inStock = "YES";

            string table = "Vehicle";
            string vehicleFields = "VIN|versionYear|Make|Model|Colour|Kms|wPrice|inStock";
            string vehicleValues = "'"+ VIN + "'" + "|" + year + "|" + "'"+ made + "'"+ "|" + "'"+ model + "'"+ "|" +
                                   "'"+ colour + "'"+ "|" + Kms + "|" + wPrice + "|" + "'"+ inStock + "'";


            //-------------------------------------------------
            // get TradeIn
            //-------------------------------------------------
            tradeIn = wPrice;

            SLWally.Insert(table, vehicleFields, vehicleValues);
            
        }





        //Function: ValidatePhone
        //Parameter: input phone number
        //Return: true if match false if not
        //Description: this function validates input phone format
        public bool ValidatePhone(string phoneNum)
        {
            

            string format = @"\d{3}-\d{3}-\d{4}";

            Match match = Regex.Match(phoneNum, format);

            if(match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Function: ValidateDate
        //Parameter: string date
        //Return: true of date is valid format, false otherwise
        //Description: This function validates input date format
        public bool ValidateDate(string date)
        {
            string format = @"([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))";
            Match match = Regex.Match(date, format);

            if(match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Function: ValidateOrderStatus
        //Parameter: string status
        //Return: true if match and false otherwise
        //Description:  validates the order status 
        public bool ValidateOrderStatus(string status)
        {
           status = status.ToUpper();

            if(status == "PAID" || status == "HOLD")
            {
                return true;
            }

            else
            {
                return false;
            }

        }




      

    }

   
}
