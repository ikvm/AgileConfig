﻿using AgileConfig.Server.Data.Abstraction;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.Data.Freesql;

namespace AgileConfig.Server.Data.Repository.Freesql
{
    public class UserAppAuthRepository : FreesqlRepository<UserAppAuth, string>, IUserAppAuthRepository
    {

        private readonly IFreeSql freeSql;

        public UserAppAuthRepository(IFreeSql freeSql) : base(freeSql)
        {
            this.freeSql = freeSql;
        }
    }
}
