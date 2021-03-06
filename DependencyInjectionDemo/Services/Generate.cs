﻿using System;

namespace DependencyInjectionDemo.Services
{
    public class Generate : ITransient, IScoped, ISingleton
    {
        #region Generate
        private Guid _guid;

        public Generate()
        {
            _guid = Guid.NewGuid();
        }

        public Guid GetGuid()
        {
            return _guid;
        }
        #endregion
    }
}
