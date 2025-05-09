using System.ComponentModel.DataAnnotations;

namespace FruitsBasket.Api;

public class DateRangeAttribute : ValidationAttribute
{
    private readonly DateTime _minDate;
    private readonly DateTime _maxDate;

    public DateRangeAttribute(string minDate, string maxDate)
    {
        if (!DateTime.TryParse(minDate, out _minDate))
            throw new ArgumentException("Invalid date format", nameof(minDate));

        if (!DateTime.TryParse(maxDate, out _maxDate))
            throw new ArgumentException("Invalid date format", nameof(maxDate));

        if (_minDate > _maxDate)
            throw new ArgumentException("Minimum date must be lower than maximum date");

        ErrorMessage = "The field {0} must be between {1} and {2}.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not DateTime date)
            return false;

        return date >= _minDate && date <= _maxDate;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, _minDate.ToShortDateString(), _maxDate.ToShortDateString());
    }
}