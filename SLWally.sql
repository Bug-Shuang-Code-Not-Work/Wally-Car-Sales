/*
File: SLWally.sql
Project: RD A-04
Programmer: Shuang Liang 7492259
First version: 2018-11-30
Description: This is the SQL script to create and populate the Wally Database
*/

-- create database 
CREATE DATABASE IF NOT EXISTS SLWALLY;

USE SLWALLY;


-- Customer table
DROP TABLE IF EXISTS Customer;
CREATE TABLE Customer(
	customerID int not null auto_increment primary key,
    firstName varchar(30),
    lastName varchar(30),
    phoneNumber varchar(30) not null
);

-- Customers
INSERT INTO Customer(firstName, lastName, phoneNumber)
VALUES
('Wallys World of Wheels Inc.', '', '519-555-0000'),
('Norbert','Mika', '416-555-1111'),
('Russell', 'Foubert', '519-555-2222'),
('Sean', 'Clarke', '519-555-3333');



-- Dealership table
DROP TABLE IF EXISTS Dealership;
CREATE TABLE Dealership(
	DealershipID int not null auto_increment primary key,
    Location varchar(30) not null
);




-- Dealerships
INSERT INTO Dealership(Location)
VALUES
('Sports World'),
('Guelph Auto Mall'),
('Waterloo');
    
    
-- Vehicle Table
DROP TABLE IF EXISTS Vehicle;
CREATE TABLE Vehicle(
	VIN varchar(30) not null primary key,
    versionYear int,
    Make varchar(30),
    Model varchar(30),
    Colour varchar(30),
    Kms int,
    wPrice int,
    inStock varchar(10)
);


-- vehicles
INSERT INTO Vehicle
VALUES
('58847722BRB', 2010, 'Honda', 'Civic', 'Blue', 120332, 6500, 'YES'),
('26663747GTG', 2009, 'Ford', 'Focus', 'Black', 89221 , 8950 , 'YES'),
('99277544LOL', 2012, 'Volkswagen', 'Jetta', 'Silver', 156233, 13450, 'YES'),
('27764534RTB', 2013,'Dodge','Ram', 'Red', 211023, 10900, 'YES');



-- Order Table
DROP TABLE IF EXISTS orders;
CREATE TABLE orders(
	orderID int not null auto_increment primary key,
    fk_customerID int,

	lastUpdateDate varchar(30),
    orderStatus varchar(10),
    
     FOREIGN KEY(fk_customerID) REFERENCES Customer(customerID)
);


-- order line table
DROP TABLE IF EXISTS orderline;
CREATE TABLE orderline(
	 fk_orderID int,
	 fk_dealershipID int,
	 fk_VIN varchar(30),
     
     sPrice int,
     tradeIn int,
     
    
	   FOREIGN KEY(fk_orderID) REFERENCES orders(orderID),
       FOREIGN KEY(fk_VIN) REFERENCES Vehicle(VIN),
       FOREIGN KEY(fk_dealershipID) REFERENCES dealership(dealershipID)
);




-- -------------------------------------------------------------------------
-- previous orders
-- -------------------------------------------------------------------------


-- -------------------------------------------------------------------------
--   Sean Ckarke
-- -------------------------------------------------------------------------

-- insert into orders table
INSERT INTO orders(fk_customerID, lastUpdateDate, orderStatus)
VALUES
((SELECT customerID FROM customer WHERE firstName = 'Sean'), '2017-09-20', 'PAID');

-- insert into orderline table
INSERT INTO orderline(fk_orderID, fk_dealershipID, fk_VIN, sPrice, tradeIn)
VALUES
((SELECT orderID FROM orders WHERE lastUpdateDate = '2017-09-20'),
 (SELECT dealershipID FROM dealership WHERE location = 'Sports World'),
 (SELECT VIN FROM vehicle WHERE model = 'Ram'), 15260, 0);

-- update vehicle table
UPDATE vehicle SET inStock = 'NO' WHERE VIN = '27764534RTB';



-- -------------------------------------------------------------------------
--   Wally's World of Wheels Inc.
-- -------------------------------------------------------------------------
-- create a order
INSERT INTO orders(fk_customerID, lastUpdateDate, orderStatus)
VALUES
((SELECT customerID FROM customer WHERE firstName = 'Wallys World of Wheels Inc.'),'2017-09-22', 'PAID');

-- insert into vehicle
INSERT INTO Vehicle
VALUES
('53347223WTF', 2011, 'Buick', 'Regal', 'Mint', 134538, 7950, 'YES');

-- create an orderline
INSERT INTO orderline(fk_orderID, fk_dealershipID, fk_VIN, sPrice, tradeIn)
VALUES
 ((SELECT orderID FROM orders WHERE lastUpdateDate = '2017-09-22'),
 (SELECT dealershipID FROM dealership WHERE location = 'Waterloo'),
 (SELECT VIN FROM vehicle WHERE model = 'Regal'), -7950, 0);


-- -------------------------------------------------------------------------
--   Russell Foubert
-- -------------------------------------------------------------------------
-- create a order
INSERT INTO orders(fk_customerID, lastUpdateDate, orderStatus)
VALUES
((SELECT customerID FROM customer WHERE firstName ='Russell'), '2017-10-06', 'HOLD');


-- create an orderline
INSERT INTO orderline(fk_orderID, fk_dealershipID, fk_VIN, sPrice, tradeIn)
VALUES
  ((SELECT orderID FROM orders WHERE lastUpdateDate = '2017-10-06'),
 (SELECT dealershipID FROM dealership WHERE location = 'Guelph Auto Mall'),
 (SELECT VIN FROM vehicle WHERE model = 'Civic'), 9100, 0);
 
 -- update vehicle inStock
UPDATE vehicle SET inStock = 'HOLD' WHERE VIN = '58847722BRB';


-- --------------------------------------------------
-- Russell Cancel order
-- --------------------------------------------------

UPDATE vehicle SET inStock = 'YES' WHERE VIN = '58847722BRB';
UPDATE orders SET lastUpdateDate = '2017-10-20', orderStatus = 'CNCL' WHERE fk_customerID = (SELECT customerID FROM customer WHERE firstName = 'Russell');
UPDATE orderline SET sPrice = 0 WHERE fk_orderID = (SELECT orderID FROM orders WHERE lastUpdateDate = '2017-10-20');


-- --------------------------------------------------
-- Nobert Mika
-- --------------------------------------------------
-- insert into orders table
INSERT INTO orders(fk_customerID, lastUpdateDate, orderStatus)
VALUES
((SELECT customerID FROM customer WHERE firstName = 'Norbert'), '2017-11-02', 'PAID');


-- insert into orderline table
INSERT INTO orderline(fk_orderID, fk_dealershipID, fk_VIN, sPrice, tradeIn)
VALUES
   ((SELECT orderID FROM orders WHERE lastUpdateDate = '2017-11-02'),
 (SELECT dealershipID FROM dealership WHERE location = 'Waterloo'),
 (SELECT VIN FROM vehicle WHERE model = 'Jetta'), 18830, 2500);

UPDATE vehicle SET inStock = 'NO' WHERE VIN = '99277544LOL';

-- the trade in car
INSERT INTO Vehicle
VALUES
('99146514OMG', 2008,'Volkswagen', 'Jetta', 'White', 199012, 2500,'YES');













      






 



    
    
    
    