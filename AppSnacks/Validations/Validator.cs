using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppLanches.Validations
{
    public class Validator : IValidator
    {
        public string NameError{ get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneNumberError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string NameEmptyErrorMsg = "Please provide your name.";
        private const string NameInvalidErrorMsg = "Please provide a valid name.";
        private const string EmailEmptyErrorMsg = "Please provide an email.";
        private const string EmailInvalidErrorMsg = "Please provide a valid email.";
        private const string PhoneEmptyErrorMsg = "Please provide a phone number.";
        private const string PhoneInvalidErrorMsg = "Please provide a valid phone number.";
        private const string PasswordEmptyErrorMsg = "Please provide a password.";
        private const string PasswordInvalidErrorMsg = "The password must be at least 8 characters long and contain both letters and numbers.";

        public Task<bool> Validate(string name, string email, string phone, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPhoneValid = ValidatePhone(phone);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = NameEmptyErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = NameInvalidErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmailEmptyErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = EmailInvalidErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                PhoneNumberError = PhoneEmptyErrorMsg;
                return false;
            }

            if (phone.Length < 12)
            {
                PhoneNumberError = PhoneInvalidErrorMsg;
                return false;
            }

            PhoneNumberError = "";
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = PasswordEmptyErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = PasswordInvalidErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }
}
