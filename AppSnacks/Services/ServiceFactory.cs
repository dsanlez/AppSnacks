﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public class ServiceFactory
    {
        public static FavoriteService CreateFavoriteService()
        {
            return new FavoriteService();
        }
    }
}
