using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library {
    public static class Message {
        public const byte Authorization = 1;
        public const byte Сonnection = 2;
        public const byte SurveyList = 3;
        public const byte EmployeeAnswers = 4;
        public const byte DataSaveSuccess = 5;
        public const byte DataSaveUnsuccess = 6;
        public const byte EmployeeList = 7;
        public const byte AddNewEmployee = 8;
        public const byte EditEmployee = 9;
        public const byte RemoveEmployee = 10;
        public const byte AddNewSurvey = 11;
        public const byte EditSurvey = 12;
        public const byte RemoveSurvey = 13;
    }
}
