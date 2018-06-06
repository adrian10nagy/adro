﻿
namespace Administratoro.BL.Managers
{
    using Administrataro.BL.Models;
    using Administratoro.BL.Constants;
    using Administratoro.DAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AssociationExpensesManager
    {
        private static AdministratoroEntities _administratoroEntities;

        private static AdministratoroEntities GetContext(bool shouldRefresh = false)
        {
            if (_administratoroEntities == null || shouldRefresh)
            {
                _administratoroEntities = new AdministratoroEntities();
            }

            return _administratoroEntities;
        }

        public static IEnumerable<AssociationExpenses> GetByMonthAndYearNotDisabled(int associationId, int year, int month)
        {
            return GetContext(true).AssociationExpenses.Where(
                ee => ee.Id_Estate == associationId &&
                ee.Year == year && ee.Month == month &&
                !ee.WasDisabled && !ee.Expenses.specialType.HasValue);
        }

        public static IEnumerable<AssociationExpenses> GetForAddPage(int associationId, int year, int month)
        {
            return GetContext(true).AssociationExpenses.Where(
                ee => ee.Id_Estate == associationId && ee.Year == year && ee.Month == month &&
                !ee.WasDisabled && !ee.Expenses.specialType.HasValue && ee.Expenses.Id != 25);
        }

        public static IEnumerable<AssociationExpenses> GetByMonthAndYearwithDiverse(int associationId, int year, int month)
        {
            return GetContext(true).AssociationExpenses.Where(
                ee => ee.Id_Estate == associationId &&
                ee.Year == year && ee.Month == month &&
                !ee.WasDisabled);
        }

        public static IEnumerable<AssociationExpenses> GetFromLastestOpenedMonth(int associationId, bool shouldRefresh = false)
        {
            if (GetContext(shouldRefresh).AssociationExpenses.Count() > 0)
            {
                int? maxYear = GetContext(shouldRefresh).AssociationExpenses.Max(i => i.Year);
                if (maxYear.HasValue)
                {
                    List<AssociationExpenses> eeFromMaxYear = GetContext(shouldRefresh).AssociationExpenses.Where(c => c.Year == maxYear && c.Id_Estate == associationId).ToList();

                    if (eeFromMaxYear != null && eeFromMaxYear.Count > 0)
                    {
                        int maxMonth = eeFromMaxYear.Max(i => i.Month);

                        return GetContext(shouldRefresh).AssociationExpenses.Where(ee => ee.Month == maxMonth && ee.Year == maxYear &&
                            ee.Id_Estate == associationId && !ee.WasDisabled && !ee.Expenses.specialType.HasValue).ToList();
                    }
                    else
                    {
                        return GetDefault(associationId);
                    }
                }
            }

            return GetDefault(associationId);

        }

        public static IEnumerable<AssociationExpenses> GetDefault(int associationId)
        {
            return GetContext().AssociationExpenses.Where(ee => ee.Id_Estate == associationId && ee.isDefault && !ee.WasDisabled && !ee.Expenses.specialType.HasValue);
        }

        public static IEnumerable<AssociationExpenses> GetAllAssociationExpensesByMonthAndYearIncludingDisabled(int associationId, int year, int month)
        {
            return GetContext().AssociationExpenses.Where(
                ee => ee.Id_Estate == associationId &&
                ee.Year == year && ee.Month == month && !ee.Expenses.specialType.HasValue);
        }

        public static AssociationExpenses GetAssociationExpensesByMonthAndYearAndDisabled(int associationId, int expenseId, int year, int month, bool wasDisabled = true)
        {
            return GetContext().AssociationExpenses.FirstOrDefault(
                ee => ee.Id_Estate == associationId &&
                ee.Year == year && ee.Month == month &&
                ee.Id_Expense == expenseId &&
                ee.WasDisabled == wasDisabled);
        }

        public static AssociationExpenses GetAssociationExpense(int associationId, int expenseId, int year, int month)
        {
            return GetContext(true).AssociationExpenses.FirstOrDefault(
                ee => ee.Id_Estate == associationId &&
                ee.Year == year && ee.Month == month &&
                ee.Id_Expense == expenseId && !ee.Expenses.specialType.HasValue);
        }

        public static AssociationExpenses GetById(int idExpenseEstate)
        {
            return GetContext(true).AssociationExpenses.FirstOrDefault(
                ee => ee.Id == idExpenseEstate);
        }

        public static AssociationExpenses GetByIdNotSpecialtype(int idExpenseEstate)
        {
            return GetContext().AssociationExpenses.FirstOrDefault(ee => ee.Id == idExpenseEstate && !ee.Expenses.specialType.HasValue);
        }


        public static AssociationExpenses Add(int associationId, int expenseId, int month, int year, string expenseType, bool? isStairCaseSplit)
        {
            AssociationExpenses ee = null;

            ExpenseType expenseTypeEnum;
            if (Enum.TryParse<ExpenseType>(expenseType, out  expenseTypeEnum))
            {
                ee = new AssociationExpenses
                {
                    Id_Expense = expenseId,
                    Id_ExpenseType = (int)expenseTypeEnum,
                    Id_Estate = associationId,
                    Month = month,
                    Year = year,
                    isDefault = false,
                    WasDisabled = false,
                    SplitPerStairCase = isStairCaseSplit
                };

                GetContext().AssociationExpenses.Add(ee);
                GetContext().SaveChanges();
            }

            return ee;
        }

        public static void AddAssociationExpensesByApartmentAndMonth(int associationId, Dictionary<int, int> expenses)
        {

            foreach (var expense in expenses)
            {
                AssociationExpenses ee = null;
                ee = new AssociationExpenses
                {
                    Id_Expense = expense.Key,
                    Id_ExpenseType = expense.Value,
                    Id_Estate = associationId,
                    Year = -1,
                    Month = -1,
                    isDefault = true,
                    WasDisabled = false
                };

                GetContext().AssociationExpenses.Add(ee);
            }

            GetContext().SaveChanges();
        }

        public static void MarkAssociationExpensesDisableProperty(AssociationExpenses ee, bool isDisabled, bool? isStairCaseSplit)
        {
            if (ee != null)
            {
                ee.WasDisabled = isDisabled;
                ee.SplitPerStairCase = isStairCaseSplit;
                GetContext().SaveChanges();
            }
        }

        public static void UpdateAssociationExpenseType(AssociationExpenses ee, ExpenseType selectedExpenseType)
        {
            var et = GetContext().ExpenseTypes.FirstOrDefault(ext => ext.Id == (int)selectedExpenseType);
            if (ee != null && et != null)
            {
                ee.ExpenseTypes = et;
                GetContext().SaveChanges();
            }
        }

        public static void UpdatePricePerUnit(int idAssociationExpense, decimal? newPricePerUnit)
        {
            var associationExpense = AssociationExpensesManager.GetByIdNotSpecialtype(idAssociationExpense);

            if (associationExpense != null)
            {
                associationExpense.PricePerExpenseUnit = newPricePerUnit;
                GetContext().SaveChanges();

                ApartmentExpensesManager.UpdateValueForPriceUpdate(associationExpense.Id, newPricePerUnit);
            }
        }

        public static void UpdatePricePerUnit(int idAssociationExpense, List<InvoiceSubcategories> invoiceSubcategories)
        {
            var associationExpense = AssociationExpensesManager.GetByIdNotSpecialtype(idAssociationExpense);

            if (associationExpense != null)
            {
                decimal? newPricePerUnit = null;

                if (associationExpense.Id_Expense == (int)Expense.ApaRece)
                {
                    newPricePerUnit = GetPriceForColdWather(invoiceSubcategories);
                }
                else if (associationExpense.Id_Expense == (int)Expense.ApaCalda)
                {
                    newPricePerUnit = GetPriceForHotWather(associationExpense, invoiceSubcategories);
                }

                UpdatePricePerUnit(idAssociationExpense, newPricePerUnit);
            }
        }

        private static decimal? GetPriceForHotWather(AssociationExpenses associationExpense, List<InvoiceSubcategories> invoiceSubcategories)
        {
            decimal? result = null;
            Invoices invoice = InvoicesManager.GetByAssociationExpenseForExpense(associationExpense, Expense.ApaRece);

            if (invoice != null)
            {
                var invoiceSubcategoryForApaCT = InvoicesSubcategoriesManager.GetByInvoiceId(invoice.Id, (int)InvoiceSubcategoryType.ApaCT);
                if (invoiceSubcategoryForApaCT != null && invoiceSubcategoryForApaCT.quantity.HasValue)
                {
                    var coldWAtherExpense = AssociationExpensesManager.GetAssociationExpense(associationExpense.Id_Estate, (int)Expense.ApaRece, associationExpense.Year, associationExpense.Month);
                    if (coldWAtherExpense != null && coldWAtherExpense.PricePerExpenseUnit.HasValue && invoiceSubcategoryForApaCT.Value.HasValue)
                    {
                        result = ((coldWAtherExpense.PricePerExpenseUnit.Value * invoiceSubcategoryForApaCT.quantity.Value) + invoiceSubcategoryForApaCT.Value.Value) / invoiceSubcategoryForApaCT.quantity.Value;

                    }
                }
            }

            return result;
        }

        private static decimal? GetPriceForColdWather(List<InvoiceSubcategories> invoiceSubcategories)
        {
            decimal? result = null;
            int[] subcategoryIds = { (int)InvoiceSubcategoryType.ApaRetea, (int)InvoiceSubcategoryType.ApaCT, (int)InvoiceSubcategoryType.Canal };

            var subcategories = invoiceSubcategories.Where(i => subcategoryIds.Any(s => s == i.Id_subCategType));
            var counsumptionWather = invoiceSubcategories.FirstOrDefault(i => i.Id_subCategType == (int)InvoiceSubcategoryType.Canal);

            if (counsumptionWather != null && counsumptionWather.quantity.HasValue && subcategories.All(s => s.quantity.HasValue && s.PricePerUnit.HasValue))
            {
                foreach (var subcategory in subcategories)
                {
                    if (subcategory.quantity.HasValue && subcategory.PricePerUnit.HasValue)
                    {
                        var value = (subcategory.quantity.Value * subcategory.PricePerUnit.Value);
                        result = result.HasValue ? result + value : value;
                    }
                }
                result = result / counsumptionWather.quantity.Value;
            }

            return result;
        }

        public static List<YearMonth> GetAllMonthsAndYearsAvailableByAssociationId(int assotiationId)
        {
            return GetContext().AssociationExpenses.Where(ee => ee.Id_Estate == assotiationId &&
                ee.Year != -1 && ee.Month != -1 && !ee.Expenses.specialType.HasValue)
                .Select(s => new YearMonth { Year = s.Year, Month = s.Month }).Distinct().OrderBy(ee => ee.Year).ToList();
        }

        public static List<YearMonth> GetAllMonthsAndYearsNotClosedByAssociationId(int assotiationId)
        {
            return GetContext().AssociationExpenses.Where(ee => ee.Id_Estate == assotiationId &&
                ee.Year != -1 && ee.Month != -1 && !ee.Expenses.specialType.HasValue && (!ee.IsClosed.HasValue || !ee.isDefault))
                .Select(s => new YearMonth { Year = s.Year, Month = s.Month }).Distinct().OrderBy(ee => ee.Year).ToList();
        }

        public static void UpdatePricePerUnitDefaultPrevieousMonth(AssociationExpenses newEE, IEnumerable<AssociationExpenses> oldEEs)
        {
            if (newEE != null)
            {
                AssociationExpenses oldEE = oldEEs.FirstOrDefault(ee => ee.Id_Expense == newEE.Id_Expense && !ee.Expenses.specialType.HasValue);
                if (oldEE != null && oldEE.Id_ExpenseType == (int)ExpenseType.PerIndex)
                {
                    UpdatePricePerUnit(newEE.Id, oldEE.PricePerExpenseUnit);
                    UpdateRedistributeMethod(newEE.Id, oldEE.RedistributeType);
                }
            }
        }

        public static AssociationExpenses GetPreviousMonth(AssociationExpenses ae)
        {
            if (ae == null)
            {
                return null;
            }

            //todo -1 does not work for month 1(january)
            var lastMonthEE = GetContext(true).AssociationExpenses.FirstOrDefault(e => e.Month == ae.Month - 1 &&
                e.Id_Estate == ae.Id_Estate && e.Id_Expense == ae.Id_Expense && e.Id_ExpenseType == ae.Id_ExpenseType &&
                e.isDefault == ae.isDefault && e.Year == ae.Year && e.WasDisabled == ae.WasDisabled);

            return lastMonthEE;
        }

        public static void UpdateRedistributeMethod(int associationExpenseId, int? type)
        {
            AssociationExpenses result = new AssociationExpenses();
            result = GetContext(true).AssociationExpenses.FirstOrDefault(ee => ee.Id == associationExpenseId && !ee.Expenses.specialType.HasValue);
            if (result != null)
            {
                result.RedistributeType = type;
                GetContext().Entry(result).CurrentValues.SetValues(result);
                GetContext().SaveChanges();
            }
        }

        public static AssociationExpenses GetMonthYearAssoiationExpense(int associationId, int expenseId, int year, int month)
        {
            return GetContext(true).AssociationExpenses.FirstOrDefault(ee => ee.Id_Estate == associationId && ee.Id_Expense == expenseId
                && ee.Year == year && ee.Month == month);
        }

        #region statusOfinvoiceFor Split -NoSplit

        private static string StatusOfInvoicesForSplit(AssociationExpenses associationExpense, string result, string redistributeValue, string percentage)
        {
            if (associationExpense.ExpenseTypes.Id == (int)ExpenseType.PerIndex)
            {
                if (StatusAllInvoicesHaveValue(associationExpense, 1) && StatusHasAddedAllExpenses(percentage) &&
                    StatusHasRedistributedTheValue(associationExpense, redistributeValue))
                {
                    result = "<i class='fa fa-check'></i> 100%";
                }
                else if (!StatusHasAddedAllExpenses(percentage) && !StatusAllInvoicesHaveValue(associationExpense, 1))
                {
                    result = "Adaugă facturile, cheltuielile individuale! 0%";
                }
                else if (!StatusHasAddedAllExpenses(percentage))
                {
                    result = "Cheltuieli neadăugate! 20%";
                }
                else if (!StatusAllInvoicesHaveValue(associationExpense, 1))
                {
                    result = "Facturi neadăugate! 50%";
                }
                else if (!StatusHasRedistributedTheValue(associationExpense, redistributeValue))
                {
                    result = "Redistribuie cheltuiala! 80%";
                }
            }
            else
            {
                if (StatusAllInvoicesHaveValue(associationExpense, 1))
                {
                    result = "<i class='fa fa-check'></i> 100%";
                }
                else
                {
                    result = "Facturi neadăugate! 0%";
                }
            }

            return result;
        }

        private static bool StatusAllInvoicesHaveValue(AssociationExpenses associationExpense, int numberOfInvoices)
        {
            return associationExpense.Invoices.All(i => i.Value.HasValue) && associationExpense.Invoices.Count == numberOfInvoices;
        }

        private static bool StatusHasRedistributedTheValue(AssociationExpenses associationExpense, string redistributeValue)
        {
            bool result = false;

            if (associationExpense.RedistributeType.HasValue)
            {
                result = true;
            }
            else if (string.IsNullOrEmpty(redistributeValue) || redistributeValue == "0,0000000")
            {
                result = true;
            }

            return result;
        }

        private static bool StatusHasAddedAllExpenses(string percentage)
        {
            return (string.IsNullOrEmpty(percentage) || percentage == "100");
        }

        private static string StatusOfInvoicesForNoSplit(string result, string redistributeValue, string percentage, AssociationExpenses associationExpense)
        {
            if (associationExpense.ExpenseTypes.Id == (int)ExpenseType.PerIndex)
            {
                if (associationExpense != null && associationExpense.Invoices.All(i => i.Value.HasValue) && associationExpense.Invoices.Count > 0 && (string.IsNullOrEmpty(percentage) || percentage == "100") &&
                    (associationExpense.RedistributeType.HasValue || string.IsNullOrEmpty(redistributeValue) || redistributeValue == "0,00"))
                {
                    result = "<i class='fa fa-check'></i> 100%";
                }
                else if (!associationExpense.RedistributeType.HasValue && associationExpense.Invoices.Any(i => !i.Value.HasValue) && percentage == "0")
                {
                    result = "Adaugă factura, cheltuielile! 0%";
                }
                else if ((string.IsNullOrEmpty(percentage) || percentage != "100"))
                {
                    result = "Cheltuieli neadăugate! 20%";
                }
                else if (associationExpense.Invoices.Any(i => !i.Value.HasValue) || associationExpense.Invoices.Count == 0)
                {
                    result = "Facturi neadăugate! 50%";
                }
                else if (!associationExpense.RedistributeType.HasValue)
                {
                    result = "Redistribuie cheltuiala! 80%";
                }
            }
            else if (associationExpense.ExpenseTypes.Id == (int)ExpenseType.Individual)
            {
                if (associationExpense.ApartmentExpenses.Any(te => !te.Value.HasValue) || associationExpense.Associations.Apartments.Where(t => t.HasHeatHelp.HasValue).Count() != associationExpense.ApartmentExpenses.Count())
                {
                    result = "Cheltuieli neadăugate! 0%";
                }
                else
                {
                    result = "<i class='fa fa-check'></i> 100%";
                }
            }
            else
            {
                if (associationExpense != null && associationExpense.Invoices.All(i => i.Value.HasValue) && associationExpense.Invoices.Count > 0)
                {
                    result = "<i class='fa fa-check'></i> 100%";
                }
                else if (associationExpense.Invoices.Any(i => !i.Value.HasValue) || associationExpense.Invoices.Count == 0)
                {
                    result = "Facturi neadăugate! 50%";
                }
                else if (!associationExpense.RedistributeType.HasValue)
                {
                    result = "Redistribuie cheltuiala! 80%";
                }
            }

            return result;
        }

        public static ExpensesCompletedStatus StatusOfInvoicesForNoSplit2(string result, string redistributeValue, string percentage, AssociationExpenses associationExpense)
        {
            return ExpensesCompletedStatus.All;
        }

        public static string StatusOfInvoices(AssociationExpenses associationExpense, bool isExpensePerIndex)
        {
            string result = string.Empty;
            var redistributeValue = RedistributionManager.CalculateRedistributeValueAsString(associationExpense.Id);
            var percentage = string.Empty;

            if (isExpensePerIndex)
            {
                percentage = GetPercentageAsString(associationExpense);
            }

            //if (estateExpense.SplitPerStairCase.HasValue && estateExpense.SplitPerStairCase.Value)
            //{
            result = StatusOfInvoicesForSplit(associationExpense, result, redistributeValue, percentage);
            //}
            //else
            //{
            //    result = StatusOfInvoicesForNoSplit(result, redistributeValue, percentage, estateExpense);
            //}

            return result;
        }

        public static string GetPercentageAsString(AssociationExpenses associationExpense)
        {
            string percentage = string.Empty;

            int allApartmentExpenses = associationExpense.ApartmentExpenses.Count();

            if (allApartmentExpenses > 0)
            {
                percentage = ExpensePercentageFilledInAsString(associationExpense, allApartmentExpenses);
            }
            else
            {
                percentage = "100";
            }

            return percentage;
        }

        public static decimal GetPercentage(AssociationExpenses associationExpense)
        {
            decimal percentage = 0.0m;

            int allApartmentExpenses = associationExpense.ApartmentExpenses.Count();

            if (allApartmentExpenses > 0)
            {
                percentage = ExpensePercentageFilledIn(associationExpense, allApartmentExpenses);
            }
            else
            {
                percentage = 100m;
            }

            return percentage;
        }

        private static string ExpensePercentageFilledInAsString(AssociationExpenses associationExpense, int apartmentsWithCounters)
        {
            var addedExpenses = associationExpense.ApartmentExpenses.Count(te => te.IndexNew.HasValue);
            var percentage = (((decimal)addedExpenses / (decimal)apartmentsWithCounters) * 100).ToString("0.##");

            return percentage;
        }

        private static decimal ExpensePercentageFilledIn(AssociationExpenses associationExpense, int apartmentsWithCounters)
        {
            var addedExpenses = associationExpense.ApartmentExpenses.Count(te => te.IndexNew.HasValue);
            var percentage = (((decimal)addedExpenses / (decimal)apartmentsWithCounters) * 100);

            return percentage;
        }

        public static string ExpensePercentageFilledInMessage(AssociationExpenses associationExpense)
        {
            var addedExpenses = associationExpense.ApartmentExpenses.Count(te => te.IndexNew.HasValue);

            int apartmentsWithCountersOfThatExpense = associationExpense.ApartmentExpenses.Count();

            return "<b>" + addedExpenses + "</b> cheltuieli adăugate din <b>" + apartmentsWithCountersOfThatExpense + "</b> ";
        }

        public static void ConfigurePerIndex(Associations association, int year, int month)
        {
            IEnumerable<Administratoro.DAL.Expenses> allExpenses = ExpensesManager.GetAllExpenses();
            var associationExpenses = AssociationExpensesManager.GetByMonthAndYearNotDisabled(association.Id, year, month);

            foreach (Expenses expense in allExpenses)
            {
                var associationExpense = associationExpenses.FirstOrDefault(ea => ea.Id_Expense == expense.Id);
                if (associationExpense != null)
                {
                    var apartments = ApartmentsManager.GetAllThatAreRegisteredWithSpecificCounters(association.Id, associationExpense.Id);
                    ApartmentExpensesManager.ConfigurePerIndex(associationExpense, apartments);
                }
            }
        }

        #endregion

        public static void OpenCloseMonth(int association, int year, int month, bool shouldClose = true)
        {
            List<AssociationExpenses> allEE = GetContext(true).AssociationExpenses.Where(ee => ee.Id_Estate == association &&
                ee.Year == year && ee.Month == month).ToList();
            foreach (var associationExpense in allEE)
            {
                associationExpense.IsClosed = shouldClose;
                GetContext().Entry(associationExpense).CurrentValues.SetValues(associationExpense);
                GetContext().SaveChanges();
            }
        }


        public static List<Expense> CheckCloseMonth(int association, int year, int month)
        {
            List<Expense> result = new List<Expense>();

            IQueryable<AssociationExpenses> allEE = GetContext(true).AssociationExpenses.Where(ee => ee.Id_Estate == association &&
                ee.Year == year && ee.Month == month);
            foreach (var associationExpense in allEE)
            {
                if (associationExpense.Invoices.Count() == 0 || associationExpense.Invoices.Any(i => !i.Value.HasValue))
                {
                    result.Add((Expense)associationExpense.Id_Expense);
                    continue;
                }

                if (associationExpense.SplitPerStairCase.HasValue && associationExpense.SplitPerStairCase.Value)
                {
                    if (associationExpense.Id_ExpenseType == (int)ExpenseType.PerNrTenants || associationExpense.Id_ExpenseType == (int)ExpenseType.PerCotaIndiviza)
                    {
                        if (associationExpense.Invoices.Count != associationExpense.Associations.StairCases.Count || associationExpense.Invoices.Any(i => !i.Value.HasValue))
                        {
                            result.Add((Expense)associationExpense.Id_Expense);
                        }
                    }
                    else if (associationExpense.Id_ExpenseType == (int)ExpenseType.PerIndex)
                    {
                        var percentage = GetPercentage(associationExpense);
                        if (percentage != 100m)
                        {
                            result.Add((Expense)associationExpense.Id_Expense);
                        }
                    }
                }
                else
                {
                    if (associationExpense.Id_ExpenseType == (int)ExpenseType.PerNrTenants || associationExpense.Id_ExpenseType == (int)ExpenseType.PerCotaIndiviza)
                    {
                        if (associationExpense.Invoices.Any(i => !i.Value.HasValue) && associationExpense.Invoices.Count != 0)
                        {
                            result.Add((Expense)associationExpense.Id_Expense);
                        }
                    }
                    else if (associationExpense.Id_ExpenseType == (int)ExpenseType.PerIndex)
                    {
                        var percentage = GetPercentage(associationExpense);
                        if (percentage != 100m)
                        {
                            result.Add((Expense)associationExpense.Id_Expense);
                        }
                    }
                    else if (associationExpense.Id_ExpenseType == (int)ExpenseType.Individual)
                    {
                        if (associationExpense.ApartmentExpenses.Any(te => !te.Value.HasValue))
                        {
                            result.Add((Expense)associationExpense.Id_Expense);
                        }
                    }
                }
            }

            return result;
        }

        public static bool IsMonthClosed(int association, int year, int month)
        {
            List<AssociationExpenses> allEE = GetContext(true).AssociationExpenses.Where(ee => ee.Id_Estate == association &&
                ee.Year == year && ee.Month == month).ToList();
            if (allEE.Count() == 0)
            {
                return false;
            }

            return allEE.All(e => e.IsClosed.HasValue && e.IsClosed.Value);
        }

        public static IEnumerable<Apartments> GetApartmentsNrThatShouldRedistributeTo(int associationExpenseId)
        {
            IEnumerable<Apartments> result = new List<Apartments>();

            var associationExpense = AssociationExpensesManager.GetById(associationExpenseId);
            if (associationExpense != null)
            {
                result = associationExpense.ApartmentExpenses.Where(te => te.Apartments.AssociationCountersApartment.Any(ac => ac.AssociationCounters.Id_Expense == associationExpense.Id_Expense)).Select(te => te.Apartments).ToList();
            }

            return result;
        }

        public static bool HasCounterOfExpense(int apartmentId, int expenseId)
        {
            var counters = CountersManager.GetByApartment(apartmentId);
            return counters.Any(c => c.Id_Expense == expenseId);
        }

        public static AssociationExpenses GetForSameMonthByExpense(int id, Expense expense)
        {
            AssociationExpenses result = null;
            var ae = GetById(id);

            if (ae != null)
            {
                result = GetContext(true).AssociationExpenses.FirstOrDefault(x => x.Year == ae.Year && x.Month == ae.Month && x.Id_Estate == ae.Id_Estate && x.Id_Expense == (int)expense);
            }

            return result;
        }
    }
}
