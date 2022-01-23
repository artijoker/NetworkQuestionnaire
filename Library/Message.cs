using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public static class Message {
        public const byte AdminConnect = 1;
        public const byte AdminConnectSuccessful = 2;
        public const byte AdminConnectFailed = 3;
        public const byte Authorization = 4;
        public const byte AuthorizationSuccessful = 5;
        public const byte AuthorizationFailed = 6;
        public const byte ListSurveysNotСompletedEmployee = 7;
        public const byte EmployeeAnswers = 8;
        public const byte DataSaveSuccess = 9;
        public const byte DataSaveUnsuccess = 10;
        public const byte EmployeesList = 11;
        public const byte AddNewEmployee = 12;
        public const byte EditEmployee = 13;
        public const byte RemoveEmployee = 14;
        public const byte AddNewSurvey = 15;
        public const byte EditSurvey = 16;
        public const byte RemoveSurvey = 17;
        public const byte AllSurveyFromDBAndQuestionTypes = 18;
        public const byte SurveysСompletedEmployee = 19;
    }
}
