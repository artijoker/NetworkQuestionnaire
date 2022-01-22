using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public static class Message {
        public const byte Authorization = 1;
        public const byte AuthorizationSuccessful = 2;
        public const byte AuthorizationFailed = 3;
        public const byte ListSurveysNotСompletedEmployee = 4;
        public const byte EmployeeAnswers = 5;
        public const byte DataSaveSuccess = 6;
        public const byte DataSaveUnsuccess = 7;
        public const byte EmployeesList = 8;
        public const byte AddNewEmployee = 9;
        public const byte EditEmployee = 10;
        public const byte RemoveEmployee = 11;
        public const byte AddNewSurvey = 12;
        public const byte EditSurvey = 13;
        public const byte RemoveSurvey = 14;
        public const byte AllSurveyFromDBAndQuestionTypes = 15;
        public const byte SurveysСompletedEmployee = 16;
    }
}
