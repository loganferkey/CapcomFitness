using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace CapcomFitness.Utility
{
    public class ToastNotification
    {
        public string Message { get; set; }

        public ToastNotification(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }

        public static void AddFeedbackToTempData(Feedback feedback, ITempDataDictionary tempData)
        {
            //create variables to store error and success messages
            string errorMessage = string.Empty;
            string successMessage = string.Empty;

            //if there are success messages, add them to the local variable
            if (feedback.SuccessMessages != null)
            {
                foreach (string s in feedback.SuccessMessages)
                {
                    successMessage += s + "\n";
                }
            }

            //if there are error messages, add them to the local variable
            if (feedback.ErrorMessages != null) 
            { 
                foreach (string s in feedback.ErrorMessages)
                {
                    errorMessage += s + "\n";
                }
            }
            
            // store error and success messages as tempData
            if (!String.IsNullOrEmpty(successMessage))
            {
                tempData["SuccessMessage"] = successMessage;
            }

            if (!String.IsNullOrEmpty(errorMessage))
            {
                tempData["ErrorMessage"] = errorMessage;
            }
        }

    }
}
