using System;
using System.Collections.Generic;
using System.Text;
using TBTT_Data.Interface;
using TBTT_Data.Entities;
using TBTT_Data.Infrastructure;
using Dapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Bson;
namespace TBTT_Data.Infrastructure
{
    public class MemberRepository : BaseRepository, IMemberRepository
    {
        private IConfiguration configuration;
        private IMongoCollection<Member> _memberList;

        public MemberRepository()
        {
            configuration = base.configuration;
            _memberList = GetDatabase.GetCollection<Member>("TBTT_Member");
        }

        public int AddMember(Member member)
        {
            int intReturnValue = 0;
            Member mem = new Member();
            DateTime todaysDate = DateTime.Now.Date;

            try
            {
                mem = _memberList.Find(m => m.MemberID == member.MemberID).FirstOrDefault();

                if (mem == null)
                {
                    member.UpdatedDate = todaysDate;
                    _memberList.InsertOne(member);
                    intReturnValue = 1;
                }
                else
                {
                    intReturnValue = 1;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return intReturnValue;
        }

        public Member GetMember(string MemberID)
        {
            Member member = new Member();

            try
            {
                if ((MemberID != null) && (MemberID != string.Empty))
                {
                    member = _memberList.Find(mem => mem.MemberID == MemberID && mem.Status == 1).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return member;
        }
    }
}
