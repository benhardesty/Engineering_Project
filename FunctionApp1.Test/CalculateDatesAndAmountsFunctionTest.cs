using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FunctionApp1;

namespace FunctionApp1.Test
{
    [TestClass]
    public class CalculateDatesAndAmountsFunctionTest
    {
        [TestMethod]
        public void Test_CalculateApproximateLoanDate1()
        {
            // Arange
            DateTime startDate = DateTime.Parse("02/24/2020");
            DateTime expected = DateTime.Parse("03/09/2020");

            // Act
            DateTime actual = new CalculateDatesAndAmountsFunction().calculateApproximateLoanDate(startDate, 10);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_CalculateApproximateLoanDate2()
        {

            // Arange
            DateTime startDate = DateTime.Parse("02/28/2020");
            DateTime expected = DateTime.Parse("03/09/2020");

            // Act
            DateTime actual = new CalculateDatesAndAmountsFunction().calculateApproximateLoanDate(startDate, 6);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_CalculateApproximateLoanDate3()
        {

            // Arange
            DateTime startDate = DateTime.Parse("02/28/2020");
            DateTime expected = DateTime.Parse("03/10/2020");

            // Act
            DateTime actual = new CalculateDatesAndAmountsFunction().calculateApproximateLoanDate(startDate, 7);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_CalculateLoanLikelihood1()
        {

            // Arange
            double expected = 50;

            // Act
            double actual = new CalculateDatesAndAmountsFunction().calculateLoanLikelihood(10000, 5000);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_CalculateLoanLikelihood2()
        {

            // Arange
            double expected = 0;

            // Act
            double actual = new CalculateDatesAndAmountsFunction().calculateLoanLikelihood(10000, 12000);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_CalculateLoanLikelihood3()
        {

            // Arange
            double expected = 80;

            // Act
            double actual = new CalculateDatesAndAmountsFunction().calculateLoanLikelihood(10000, 2000);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
