using PLNKTNv2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PLNKTNv2.BusinessLogic.AttributeDecorators
{
    public sealed class NotificationStatusAttribute : ValidationAttribute
    {
        private readonly IList<NotificationStatus> _validationAttribute = new
            List<NotificationStatus> { NotificationStatus.Notified, NotificationStatus.Not_Notified,
                NotificationStatus.Not_Complete, NotificationStatus.Info_Showed };

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (int.TryParse(value.ToString(), out _))
            {
                SetErrorMessage();
                return false;
            }

            NotificationStatus _value = (NotificationStatus)value;

            foreach (var item in _validationAttribute)
            {
                if (_value == item)
                {
                    return true;
                }
            }

            SetErrorMessage();
            return false;
        }

        private void SetErrorMessage()
        {
            ErrorMessage = string.Format("The NotificationStatus attribute value must match one of the following NotificationStatus Enums - " +
                "{0}, {1}, {2}, {3}", _validationAttribute[0], _validationAttribute[1], _validationAttribute[2], _validationAttribute[3]);
        }
    }
}