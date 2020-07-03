using PLNKTNv2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PLNKTNv2.BusinessLogic.AttributeDecorators
{
    public sealed class RewardChallengeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ErrorMessage = string.Format("The Challenges attribute must be a collection and have 6 challenges in it.");

            if (value == null)
            {
                return false;
            }

            ICollection<RewardChallenge> _value = (ICollection<RewardChallenge>)value;
            return _value.Count() == 6 ? true : false;
        }
    }
}