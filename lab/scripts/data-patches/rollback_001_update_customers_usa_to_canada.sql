-- ============================================
-- Rollback: Revert Canadian customers back to original USA addresses
-- Author: Copilot
-- Date: 2026-03-23
-- Ticket: N/A
-- ============================================

-- Pre-check: verify current state
-- Expect 15 rows with Country = 'Canada'
SELECT COUNT(*) AS CanadaCustomerCount
FROM Customers
WHERE Country = 'Canada';

BEGIN TRANSACTION;
BEGIN TRY

    UPDATE Customers SET
        Address    = '10 Oak St',
        City       = 'New York',
        State      = 'NY',
        PostalCode = '10001',
        Country    = 'USA'
    WHERE CustomerId = 1; -- Emma Wilson

    UPDATE Customers SET
        Address    = '22 Elm Ave',
        City       = 'Los Angeles',
        State      = 'CA',
        PostalCode = '90001',
        Country    = 'USA'
    WHERE CustomerId = 2; -- Liam Garcia

    UPDATE Customers SET
        Address    = '45 Pine Rd',
        City       = 'Chicago',
        State      = 'IL',
        PostalCode = '60601',
        Country    = 'USA'
    WHERE CustomerId = 3; -- Olivia Martinez

    UPDATE Customers SET
        Address    = '78 Cedar Blvd',
        City       = 'Houston',
        State      = 'TX',
        PostalCode = '77001',
        Country    = 'USA'
    WHERE CustomerId = 4; -- Noah Davis

    UPDATE Customers SET
        Address    = '99 Birch Ln',
        City       = 'Phoenix',
        State      = 'AZ',
        PostalCode = '85001',
        Country    = 'USA'
    WHERE CustomerId = 5; -- Sophia Brown

    UPDATE Customers SET
        Address    = '200 Maple St',
        City       = 'Philadelphia',
        State      = 'PA',
        PostalCode = '19101',
        Country    = 'USA'
    WHERE CustomerId = 6; -- Jackson Taylor

    UPDATE Customers SET
        Address    = '315 Walnut Ave',
        City       = 'San Antonio',
        State      = 'TX',
        PostalCode = '78201',
        Country    = 'USA'
    WHERE CustomerId = 7; -- Ava Anderson

    UPDATE Customers SET
        Address    = '420 Hickory Dr',
        City       = 'San Diego',
        State      = 'CA',
        PostalCode = '92101',
        Country    = 'USA'
    WHERE CustomerId = 8; -- Lucas Thomas

    UPDATE Customers SET
        Address    = '550 Willow Ct',
        City       = 'Dallas',
        State      = 'TX',
        PostalCode = '75201',
        Country    = 'USA'
    WHERE CustomerId = 9; -- Mia White

    UPDATE Customers SET
        Address    = '660 Spruce Way',
        City       = 'San Jose',
        State      = 'CA',
        PostalCode = '95101',
        Country    = 'USA'
    WHERE CustomerId = 10; -- Ethan Harris

    UPDATE Customers SET
        Address    = '720 Aspen Pl',
        City       = 'Austin',
        State      = 'TX',
        PostalCode = '78701',
        Country    = 'USA'
    WHERE CustomerId = 11; -- Amelia Jackson

    UPDATE Customers SET
        Address    = '830 Chestnut Rd',
        City       = 'Jacksonville',
        State      = 'FL',
        PostalCode = '32201',
        Country    = 'USA'
    WHERE CustomerId = 12; -- Benjamin Lee

    UPDATE Customers SET
        Address    = '940 Poplar St',
        City       = 'Fort Worth',
        State      = 'TX',
        PostalCode = '76101',
        Country    = 'USA'
    WHERE CustomerId = 13; -- Charlotte Robinson

    UPDATE Customers SET
        Address    = '1050 Sycamore Blvd',
        City       = 'Columbus',
        State      = 'OH',
        PostalCode = '43201',
        Country    = 'USA'
    WHERE CustomerId = 14; -- Henry Clark

    UPDATE Customers SET
        Address    = '1160 Dogwood Ave',
        City       = 'Charlotte',
        State      = 'NC',
        PostalCode = '28201',
        Country    = 'USA'
    WHERE CustomerId = 15; -- Abigail Lewis

    COMMIT TRANSACTION;
    PRINT 'Rollback completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH

-- Post-check: verify rollback result
-- Expect 15 USA customers, 0 Canada customers
SELECT Country, COUNT(*) AS CustomerCount
FROM Customers
GROUP BY Country;

