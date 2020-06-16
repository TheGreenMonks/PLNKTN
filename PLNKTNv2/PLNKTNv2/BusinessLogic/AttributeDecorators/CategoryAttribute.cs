using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PLNKTNv2.BusinessLogic.AttributeDecorators
{
    public sealed class CategoryAttribute : ValidationAttribute
    {
        private readonly IList<string> _validationAttribute = new
            List<string> { "Transport", "Diet", "Electronics", "Clothing", "Footwear" };

        public override bool IsValid(object value)
        {
            StringBuilder _acceptedItems = new StringBuilder();
            _acceptedItems.AppendJoin(", ", _validationAttribute);
            _acceptedItems.Append(".");

            if (value == null)
            {
                ErrorMessage = string.Format("The Category attribute must have a value and must match one of the following exactly - {0}", _acceptedItems.ToString());
                return false;
            }

            string _value = value.ToString();

            foreach (var item in _validationAttribute)
            {
                if (_value == item)
                {
                    return true;
                }
            }

            ErrorMessage = string.Format("The Category attribute value must match one of the following exactly - {0}", _acceptedItems.ToString());
            return false;
        }
    }
}