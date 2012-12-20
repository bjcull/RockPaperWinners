using RockPaperWinners.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace RockPaperWinners.Web.Controllers
{
    public class BaseController : Controller
    {
        private UserProfile _currentUser;
        protected UserProfile CurrentUser
        {
            get
            {
                if (_currentUser == null)
                    _currentUser = GetCurrentUser();

                return _currentUser;
            }
        }

        private UserProfile GetCurrentUser()
        {
            if (!WebSecurity.IsAuthenticated)
                return null;

            using (var db = new RockPaperWinnersContext())
            {
                return db.UserProfiles.Find(WebSecurity.CurrentUserId);
            }
        }
    }
}
