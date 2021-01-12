using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Entities;

namespace TBTT_Data.Interface
{
    public interface IMemberRepository
    {
        Member GetMember(string MemberID);
        int AddMember(Member member);
    }
}
