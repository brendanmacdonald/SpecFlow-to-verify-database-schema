Feature: Migration tests
	In order to test the Dynamics database schema
	As a user with access to the Dynamics database
	I want to confirm the schema as defined in document TBC.

Background:
	Given there is a connection to the Dynamics database

@schema
# NOTE: NULL values are represented here as blanks because the sql query
# returns {} for null values. {} is converted to an empty string using .ToString().
Scenario: Verify the schema of the Customer table
	Given the Customer table exists
	Then verify the schema of the Customer table
		| TABLE_NAME | COLUMN_NAME | DATA_TYPE | IS_NULLABLE | CHARACTER_MAXIMUM_LENGTH |
		| Customer   | CustomerID  | int       | NO          |                          |
		| Customer   | firstname   | varchar   | YES         | 255                      |
		| Customer   | lastname    | varchar   | YES         | 255                      |
		| Customer   | birthdate   | varchar   | YES         | 255                      |
		| Customer   | city        | varchar   | YES         | 255                      |
	And verify the Customer record count is 100
	#And verify record types

@schema
Scenario: Verify the schema of the Address table
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