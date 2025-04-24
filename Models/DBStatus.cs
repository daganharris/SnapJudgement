using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapJudgement.Models
{
    public enum DBStatus
    {
        LoadEmpty,
        LoadSuccess, 
        LoadFail,
        SaveSuccess,
        SaveFail,
        ClearSuccess,
        ClearFail,
    }
    public enum UserStatus
    {
        PasswordIncorrect,
        PasswordCorrect,
        PasswordUserInvalid,
        LoadFail,
        CreateSuccess,
        CreateFail,
        UserNotTaken,
        UserTaken,
    }
}
