using Lunchtizzle.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Lunchtizzle.Controllers
{
    public class BaseController : Controller
    {
        private User _currentUser;
        protected User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                    _currentUser = GetCurrentUser();

                return _currentUser;
            }
        }

        private User GetCurrentUser()
        {
            if (!WebSecurity.IsAuthenticated)
                return null;

            using (var db = new LunchtizzleContext())
            {
                var user = (from u in db.Users
                            where u.ID == WebSecurity.CurrentUserId
                            select u).FirstOrDefault();

                return user;
            }
        }
    }
}
