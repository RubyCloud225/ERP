using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ERP.Services
{
    public class CacheService
    {
        private readonly Dictionary<Type, Dictionary<int, object>> _cache = new();
        private readonly Dictionary<Type, IRepository<object>> _repositories;
    }
}