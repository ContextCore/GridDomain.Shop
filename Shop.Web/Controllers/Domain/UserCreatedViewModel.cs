using System;

namespace Shop.Web.Controllers.Domain {
    public class UserCreatedViewModel
    {
        public UserCreatedViewModel(Guid userId, Guid accountId, long accountNumber)
        {
            UserId = userId;
            AccountId = accountId;
            AccountNumber = accountNumber;
        }
        public Guid UserId { get; }
        public Guid AccountId { get; }
        public long AccountNumber { get; }
    }
}