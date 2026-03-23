-- ============================================
-- Migration: Update all USA customers to Canada with valid Canadian addresses
-- Author: Copilot
-- Date: 2026-03-23
-- Ticket: N/A
-- ============================================

-- Pre-check: verify current state
-- Expect 15 rows with Country = 'USA'
SELECT COUNT(*) AS UsaCustomerCount
FROM Customers
WHERE Country = 'USA';

BEGIN TRANSACTION;
BEGIN TRY

    UPDATE Customers SET
        Address    = '10 King St W',
        City       = 'Toronto',
        State      = 'ON',
        PostalCode = 'M5H 2N2',
        Country    = 'Canada'
    WHERE CustomerId = 1; -- Emma Wilson (was New York, NY)

    UPDATE Customers SET
        Address    = '22 Robson St',
        City       = 'Vancouver',
        State      = 'BC',
        PostalCode = 'V6B 1A1',
        Country    = 'Canada'
    WHERE CustomerId = 2; -- Liam Garcia (was Los Angeles, CA)

    UPDATE Customers SET
        Address    = '45 Rue Sainte-Catherine O',
        City       = 'Montreal',
        State      = 'QC',
        PostalCode = 'H2X 1Y4',
        Country    = 'Canada'
    WHERE CustomerId = 3; -- Olivia Martinez (was Chicago, IL)

    UPDATE Customers SET
        Address    = '78 Stephen Ave SW',
        City       = 'Calgary',
        State      = 'AB',
        PostalCode = 'T2P 1J9',
        Country    = 'Canada'
    WHERE CustomerId = 4; -- Noah Davis (was Houston, TX)

    UPDATE Customers SET
        Address    = '99 Jasper Ave NW',
        City       = 'Edmonton',
        State      = 'AB',
        PostalCode = 'T5J 0R2',
        Country    = 'Canada'
    WHERE CustomerId = 5; -- Sophia Brown (was Phoenix, AZ)

    UPDATE Customers SET
        Address    = '200 Sparks St',
        City       = 'Ottawa',
        State      = 'ON',
        PostalCode = 'K1P 5G3',
        Country    = 'Canada'
    WHERE CustomerId = 6; -- Jackson Taylor (was Philadelphia, PA)

    UPDATE Customers SET
        Address    = '315 Portage Ave',
        City       = 'Winnipeg',
        State      = 'MB',
        PostalCode = 'R3C 0V8',
        Country    = 'Canada'
    WHERE CustomerId = 7; -- Ava Anderson (was San Antonio, TX)

    UPDATE Customers SET
        Address    = '420 Government St',
        City       = 'Victoria',
        State      = 'BC',
        PostalCode = 'V8W 1H1',
        Country    = 'Canada'
    WHERE CustomerId = 8; -- Lucas Thomas (was San Diego, CA)

    UPDATE Customers SET
        Address    = '550 Broadway Ave',
        City       = 'Saskatoon',
        State      = 'SK',
        PostalCode = 'S7K 0J5',
        Country    = 'Canada'
    WHERE CustomerId = 9; -- Mia White (was Dallas, TX)

    UPDATE Customers SET
        Address    = '660 Hurontario St',
        City       = 'Mississauga',
        State      = 'ON',
        PostalCode = 'L5B 1M2',
        Country    = 'Canada'
    WHERE CustomerId = 10; -- Ethan Harris (was San Jose, CA)

    UPDATE Customers SET
        Address    = '720 Barrington St',
        City       = 'Halifax',
        State      = 'NS',
        PostalCode = 'B3H 1A1',
        Country    = 'Canada'
    WHERE CustomerId = 11; -- Amelia Jackson (was Austin, TX)

    UPDATE Customers SET
        Address    = '830 James St N',
        City       = 'Hamilton',
        State      = 'ON',
        PostalCode = 'L8N 1A1',
        Country    = 'Canada'
    WHERE CustomerId = 12; -- Benjamin Lee (was Jacksonville, FL)

    UPDATE Customers SET
        Address    = '940 Victoria Ave',
        City       = 'Regina',
        State      = 'SK',
        PostalCode = 'S4P 0B1',
        Country    = 'Canada'
    WHERE CustomerId = 13; -- Charlotte Robinson (was Fort Worth, TX)

    UPDATE Customers SET
        Address    = '1050 Dundas St',
        City       = 'London',
        State      = 'ON',
        PostalCode = 'N6A 1B5',
        Country    = 'Canada'
    WHERE CustomerId = 14; -- Henry Clark (was Columbus, OH)

    UPDATE Customers SET
        Address    = '1160 King St W',
        City       = 'Kitchener',
        State      = 'ON',
        PostalCode = 'N2G 1C5',
        Country    = 'Canada'
    WHERE CustomerId = 15; -- Abigail Lewis (was Charlotte, NC)

    COMMIT TRANSACTION;
    PRINT 'Migration completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH

-- Post-check: verify migration result
-- Expect 0 USA customers, 15 Canada customers
SELECT Country, COUNT(*) AS CustomerCount
FROM Customers
GROUP BY Country;

