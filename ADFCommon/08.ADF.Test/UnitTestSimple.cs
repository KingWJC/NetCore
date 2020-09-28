using System;
using Xunit;
using ADF.Entity;
using ADF.IBusiness;
using System.Collections.Generic;

namespace ADF.Test
{
    public class UnitTestSimple
    {
        IBaseBusiness<Base_User> service;

        public UnitTestSimple()
        {
            // service = new ADF.Business.Simple.BaseService();
        }

        // [Fact]
        // public void TestGetNextId()
        // {
        //     int sequence = service.GetNextId("SEQ_Patient");
        // }

        // [Fact]
        // public void TestGetCount()
        // {
        //     int count = service.GetCount<JHZM_PAT_MAIN>();
        // }
        
        // [Fact]
        // public void TestGetList()
        // {
        //     var list = service.GetEntitys<JHZM_PAT_MAIN>();
        // }
        // [Fact]
        // public void TestGetListCondition()
        // {
        //     Dictionary<string, object> dicts = new Dictionary<string, object>(){
        //         {"PATIENT_NAME","测试"},
        //         {"SEX", "男"},
        //         {"AGE", 38}
        //     };
        //     var list = service.GetEntitys<JHZM_PAT_MAIN>(null);
        //     Assert.True(list.Count > 0);
        // }
        // [Fact]
        // public void TestInsert()
        // {
        //     JHZM_PAT_MAIN user = new JHZM_PAT_MAIN()
        //     {
        //         PAT_MAIN_ID = 0,
        //         PATIENT_NAME = "wangjicheng",
        //         OUT_PATIENT_NO = "10004",
        //         HOSPITAL_NO = "1001",
        //         SEX = "女",
        //         AGE = 30
        //     };

        //     Assert.Equal(1, service.UpdateEntityList(new List<JHZM_PAT_MAIN>() { user }));
        // }
        // [Fact]
        // public void TestUpdate()
        // {
        //     JHZM_PAT_MAIN user = new JHZM_PAT_MAIN()
        //     {
        //         PAT_MAIN_ID = 1182,
        //         PATIENT_NAME = "wangjichengTest",
        //         OUT_PATIENT_NO = "10004",
        //         HOSPITAL_NO = "1001",
        //         SEX = "女",
        //         AGE = 60
        //     };
        //     Assert.Equal(1, service.UpdateEntityList(new List<JHZM_PAT_MAIN>() { user }));
        // }
        // [Fact] 
        // public void TestUpdateFields()
        // {
        //     //Given
        //     Dictionary<string, object> dicts = new Dictionary<string, object>(){
        //         {"PATIENT_NAME","wangjichengTest2"},
        //         {"SEX", "男"},
        //         {"AGE", 44}
        //     };
        //     //When

        //     //Then
        //     Assert.Equal(1, service.UpdateEntity<JHZM_PAT_MAIN>(1182, dicts));
        // }
        // [Fact]
        // public void TestUpdateList()
        // {
        //     //Given
        //     JHZM_PAT_MAIN user = new JHZM_PAT_MAIN()
        //     {
        //         PAT_MAIN_ID = 0,
        //         PATIENT_NAME = "wangjichengTest",
        //         OUT_PATIENT_NO = "10004",
        //         HOSPITAL_NO = "1001",
        //         SEX = "女",
        //         AGE = 60
        //     };

        //     JHZM_PAT_MAIN user2 = new JHZM_PAT_MAIN()
        //     {
        //         PAT_MAIN_ID = 1181,
        //         PATIENT_NAME = "wjc",
        //         OUT_PATIENT_NO = "10004",
        //         HOSPITAL_NO = "1001",
        //         SEX = "女",
        //         AGE = 60
        //     };

        //     //When

        //     //Then
        //     Assert.Equal(2, service.UpdateEntityList(new List<JHZM_PAT_MAIN>() { user, user2 }));
        // }
        // [Fact]
        // public void TestDelete()
        // {
        //     //Then
        //     Assert.Equal(3, service.DeleteEntityByIDs<JHZM_PAT_MAIN>(new List<string>() { "10081", "10082", "10085" }, false));
        // }
    }
}
