﻿
namespace Administratoro.BL.Managers
{
    using System.Collections.Generic;
    using DAL;
    using Toolbox.Email;

    public static class EmailsManager
    {
        internal static bool SendEmail(string addressFrom, string addressTo, string mailSubject, string mailBody, string filePath)
        {
            return EmailClient.Send(addressFrom, addressTo, mailSubject, mailBody, filePath);
        }

        public static void SendMonthlyEmails(IEnumerable<Apartments> apartments, string luna, int year)
        {
            foreach (var apartment in apartments)
            {
                if (!string.IsNullOrEmpty(apartment.Email))
                {
                    foreach (var email in apartment.Email.Split(';'))    
                    {
                        var filePath = string.Format("C:\\Users\\Adrian\\Documents\\fluturasi\\1077\\2018-09\\p{0}.pdf", apartment.Number);

                        string message = @"Bună ziua, <br> <br> 
Vă  facem  cunoscut  că  s-au  afișat  listele  de  cheltuileli  aferente  lunii  " + luna + "  " + year+ ".<br> " +
@"Nota  de  plată  aferentă  apartamentului  dumneavoastră  este  anexata prezentului  mail.<br> 
Vă  rugăm  să  efectuați  plata  în  contul  acociației  deschis  la  Banca  Transilvania  Cluj  cu  IBAN  RO13BTRLRONCRT0409298101  specificând  numărul  
apartamentului  pentru  care  faceți  plata.  Pentru  încasari  în  numerar  vă  așteptăm  joi  25.09.2018  între  orele  19.30 - 21  la  etajul  tehnic.<br> <br> 
Termenul  scadent  este  30.09.2018.<br> <br> 
<br><br>
Pentru orice informație/cerere vă rugăm adresați-vă administratorului la adresa <b>octombrie6@yahoo.com<b><br><br>
COMITET  EXECUTIV <br>21.09.2018";

                        SendEmail("asociatie.online@gmail.com", email,
                            "Fluturasi de cheltuieli pentru luna " + luna +" "+ + year, message, filePath);
                    }
                }

            }
        }
    }
}
