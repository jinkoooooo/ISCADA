using Dapper;
using I_SCADA_SERVER.common;
using Oracle.ManagedDataAccess.Client;
using I_SCADA_SERVER.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I_SCADA_SERVER.domain;

namespace I_SCADA_SERVER.dao {
    class ServerDao : IDatabase {

        #region default defined
        private readonly string _connectionString;

        public ServerDao(string connectionString) {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionString = connectionString;
        }

        public string ConnectionString() {
            return _connectionString;
        }
        #endregion

        
        public List<MD_USER> SelectMDUser() {
            using (var conn = new OracleConnection(ConnectionString())) {
                return conn.Query<MD_USER>(
                    @"select    *
                      from      MD_USER").ToList();
            }
        }

        public void UpdateState(TB_PRO_STATE param)
        {
            using (var conn = new OracleConnection(ConnectionString()))
            {
                conn.Execute(
                    @"update tb_pro_state 
                        set now_state=:STATE, change_time=sysdate 
                        where user_id=:USER_ID", param);
            }
        }

        public SearchState GetState(SearchState param)
        {
            using (var conn = new OracleConnection(ConnectionString()))
            {
                return conn.Query<SearchState>(
                    @"select user_id as PROF_ID
                        , now_state as STATE
                        from tb_pro_state
                        where user_id = (select a.user_id
                                            from md_user a
                                            where user_name=:PROF_NAME)", param).FirstOrDefault();
            }
        }

        public List<AllState> GetStateAll()
        {
            using (var conn = new OracleConnection(ConnectionString()))
            {
                return conn.Query<AllState>(
                    @"select *
                    from tb_pro_state").ToList();
            }
        }

        public List<Message> GetMessage(MD_USER param)
        {
            using (var conn = new OracleConnection(ConnectionString()))
            {
                return conn.Query<Message>(
                    @"select message_to as TO_ID 
                            ,message_from as FROM_ID
                            , message_data as MESSAGE
                        from tb_message_data
                        where message_to = :USER_ID", param).ToList();
            }
        }

        public void InsertMessage(Message param)
        {
            using (var conn = new OracleConnection(ConnectionString()))
            {
                conn.Execute(
                    @"insert 
                        into tb_message_data(message_to, message_from, message_data, MESSAGE_DATE)
                        values(:TO_ID,:FROM_ID,:MESSAGE, sysdate)", param);
            }
        }
    }
}
