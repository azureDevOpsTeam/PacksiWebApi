namespace ApplicationLayer.Extensions
{
    public static class PhoneNumberHelper
    {
        /// <summary>
        /// ترکیب کد کشور و شماره موبایل به شکل استاندارد. مانند: +989123456789
        /// </summary>
        public static string NormalizePhoneNumber(string phonePrefix, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phonePrefix))
                throw new ArgumentException("Country code is required.");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.");

            // حذف فاصله‌ها یا کاراکترهای اضافی
            phonePrefix = phonePrefix.Trim().Replace(" ", "");
            phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

            // اطمینان از اینکه کد کشور با "+" شروع می‌شود
            if (!phonePrefix.StartsWith("+"))
                phonePrefix = "+" + phonePrefix;

            // حذف صفر ابتدایی از شماره موبایل (مثلاً 0912 → 912)
            if (phoneNumber.StartsWith("0"))
                phoneNumber = phoneNumber.Substring(1);

            return phonePrefix + phoneNumber;
        }

        /// <summary>
        /// حذف صفر ابتدایی از شماره موبایل (اگر وجود داشته باشد)
        /// </summary>
        public static string RemoveLeadingZero(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.");

            phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

            if (phoneNumber.StartsWith("0"))
                return phoneNumber.Substring(1);

            return phoneNumber;
        }
    }
}