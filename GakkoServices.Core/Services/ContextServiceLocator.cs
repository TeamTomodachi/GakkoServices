using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GakkoServices.Core.Services
{
    public class ContextServiceLocator
    {
        public IHttpContextAccessor HttpContextAccessor { get; protected set; }

        public ContextServiceLocator(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public T GetService<T>()
        {
            return HttpContextAccessor.HttpContext.RequestServices.GetRequiredService<T>();
        }
    }
}
