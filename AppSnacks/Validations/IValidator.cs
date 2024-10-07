﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Validations
{
    public interface IValidator
    {
        string NameError { get; set; }
        string EmailError { get; set; }
        string PhoneNumberError { get; set; }
        string PasswordError { get; set; }
        Task<bool> Validate(string name, string email, string password, string  phoneNumber);
    }
}