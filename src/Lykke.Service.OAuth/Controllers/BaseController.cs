﻿using Microsoft.AspNetCore.Mvc;

namespace WebAuth.Controllers
{
    public class BaseController : Controller
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("tolocal")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult RedirectToLocal(string url)
        {
            if (Url.IsLocalUrl(url))
                return Redirect(url);
            return Redirect("/");
        }
    }
}
