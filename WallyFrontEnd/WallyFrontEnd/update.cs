using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallyFrontEnd
{
   partial class Program
    {


        //sales order need  1. location from dealership table 
        public string location { get; set; }
        // 2. lastUpdateDate, 3. orderstatus   14. orderID from order table
        public string lastUpdateDate { get; set; }
        public string orderID { get; set; }

        // 4. first Name, 5. last Name from customer table
        public string firstName { get; set; }
        public string lastName { get; set; }
        //6. year, 7. made, 8. model, 9. colour 10. Kms from vehicle table
        public string versionYear { get; set; }
        public string made { get; set; }
        public string model { get; set; }
        public string colour { get; set; }
        public string Kms { get; set; }
        // 11. VIN, 12.tradein, 13. sprice, dealershipID from orderline


        public const int isHOLDToPAID = 1;
        public const int isHOLDToCNCL = 2;
        public const int isPAIDToRFND = 3;



        //Function: FindUpdateOrder
        //Parameter: none
        //Return: none
        //Description: This is the find and update order subroutine.
        public void FindUpdateOrder()
        {
            //ask for phone num to find all order in this name
            Console.Write("Please input your phone number:");
            string phone = Console.ReadLine();

            //find the customer with this number
            List<string> column = new List<string>();
            column.Add("customerID");
            column.Add("firstName");
            column.Add("lastName");

            List<string> table = new List<string>();
            table.Add("customer");

            //find customerID by phone
            List<string> condition = new List<string>();
            condition.Add("phoneNumber|" + "'" + phone + "'");

            List<string[]> customerList = SLWally.SelectItem(column, table, condition);

            if (customerList.Count == 0)
            {
                Console.WriteLine("Customer not found in the database.");
                return;
            }

         
            else
            {   
                //now we get  the customerID
                fk_customerID = customerList[0][0];


                //----------------------------------------
                // got the first name
                //----------------------------------------
                firstName = customerList[0][1];


                //----------------------------------------
                // got the last name
                //----------------------------------------
                lastName = customerList[0][2];


                //find all previous orders of this customer
                column.Clear();
                column.Add("*");

                table.Clear();
                table.Add("orders");

                condition.Clear();
                condition.Add("fk_customerID|" + fk_customerID);

              //get all return orders of this customer
                List<string[]> orderList = SLWally.SelectItem(column, table,condition);
                string[] theOrder = new string[4];
                if(orderList.Count == 0)
                {
                    Console.WriteLine("There are no previous orders!");
                    return;
                }

                else
                {
                    Console.Clear();

                    Console.WriteLine("Order   Customer      Date     OrderStatus");
                    for(int i = 0; i < orderList.Count; i++)
                    {
                        Console.WriteLine("{0}.    {1} {2}       {3}      {4}", i + 1, firstName, lastName, orderList[i][2], orderList[i][3]);
                    }

                    // let the customer choose one
                    Console.Write("Select an Order:");
                    String inputSelect = Console.ReadLine();
                    int customerChoice = 0;
                    while(!int.TryParse(inputSelect, out customerChoice) || (customerChoice < 1 || customerChoice > orderList.Count))
                    {
                        Console.WriteLine("Invalid option!");
                        Console.Write("Select an Order:");
                        inputSelect = Console.ReadLine();
                    }

                    //this is the order we need

                    theOrder = orderList[customerChoice - 1];
                    
                }


                //-----------------------------------------
                // got the order ID
                //-----------------------------------------
                orderID = theOrder[0];


                //-----------------------------------------
                // got the order status
                //-----------------------------------------
                orderStatus = theOrder[3];

                //-----------------------------------------
                // got the lastUpdatedate
                //-----------------------------------------
                lastUpdateDate = theOrder[2];

                //now find more details in the orderline with theOrder[0] which is the orderID

                column.Clear();
                column.Add("fk_dealershipID");
                column.Add("fk_VIN");
                column.Add("sPrice");
                column.Add("tradeIn");

                table.Clear();
                table.Add("orderline");

                condition.Clear();
                condition.Add("fk_orderID|" + theOrder[0]);

                List<string[]> orderlineList = SLWally.SelectItem(column, table, condition);
                string[] theOrderLine = new string[4];
                if(orderlineList.Count == 0)
                {
                    Console.WriteLine("Database referencing error");
                    return;
                }
                else
                {
                    theOrderLine = orderlineList[0];
                }


                //------------------------------
                // got the VIN of the car
                //------------------------------
                fk_VIN = theOrderLine[1];

                //------------------------------
                // got the Sprice of the car
                //------------------------------
                sPrice = theOrderLine[2];

                //------------------------------
                // got the tradein of the car
                //------------------------------
                tradeIn = theOrderLine[3];


                //now find out the details of the vehicle with te fk_VIN  

                column.Clear();
                column.Add("versionYear");
                column.Add("Make");
                column.Add("model");
                column.Add("Colour");
                column.Add("Kms");

                table.Clear();
                table.Add("vehicle");

                condition.Clear();
                condition.Add("VIN|" + "'" + fk_VIN+ "'");


                List<string[]> vehicleList = SLWally.SelectItem(column, table, condition);
                string[] TheVehicle = new string[5];
                if(vehicleList.Count == 0)
                {
                    Console.WriteLine("Database referencing error");
                    return;
                }
                else
                {

                    TheVehicle = vehicleList[0];

                }
                //------------------------------
                // got the versionyear of the car
                //------------------------------
                versionYear = TheVehicle[0];
                //------------------------------
                // got the make of the car
                //------------------------------
                made = TheVehicle[1];
                //------------------------------
                // got the model of the car
                //------------------------------
                model = TheVehicle[2];
                //------------------------------
                // got the colour of the car
                //------------------------------
                colour = TheVehicle[3];
                //------------------------------
                // got the Kms of the car
                //------------------------------
                Kms = TheVehicle[4];


                //now find the location of dealership with fk_dealershipID in the orderLine[0]

                column.Clear();
                column.Add("Location");

                table.Clear();
                table.Add("dealership");

                condition.Clear();
                condition.Add("dealershipID|" + theOrderLine[0]);

                List<string[]> dealerList = SLWally.SelectItem(column, table, condition);

                if(dealerList.Count == 0)
                {
                    Console.WriteLine("Database referencing error");
                    return;
                }
                else
                {
                    //------------------------------
                    // got the location of dealership
                    //------------------------------
                    location = dealerList[0][0];
                }
                //display sales record
                DisplaySalesRecord();
                

                //ask to update order status depending on the current order status
                if(orderStatus == "RFND" || orderStatus == "CNCL")
                {
                    Console.Write("Press any key to go back to main menu...");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
               
                else
                {
                    Console.Write("Update Status(press X to quit Update):");
                    string newStatus = Console.ReadLine();
                    newStatus = newStatus.ToUpper();
                    if(newStatus == "X")
                    {
                        Console.Clear();
                        return;
                    }

                    while(!ValidateChangeStatus(newStatus))
                    {
                        Console.WriteLine("Invalid status! HOLD can be changed to CNCL or PAID and PAID can only be changed to RFND");
                        Console.Write("Update Status(press X to quit Update):");
                         newStatus = Console.ReadLine();
                        newStatus = newStatus.ToUpper();

                        if (newStatus == "X")
                        {
                            Console.Clear();
                            return;
                        }
                    }

                    Console.Write("Please Input Update date:");
                    string newDate = Console.ReadLine();

                    while(!ValidateDate(newDate))
                    {
                        Console.WriteLine("Invalid Date format! The format is yyyy-mm-dd.");
                        Console.Write("Please Input Update date:");
                        newDate = Console.ReadLine();

                    }

                    //get the new status date
                    lastUpdateDate = newDate;


                    //hold to paid
                    if(orderStatus == "HOLD" && newStatus == "PAID")
                    {
                        UpdateStatus(newStatus, isHOLDToPAID);
                    }


                    //hold to cncl
                    if(orderStatus == "HOLD" && newStatus == "CNCL")
                    {
                        UpdateStatus(newStatus, isHOLDToCNCL);
                    }

                    //paid to rfnd
                    if(orderStatus == "PAID" && newStatus == "RFND")
                    {
                        UpdateStatus(newStatus, isPAIDToRFND);
                    }



                }


            }


            Console.Write("Order Status Updated! Press any key to go back to main menu...");
            Console.ReadKey();
            Console.Clear();
        }



        //Function: DisplaySalesRecord
        //Parameter: none
        //Return: none
        //Description: this method display the sales record
        public void DisplaySalesRecord()
        {
            int subtotal = int.Parse(sPrice) - int.Parse(tradeIn);
            decimal HST = subtotal*13/100;
            decimal saleTotal = subtotal + HST;

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Thank you for choosing Wally's World of Wheels at");
            Console.WriteLine("{0} for your quality used vehicle!", location);
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Date: {0}", lastUpdateDate);
            Console.WriteLine("Customer: {0} {1}!", firstName, lastName);
            Console.WriteLine("Order ID: {0} - {1}", orderID, orderStatus);
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "{0} {1} {2}, {3}", versionYear, made, model, colour);
            Console.WriteLine("VIN: {0} KMS: {1}", fk_VIN, Kms);
            Console.WriteLine(Environment.NewLine + "Purchase Price: ${0:N2}", sPrice);
            Console.WriteLine(Environment.NewLine + "Trade In: ${0:N2}", tradeIn);
            Console.WriteLine(Environment.NewLine + "Subtotal =$ {0:N2}", subtotal);
            Console.WriteLine("HST(13%)=$ {0:N2}", HST);
            Console.WriteLine("Sale Total=$ {0:N2}", saleTotal);
            Console.WriteLine("---------------------------------------------");

        }




        //Function: UpdateStatus
        //Parameter: string newStatus, int indicator
        //Return: none
        //Description: this method updates database from HOLD to PAID
        public void UpdateStatus(string newStatus, int indicator)
        {
            List<string> UpdateFields = new List<string>();
            List<string> Conditions = new List<string>();

            //update instock status to NO in vehicle table
            string updateTable = "Vehicle";

            string updateFields = "";

            if (indicator == isHOLDToPAID)
            {
                updateFields = "inStock|'NO'";
            }
           else
            {
                updateFields = "inStock|'YES'";
            }

            string conditions = "VIN|" + "'" + fk_VIN + "'";
      
            UpdateFields.Add(updateFields);
            Conditions.Add(conditions);

            SLWally.Update(updateTable, UpdateFields, Conditions);

            UpdateFields.Clear();
            Conditions.Clear();


            //update lastupdateStatus to newDate and orderstatus in orders table
            updateTable = "orders";
           
            string updateFieldStatus = "";
              
            if(indicator == isHOLDToPAID)
            {
                updateFieldStatus = "orderStatus|'PAID'";
            }
            if(indicator == isHOLDToCNCL)
            {
                updateFieldStatus = "orderStatus|'CNCL'";
            }
            if(indicator == isPAIDToRFND)
            {
                updateFieldStatus = "orderStatus|'RFND'";
            }
                                    
            string updateFieldsnewDate = "lastUpdateDate|" + "'" + lastUpdateDate + "'";

            UpdateFields.Add(updateFieldStatus);
            UpdateFields.Add(updateFieldsnewDate);

            conditions = "orderID|" + orderID;
            Conditions.Add(conditions);

            SLWally.Update(updateTable, UpdateFields, Conditions);

            UpdateFields.Clear();
            Conditions.Clear();


            if (indicator != isHOLDToPAID)
            {
                //zero out sprice and trade in orderline table

                updateTable = "orderline";
                string updateFieldsPrice = "sPrice|0";
                string updateFieldTradeIn = "tradeIn|0";
                UpdateFields.Add(updateFieldsPrice);
                UpdateFields.Add(updateFieldTradeIn);

                conditions = "fk_orderID|" + orderID;
                Conditions.Add(conditions);

                SLWally.Update(updateTable, UpdateFields, Conditions);


                UpdateFields.Clear();
                Conditions.Clear();

            }


        }





        //Function: ValidateChangeStatus
        //Parameter: string newStatus
        //Return: true if valid, false otherwise
        //Description: this method validate the new change status
        public bool ValidateChangeStatus(string newStatus)
        {
            bool status = true;

            if (orderStatus == "HOLD")
            {
                if (newStatus == "PAID" || newStatus == "CNCL")
                {
                    status = true;
                }

                else
                {
                    status = false;
                }
            }

            else if(orderStatus == "PAID")

            {
                if(newStatus == "CNCL")
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }

            return status;
        }


        //Function: DisplayInventoryLevel
        //Parameter: none
        //Return: none
        //Description: display inventory level
        public void DisplayInventoryLevel()
        {
            string column = "model";
            string table = "vehicle";
            //select all distinct model from vehicle
            List<string[]> retList = SLWally.SelectType(column, table, true);

            //get the model type from the 1 element
            List<string> models = new List<string>();

            foreach (string[] row in retList)
            {
                models.Add(row[0]);
            }

            List<int> howMany = new List<int>();
            //for each vehicle select count(*)
            foreach (string model in models)
            {

                howMany.Add(SLWally.CountInStockVehicle(column, model));

            }

            Console.WriteLine("All Model           Num in Stock");
            for (int i = 0; i < models.Count; i++)
            {
                Console.WriteLine("{0,-10}          {1, -10}", models[i], howMany[i]);
            }




            Console.Write("Press any key to go back to main menu...");
            Console.ReadKey();
            Console.Clear();
        }
       
    }
}
