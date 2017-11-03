using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using Microsoft.EntityFrameworkCore;
using Shop.ReadModel.Context;

namespace Shop.ReadModel.Queries
{
    public interface IUserInfoQuery:ISingleQuery<Guid,User>
    {
        
    }

    public class UserInfoQuery: IUserInfoQuery
    {
        private readonly ShopDbContext _shopDbContext;

        public UserInfoQuery(ShopDbContext ctx)
        {
            _shopDbContext = ctx;
        }
        public Task<User> Execute(Guid p1)
        {
            return _shopDbContext.Users.FindAsync(p1);
        }
    }
}
