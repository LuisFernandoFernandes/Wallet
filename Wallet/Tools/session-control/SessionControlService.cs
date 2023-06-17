//using Microsoft.EntityFrameworkCore;
//using System;
//using Wallet.Modules.user_module;
//using Wallet.Tools.database;
//using Wallet.Tools.generic_module;
//using Wallet.Tools.helpers;

//namespace Wallet.Tools.session_control
//{
//    public class SessionControlService : GenericService<SessionControl>, ISessionControlService
//    {
//        private readonly Context _context;

//        //public SessionControlService(Context context)
//        //{
//        //    _context = context;
//        //}

//        public async Task<SessionControl> RegisterLogin(User user)
//        {
//            await this.LogoutSession(user);

//            var accessControl = new SessionControl() { DateLogin = DateTime.Now, SessionId = GuidHelpers.NewGuid(), UserId = user.Id, Status = eStatusSessionControl.Logged };
//            await this.IncluirAsync(accessControl); //isso nãio funciona ainda, implementar os generic
//            return accessControl;
//        }

//        private async Task LogoutSession(User user)
//        {
//            var listLogin = await this.AsQueryable().Where(a => a.UserId == user.Id && a.DateLogout == null).ToListAsync();
//            foreach (var sessions in listLogin)
//            {
//                sessions.DateLogout = sessions.DateLogin?.AddHours(12);
//                sessions.Status = eStatusSessionControl.LogoutFromSystem;
//                await this.AlterarAsync(sessions); //implementar os generics com range
//            }
//        }


//    }
//}

