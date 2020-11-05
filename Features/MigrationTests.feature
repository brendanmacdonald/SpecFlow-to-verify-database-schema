Feature: Migration tests
	In order to test the Dynamics database schema
	As a user with access to the Dynamics database
	I want to confirm the schema as defined in document TBC.

Background:
	Given there is a connection to the Dynamics database

@schema
# NOTE: NULL values are represented here as blanks because the sql query
# returns {} for null values. {} is converted to an empty string using .ToString().
Scenario: Verify the schema and values within the Customer table
	Given the Customer table exists
	Then verify the schema of the Customer table
		| TABLE_NAME | COLUMN_NAME | DATA_TYPE | IS_NULLABLE | CHARACTER_MAXIMUM_LENGTH |
		| Customer   | CustomerID  | int       | NO          |                          |
		| Customer   | firstname   | varchar   | YES         | 255                      |
		| Customer   | lastname    | varchar   | YES         | 255                      |
		| Customer   | birthdate   | varchar   | YES         | 255                      |
		| Customer   | city        | varchar   | YES         | 255                      |
	And verify the Customer record count is 100
	And verify the data in the first row of the Customer table is
		| CustomerID | firstname | lastname | birthdate | city |
		| 1          | Josiah    | Mcgowan  | 14-06-21  | Ruza |
	And verify the data in the last row of the Customer table is
		| CustomerID | firstname | lastname | birthdate | city       |
		| 100        | Lucius    | Dennis   | 30-10-20  | Mascalucia |
	And verify the data in row 2 of the Customer table is
		| CustomerID | firstname | lastname | birthdate | city     |
		| 2          | Brent     | Baker    | 01-07-20  | Paulista |
	And verify the data with CustomerID equal to 3 in the Customer table is
		| CustomerID | firstname | lastname | birthdate | city                  |
		| 3          | Ira       | Fowler   | 23-04-20  | Ashoknagar-Kalyangarh |
	And verify the count of records in the Customer table with a lastname of Young is 1
	And verify the count of records in the Customer table with a lastname of Cross is 2
	And verify the sum of all values in the CustomerId column of the Customer table is 5050
	And verify the DXAddress in the CustomerContact table always start with DX- 
	And verify the birthdate in the Customer table always has the format [0-9][0-9]-[0-9][0-9]-[0-9][0-9]


@schema
Scenario: Verify the schema and values within the Address table
	Given the Address table exists
	Then verify the schema of the Address table
		| TABLE_NAME | COLUMN_NAME  | DATA_TYPE | IS_NULLABLE | CHARACTER_MAXIMUM_LENGTH |
		| Address    | AddressID    | int       | NO          |                          |
		| Address    | AddressLine1 | varchar   | YES         | 255                      |
		| Address    | AddressLine2 | varchar   | YES         | 255                      |
		| Address    | City         | varchar   | YES         | 255                      |
		| Address    | Postcode     | varchar   | YES         | 10                       |
	And verify the Address record count is 80
#And verify record types