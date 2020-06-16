using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PLNKTNv2.BusinessLogic.AttributeDecorators
{
    public sealed class SubCategoryAttribute : ValidationAttribute
    {
        private readonly IList<string> _dietSubCategories = new
            List<string> { "Plant_based", "Beef", "Pork", "Poultry", "Egg", "Dairy", "Seafood" };

        private readonly IList<string> _transportSubCategories = new
            List<string> { "Bicycle", "Car", "Bus", "Flight", "Subway", "Walking" };

        public override bool IsValid(object value)
        {
            StringBuilder _acceptedItems = new StringBuilder();
            _acceptedItems.AppendJoin(", ", _dietSubCategories);
            _acceptedItems.Append(", ");
            _acceptedItems.AppendJoin(", ", _transportSubCategories);
            _acceptedItems.Append(".");

            if (value == null)
            {
                ErrorMessage = string.Format("The SubCategory attribute must have a value and must match one of the following exactly - {0}", _acceptedItems.ToString());
                return false;
            }

            string _value = value.ToString();

            if (CheckListItems(_dietSubCategories, _value))
            {
                return true;
            }
            else if (CheckListItems(_transportSubCategories, _value))
            {
                return true;
            }

            ErrorMessage = string.Format("The SubCategory attribute value must match one of the following exactly - {0}", _acceptedItems.ToString());
            return false;
        }

        private bool CheckListItems(IList<string> validationAttribute, string value)
        {
            foreach (var item in validationAttribute)
            {
                if (value == item)
                {
                    return true;
                }
            }

            return false;
        }
    }
}