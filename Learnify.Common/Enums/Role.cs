using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnify.Common.Enums
{
    public enum Role
    {
        USER,
        ADMIN,
        INSTRUCTOR,
        REVIEWER
    }
    public enum DiamondTransactionType
    {
        [PgName("EARN")] EARN,
        [PgName("SPEND")] SPEND
    }

    public enum DiamondSource
    {
        [PgName("SYSTEM_STREAK")] SYSTEM_STREAK,
        [PgName("INSTRUCTOR_REWARD")] INSTRUCTOR_REWARD,
        [PgName("BUY_COURSE_REWARD")] BUY_COURSE_REWARD,
        [PgName("VOUCHER_EXCHANGE")] VOUCHER_EXCHANGE,
        [PgName("REFUND")] REFUND
    }
}
