using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Simple.Data;
using Simple.Models.Entities;
using Simple.Models.Entities.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simple.Services.Identity.Managers
{
    public class AppUserManager : UserManager<User>
    {
        public AppUserManager(
            ApplicationDbContext context,
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger) : base(
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            Context = context;
        }

        public ApplicationDbContext Context { get; }


        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var result = await base.CreateAsync(user, password);

            if (result.Succeeded)
            {
                Context.PasswordHistories.Add(new PasswordHistory { Password = password, UserId = user.Id });
                await Context.SaveChangesAsync();
            }

            return result;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            bool CheckPassword = await Context.PasswordHistories.AsNoTracking()
               .Where(p => p.UserId == user.Id)
               .AnyAsync(p => p.Password == newPassword)
            ;

            if (CheckPassword)
            {
                return IdentityResult.Failed(new IdentityError { Code = "", Description = "شما قبلا از این رمز استفاده کرده اید لطفا یک رمز جدید انتخاب کنید." });
            }
            else
            {
                var result = await base.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    Context.PasswordHistories.Add(new PasswordHistory { Password = newPassword, UserId = user.Id });
                    await Context.SaveChangesAsync();
                }

                return result;
            }
        }
    }
}