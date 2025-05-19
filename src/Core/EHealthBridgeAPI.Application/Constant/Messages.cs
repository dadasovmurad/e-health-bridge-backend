using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.Constant
{
    public static class Messages
    {
        public static string UserNotFound = "İstifadəçi tapilmadi";
        public static string LoginSuccess = "Login oldunuz";
        public static string LoginFailure = "Username or password incorrect";

        public static string InvalidRequest = "Invalid request";


        public static string UserNotCreated = "İstifadəçi qeydiyyata alınmadı";
        public static string Usercreated = "İstifadəçi qeydə alındı";

        public static string UserSuccessfullyUpdated = "İstifadəçi dəyişdirildi";
        public static string UserNotUpdated = "İstifadəçi dəyişdirilmədi";

        public static string UserDeleted = "İstifadəçi silindi";
        public static string UserNotDeleted = "İstifadəçi silinmədi!";
        public static string UserAlreadyExists = "User already exists !";
    }
}